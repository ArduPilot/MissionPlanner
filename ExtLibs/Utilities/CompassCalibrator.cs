using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using int16_t = System.Int16;
using uint32_t = System.UInt32;
using staticuint16_t = System.UInt16;
using staticfloat = System.Single;

namespace MissionPlanner.Utilities
{
    public class CompassCalibrator : Utils
    {
        public enum compass_cal_status_t
        {
            COMPASS_CAL_NOT_STARTED = 0,
            COMPASS_CAL_WAITING_TO_START = 1,
            COMPASS_CAL_RUNNING_STEP_ONE = 2,
            COMPASS_CAL_RUNNING_STEP_TWO = 3,
            COMPASS_CAL_SUCCESS = 4,
            COMPASS_CAL_FAILED = 5
        };

        private int COMPASS_CAL_NUM_SPHERE_PARAMS = 4;
        private int COMPASS_CAL_NUM_ELLIPSOID_PARAMS = 9;
        private ushort COMPASS_CAL_NUM_SAMPLES = 300;
        private float COMPASS_CAL_DEFAULT_TOLERANCE = 5.0f;

        compass_cal_status_t _status;

        // timeout watchdog state
        uint32_t _last_sample_ms;

        // behavioral state
        float _delay_start_sec;
        uint32_t _start_time_ms;
        bool _retry;
        float _tolerance;
        uint8_t _attempt;
        uint16_t _offset_max;

        //10
        uint8_t[] _completion_mask = new uint8_t[10];

        private uint8_t[] completion_mask_t {
            get { return _completion_mask; }
        }

        //fit state
        param_t _params = new param_t();
        uint16_t _fit_step;
        CompassSample[] _sample_buffer;
        float _fitness; // mean squared residuals
        float _initial_fitness;
        float _sphere_lambda;
        float _ellipsoid_lambda;
        uint16_t _samples_collected;
        uint16_t _samples_thinned;

        ////////////////////////////////////////////////////////////
        ///////////////////// PUBLIC INTERFACE /////////////////////
        ////////////////////////////////////////////////////////////
        
        public CompassCalibrator()
        {
            _tolerance = COMPASS_CAL_DEFAULT_TOLERANCE;
            _sample_buffer = null;

            clear();
        }

        public void clear()
        {
            set_status(compass_cal_status_t.COMPASS_CAL_NOT_STARTED);
        }

        public void start(bool retry, float delay, uint16_t offset_max)
        {
            if (running())
            {
                return;
            }
            _offset_max = offset_max;
            _attempt = 1;
            _retry = retry;
            _delay_start_sec = delay;
            _start_time_ms = AP_HAL.millis();
            set_status(compass_cal_status_t.COMPASS_CAL_WAITING_TO_START);
        }

        public void get_calibration(ref Vector3f offsets, ref Vector3f diagonals, ref Vector3f offdiagonals)
        {
            if (_status != compass_cal_status_t.COMPASS_CAL_SUCCESS)
            {
                return;
            }

            offsets = _params.offset;
            diagonals = _params.diag;
            offdiagonals = _params.offdiag;
        }

        public float get_completion_percent()
        {
            // first sampling step is 1/3rd of the progress bar
            // never return more than 99% unless _status is COMPASS_CAL_SUCCESS
            switch (_status)
            {
                case compass_cal_status_t.COMPASS_CAL_NOT_STARTED:
                case compass_cal_status_t.COMPASS_CAL_WAITING_TO_START:
                    return 0.0f;
                case compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_ONE:
                    return 33.3f * _samples_collected / COMPASS_CAL_NUM_SAMPLES;
                case compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_TWO:
                    return 33.3f + 65.7f * ((float) (_samples_collected - _samples_thinned) /
                                            (COMPASS_CAL_NUM_SAMPLES - _samples_thinned));
                case compass_cal_status_t.COMPASS_CAL_SUCCESS:
                    return 100.0f;
                case compass_cal_status_t.COMPASS_CAL_FAILED:
                default:
                    return 0.0f;
            }
            ;
        }

        public void update_completion_mask(Vector3f v)
        {
            Matrix3 softiron = new Matrix3(
                _params.diag.x, _params.offdiag.x, _params.offdiag.y,
                _params.offdiag.x, _params.diag.y, _params.offdiag.z,
                _params.offdiag.y, _params.offdiag.z, _params.diag.z
            );
            Vector3f corrected = softiron * (v + _params.offset);
            int section = AP_GeodesicGrid.section(corrected, true);
            if (section < 0)
            {
                return;
            }
            _completion_mask[section / 8] |= (byte)(1 << (section % 8));
        }

        public void update_completion_mask()
        {
            memset(_completion_mask, 0, 10);
            for (int i = 0; i < _samples_collected; i++)
            {
                update_completion_mask(_sample_buffer[i].get());
            }
        }

        public byte[] get_completion_mask()
        {
            return _completion_mask;
        }

        public bool check_for_timeout()
        {
            uint32_t tnow = AP_HAL.millis();
            if (running() && tnow - _last_sample_ms > 1000)
            {
                _retry = false;
                set_status(compass_cal_status_t.COMPASS_CAL_FAILED);
                return true;
            }
            return false;
        }

        public void new_sample(Vector3f  sample)
        {
            _last_sample_ms = AP_HAL.millis();

            if (_status == compass_cal_status_t.COMPASS_CAL_WAITING_TO_START)
            {
                set_status(compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_ONE);
            }

            if (running() && _samples_collected < COMPASS_CAL_NUM_SAMPLES && accept_sample(sample))
            {
                update_completion_mask(sample);
                _sample_buffer[_samples_collected].set(sample);
                _samples_collected++;
            }
        }

        public void update(ref bool failure)
        {
            failure = false;

            if (!fitting())
            {
                return;
            }

            if (_status == compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_ONE)
            {
                if (_fit_step >= 10)
                {
                    if (is_equal(_fitness, _initial_fitness) || isnan(_fitness))
                    {
                        //if true, means that fitness is diverging instead of converging
                        set_status(compass_cal_status_t.COMPASS_CAL_FAILED);
                        failure = true;
                    }
                    set_status(compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_TWO);
                }
                else
                {
                    if (_fit_step == 0)
                    {
                        calc_initial_offset();
                    }
                    run_sphere_fit();
                    _fit_step++;
                }
            }
            else if (_status == compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_TWO)
            {
                if (_fit_step >= 35)
                {
                    if (fit_acceptable())
                    {
                        set_status(compass_cal_status_t.COMPASS_CAL_SUCCESS);
                    }
                    else
                    {
                        set_status(compass_cal_status_t.COMPASS_CAL_FAILED);
                        failure = true;
                    }
                }
                else if (_fit_step < 15)
                {
                    run_sphere_fit();
                    _fit_step++;
                }
                else
                {
                    run_ellipsoid_fit();
                    _fit_step++;
                }
            }
        }

/////////////////////////////////////////////////////////////
////////////////////// PRIVATE METHODS //////////////////////
/////////////////////////////////////////////////////////////
        bool running()
        {
            return _status == compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_ONE || _status == compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_TWO;
        }

        bool fitting()
        {
            return running() && _samples_collected == COMPASS_CAL_NUM_SAMPLES;
        }

        void initialize_fit()
        {
            //initialize _fitness before starting a fit
            if (_samples_collected != 0)
            {
                _fitness = calc_mean_squared_residuals(_params);
            }
            else
            {
                _fitness = 1.0e30f;
            }
            _ellipsoid_lambda = 1.0f;
            _sphere_lambda = 1.0f;
            _initial_fitness = _fitness;
            _fit_step = 0;
        }

        void reset_state()
        {
            _samples_collected = 0;
            _samples_thinned = 0;
            _params.radius = 200;
            _params.offset.zero();
            _params.diag = new Vector3f(1.0f, 1.0f, 1.0f);
            _params.offdiag.zero();

            memset(_completion_mask, 0, 10);
            initialize_fit();
        }

        bool set_status(compass_cal_status_t status)
        {
            if (status != compass_cal_status_t.COMPASS_CAL_NOT_STARTED && _status == status)
            {
                return true;
            }

            switch (status)
            {
                case compass_cal_status_t.COMPASS_CAL_NOT_STARTED:
                    reset_state();
                    _status = compass_cal_status_t.COMPASS_CAL_NOT_STARTED;

                    if (_sample_buffer != null)
                    {
                        free(_sample_buffer);
                        _sample_buffer = null;
                    }
                    return true;

                case compass_cal_status_t.COMPASS_CAL_WAITING_TO_START:
                    reset_state();
                    _status = compass_cal_status_t.COMPASS_CAL_WAITING_TO_START;

                    set_status(compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_ONE);
                    return true;

                case compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_ONE:
                    if (_status != compass_cal_status_t.COMPASS_CAL_WAITING_TO_START)
                    {
                        return false;
                    }

                    if (_attempt == 1 && (AP_HAL.millis() - _start_time_ms) * 1.0e-3f < _delay_start_sec)
                    {
                        return false;
                    }

                    if (_sample_buffer == null)
                    {
                        _sample_buffer = new CompassSample[COMPASS_CAL_NUM_SAMPLES];
                        for (var i = 0; i < _sample_buffer.Length; i++)
                        {
                            _sample_buffer[i] = new CompassSample();
                        }
                        //(CompassSample) malloc(Marshal.SizeOf(CompassSample) *
                            //                      COMPASS_CAL_NUM_SAMPLES);
                    }

                    if (_sample_buffer != null)
                    {
                        initialize_fit();
                        _status = compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_ONE;
                        return true;
                    }

                    return false;

                case compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_TWO:
                    if (_status != compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_ONE)
                    {
                        return false;
                    }
                    thin_samples();
                    initialize_fit();
                    _status = compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_TWO;
                    return true;

                case compass_cal_status_t.COMPASS_CAL_SUCCESS:
                    if (_status != compass_cal_status_t.COMPASS_CAL_RUNNING_STEP_TWO)
                    {
                        return false;
                    }

                    if (_sample_buffer != null)
                    {
                        free(_sample_buffer);
                        _sample_buffer = null;
                    }

                    _status = compass_cal_status_t.COMPASS_CAL_SUCCESS;
                    return true;

                case compass_cal_status_t.COMPASS_CAL_FAILED:
                    if (_status == compass_cal_status_t.COMPASS_CAL_NOT_STARTED)
                    {
                        return false;
                    }

                    if (_retry && set_status(compass_cal_status_t.COMPASS_CAL_WAITING_TO_START))
                    {
                        _attempt++;
                        return true;
                    }

                    if (_sample_buffer != null)
                    {
                        free(_sample_buffer);
                        _sample_buffer = null;
                    }

                    _status = compass_cal_status_t.COMPASS_CAL_FAILED;
                    return true;

                default:
                    return false;
            }
            ;
        }

        bool fit_acceptable()
        {
            if (!isnan(_fitness) &&
                _params.radius > 150 && _params.radius < 950 && //Earth's magnetic field strength range: 250-850mG
                fabsf(_params.offset.x) < _offset_max &&
                fabsf(_params.offset.y) < _offset_max &&
                fabsf(_params.offset.z) < _offset_max &&
                _params.diag.x > 0.2f && _params.diag.x < 5.0f &&
                _params.diag.y > 0.2f && _params.diag.y < 5.0f &&
                _params.diag.z > 0.2f && _params.diag.z < 5.0f &&
                fabsf(_params.offdiag.x) < 1.0f && //absolute of sine/cosine output cannot be greater than 1
                fabsf(_params.offdiag.y) < 1.0f &&
                fabsf(_params.offdiag.z) < 1.0f)
            {

                return _fitness <= sq(_tolerance);
            }
            return false;
        }

        void thin_samples()
        {
            if (_sample_buffer == null)
            {
                return;
            }

            if (_samples_collected == 0)
            {
                return;
            }

            _samples_thinned = 0;
            // shuffle the samples http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
            // this is so that adjacent samples don't get sequentially eliminated
            for (uint16_t i = (uint16_t)(_samples_collected - 1); i >= 1; i--)
            {
                uint16_t j = (uint16_t)(get_random16() % (i + 1));
                CompassSample temp = _sample_buffer[i];
                _sample_buffer[i] = _sample_buffer[j];
                _sample_buffer[j] = temp;
            }

            for (uint16_t i = 0; i < _samples_collected; i++)
            {
                if (!accept_sample(_sample_buffer[i]))
                {
                    _sample_buffer[i] = _sample_buffer[_samples_collected - 1];
                    _samples_collected--;
                    _samples_thinned++;
                }
            }

            update_completion_mask();
        }

/*
 * The sample acceptance distance is determined as follows:
 * For any regular polyhedron with triangular faces, the angle theta subtended
 * by two closest points is defined as
 *
 *      theta = arccos(cos(A)/(1-cos(A)))
 *
 * Where:
 *      A = (4pi/F + pi)/3
 * and
 *      F = 2V - 4 is the number of faces for the polyhedron in consideration,
 *      which depends on the number of vertices V
 *
 * The above equation was proved after solving for spherical triangular excess
 * and related equations.
 */
        bool accept_sample(Vector3f sample)
        {
            staticuint16_t faces = (staticuint16_t)(2 * COMPASS_CAL_NUM_SAMPLES - 4);
            staticfloat a = (4.0f * Utils.PI / (3.0f * faces)) + Utils.PI / 3.0f;
            staticfloat theta = 0.5f * acosf(cosf(a) / (1.0f - cosf(a)));

            if (_sample_buffer == null)
            {
                return false;
            }

            float min_distance = _params.radius * 2 * sinf(theta / 2);

            for (uint16_t i = 0; i < _samples_collected; i++)
            {
                float distance = (sample - _sample_buffer[i].get()).length();
                if (distance < min_distance)
                {
                    return false;
                }
            }
            return true;
        }

        bool accept_sample(CompassSample  sample)
        {
            return accept_sample(sample.get());
        }

        float calc_residual(Vector3f  sample, param_t @params) {
            Matrix3 softiron = new Matrix3(
                @params.diag.x , @params.offdiag.x , @params.offdiag.y,
                @params.offdiag.x , @params.diag.y , @params.offdiag.z,
                @params.offdiag.y , @params.offdiag.z , @params.diag.z
                );
            return (float)(@params.radius - (softiron * (sample +@params.offset)).length());
        }

        float calc_mean_squared_residuals()
        {
            return calc_mean_squared_residuals(_params);
        }

        float calc_mean_squared_residuals(param_t @params)
        {
            if (_sample_buffer == null || _samples_collected == 0)
            {
                return 1.0e30f;
            }
            float sum = 0.0f;
            for (uint16_t i = 0; i < _samples_collected; i++)
            {
                Vector3f sample = _sample_buffer[i].get();
                float resid = calc_residual(sample,  @params);
                sum += sq(resid);
            }
            sum /= _samples_collected;
            return sum;
        }

        void calc_sphere_jacob(Vector3f  sample,param_t @params,ref float[] ret) {
            Vector3f  offset = @params.offset;
            Vector3f  diag = @params.diag;
            Vector3f  offdiag = @params.offdiag;
            Matrix3
            softiron = new Matrix3(
                diag.x, offdiag.x, offdiag.y,
                offdiag.x, diag.y, offdiag.z,
                offdiag.y, offdiag.z, diag.z
            );

            float A = (diag.x * (sample.x + offset.x)) + (offdiag.x * (sample.y + offset.y)) +
                      (offdiag.y * (sample.z + offset.z));
            float B = (offdiag.x * (sample.x + offset.x)) + (diag.y * (sample.y + offset.y)) +
                      (offdiag.z * (sample.z + offset.z));
            float C = (offdiag.y * (sample.x + offset.x)) + (offdiag.z * (sample.y + offset.y)) +
                      (diag.z * (sample.z + offset.z));
            float length = (float)(softiron * (sample + offset)).length();

// 0: partial derivative (radius wrt fitness fn) fn operated on sample
            ret[0] = 1.0f;
            // 1-3: partial derivative (offsets wrt fitness fn) fn operated on sample
            ret[1] = -1.0f * (((diag.x * A) + (offdiag.x * B) + (offdiag.y * C)) / length);
            ret[2] = -1.0f * (((offdiag.x * A) + (diag.y * B) + (offdiag.z * C)) / length);
            ret[3] = -1.0f * (((offdiag.y * A) + (offdiag.z * B) + (diag.z * C)) / length);
        }

        void calc_initial_offset()
        {
            // Set initial offset to the average value of the samples
            _params.offset.zero();
            for (uint16_t k = 0; k < _samples_collected; k++)
            {
                _params.offset -= _sample_buffer[k].get();
            }
            _params.offset /= _samples_collected;
        }

        void run_sphere_fit()
        {
            if (_sample_buffer == null)
            {
                return;
            }

            float lma_damping = 10.0f;

            float fitness = _fitness;
            float fit1, fit2;
            param_t fit1_params, fit2_params;
            fit1_params = fit2_params = _params;

            float[] JTJ = new float[COMPASS_CAL_NUM_SPHERE_PARAMS * COMPASS_CAL_NUM_SPHERE_PARAMS];
            float[] JTJ2 = new float[COMPASS_CAL_NUM_SPHERE_PARAMS * COMPASS_CAL_NUM_SPHERE_PARAMS];
            float[] JTFI = new float[COMPASS_CAL_NUM_SPHERE_PARAMS];

            // Gauss Newton Part common for all kind of extensions including LM
            for (uint16_t k = 0; k < _samples_collected; k++)
            {
                Vector3f sample = _sample_buffer[k].get();

                float[] sphere_jacob = new float[COMPASS_CAL_NUM_SPHERE_PARAMS];

                calc_sphere_jacob(sample, fit1_params,ref sphere_jacob);

                for (uint8_t i = 0; i < COMPASS_CAL_NUM_SPHERE_PARAMS; i++)
                {
                    // compute JTJ
                    for (uint8_t j = 0; j < COMPASS_CAL_NUM_SPHERE_PARAMS; j++)
                    {
                        JTJ[i * COMPASS_CAL_NUM_SPHERE_PARAMS + j] += sphere_jacob[i] * sphere_jacob[j];
                        JTJ2[i * COMPASS_CAL_NUM_SPHERE_PARAMS + j] +=
                            sphere_jacob[i] * sphere_jacob[j]; //a backup JTJ for LM
                    }
                    // compute JTFI
                    JTFI[i] += sphere_jacob[i] * calc_residual(sample, fit1_params);
                }
            }


            //------------------------Levenberg-Marquardt-part-starts-here---------------------------------//
            //refer: http://en.wikipedia.org/wiki/Levenberg%E2%80%93Marquardt_algorithm#Choice_of_damping_parameter
            for (uint8_t i = 0; i < COMPASS_CAL_NUM_SPHERE_PARAMS; i++)
            {
                JTJ[i * COMPASS_CAL_NUM_SPHERE_PARAMS + i] += _sphere_lambda;
                JTJ2[i * COMPASS_CAL_NUM_SPHERE_PARAMS + i] += _sphere_lambda / lma_damping;
            }

            if (!inverse(JTJ, JTJ, 4))
            {
                return;
            }

            if (!inverse(JTJ2, JTJ2, 4))
            {
                return;
            }

            for (uint8_t row = 0; row < COMPASS_CAL_NUM_SPHERE_PARAMS; row++)
            {
                for (uint8_t col = 0; col < COMPASS_CAL_NUM_SPHERE_PARAMS; col++)
                {
                    fit1_params.get_sphere_params()[row] -= JTFI[col] * JTJ[row * COMPASS_CAL_NUM_SPHERE_PARAMS + col];
                    fit2_params.get_sphere_params()[row] -= JTFI[col] * JTJ2[row * COMPASS_CAL_NUM_SPHERE_PARAMS + col];
                }
            }

            fit1 = calc_mean_squared_residuals(fit1_params);
            fit2 = calc_mean_squared_residuals(fit2_params);

            if (fit1 > _fitness && fit2 > _fitness)
            {
                _sphere_lambda *= lma_damping;
            }
            else if (fit2 < _fitness && fit2 < fit1)
            {
                _sphere_lambda /= lma_damping;
                fit1_params = fit2_params;
                fitness = fit2;
            }
            else if (fit1 < _fitness)
            {
                fitness = fit1;
            }
            //--------------------Levenberg-Marquardt-part-ends-here--------------------------------//

            if (!isnan(fitness) && fitness < _fitness)
            {
                _fitness = fitness;
                _params = fit1_params;
                update_completion_mask();
            }
        }

        void calc_ellipsoid_jacob(Vector3f  sample, param_t @params, ref float[] ret) {
            Vector3f  offset = @params.offset;
            Vector3f  diag = @params.diag;
            Vector3f  offdiag = @params.offdiag;
            Matrix3
            softiron = new Matrix3(
                diag.x, offdiag.x, offdiag.y,
                offdiag.x, diag.y, offdiag.z,
                offdiag.y, offdiag.z, diag.z
            );

            float A = (diag.x * (sample.x + offset.x)) + (offdiag.x * (sample.y + offset.y)) +
                      (offdiag.y * (sample.z + offset.z));
            float B = (offdiag.x * (sample.x + offset.x)) + (diag.y * (sample.y + offset.y)) +
                      (offdiag.z * (sample.z + offset.z));
            float C = (offdiag.y * (sample.x + offset.x)) + (offdiag.z * (sample.y + offset.y)) +
                      (diag.z * (sample.z + offset.z));
            float length = (float)(softiron * (sample + offset)).length();

// 0-2: partial derivative (offset wrt fitness fn) fn operated on sample
            ret[0] = -1.0f * (((diag.x * A) + (offdiag.x * B) + (offdiag.y * C)) / length);
            ret[1] = -1.0f * (((offdiag.x * A) + (diag.y * B) + (offdiag.z * C)) / length);
            ret[2] = -1.0f * (((offdiag.y * A) + (offdiag.z * B) + (diag.z * C)) / length);
            // 3-5: partial derivative (diag offset wrt fitness fn) fn operated on sample
            ret[3] = -1.0f * ((sample.x + offset.x) * A) / length;
            ret[4] = -1.0f * ((sample.y + offset.y) * B) / length;
            ret[5] = -1.0f * ((sample.z + offset.z) * C) / length;
            // 6-8: partial derivative (off-diag offset wrt fitness fn) fn operated on sample
            ret[6] = -1.0f * (((sample.y + offset.y) * A) + ((sample.x + offset.x) * B)) / length;
            ret[7] = -1.0f * (((sample.z + offset.z) * A) + ((sample.x + offset.x) * C)) / length;
            ret[8] = -1.0f * (((sample.z + offset.z) * B) + ((sample.y + offset.y) * C)) / length;
        }

        void run_ellipsoid_fit()
        {
            if (_sample_buffer == null)
            {
                return;
            }

            float lma_damping = 10.0f;


            float fitness = _fitness;
            float fit1, fit2;
            param_t fit1_params, fit2_params;
            fit1_params = fit2_params = _params;


            float[] JTJ = new float[COMPASS_CAL_NUM_ELLIPSOID_PARAMS * COMPASS_CAL_NUM_ELLIPSOID_PARAMS];
            float[] JTJ2 = new float[COMPASS_CAL_NUM_ELLIPSOID_PARAMS * COMPASS_CAL_NUM_ELLIPSOID_PARAMS];
            float[] JTFI = new float[COMPASS_CAL_NUM_ELLIPSOID_PARAMS];

            // Gauss Newton Part common for all kind of extensions including LM
            for (uint16_t k = 0; k < _samples_collected; k++)
            {
                Vector3f sample = _sample_buffer[k].get();

                float[] ellipsoid_jacob = new float[COMPASS_CAL_NUM_ELLIPSOID_PARAMS];

                calc_ellipsoid_jacob(sample, fit1_params, ref ellipsoid_jacob);

                for (uint8_t i = 0; i < COMPASS_CAL_NUM_ELLIPSOID_PARAMS; i++)
                {
                    // compute JTJ
                    for (uint8_t j = 0; j < COMPASS_CAL_NUM_ELLIPSOID_PARAMS; j++)
                    {
                        JTJ[i * COMPASS_CAL_NUM_ELLIPSOID_PARAMS + j] += ellipsoid_jacob[i] * ellipsoid_jacob[j];
                        JTJ2[i * COMPASS_CAL_NUM_ELLIPSOID_PARAMS + j] += ellipsoid_jacob[i] * ellipsoid_jacob[j];
                    }
                    // compute JTFI
                    JTFI[i] += ellipsoid_jacob[i] * calc_residual(sample, fit1_params);
                }
            }



            //------------------------Levenberg-Marquardt-part-starts-here---------------------------------//
            //refer: http://en.wikipedia.org/wiki/Levenberg%E2%80%93Marquardt_algorithm#Choice_of_damping_parameter
            for (uint8_t i = 0; i < COMPASS_CAL_NUM_ELLIPSOID_PARAMS; i++)
            {
                JTJ[i * COMPASS_CAL_NUM_ELLIPSOID_PARAMS + i] += _ellipsoid_lambda;
                JTJ2[i * COMPASS_CAL_NUM_ELLIPSOID_PARAMS + i] += _ellipsoid_lambda / lma_damping;
            }

            if (!inverse(JTJ, JTJ, 9))
            {
                return;
            }

            if (!inverse(JTJ2, JTJ2, 9))
            {
                return;
            }

            for (uint8_t row = 0; row < COMPASS_CAL_NUM_ELLIPSOID_PARAMS; row++)
            {
                for (uint8_t col = 0; col < COMPASS_CAL_NUM_ELLIPSOID_PARAMS; col++)
                {
                    fit1_params.get_sphere_params()[row+1] -=
                        JTFI[col] * JTJ[row * COMPASS_CAL_NUM_ELLIPSOID_PARAMS + col];
                    fit2_params.get_sphere_params()[row+1] -=
                        JTFI[col] * JTJ2[row * COMPASS_CAL_NUM_ELLIPSOID_PARAMS + col];
                }
            }

            fit1 = calc_mean_squared_residuals(fit1_params);
            fit2 = calc_mean_squared_residuals(fit2_params);

            if (fit1 > _fitness && fit2 > _fitness)
            {
                _ellipsoid_lambda *= lma_damping;
            }
            else if (fit2 < _fitness && fit2 < fit1)
            {
                _ellipsoid_lambda /= lma_damping;
                fit1_params = fit2_params;
                fitness = fit2;
            }
            else if (fit1 < _fitness)
            {
                fitness = fit1;
            }
            //--------------------Levenberg-part-ends-here--------------------------------//

            if (fitness < _fitness)
            {
                _fitness = fitness;
                _params = fit1_params;
                update_completion_mask();
            }
        }

        /*
  simple 16 bit random number generator
 */
        static uint32_t m_z = 1234;
        static uint32_t m_w = 76542;

        uint16_t get_random16()
        {
            m_z = 36969 * (m_z & 0xFFFFu) + (m_z >> 16);
            m_w = 18000 * (m_w & 0xFFFFu) + (m_w >> 16);
            return (uint16_t) (((m_z << 16) + m_w) & 0xFFFF);
        }

        public class param_t
        {
            public ref float[] get_sphere_params()
            {
                return ref data;
            }

          /*  public ref float[] get_ellipsoid_params()
            {
                return ref data[1];
            }
            */
            private float[] data = new float[10];

            public float radius
            {
                get { return data[0]; }
                set { data[0] = value; }
            }

            public Vector3f offset
            {
                get { return new Vector3f(data[1], data[2], data[3]); }
                set
                {
                    data[1] = value.x;
                    data[2] = value.y;
                    data[3] = value.z;
                }
            }
            public Vector3f diag
            {
                get { return new Vector3f(data[4], data[5], data[6]); }
                set
                {
                    data[4] = value.x;
                    data[5] = value.y;
                    data[6] = value.z;
                }
            }
            public Vector3f offdiag
            {
                get { return new Vector3f(data[7], data[8], data[9]); }
                set
                {
                    data[7] = value.x;
                    data[8] = value.y;
                    data[9] = value.z;
                }
            }
        }

public class CompassSample
        {
            private int16_t INT16_MIN = Int16.MinValue;
            private int16_t INT16_MAX = Int16.MaxValue;

            int16_t COMPASS_CAL_SAMPLE_SCALE_TO_FIXED(float __X)
            {
                return ((int16_t) constrain_float(roundf(__X * 8.0f), INT16_MIN, INT16_MAX));
            }

            float COMPASS_CAL_SAMPLE_SCALE_TO_FLOAT(float __X)
            {
                return (__X / 8.0f);
            }

            public int16_t x;
            public int16_t y;
            public int16_t z;

    public Vector3f get()
            {
                return new Vector3f(COMPASS_CAL_SAMPLE_SCALE_TO_FLOAT(x),
                    COMPASS_CAL_SAMPLE_SCALE_TO_FLOAT(y),
                    COMPASS_CAL_SAMPLE_SCALE_TO_FLOAT(z));
            }

            public void set(Vector3f @in)
            {
                x = COMPASS_CAL_SAMPLE_SCALE_TO_FIXED(@in.x);
                y = COMPASS_CAL_SAMPLE_SCALE_TO_FIXED(@in.y);
                z = COMPASS_CAL_SAMPLE_SCALE_TO_FIXED(@in.z);
            }
        }
    }
}
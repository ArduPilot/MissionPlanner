namespace MissionPlanner.Joystick
{
    public struct JoyButton
    {
        /// <summary>
        /// System button number
        /// </summary>
        public int buttonno;

        /// <summary>
        /// Fucntion we are doing for this button press
        /// </summary>
        public buttonfunction function;

        /// <summary>
        /// Mode we are changing to on button press
        /// </summary>
        public string mode;

        /// <summary>
        /// param 1
        /// </summary>
        public float p1;

        /// <summary>
        /// param 2
        /// </summary>
        public float p2;

        /// <summary>
        /// param 3
        /// </summary>
        public float p3;

        /// <summary>
        /// param 4
        /// </summary>
        public float p4;

        /// <summary>
        /// Relay state
        /// </summary>
        public bool state;
    }
}
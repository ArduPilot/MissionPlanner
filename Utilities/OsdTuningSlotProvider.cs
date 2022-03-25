using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MAVLink;

namespace MissionPlanner.Utilities
{
    public class OsdTuningSlotProvider: IDisposable
    {
        private readonly ConcurrentDictionary<uint, (byte Screen, byte Index)> paramShowRequests 
            = new ConcurrentDictionary<uint, (byte Screen, byte Index)>();

        private readonly ConcurrentDictionary<uint, (byte Screen, byte Index)> paramSetRequests
            = new ConcurrentDictionary<uint, (byte Screen, byte Index)>();

        public event Action<(byte Screen, byte Index, string Name)> OnParamShowResponce;

        public event Action<(byte Screen, byte Index, bool Success)> OnParamSetResponce;

        private readonly KeyValuePair<MAVLINK_MSG_ID, Func<MAVLinkMessage, bool>> sub1;
        private readonly KeyValuePair<MAVLINK_MSG_ID, Func<MAVLinkMessage, bool>> sub2;

        private uint request;

        public OsdTuningSlotProvider()
        {
            sub1 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.OSD_PARAM_SHOW_CONFIG_REPLY,
                    HandleParamShowResponse);

            sub2 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.OSD_PARAM_CONFIG_REPLY,
                    HandleParamSetResponse);
        }

        public void ParamShow(byte screen, byte index)
        {
            MainV2.comPort.sendPacket(new MAVLink.mavlink_osd_param_show_config_t(++request,
                    (byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent, screen, index), (byte)MainV2.comPort.sysidcurrent,
                (byte)MainV2.comPort.compidcurrent);

            if (!paramShowRequests.TryAdd(request, (screen, index)))
                throw new Exception($"Unable to store the request info for {screen} {index}");
        }

        public void ParamSet(byte screen, byte index, string name, MAVLink.OSD_PARAM_CONFIG_TYPE type, float min, float max, float increment)
        {
            MainV2.comPort.sendPacket(new MAVLink.mavlink_osd_param_config_t(++request, min, max, increment,
                    (byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent, screen, index, name.ToCharArray().ToByteArray(),
                    (byte)type), (byte)MainV2.comPort.sysidcurrent,
                (byte)MainV2.comPort.compidcurrent);

            if (!paramSetRequests.TryAdd(request, (screen, index)))
                throw new Exception($"Unable to store the request info for {screen} {index}");
        }

        private bool HandleParamShowResponse(MAVLink.MAVLinkMessage arg)
        {
            if (arg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.OSD_PARAM_SHOW_CONFIG_REPLY)
            {
                var rep = (MAVLink.mavlink_osd_param_show_config_reply_t)arg.data;

                if (paramShowRequests.TryGetValue(rep.request_id, out var requestInfo))
                {
                    (byte Screen, byte Index, string Name) result = (requestInfo.Screen, requestInfo.Index, null);

                    if (rep.result == (byte)MAVLink.OSD_PARAM_CONFIG_ERROR.OSD_PARAM_SUCCESS)
                    {
                        var zeroIndex = Array.IndexOf(rep.param_id, (byte)0);

                        if (zeroIndex > 0)
                        {
                            result.Name = System.Text.Encoding.UTF8.GetString(rep.param_id, 0, zeroIndex);
                        }
                    }
                    else
                    {
                        // TODO: Log
                    }

                    OnParamShowResponce?.Invoke(result);
                }
                else
                {
                    // TODO: Log
                }

                return true;
            }
            else
                throw new Exception($"Unexpected message: {arg.msgid}");
        }

        private bool HandleParamSetResponse(MAVLinkMessage arg)
        {
            if (arg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.OSD_PARAM_CONFIG_REPLY)
            {
                var rep = (MAVLink.mavlink_osd_param_config_reply_t)arg.data;

                if (paramSetRequests.TryGetValue(rep.request_id, out var requestInfo))
                {
                    bool success = rep.result == (byte)MAVLink.OSD_PARAM_CONFIG_ERROR.OSD_PARAM_SUCCESS;
                    OnParamSetResponce((requestInfo.Screen, requestInfo.Index, success));
                }
                else
                {
                    // TODO: Log
                }

                return true;
            }
            else
                throw new Exception($"Unexpected message: {arg.msgid}");
        }

        public void Dispose()
        {
            MainV2.comPort.UnSubscribeToPacketType(sub1);
            MainV2.comPort.UnSubscribeToPacketType(sub2);
        }
    }

    public class OsdTuningSlotGetter
    {
        private ConcurrentBag<(byte Screen, byte Index, string Name)> results 
            = new ConcurrentBag<(byte Screen, byte Index, string Name)>();

        private DateTime lastResponseTime;

        public static ICollection<(byte Screen, byte Index, string Name)> LoadAll(int retries, CancellationToken cancellationToken)
        {
            var requests = new List<(byte Screen, byte Index)> ();
            requests.AddRange(Enumerable.Range(1, 9).Select(i => ((byte)5, (byte)i)));
            requests.AddRange(Enumerable.Range(1, 9).Select(i => ((byte)6, (byte)i)));

            return Load(requests, retries, cancellationToken);
        }

        public static ICollection<(byte Screen, byte Index, string Name)> Load(
            IEnumerable<(byte Screen, byte Index)> itemsEnumerable,
            int retries,
            CancellationToken cancellationToken)
        {
            var items = new List<(byte Screen, byte Index)>(itemsEnumerable);

            var result = new List<(byte Screen, byte Index, string Name)>();

            int tryNumber = 0;

            while (items.Any() && tryNumber++ <= retries)
            {
                items.RemoveAll(item => result.Any(res => item.Screen == res.Screen && item.Index == res.Index));

                result.AddRange(new OsdTuningSlotGetter().DoLoad(items, cancellationToken));
            }

            return result;
        }

        private OsdTuningSlotGetter()
        {
        }

        private IEnumerable<(byte Screen, byte Index, string Name)> DoLoad(
            IEnumerable<(byte Screen, byte Index)> items, 
            CancellationToken cancellationToken)
        {
            using (var provider = new OsdTuningSlotProvider())
            {
                provider.OnParamShowResponce += HandleParamShowResponce;

                int requestsCount = 0;

                // Sent requests
                foreach (var item in items)
                {
                    provider.ParamShow(item.Screen, item.Index);
                    requestsCount++;
                    Thread.Sleep(20);
                }

                lastResponseTime = DateTime.Now;

                // Waiting for responses
                while (results.Count < requestsCount && (DateTime.Now - lastResponseTime).TotalSeconds < 10)
                {
                    Thread.Sleep(100);

                    if (cancellationToken.IsCancellationRequested)
                        return null;
                }

                return results;
            }
        }

        private void HandleParamShowResponce((byte Screen, byte Index, string Name) response)
        {
            lastResponseTime = DateTime.Now;
            results.Add(response);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltitudeAngelWings.ApiClient.Client.FlightClient;
using AltitudeAngelWings.ApiClient.Models.OutboundNotifs;
using AltitudeAngelWings.Extra;
using AltitudeAngelWings.Extra.OutboundNotifsWebSocket;
using AltitudeAngelWings.Models;
using AltitudeAngelWings.Service.Messaging;
using Newtonsoft.Json;

namespace AltitudeAngelWings.Service.OutboundNotifs
{
    public class OutboundNotifsService : IOutboundNotifsService
    {
        private readonly ISettings _settings;
        private readonly IMissionPlanner _missionPlanner;
        private readonly IMessagesService _messagesService;
        private readonly IFlightClient _flightServiceClient;
        private readonly IMissionPlannerState _missionPlannerState;
        private AaClientWebSocket _aaClientWebSocket;

        public OutboundNotifsService(
            IMissionPlanner missionPlanner,
            ISettings settings,
            IMessagesService messagesService,
            IFlightClient flightServiceClient,
            IMissionPlannerState missionPlannerState)
        {
            _missionPlanner = missionPlanner;
            _settings = settings;
            _messagesService = messagesService;
            _flightServiceClient = flightServiceClient;
            _missionPlannerState = missionPlannerState;
        }

        public async Task StartWebSocket()
        {
            if (null == _aaClientWebSocket)
            {
                await SetupAaClientWebSocket();
            }
        }

        public async Task StopWebSocket()
        {
            if (null != _aaClientWebSocket)
            {
                await TearDownAaClientWebSocket();
            }
        }

        private Task SetupAaClientWebSocket()
        {
            _aaClientWebSocket = new AaClientWebSocket
            {
                OnError = OnError,
                OnDisconnected = OnDisconnected, // TODO: Always attempt to reconnect on disconnect
                OnConnected = OnConnected,
                OnMessage = OnMessage
            };

            _aaClientWebSocket.SetSocketHeaders(new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {_settings.TokenResponse.AccessToken}" }
            });

            return _aaClientWebSocket.OpenAsync(new Uri(_settings.OutboundNotifsEndpointUrl));
        }

        private async Task OnMessage(byte[] bytes)
        {
            await _messagesService.AddMessageAsync(new Message($"INFO: Received notification message of {bytes.Length} bytes."));
            var notification = new NotificationMessage();
            try
            {
                var msg = Encoding.UTF8.GetString(bytes);
                notification = JsonConvert.DeserializeObject<NotificationMessage>(msg);
                if (notification.Acknowledge)
                {
                    await SendAck(_aaClientWebSocket, notification.Id);
                }
            }
            catch (Exception e)
            {
                await _messagesService.AddMessageAsync(new Message($"ERROR: Failed to deserialize and acknowledge notification message. {e}"));
            }

            await _messagesService.AddMessageAsync(new Message($"INFO: Processing {notification.Type} notification message."));

            try
            {
                switch (notification.Type)
                {
                    case OutboundNotifsCommands.Land:
                        LandNotificationProperties landProps = notification.Properties.ToObject<LandNotificationProperties>();
                        await _missionPlanner.CommandDroneToLand((float)landProps.Latitude, (float)landProps.Longitude);
                        break;
                    case OutboundNotifsCommands.Loiter:
                        LoiterNotificationProperties loiterProps = notification.Properties.ToObject<LoiterNotificationProperties>();
                        await _missionPlanner.CommandDroneToLoiter((float)loiterProps.Latitude, (float)loiterProps.Longitude, (float)loiterProps.Altitude.Meters);
                        break;
                    case OutboundNotifsCommands.AllClear:
                        await _missionPlanner.CommandDroneAllClear();
                        break;
                    case OutboundNotifsCommands.ReturnToBase:
                        await _missionPlanner.CommandDroneToReturnToBase();
                        break;
                    case OutboundNotifsCommands.PermissionUpdate:
                        var permissionProperties = notification.Properties.ToObject<PermissionNotificationProperties>();
                        await _messagesService.AddMessageAsync(new Message($"Flight permissions updated: {permissionProperties.PermissionState}")
                            { TimeToLive = TimeSpan.FromSeconds(10) });
                        break;
                    case OutboundNotifsCommands.ConflictInformation:
                        var conflictProperties = notification.Properties.ToObject<ConflictInformationProperties>();
                        await _missionPlanner.NotifyConflict(conflictProperties.Message);
                        break;
                    case OutboundNotifsCommands.ConflictClearedInformation:
                        var conflictClearedProperties = notification.Properties.ToObject<ConflictClearedNotificationProperties>();
                        await _missionPlanner.NotifyConflictResolved(conflictClearedProperties.Message);
                        break;
                    case OutboundNotifsCommands.Instruction:
                        var instructionProperties = notification.Properties.ToObject<InstructionNotificationProperties>();
                        if (await _missionPlanner.ShowYesNoMessageBox(
                            $"You have been sent the following instruction:\r\n\r\n\"{instructionProperties.Instruction}\"\r\n\r\nDo you wish to accept and follow the instruction?",
                            "Instruction"))
                        {
                            await _flightServiceClient.AcceptInstruction(instructionProperties.InstructionId);
                            if (instructionProperties.Instruction.IndexOf("hold", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                await  _missionPlanner.CommandDroneToLoiter((float)_missionPlannerState.Latitude, (float)_missionPlannerState.Longitude, _missionPlannerState.Altitude);
                                break;
                            }

                            if (instructionProperties.Instruction.IndexOf("resume", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                await _missionPlanner.CommandDroneAllClear();
                                break;
                            }

                            if (instructionProperties.Instruction.IndexOf("land", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                await _missionPlanner.CommandDroneToLand((float)_missionPlannerState.Latitude, (float)_missionPlannerState.Longitude);
                                break;
                            }

                            if (instructionProperties.Instruction.IndexOf("return", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                await _missionPlanner.CommandDroneToReturnToBase();
                            }
                        }
                        else
                        {
                            await _flightServiceClient.RejectInstruction(instructionProperties.InstructionId);
                        }
                        break;
                    default:
                        await _messagesService.AddMessageAsync(new Message($"WARN: Unknown notification message type '{notification.Type}'."));
                        break;
                }
            }
            catch (Exception e)
            {
                await _messagesService.AddMessageAsync(new Message($"ERROR: Failed to process {notification.Type} notification message. {e}"));
            }
        }

        private Task OnConnected()
            => _messagesService.AddMessageAsync(new Message($"INFO: Notifications web socket connected to {_settings.OutboundNotifsEndpointUrl}."));

        private Task OnDisconnected()
            => _messagesService.AddMessageAsync(new Message("WARNING: Notifications web socket disconnected."));

        private Task OnError(Exception e)
            => _messagesService.AddMessageAsync(new Message($"ERROR: Notifications web socket error. {e}"));

        private async Task TearDownAaClientWebSocket()
        {
            await _aaClientWebSocket.CloseAsync();
            _aaClientWebSocket = null;
        }

        private async Task SendAck(AaWebSocket socket, string id)
        {
            await _messagesService.AddMessageAsync(new Message($"INFO: Sending notification acknowledgement: {JsonConvert.SerializeObject(new CommandAcknowledgement { Id = id })}"));
            await socket.SendMessageAsync(
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(
                        new CommandAcknowledgement
                        {
                            Id = id
                        })));
        }
    }
}

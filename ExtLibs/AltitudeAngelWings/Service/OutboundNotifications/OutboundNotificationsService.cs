using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AltitudeAngelWings.Clients.Flight;
using AltitudeAngelWings.Clients.OutboundNotifications.Model;
using AltitudeAngelWings.Model;
using AltitudeAngelWings.Service.Messaging;
using Newtonsoft.Json;

namespace AltitudeAngelWings.Service.OutboundNotifications
{
    public class OutboundNotificationsService : IOutboundNotificationsService
    {
        private readonly ISettings _settings;
        private readonly IMissionPlanner _missionPlanner;
        private readonly IMessagesService _messagesService;
        private readonly IFlightClient _flightServiceClient;
        private readonly IMissionPlannerState _missionPlannerState;
        private ClientWebSocket _clientWebSocket;

        public OutboundNotificationsService(
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

        public async Task StartWebSocket(CancellationToken cancellationToken = default)
        {
            if (null == _clientWebSocket)
            {
                await SetupAaClientWebSocket(cancellationToken);
            }
        }

        public async Task StopWebSocket(CancellationToken cancellationToken = default)
        {
            if (null != _clientWebSocket)
            {
                await TearDownAaClientWebSocket(cancellationToken);
            }
        }

        private Task SetupAaClientWebSocket(CancellationToken cancellationToken)
        {
            _clientWebSocket = new ClientWebSocket
            {
                OnError = OnError,
                OnDisconnected = OnDisconnected, // TODO: Always attempt to reconnect on disconnect
                OnConnected = OnConnected,
                OnMessage = OnMessage
            };

            _clientWebSocket.SetSocketHeaders(new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {_settings.TokenResponse.AccessToken}" }
            });

            return _clientWebSocket.OpenAsync(new Uri(_settings.OutboundNotificationsUrl), cancellationToken);
        }

        private async Task OnMessage(byte[] bytes, CancellationToken cancellationToken)
        {
            var notification = new NotificationMessage();
            try
            {
                var msg = Encoding.UTF8.GetString(bytes);
                notification = JsonConvert.DeserializeObject<NotificationMessage>(msg);
                if (notification.Acknowledge)
                {
                    await SendAck(_clientWebSocket, notification.Id);
                }
            }
            catch (Exception e)
            {
                await _messagesService.AddMessageAsync(Message.ForError("Failed to deserialize and acknowledge notification message.", e));
            }

            try
            {
                switch (notification.Type)
                {
                    case OutboundNotificationsCommands.Land:
                        var landProps = notification.Properties.ToObject<LandNotificationProperties>();
                        await _missionPlanner.CommandDroneToLand((float)landProps.Latitude, (float)landProps.Longitude);
                        break;
                    case OutboundNotificationsCommands.Loiter:
                        var loiterProps = notification.Properties.ToObject<LoiterNotificationProperties>();
                        await _missionPlanner.CommandDroneToLoiter((float)loiterProps.Latitude, (float)loiterProps.Longitude, (float)loiterProps.Altitude.Meters);
                        break;
                    case OutboundNotificationsCommands.AllClear:
                        await _missionPlanner.CommandDroneAllClear();
                        break;
                    case OutboundNotificationsCommands.ReturnToBase:
                        await _missionPlanner.CommandDroneToReturnToBase();
                        break;
                    case OutboundNotificationsCommands.PermissionUpdate:
                        var permissionProperties = notification.Properties.ToObject<PermissionNotificationProperties>();
                        await _messagesService.AddMessageAsync(Message.ForInfo($"Flight permissions updated: {permissionProperties.PermissionState}", TimeSpan.FromSeconds(10)));
                        break;
                    case OutboundNotificationsCommands.ConflictInformation:
                        var conflictProperties = notification.Properties.ToObject<ConflictInformationProperties>();
                        await _missionPlanner.NotifyConflict(conflictProperties.Message);
                        break;
                    case OutboundNotificationsCommands.ConflictClearedInformation:
                        var conflictClearedProperties = notification.Properties.ToObject<ConflictClearedNotificationProperties>();
                        await _missionPlanner.NotifyConflictResolved(conflictClearedProperties.Message);
                        break;
                    case OutboundNotificationsCommands.Instruction:
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
                        await _messagesService.AddMessageAsync(Message.ForInfo($"Unknown notification message type {notification.Type}."));
                        break;
                }
            }
            catch (Exception e)
            {
                await _messagesService.AddMessageAsync(Message.ForError($"Failed to process {notification.Type} notification message.", e));
            }
        }

        private Task OnConnected(CancellationToken cancellationToken = default)
            => _messagesService.AddMessageAsync(Message.ForInfo("WebSocket", "Notifications web socket connected."));

        private Task OnDisconnected(CancellationToken cancellationToken = default)
            => _messagesService.AddMessageAsync(Message.ForInfo("WebSocket", "Notifications web socket disconnected."));

        private Task OnError(Exception e, CancellationToken cancellationToken = default)
            => _messagesService.AddMessageAsync(Message.ForError("WebSocket", "Notifications web socket error.", e));

        private async Task TearDownAaClientWebSocket(CancellationToken cancellationToken)
        {
            await _clientWebSocket.CloseAsync(cancellationToken);
            _clientWebSocket = null;
        }

        private static async Task SendAck(WebSocket socket, string id)
            => await socket.SendMessageAsync(
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(
                        new CommandAcknowledgement
                        {
                            Id = id
                        })));
    }
}

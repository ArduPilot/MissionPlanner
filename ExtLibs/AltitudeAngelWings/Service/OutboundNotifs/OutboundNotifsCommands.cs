namespace AltitudeAngelWings.Service.OutboundNotifs
{
    /// <summary>
    /// List of valid Outbound Notifications commands
    /// </summary>
    public static class OutboundNotifsCommands
    {
        /// <summary>
        /// UAV should land immediately.
        /// </summary>
        public const string Land = "LAND";

        /// <summary>
        /// UAV should loiter immediately.
        /// </summary>
        public const string Loiter = "LOITER";

        /// <summary>
        /// Mission Planner should resume mission
        /// </summary>
        public const string AllClear = "ALL CLEAR";

        /// <summary>
        /// Mission Planner should return to base
        /// </summary>
        public const string ReturnToBase = "RTB";

        /// <summary>
        /// Flight permissions have been updated
        /// </summary>
        public const string PermissionUpdate = "flightPermissionsUpdated";

        public const string ConflictInformation = "Conflict";

        public const string ConflictClearedInformation = "Conflict Cleared";

        public const string ChangeVector = "Vector";
        public const string Instruction = "Instruction";
    }
}
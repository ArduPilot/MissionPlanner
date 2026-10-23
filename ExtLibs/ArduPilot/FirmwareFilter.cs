using System;
using System.Collections.Generic;
using System.Linq;

namespace MissionPlanner.ArduPilot
{
    /// <summary>
    /// Faceted filter over manifest firmware entries. Each field can be constrained to a single value,
    /// and the options offered for a field are those still reachable given every other field's selection.
    /// </summary>
    public class FirmwareFilter
    {
        public enum Field
        {
            MavType,
            VersionType,
            Platform,
            Version,
            Format,
            UsbId,
            BoardId,
            BootloaderStr
        }

        public static readonly Field[] AllFields = (Field[])Enum.GetValues(typeof(Field));

        private readonly List<APFirmware.FirmwareInfo> _firmware;
        private readonly Dictionary<Field, string> _selected = new Dictionary<Field, string>();

        public FirmwareFilter(IEnumerable<APFirmware.FirmwareInfo> firmware)
        {
            _firmware = firmware?.Where(a => a != null).ToList() ?? new List<APFirmware.FirmwareInfo>();
        }

        public IReadOnlyList<APFirmware.FirmwareInfo> Firmware => _firmware;

        /// <summary>
        /// Constrain a field to a value. null or empty clears the constraint.
        /// </summary>
        public void Set(Field field, string value)
        {
            if (string.IsNullOrEmpty(value))
                _selected.Remove(field);
            else
                _selected[field] = value;
        }

        public string Get(Field field)
        {
            string value;
            return _selected.TryGetValue(field, out value) ? value : null;
        }

        public void Clear()
        {
            _selected.Clear();
        }

        /// <summary>
        /// Firmware matching every current selection.
        /// </summary>
        public List<APFirmware.FirmwareInfo> GetMatches()
        {
            return Apply(null).ToList();
        }

        /// <summary>
        /// Distinct values available for a field, given the selections on all the other fields.
        /// </summary>
        public List<string> GetOptions(Field field)
        {
            var values = Apply(field).SelectMany(a => GetValues(a, field))
                .Where(a => !string.IsNullOrEmpty(a))
                .Distinct();

            switch (field)
            {
                case Field.BoardId:
                    return values.OrderBy(a => { long l; return long.TryParse(a, out l) ? l : long.MaxValue; })
                        .ThenBy(a => a, StringComparer.Ordinal).ToList();
                case Field.Version:
                    // unparseable versions go last, as unparseable board ids do
                    return values.OrderBy(a => { Version v; return Version.TryParse(a, out v) ? 0 : 1; })
                        .ThenBy(a => { Version v; return Version.TryParse(a, out v) ? v : null; })
                        .ThenBy(a => a, StringComparer.Ordinal).ToList();
                default:
                    return values.OrderBy(a => a, StringComparer.OrdinalIgnoreCase).ToList();
            }
        }

        private IEnumerable<APFirmware.FirmwareInfo> Apply(Field? except)
        {
            IEnumerable<APFirmware.FirmwareInfo> list = _firmware;
            foreach (var item in _selected)
            {
                if (except.HasValue && item.Key == except.Value)
                    continue;
                var field = item.Key;
                var value = item.Value;
                list = list.Where(a => GetValues(a, field).Contains(value));
            }

            return list;
        }

        public static IEnumerable<string> GetValues(APFirmware.FirmwareInfo fw, Field field)
        {
            switch (field)
            {
                case Field.MavType:
                    return new[] { fw.MavType };
                case Field.VersionType:
                    return new[] { fw.MavFirmwareVersionType };
                case Field.Platform:
                    return new[] { fw.Platform };
                case Field.Version:
                    return new[] { fw.MavFirmwareVersion?.ToString() };
                case Field.Format:
                    return new[] { fw.Format };
                case Field.UsbId:
                    return fw.Usbid ?? Array.Empty<string>();
                case Field.BoardId:
                    return new[] { fw.BoardId.ToString() };
                case Field.BootloaderStr:
                    return fw.BootloaderStr ?? Array.Empty<string>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(field));
            }
        }
    }
}

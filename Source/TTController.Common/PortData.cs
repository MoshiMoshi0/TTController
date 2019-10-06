using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTController.Common
{
    public class PortData
    {
        public byte? PortId { get; set; }
        public byte? Speed { get; set; }
        public int? Rpm { get; set; }
        public float? Temperature { get; set; }

        private Dictionary<string, object> _additionalData;

        public object this[string key]
        {
            get
            {
                if (_additionalData != null && _additionalData.TryGetValue(key, out var value))
                    return value;
                return null;
            }
            set
            {
                if (_additionalData == null)
                    _additionalData = new Dictionary<string, object>();
                _additionalData[key] = value;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("[");
            sb.Append(string.Join(", ", GetType().GetProperties()
                .Where(p => Nullable.GetUnderlyingType(p.PropertyType) != null)
                .Where(p => p.GetValue(this) != null)
                .Select(p => $"{p.Name}: {p.GetValue(this)}")));

            if (_additionalData.Count > 0)
            {
                sb.Append(", ");
                sb.Append(string.Join(", ", _additionalData.Select(x => x.Key + ": " + x.Value)));
            }

            sb.Append("]");

            return sb.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TgwAssignment
{
    public class StringConverter
    {
        public delegate bool TypedConvertDelegate<T>(string value, out T result);
        private delegate bool UntypedConvertDelegate(string value, out object result);
        private readonly List<UntypedConvertDelegate> _converters = new List<UntypedConvertDelegate>();
        private static readonly Lazy<StringConverter> _default = new Lazy<StringConverter>(CreateDefault, true);

        public static StringConverter Default => _default.Value;

        private static StringConverter CreateDefault()
        {
            var stringConverter = new StringConverter();
     
            stringConverter.AddConverter<bool>(bool.TryParse);
            stringConverter.AddConverter((string value, out int result) => int.TryParse(value, NumberStyles.Integer, stringConverter.Culture, out result));
            stringConverter.AddConverter((string value, out long result) => long.TryParse(value, NumberStyles.Integer, stringConverter.Culture, out result));
            stringConverter.AddConverter((string value, out double result) => double.TryParse(value, NumberStyles.Number, stringConverter.Culture, out result));
            stringConverter.AddConverter((string value, out DateTime result) => DateTime.TryParse(value, stringConverter.Culture, DateTimeStyles.None, out result));
            stringConverter.AddConverter((string value, out DateTime result) => DateTime.TryParseExact(value, "dd:MM:yyyy", stringConverter.Culture, DateTimeStyles.None, out result));
            return stringConverter;
        }

        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        public void AddConverter<T>(TypedConvertDelegate<T> constructor)
        {
            _converters.Add(FromTryPattern<T>(constructor));
        }

        public bool TryConvert(string value, out object result)
        {
            if (this != Default)
            {
                if (Default.TryConvert(value, out result))
                    return true;
            }

            object tmp = null;
            bool anyMatch = _converters.Any(c => c(value, out tmp));
            result = tmp;
            return anyMatch;
        }

        private static UntypedConvertDelegate FromTryPattern<T>(TypedConvertDelegate<T> inner)
        {
            return (string value, out object result) => {
                T tmp;
                if (inner.Invoke(value, out tmp))
                {
                    result = tmp;
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            };
        }
    }
}

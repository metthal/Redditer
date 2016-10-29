using System;
using Windows.UI.Xaml.Data;

namespace Redditer.Converters
{
    public class IntToPluralStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var ivalue = (int)value;
            var format = (string)parameter;

            if (ivalue == 1 || ivalue == -1)
                return string.Format("{0} " + format, ivalue);

            return string.Format("{0} " + format + "s", ivalue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

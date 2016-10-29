using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Redditer.Utilities;

namespace Redditer.Converters
{
    public class DateTimeToRelativeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dateTime = (DateTime)value;
            var format = (string)parameter;
            return format == null ? dateTime.RelativeFromNow() : string.Format(format, dateTime.RelativeFromNow());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

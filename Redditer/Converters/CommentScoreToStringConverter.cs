using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Redditer.Converters
{
    public class CommentScoreToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var score = (int)value;
            if (score == 1 || score == -1)
                return string.Format("{0} point", score);
            else
                return string.Format("{0} points", score);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

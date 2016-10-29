using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Redditer.Converters
{
    public class CommentDepthToPaddingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new Thickness { Left = (int)value * 12 };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Redditer.Converters
{
    public class UriToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var uriStr = (string)value;
            if (uriStr == "" || uriStr == "self" || uriStr == "default" || uriStr == "nsfw" || uriStr == "image")
                return null;

            return new BitmapImage(new Uri(uriStr));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

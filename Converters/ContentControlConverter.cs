using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Melista.Converters
{
    public class ContentControlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var height = (double)value;
            int percent = (int)height * 80 / 100;
            
            int height2 = (percent / 9) * 9;

            int percent2 = (int)height * 80 / 100;
            int width = (percent2 / 9) * 16;
            if(height2 >= 5 && width >= 5)
            {
                return new Rect(0, 0, width - 5, height2 - 5);
            }
            else
            {
                return new Rect(0, 0, width, height2);
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

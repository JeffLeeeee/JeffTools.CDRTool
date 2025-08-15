using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace JeffTools.CDRTool.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Boolean)value) ? "Collapsed" : "Visible";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string visibility = value as string;
            switch (visibility)
            {
                case "Visible":
                    return false;
                case "Collapsed":
                    return true;
                case "Hidden":
                    return true;
                default:
                    return true;
            }

            //return false;
        }
    }
}

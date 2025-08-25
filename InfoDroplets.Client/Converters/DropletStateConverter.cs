using InfoDroplets.Utils.Enums;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WinRT;

namespace InfoDroplets.Client.Converters
{
    public class DropletStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string displayValue;
            switch ((int)value)
            {
                case (0):
                    displayValue = " - no fix";
                    break;

                case (1):
                    displayValue = " - time only";
                    break;

                case (2):
                    displayValue = "";
                    break;

                default:
                    throw new InvalidEnumArgumentException($"Value '{value.ToString()}' of enum is not known");
            }

            return displayValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return DependencyProperty.UnsetValue;
        }
    }
}

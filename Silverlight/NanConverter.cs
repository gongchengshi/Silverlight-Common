///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Data;
using System.Globalization;

namespace SEL.Silverlight.Converters
{
    public class DoubleNanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Double.IsNaN((double)value))
            {
                return parameter;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == parameter)
            {
                return Double.NaN;
            }

            return value;
        }
    }

    public class FloatNanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (float.IsNaN((float)value))
            {
                return parameter;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == parameter)
            {
                return float.NaN;
            }

            return value;
        }
    }
}

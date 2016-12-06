///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Media;
using System.Windows.Data;
using System.Globalization;

namespace SEL.Silverlight.Converters
{
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as SolidColorBrush).Color;
        }
    }

    public class ColorToOppositeSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            return new SolidColorBrush(GetOpposite((Color)value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetOpposite((value as SolidColorBrush).Color);
        }

        public static Color GetOpposite(Color color)
        {
            return Color.FromArgb(color.A,
                (byte)(byte.MaxValue ^ color.R),
                (byte)(byte.MaxValue ^ color.G),
                (byte)(byte.MaxValue ^ color.B));
        }
    }
}

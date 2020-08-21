using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace XCoderWpf.Common.Converaters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) => value != null && (Boolean)value ? Visibility.Visible : Visibility.Collapsed;

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

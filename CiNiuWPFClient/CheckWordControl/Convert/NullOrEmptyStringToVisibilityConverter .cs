using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CheckWordControl
{
    public class NullOrEmptyStringToVisibilityConverter : IValueConverter
    {

        public NullOrEmptyStringToVisibilityConverter()
        {

            NullOrEmpty = Visibility.Collapsed;
            NotNullOrEmpty = Visibility.Visible;

        }

        public Visibility NullOrEmpty { get; set; }

        public Visibility NotNullOrEmpty { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string strValue = value == null ? string.Empty : value.ToString();
            return string.IsNullOrEmpty(strValue) ? NullOrEmpty : NotNullOrEmpty;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}

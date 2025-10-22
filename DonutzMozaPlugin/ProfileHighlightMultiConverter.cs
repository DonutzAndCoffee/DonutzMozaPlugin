using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DonutzMozaPlugin
{
    public class ProfileHighlightMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            SimHub.Logging.Current.Info("[ProfileHighlightMultiConverter] Convert called");
            string profileKey = values[0] as string;
            string activeKey = values[1] as string;

            if (string.IsNullOrEmpty(profileKey) || string.IsNullOrEmpty(activeKey))
                return null;

            return profileKey == activeKey
                ? new SolidColorBrush(Color.FromArgb(128, 160, 255, 160))
                : null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProfileHighlightMatchOnlyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string profileKey = values[0] as string;
            string activeKey = values[1] as string;

            if (string.IsNullOrEmpty(profileKey) || string.IsNullOrEmpty(activeKey))
                return false;

            return profileKey == activeKey;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}


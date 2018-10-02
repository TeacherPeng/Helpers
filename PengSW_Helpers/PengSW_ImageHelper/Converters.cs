using System;
using System.Globalization;
using System.Windows.Data;
using static PengSW.ImageHelper.ImageHelper;

namespace PengSW.ImageHelper
{
    public class BytesToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return CreateBitmapImage((byte[])value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

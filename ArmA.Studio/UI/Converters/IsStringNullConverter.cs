﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ArmA.Studio.UI.Converters
{
    public class IsStringNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag;
            if (parameter is string)
            {
                bool.TryParse(parameter as string, out flag);
            }
            else if (parameter is bool)
            {
                flag = (bool)parameter;
            }
            else
            {
                flag = false;
            }
            return flag ? !string.IsNullOrWhiteSpace(value as string) : string.IsNullOrWhiteSpace(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

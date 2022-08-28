using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slidecrew.ValueConverters
{
    public static class ArmLengths
    {
        public static float Hour = 7f;
        public static float Minute = 10f;
        public static float center = 11f;

        public static float GetHourX(double angle)
        {
            return center + Hour * (float)Math.Sin((Math.PI / 180) * angle);
        }

        public static float GetHourY(double angle)
        {
            return center - Hour * (float)Math.Cos((Math.PI / 180) * angle);
        }

        public static float GetMinuteX(double angle)
        {
            return center + Minute * (float)Math.Sin((Math.PI / 180) * angle);
        }

        public static float GetMinuteY(double angle)
        {
            return center - Minute * (float)Math.Cos((Math.PI / 180) * angle);
        }
    }

    public class HourToXYConverter : IValueConverter
    {
        /// <summary>
        /// Assume time string 16:00;
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double angle = 0;
            if (value.GetType() == typeof(string))
            {
                if (string.IsNullOrEmpty((string)value))
                    return ArmLengths.GetHourX(angle);

                string[] splt = ((string)value).Split(':');
                float parsed = float.Parse(splt[0]);
                if (parsed > 12f) parsed -= 12f;

                angle = 360f / 12f * parsed;
            }

            if (value.GetType() == typeof(DateTime))
            {

            }

            return new Point(ArmLengths.GetHourX(angle), ArmLengths.GetHourY(angle));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class MinuteToXYConverter : IValueConverter
    {
        /// <summary>
        /// Assume time string 16:00;
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double angle = 0;
            if (value.GetType() == typeof(string))
            {
                if (string.IsNullOrEmpty((string)value))
                    return ArmLengths.GetMinuteX(angle);

                string[] splt = ((string)value).Split(':');
                float parsed = float.Parse(splt[1]);
                angle = 360f / 59f * parsed;
            }

            return new Point(ArmLengths.GetMinuteX(angle), ArmLengths.GetMinuteY(angle));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

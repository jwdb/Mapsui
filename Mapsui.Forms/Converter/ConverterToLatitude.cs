namespace Mapsui.Forms.Converter
{
	using System;
	using System.Globalization;
	using Xamarin.Forms;
	using Xamarin.Forms.Maps;

	/// <summary>
	/// Converter to latitude.
	/// </summary>
	public class ConverterToLatitude : IValueConverter
	{
		/// <param name="value">Value to convert.</param>
		/// <param name="targetType">Type of value to convert.</param>
		/// <param name="parameter">Parameter for conversion.</param>
		/// <param name="culture">Culture to use while converting.</param>
		/// <summary>
		/// Convert the specified value, targetType, parameter and culture.
		/// </summary>
		/// <returns>Converted object.</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Position pos = (Position)value;

			if (pos == null)
			{
				return string.Empty;
			}

			if (parameter is string)
			{
				return string.Format((string)parameter, Converter.NumberToLatitude(pos.Latitude));
			}
			else
			{
				return Converter.NumberToLatitude(pos.Latitude);
			}
		}

		/// <param name="value">Value to convert back.</param>
		/// <param name="targetType">Type of value to convert.</param>
		/// <param name="parameter">Parameter for conversion.</param>
		/// <param name="culture">Culture to use while converting.</param>
		/// <summary>
		/// Converts the back.
		/// </summary>
		/// <returns>BackConverted object.</returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}

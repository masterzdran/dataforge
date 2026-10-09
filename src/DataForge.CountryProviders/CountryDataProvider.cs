using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DataForge
{
    /// <summary>
    /// Base class for built-in and custom <see cref="ICountryDataProvider"/> implementations.
    /// Encapsulates shared picking, format expansion and number generation.
    /// </summary>
    public abstract class CountryDataProvider : ICountryDataProvider
    {
        private readonly string[] _maleFirstNames;
        private readonly string[] _femaleFirstNames;
        private readonly string[] _lastNames;
        private readonly string[] _cities;
        private readonly string[] _streetNames;
        private readonly string[] _companySuffixes;
        private readonly string _streetFormat;
        private readonly string _postalCodeFormat;
        private readonly string _phoneNumberFormat;

        /// <summary>Initializes a new instance of the <see cref="CountryDataProvider"/> class.</summary>
        /// <param name="country">Country served by this provider.</param>
        /// <param name="maleFirstNames">Non-empty male first name pool.</param>
        /// <param name="femaleFirstNames">Non-empty female first name pool.</param>
        /// <param name="lastNames">Non-empty last name pool.</param>
        /// <param name="cities">Non-empty city pool.</param>
        /// <param name="streetNames">Non-empty street name pool.</param>
        /// <param name="streetFormat">Street format template; <c>{0}</c> is replaced by a street name.</param>
        /// <param name="companySuffixes">Non-empty company suffix pool.</param>
        /// <param name="postalCodeFormat">Postal code template where <c>#</c> is a random digit.</param>
        /// <param name="phoneNumberFormat">Phone number template where <c>#</c> is a random digit.</param>
        protected CountryDataProvider(
            Country country,
            string[] maleFirstNames,
            string[] femaleFirstNames,
            string[] lastNames,
            string[] cities,
            string[] streetNames,
            string streetFormat,
            string[] companySuffixes,
            string postalCodeFormat,
            string phoneNumberFormat)
        {
            Country = country;
            _maleFirstNames = RequirePool(maleFirstNames, nameof(maleFirstNames));
            _femaleFirstNames = RequirePool(femaleFirstNames, nameof(femaleFirstNames));
            _lastNames = RequirePool(lastNames, nameof(lastNames));
            _cities = RequirePool(cities, nameof(cities));
            _streetNames = RequirePool(streetNames, nameof(streetNames));
            _companySuffixes = RequirePool(companySuffixes, nameof(companySuffixes));
            _streetFormat = RequireFormat(streetFormat, nameof(streetFormat));
            _postalCodeFormat = RequireFormat(postalCodeFormat, nameof(postalCodeFormat));
            _phoneNumberFormat = RequireFormat(phoneNumberFormat, nameof(phoneNumberFormat));
        }

        /// <inheritdoc />
        public Country Country { get; }

        /// <inheritdoc />
        public string GetFirstName(IGenerationContext context, Gender gender)
        {
            return Pick(context, gender == Gender.Male ? _maleFirstNames : _femaleFirstNames);
        }

        /// <inheritdoc />
        public string GetLastName(IGenerationContext context)
        {
            return Pick(context, _lastNames);
        }

        /// <inheritdoc />
        public string GetCity(IGenerationContext context)
        {
            return Pick(context, _cities);
        }

        /// <inheritdoc />
        public string GetStreet(IGenerationContext context)
        {
            return string.Format(CultureInfo.InvariantCulture, _streetFormat, Pick(context, _streetNames));
        }

        /// <inheritdoc />
        public virtual string GetStreetNumber(IGenerationContext context)
        {
            var number = context.Next(1, 250).ToString(CultureInfo.InvariantCulture);
            return context.NextBool()
                ? number + (char)('A' + context.Next(0, 26))
                : number;
        }

        /// <inheritdoc />
        public virtual string GetPostalCode(IGenerationContext context)
        {
            return Expand(context, _postalCodeFormat);
        }

        /// <inheritdoc />
        public virtual string GetPhoneNumber(IGenerationContext context)
        {
            return Expand(context, _phoneNumberFormat);
        }

        /// <inheritdoc />
        public virtual string GetCompanyName(IGenerationContext context)
        {
            return Pick(context, _lastNames) + " " + Pick(context, _companySuffixes);
        }

        /// <inheritdoc />
        public abstract string GetTaxIdentifier(IGenerationContext context);

        /// <summary>Returns an element of <paramref name="items"/> chosen by the context.</summary>
        /// <param name="context">Generation context.</param>
        /// <param name="items">Non-empty pool.</param>
        /// <returns>A pseudo-random element.</returns>
        protected static string Pick(IGenerationContext context, string[] items)
        {
            return items[context.Next(0, items.Length)];
        }

        /// <summary>Returns an array of pseudo-random decimal digits.</summary>
        /// <param name="context">Generation context.</param>
        /// <param name="count">Number of digits.</param>
        /// <returns>Array of digits (0-9).</returns>
        protected static int[] RandomDigits(IGenerationContext context, int count)
        {
            var digits = new int[count];
            for (var i = 0; i < count; i++)
            {
                digits[i] = context.Next(0, 10);
            }

            return digits;
        }

        /// <summary>Concatenates digits into a string.</summary>
        /// <param name="digits">Digits to concatenate.</param>
        /// <returns>The concatenated digit string.</returns>
        protected static string ToDigits(int[] digits)
        {
            var builder = new StringBuilder(digits.Length);
            for (var i = 0; i < digits.Length; i++)
            {
                builder.Append((char)('0' + digits[i]));
            }

            return builder.ToString();
        }

        /// <summary>Expands a template by replacing every <c>#</c> with a random decimal digit.</summary>
        /// <param name="context">Generation context.</param>
        /// <param name="format">Template.</param>
        /// <returns>The expanded value.</returns>
        protected static string Expand(IGenerationContext context, string format)
        {
            var builder = new StringBuilder(format.Length);
            for (var i = 0; i < format.Length; i++)
            {
                var character = format[i];
                builder.Append(character == '#' ? (char)('0' + context.Next(0, 10)) : character);
            }

            return builder.ToString();
        }

        private static string[] RequirePool(string[] pool, string parameterName)
        {
            if (pool == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (pool.Length == 0)
            {
                throw new ArgumentException("The pool must contain at least one value.", parameterName);
            }

            return pool;
        }

        private static string RequireFormat(string format, string parameterName)
        {
            if (string.IsNullOrEmpty(format))
            {
                throw new ArgumentException("The format must not be null or empty.", parameterName);
            }

            return format;
        }
    }
}

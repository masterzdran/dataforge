using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace DataForge.UnitTests
{
    public sealed class TaxIdentifierTests
    {
        [Theory]
        [InlineData(Country.PT)]
        [InlineData(Country.ES)]
        [InlineData(Country.BR)]
        [InlineData(Country.US)]
        public void PersonTaxIdentifier_FollowsCountryRules(Country country)
        {
            var sample = Forge.ForCountry(country)
                .WithSeed(31)
                .Register<PersonTaxIdentifierProbeGenerator>()
                .Create<PersonTaxIdentifierSample>();

            switch (country)
            {
                case Country.PT:
                    IsValidPortugueseTaxNumber(sample.Value).Should().BeTrue();
                    break;
                case Country.ES:
                    IsValidSpanishDni(sample.Value).Should().BeTrue();
                    break;
                case Country.BR:
                    IsValidCpf(sample.Value).Should().BeTrue();
                    break;
                default:
                    sample.Value.Should().MatchRegex("^9[0-9]{2}-[0-9]{2}-[0-9]{4}$");
                    break;
            }
        }

        [Fact]
        public void PortugueseCompanyTaxIdentifier_IsValid()
        {
            var companies = Forge.ForCountry(Country.PT).WithSeed(32).Create<Company>(50);

            companies.Should().OnlyContain(company => IsValidPortugueseTaxNumber(company.TaxIdentifier));
        }

        [Fact]
        public void SpanishCompanyTaxIdentifier_IsValidCif()
        {
            var companies = Forge.ForCountry(Country.ES).WithSeed(33).Create<Company>(50);

            companies.Should().OnlyContain(company => IsValidCif(company.TaxIdentifier));
        }

        [Fact]
        public void BrazilianCompanyTaxIdentifier_IsValidCnpj()
        {
            var companies = Forge.ForCountry(Country.BR).WithSeed(34).Create<Company>(50);

            companies.Should().OnlyContain(company => IsValidCnpj(company.TaxIdentifier));
        }

        [Fact]
        public void FrenchCompanyTaxIdentifier_IsValidLuhn()
        {
            var companies = Forge.ForCountry(Country.FR).WithSeed(35).Create<Company>(50);

            companies.Should().OnlyContain(company => IsValidLuhn(company.TaxIdentifier));
        }

        [Fact]
        public void GermanAndUnitedKingdomAndUnitedStatesIdentifiers_FollowFormat()
        {
            Forge.ForCountry(Country.DE).WithSeed(36).Create<Company>(20).Should()
                .OnlyContain(company => Regex.IsMatch(company.TaxIdentifier, "^DE[0-9]{9}$"));

            Forge.ForCountry(Country.UK).WithSeed(37).Create<Company>(20).Should()
                .OnlyContain(company => Regex.IsMatch(company.TaxIdentifier, "^(SC|NI|OC)[0-9]{6}$"));

            Forge.ForCountry(Country.US).WithSeed(38).Create<Company>(20).Should()
                .OnlyContain(company => Regex.IsMatch(company.TaxIdentifier, "^[0-9]{2}-[0-9]{7}$"));
        }

        private static bool IsValidPortugueseTaxNumber(string value)
        {
            if (value == null || value.Length != 9 || !IsAllDigits(value))
            {
                return false;
            }

            var sum = 0;
            for (var i = 0; i < 8; i++)
            {
                sum += (value[i] - '0') * (9 - i);
            }

            var check = 11 - (sum % 11);
            if (check > 9)
            {
                check = 0;
            }

            return check == value[8] - '0';
        }

        private static bool IsValidSpanishDni(string value)
        {
            if (value == null || value.Length != 9)
            {
                return false;
            }

            var number = int.Parse(value.Substring(0, 8), System.Globalization.CultureInfo.InvariantCulture);
            const string letters = "TRWAGMYFPDXBNJZSQVHLCKE";
            return letters[number % 23] == value[8];
        }

        private static bool IsValidCif(string value)
        {
            if (value == null || value.Length != 9)
            {
                return false;
            }

            var body = new int[7];
            for (var i = 0; i < 7; i++)
            {
                body[i] = value[i + 1] - '0';
            }

            var sum = 0;
            for (var i = 0; i < 7; i++)
            {
                if (i % 2 == 0)
                {
                    var doubled = body[i] * 2;
                    sum += (doubled / 10) + (doubled % 10);
                }
                else
                {
                    sum += body[i];
                }
            }

            var control = (10 - (sum % 10)) % 10;
            var expected = "KPQS".IndexOf(value[0]) >= 0 ? "JABCDEFGHI"[control] : (char)('0' + control);
            return expected == value[8];
        }

        private static bool IsValidCpf(string value)
        {
            var digits = OnlyDigits(value);
            if (digits.Length != 11)
            {
                return false;
            }

            return Mod11(digits, 9, 10) == digits[9] - '0'
                && Mod11(digits, 10, 11) == digits[10] - '0';
        }

        private static bool IsValidCnpj(string value)
        {
            var digits = OnlyDigits(value);
            if (digits.Length != 14)
            {
                return false;
            }

            return Mod11(digits, 12, 5) == digits[12] - '0'
                && Mod11(digits, 13, 6) == digits[13] - '0';
        }

        private static bool IsValidLuhn(string value)
        {
            if (value == null || value.Length != 9 || !IsAllDigits(value))
            {
                return false;
            }

            var sum = 0;

            // ponytail: rightmost digit is the check digit and is never doubled; toggle from false.
            var doubleDigit = false;
            for (var i = value.Length - 1; i >= 0; i--)
            {
                var digit = value[i] - '0';
                if (doubleDigit)
                {
                    digit *= 2;
                    if (digit > 9)
                    {
                        digit -= 9;
                    }
                }

                sum += digit;
                doubleDigit = !doubleDigit;
            }

            return sum % 10 == 0;
        }

        private static int Mod11(string digits, int length, int weightFrom)
        {
            var sum = 0;
            var weight = weightFrom;
            for (var i = 0; i < length; i++)
            {
                sum += (digits[i] - '0') * weight;
                weight--;
                if (weight < 2)
                {
                    weight = 9;
                }
            }

            var remainder = sum % 11;
            return remainder < 2 ? 0 : 11 - remainder;
        }

        private static string OnlyDigits(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var builder = new System.Text.StringBuilder(value.Length);
            foreach (var character in value)
            {
                if (character >= '0' && character <= '9')
                {
                    builder.Append(character);
                }
            }

            return builder.ToString();
        }

        private static bool IsAllDigits(string value)
        {
            foreach (var character in value)
            {
                if (character < '0' || character > '9')
                {
                    return false;
                }
            }

            return true;
        }
    }
}

using System;

namespace DataForger
{
    /// <summary>
    /// Pure check-digit algorithms used by the country providers.
    /// </summary>
    internal static class CheckDigits
    {
        private const string DniLetters = "TRWAGMYFPDXBNJZSQVHLCKE";

        /// <summary>Computes a Portuguese tax number (NIF/NIPC) check digit from eight leading digits.</summary>
        /// <param name="digits">Exactly eight digits, most significant first.</param>
        /// <returns>The check digit (0-9).</returns>
        internal static int PortugueseTaxNumber(int[] digits)
        {
            if (digits == null)
            {
                throw new ArgumentNullException(nameof(digits));
            }

            var sum = 0;
            for (var i = 0; i < 8; i++)
            {
                sum += digits[i] * (9 - i);
            }

            var check = 11 - (sum % 11);
            return check > 9 ? 0 : check;
        }

        /// <summary>Computes the Spanish DNI/NIF control letter for an eight-digit number.</summary>
        /// <param name="number">Number in the range 0..99999999.</param>
        /// <returns>The control letter.</returns>
        internal static char SpanishDniLetter(int number)
        {
            return DniLetters[number % 23];
        }

        /// <summary>Computes the Spanish CIF control character.</summary>
        /// <param name="typeLetter">The leading organization letter (A-H, J, N, P, Q, R, S, U, V, W).</param>
        /// <param name="body">Exactly seven digits.</param>
        /// <returns>The control digit or letter.</returns>
        internal static char SpanishCifControl(char typeLetter, int[] body)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            var sum = 0;
            for (var i = 0; i < 7; i++)
            {
                var digit = body[i];
                if (i % 2 == 0)
                {
                    var doubled = digit * 2;
                    sum += (doubled / 10) + (doubled % 10);
                }
                else
                {
                    sum += digit;
                }
            }

            var control = (10 - (sum % 10)) % 10;
            var isLetter = "KPQS".IndexOf(typeLetter) >= 0;
            return isLetter ? "JABCDEFGHI"[control] : (char)('0' + control);
        }

        /// <summary>Computes the two Brazilian CPF check digits from nine base digits.</summary>
        /// <param name="baseDigits">Exactly nine digits.</param>
        /// <returns>Two check digits.</returns>
        internal static int[] Cpf(int[] baseDigits)
        {
            if (baseDigits == null)
            {
                throw new ArgumentNullException(nameof(baseDigits));
            }

            var first = Mod11(baseDigits, 10);
            var extended = new int[10];
            Array.Copy(baseDigits, extended, 9);
            extended[9] = first;
            var second = Mod11(extended, 11);
            return new[] { first, second };
        }

        /// <summary>Computes the two Brazilian CNPJ check digits from twelve base digits.</summary>
        /// <param name="baseDigits">Exactly twelve digits.</param>
        /// <returns>Two check digits.</returns>
        internal static int[] Cnpj(int[] baseDigits)
        {
            if (baseDigits == null)
            {
                throw new ArgumentNullException(nameof(baseDigits));
            }

            var first = Mod11(baseDigits, 5);
            var extended = new int[13];
            Array.Copy(baseDigits, extended, 12);
            extended[12] = first;
            var second = Mod11(extended, 6);
            return new[] { first, second };
        }

        /// <summary>Computes the Luhn check digit for a numeric string.</summary>
        /// <param name="value">Leading digits, without the check digit.</param>
        /// <returns>The Luhn check digit (0-9).</returns>
        internal static int Luhn(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var sum = 0;
            var doubleDigit = true;
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

            return (10 - (sum % 10)) % 10;
        }

        /// <summary>Computes a modulo-11 check digit using a descending weight sequence starting at <paramref name="weightFrom"/>.</summary>
        /// <param name="digits">Digits to weigh.</param>
        /// <param name="weightFrom">Weight for the first digit; the weight wraps back to 9 below 2.</param>
        /// <returns>The check digit.</returns>
        internal static int Mod11(int[] digits, int weightFrom)
        {
            var sum = 0;
            var weight = weightFrom;
            for (var i = 0; i < digits.Length; i++)
            {
                sum += digits[i] * weight;
                weight--;
                if (weight < 2)
                {
                    weight = 9;
                }
            }

            var remainder = sum % 11;
            return remainder < 2 ? 0 : 11 - remainder;
        }
    }
}

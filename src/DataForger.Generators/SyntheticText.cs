using System;
using System.Globalization;
using System.Text;

namespace DataForger
{
    /// <summary>
    /// Builds e-mail local parts and sub-domains that are safe, deterministic and clearly synthetic.
    /// Only RFC 2606 reserved domains are used, so generated addresses can never route to real mailboxes.
    /// </summary>
    internal static class SyntheticText
    {
        private static readonly string[] EmailDomains = { "example.com", "example.net", "example.org" };

        internal static string PersonEmail(IGenerationContext context, string firstName, string lastName)
        {
            return Slug(firstName) + "." + Slug(lastName) + "@" + context.Pick(EmailDomains);
        }

        internal static string CompanyEmail(IGenerationContext context, string companyName)
        {
            return "info@" + Slug(companyName) + "." + context.Pick(EmailDomains);
        }

        internal static string Slug(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "x";
            }

            var normalized = value.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);
            for (var i = 0; i < normalized.Length; i++)
            {
                var character = normalized[i];
                if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                if (character >= 'A' && character <= 'Z')
                {
                    builder.Append((char)(character + 32));
                }
                else if ((character >= 'a' && character <= 'z') || (character >= '0' && character <= '9'))
                {
                    builder.Append(character);
                }
            }

            return builder.Length == 0 ? "x" : builder.ToString();
        }
    }
}

using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Mixtape.Application.Mixtapes
{
    internal static class Slugs
    {
        // "Para el carro" -> "para-el-carro-3fa9c21b": lower case, without accents or symbols,
        // plus random letters so nobody can guess the address.
        public static string Create(string text)
        {
            string plain = new(text.Normalize(NormalizationForm.FormD)
                .Where(x => CharUnicodeInfo.GetUnicodeCategory(x) != UnicodeCategory.NonSpacingMark)
                .ToArray());

            string words = Regex.Replace(plain.ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-');
            string start = words.Length > 60 ? words[..60].Trim('-') : words;
            string random = Convert.ToHexString(RandomNumberGenerator.GetBytes(4)).ToLowerInvariant();

            return start.Length == 0 ? random : $"{start}-{random}";
        }
    }
}

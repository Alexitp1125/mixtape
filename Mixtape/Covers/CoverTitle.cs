using System.Net;
using System.Text.RegularExpressions;

namespace Mixtape.Covers
{
    // Writes a title on a generic cover. The SVG has a mark where the text goes:
    //   <!--title #FFF6FB-->  the title of a song, in one or two lines, in that color;
    //   <!--label-->          the name of a mixtape, on the label of the cassette.
    public static class CoverTitle
    {
        public static string Apply(string svg, string title)
        {
            string text = title.Trim().ToUpperInvariant();

            return Regex
                .Replace(svg, "<!--title (#[0-9A-Fa-f]{6})-->", x => SongTitle(text, x.Groups[1].Value))
                .Replace("<!--label-->", TapeLabel(text));
        }

        // One line, or two when the title is long. The longer the line, the smaller the letters.
        private static string SongTitle(string text, string color)
        {
            string[] rows = Split(text);
            int size = Math.Min(76, 860 / Math.Max(1, rows.Max(x => x.Length)));
            int top = rows.Length == 1 ? 668 : 668 - size * 55 / 100;

            return string.Concat(rows.Select((row, i) =>
                $"<text x=\"400\" y=\"{top + i * size * 110 / 100}\" text-anchor=\"middle\" font-family=\"Jost, sans-serif\" font-weight=\"300\" " +
                $"font-size=\"{size}\" letter-spacing=\"{size * 14 / 100}\" fill=\"{color}\">{WebUtility.HtmlEncode(row)}</text>"));
        }

        private static string TapeLabel(string text)
        {
            int size = Math.Min(40, 630 / Math.Max(1, text.Length));

            return $"<text x=\"400\" y=\"304\" text-anchor=\"middle\" font-family=\"Jost, sans-serif\" font-weight=\"400\" " +
                $"font-size=\"{size}\" letter-spacing=\"{size / 10}\" fill=\"#3A2766\">{WebUtility.HtmlEncode(text)}</text>";
        }

        // Short titles stay in one line; long ones are cut at the space nearest to the middle.
        private static string[] Split(string text)
        {
            int[] spaces = Enumerable.Range(0, text.Length).Where(i => text[i] == ' ').ToArray();

            if (text.Length <= 13 || spaces.Length == 0)
            {
                return [text];
            }

            int cut = spaces.MinBy(i => Math.Abs(i - text.Length / 2));

            return [text[..cut], text[(cut + 1)..]];
        }
    }
}

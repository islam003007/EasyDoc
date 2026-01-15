using System.Text;

namespace EasyDoc.Infrastructure.Services.DataNormalization;

/* * Arabic Normalization Logic
 * * Implements the standard Arabic normalization rules (Alef/Teh Marbuta unification, diacritic removal)
 * as defined in the ArabicAnalyzer component of Apache Lucene.Net.
 * * This code is a derived work and should be treated as being under the Apache License 2.0.
 * Source Logic: Lucene.Net.Analysis.Ar.ArabicNormalizer
 */

/// <summary>
/// Normalizer for Arabic.
/// <para/>
/// Normalization is done in-place for efficiency, operating on a termbuffer.
/// <para/>
/// Normalization is defined as:
/// <list type="bullet">
///     <item><description> Normalization of hamza with alef seat to a bare alef.</description></item>
///     <item><description> Normalization of teh marbuta to heh</description></item>
///     <item><description> Normalization of dotless yeh (alef maksura) to yeh.</description></item>
///     <item><description> Removal of Arabic diacritics (the harakat)</description></item>
///     <item><description> Removal of tatweel (stretching character).</description></item>
/// </list>
/// </summary>
internal static class ArabicNormalizer
{
    private const char ALEF = '\u0627';
    private const char ALEF_MADDA = '\u0622';
    private const char ALEF_HAMZA_ABOVE = '\u0623';
    private const char ALEF_HAMZA_BELOW = '\u0625';

    private const char YEH = '\u064A';
    private const char DOTLESS_YEH = '\u0649';

    private const char TEH_MARBUTA = '\u0629';
    private const char HEH = '\u0647';

    private const char TATWEEL = '\u0640';

    private const char FATHATAN = '\u064B';
    private const char DAMMATAN = '\u064C';
    private const char KASRATAN = '\u064D';
    private const char FATHA = '\u064E';
    private const char DAMMA = '\u064F';
    private const char KASRA = '\u0650';
    private const char SHADDA = '\u0651';
    private const char SUKUN = '\u0652';

    /// <summary>
    /// Normalize an input buffer of Arabic text
    /// </summary>
    /// <param name="s"> input buffer </param>
    public static string Normalize(string str)
    {
        StringBuilder s = new StringBuilder(str);

        for (int i = 0; i < s.Length; i++)
        {
            switch (s[i])
            {
                case ALEF_MADDA:
                case ALEF_HAMZA_ABOVE:
                case ALEF_HAMZA_BELOW:
                    s[i] = ALEF;
                    break;
                case DOTLESS_YEH:
                    s[i] = YEH;
                    break;
                case TEH_MARBUTA:
                    s[i] = HEH;
                    break;
                case TATWEEL:
                case KASRATAN:
                case DAMMATAN:
                case FATHATAN:
                case FATHA:
                case DAMMA:
                case KASRA:
                case SHADDA:
                case SUKUN:
                    s.Remove(i, 1);
                    i--;
                    break;
                default:
                    break;
            }
        }
        return s.ToString();
    }
}
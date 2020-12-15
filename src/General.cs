using DBManager.DBASES;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wordpress2markdown
{
    public static class General
    {
          public static MySQLClass db;

          public static DialogResult Mes(string descr, MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons butt = MessageBoxButtons.OK)
          {
              if (descr.Length > 0)
                  return MessageBox.Show(descr, Application.ProductName, butt, icon);
              else
                  return DialogResult.OK;

          }
          public static bool ToBool(this object value)
          {
              bool result = false;
              if (value != null)
                  bool.TryParse(value.ToString(), out result);

              return result;
          }

          public static string MakeSafeFilename(this string filename)
          {
              Regex pattern = new Regex("[" + string.Join(",", Path.GetInvalidFileNameChars()) + "]");

              return pattern.Replace(filename, "-");
          }

          public static string MakeFilenameOnlyAlphaNumeric(this string filename)
          { //AlphaNumeric space and dash
             return Regex.Replace(filename, @"[^\p{IsGreek}a-zA-Z0-9 -]", string.Empty);
          }

          public static string ShorterExact(this string source, int maxLength)
          {
              if ((!string.IsNullOrEmpty(source)) && (source.Length > maxLength))
                  return source.Substring(0, maxLength);
              else
                  return source;
          }

          public static bool StartsWithLetter(this string source, int maxLength)
          {
              if ((!string.IsNullOrEmpty(source)))
                  return char.IsLetter(source[0]);
              else
                  return false;
          }
        
          public static string Greek2Greeklish(this string source)
          {
              var originalChar = new List<char> { 'ς', 'α', 'β', 'γ', 'δ', 'ε', 'ζ', 'η', 'θ', 'ι', 'κ', 'λ', 'μ', 'ν', 'ξ', 'ο', 'π', 'ρ', 'σ', 'τ', 'υ', 'φ', 'χ', 'ψ', 'ω', 'ά', 'έ', 'ή', 'ί', 'ό', 'ύ', 'ώ' };
              var replaceWith = new List<char> { 's', 'a', 'b', 'g', 'd', 'e', 'z', 'h', '8', 'i', 'k', 'l', 'm', 'n', '3', 'o', 'p', 'r', 's', 't', 'u', 'f', 'x', 'c', 'w', 'a', 'e', 'h', 'i', 'o', 'u', 'w' };
              originalChar.ForEach(x => source = source.Replace(x, replaceWith[originalChar.IndexOf(x)]));

              return source;
          }
    }
}

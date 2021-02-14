using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Documents;

namespace EngBox
{
    public static class Utils
    {
        public static readonly char[] __PSEUDODIGITS = { '-', '+' };


        /// <summary>
        /// Converts units to Run with superscripts. All numbers including signs are converted to superscript
        /// </summary>
        /// <param name="text">unit text abbreviation </param>
        /// <returns>Run output</returns>
        public static ObservableCollection<Inline> UnitsToSuperScripts(string text)
        {
            List<Run> result = new List<Run>();

            int i = 0;
            int j = 0;
            result.Add(new Run());

            while (i != text.Length)
            {
                if (!IsNumeric(text[i]))
                {
                    result[j].Text += text[i];
                    i++;
                }
                else
                {
                    Run r = new Run();
                    r.BaselineAlignment = System.Windows.BaselineAlignment.Superscript;
                    
                    while (i < text.Length && IsNumeric(text[i]))
                    {
                        r.Text += text[i];
                        i++;
                    }
                    result.Add(r);
                    j++;
                    if (i < text.Length)
                    {
                        result.Add(new Run());
                        j++;
                    }
                    
                }

            }

            ObservableCollection<Inline> list = new ObservableCollection<Inline>();
            for (int k = 0; k < result.Count; k++)
            {
                Inline inl;
                inl = result[k];
                list.Add(inl);
            }
            return list;
        }

        private static bool IsNumeric(char c)
        {
            if (Char.IsDigit(c)) return true;
            for (int i = 0; i < __PSEUDODIGITS.Length; i++)
            {
                if (c == __PSEUDODIGITS[i]) return true;
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace FileComparison
{
    class Function
    {
        public static bool ifcontain(string s, ArrayList arr)
        {
            bool aa = false;
            for (int i = 0; i < arr.Count; i++)
            {
                if (Regex.IsMatch(s, arr[i].ToString(), RegexOptions.IgnoreCase))
                {
                    aa = true;
                    break;
                }
            }
            return aa;
        }

        public static bool linecontain(int l, ArrayList arr)
        {
            bool aa = false;
            for (int i = 0; i < arr.Count; i++)
            {
                if (l >= int.Parse(arr[i].ToString()) && l <= int.Parse(arr[i + 1].ToString()))
                {
                    aa = true;
                    break;
                }
                i++;
            }
            return aa;
        }

        public class fileContent
        {
            public string fContent;
            public string lineNo;
        }
    }
}

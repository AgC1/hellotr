using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.ObjectModel;

namespace FileComparison
{
    class ExactlyMatch
    {
        public static void Exactly_Match(Collection<Function.fileContent> col1, Collection<Function.fileContent> col2, bool order)
        {

            if (order)
            {
                for (int i = 0; i < col1.Count; i++)
                {
                    int j = i;
                    if (j < col2.Count)
                    {
                        if (col1[i].fContent != col2[i].fContent)
                        {
                            if (col1[i].fContent.Length > 40)
                                col1[i].fContent = col1[i].fContent.Substring(0, 39) + "...";
                            if (col2[i].fContent.Length > 40)
                                col2[i].fContent = col2[i].fContent.Substring(0, 39) + "...";
                            System.Console.WriteLine("{0,-20}{1,-50}{2,-50}", "Line " + col1[i].lineNo.ToString(), col1[i].fContent, col2[i].fContent);
                        }
                    }
                    else
                    {
                        if (col1[i].fContent.Length > 40)
                            col1[i].fContent = col1[i].fContent.Substring(0, 39) + "...";
                        System.Console.WriteLine("{0,-20}{1,-50}{2,-50}", "Line " + col1[i].lineNo, col1[i].fContent, "missing");
                    }
                }
                return;
            }
            else
            {

                for (int i = 0; i < col2.Count; i++)
                {
                    int j = i;
                    if (j < col1.Count)
                    {
                        if (col1[i].fContent != col2[i].fContent)
                        {
                            if (col1[i].fContent.Length > 40)
                                col1[i].fContent = col1[i].fContent.Substring(0, 39) + "...";
                            if (col2[i].fContent.Length > 40)
                                col2[i].fContent = col2[i].fContent.Substring(0, 39) + "...";
                            System.Console.WriteLine("{0,-20}{1,-50}{2,-50}", "Line " + col2[i].lineNo.ToString(), col1[i].fContent, col2[i].fContent);
                        }
                    }
                    else
                    {
                        if (col2[i].fContent.Length > 40)
                            col2[i].fContent = col2[i].fContent.Substring(0, 39) + "...";
                        System.Console.WriteLine("{0,-20}{1,-50}{2,-50}", "Line " + col2[i].lineNo.ToString(), "missing", col2[i].fContent);
                    }
                }

            }
            return;
        }
    }
}

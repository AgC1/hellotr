using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace FileComparison
{
    class Program
    {
        static void printUsage()
        {
            System.Console.WriteLine("\r\n***  Usage  ***:");
            System.Console.WriteLine("File Comparison Tool <Specified file1 complete path> < Specified file2 complete path > <Show details?>(y/n) <Exactly match?>(y/n) <Option> <Configure File complete path>");
            System.Console.WriteLine(@"EXAMPLE: FileComparison.exe D:\Project\rdc_p1.txt D:\Project\ rdc_p1_baseline.txt y y -p D:\Project\config.txt");
        }

        public static int Main(string[] args)
        {
            try
            {
                if (args.Length < 4 || args.Length > 6)
                {
                    System.Console.WriteLine("Error: Arguments are incorrect!");
                    printUsage();
                    return -1;
                }
                int i = 0;
                int j = 0;
                string strFile1Path = args[0];
                string strFile2Path = args[1];
                string strLaunch = args[2];
                string strExac = args[3];
                //string strFile1Path = "C:\\Yongjian\\Project\\FileComparison\\FileComparison\\TMS_Baseline.xml";
                //string strFile2Path = "C:\\Yongjian\\Project\\FileComparison\\FileComparison\\TMS.xml";
                //string strLaunch = "y";
                //string strExac = "n";
                string strOption = string.Empty;
                
                if (args.Length > 4)
                    strOption = args[4];
                string strConfigFilePath = null;
                if (args.Length == 5)
                {
                    if (strOption != "-s")
                    {
                        System.Console.WriteLine("Error: Arguments are incorrect!");
                        printUsage();
                        return -1;
                    }
                }
                if (args.Length == 6)
                {
                    strConfigFilePath = args[5];
                    if (strOption != "-p" & strOption != "-s")
                    {
                        System.Console.WriteLine("Error: Arguments are incorrect!");
                        printUsage();
                        return -1;
                    }
                }

                strLaunch = strLaunch.ToLower();
                strExac = strExac.ToLower();

                if (File.Exists(strFile1Path) == false)
                {
                    System.Console.WriteLine("Error: " + strFile1Path + " is NOT exist!");
                    return -1;
                }
                if (File.Exists(strFile2Path) == false)
                {
                    System.Console.WriteLine("Error: " + strFile2Path + " is NOT exist!");
                    return -1;
                }
                if (args.Length == 6 && File.Exists(strConfigFilePath) == false)
                {
                    System.Console.WriteLine("Error: Configure File is NOT exist!");
                    return -1;
                }
                if (strLaunch != "y" && strLaunch != "n")
                {
                    System.Console.WriteLine("Error: The parameter of \"If you want to Show Comparison Detail\" is invalid, should be y or n");
                    return -1;
                }
                if (strExac != "y" && strExac != "n")
                {
                    System.Console.WriteLine("Error: The parameter of \"If you want to Exactly Match\" is invalid, should be y or n");
                    return -1;
                }
                if (args.Length == 5 && strOption != "-s")
                {
                    System.Console.WriteLine("Error: The parameter of \"If you want to compare only the first file\" is invalid, should be -s.");
                    return -1;
                }
                if (args.Length == 6 && strOption != "-p" && strOption != "-s")
                {
                    System.Console.WriteLine("Error: The parameter of \"If you want to Ignore some lines\" is invalid, should be -p.");
                    System.Console.WriteLine("Error: The parameter of \"If you want to compare only the first file\" is invalid, should be -s.");
                    return -1;
                }

                bool isTextFile1 = FileCheck.CheckIsTextFile(strFile1Path);
                bool isTextFile2 = FileCheck.CheckIsTextFile(strFile2Path);

                FileInfo fi1 = new FileInfo(strFile1Path);
                FileInfo fi2 = new FileInfo(strFile2Path);

                string temp1 = null;
                string temp2 = null;

                if (fi1.Length + fi2.Length <= 100000000)
                {
                    StreamReader sr1 = new StreamReader(strFile1Path, System.Text.Encoding.Default);
                    StreamReader sr2 = new StreamReader(strFile2Path, System.Text.Encoding.Default);

                    temp1 = sr1.ReadToEnd().Trim();
                    temp2 = sr2.ReadToEnd().Trim();

                    sr1.Close();
                    sr2.Close();

                    if (temp1 == temp2)
                    {
                        if (temp1 != "")
                        {
                            System.Console.WriteLine("Two files are totally match!");
                            return 0;
                        }
                        else if (args.Length == 4)
                        {
                            System.Console.WriteLine("Error: Two files are both Empty!");
                            return -1;
                        }
                    }
                    else if (!isTextFile1 && !isTextFile2)
                    {
                        if (strLaunch == "n" && strExac == "y")
                        {
                            System.Console.WriteLine("Failed: Two files are not match! ");
                            return 1;
                        }
                        System.Console.WriteLine("Error: Invalid parameter, Match is NOT supported for binary files, and can NOT show detail..");
                        return -1;
                    }
                    //if (strLaunch == "n" && strExac == "y" && args.Length == 4)
                    //{
                    //    System.Console.WriteLine("Failed: Two files are not match! ");
                    //    return 1;
                    //}
                    if (isTextFile1 != isTextFile2)
                    {
                        System.Console.WriteLine("Error: Two files are different format!");
                        return -1;
                    }

                    if (temp1 == "")
                    {
                        System.Console.WriteLine("Error: " + strFile1Path + " is Empty!");
                        return -1;
                    }

                    if (temp2 == "")
                    {
                        System.Console.WriteLine("Error: " + strFile2Path + " is Empty!");
                        return -1;
                    }
                    temp1 = null;
                    temp2 = null;
                }
                else if (!isTextFile1 || !isTextFile2)
                {
                    System.Console.WriteLine("Error: Invalid parameter, Match is NOT supported for binary files, and can NOT show detail..");
                    return -1;
                }

                ArrayList lineList = new ArrayList();
                ArrayList groupList = new ArrayList();
                ArrayList keyList = new ArrayList();
                ArrayList blockList = new ArrayList();

                if (args.Length == 6)
                {
                    string[] sp0 = new string[2];
                    sp0[0] = ",";
                    sp0[1] = "-";
                    string[] sp1 = new string[1];
                    sp1[0] = ",";

                    StreamReader sr3 = new StreamReader(strConfigFilePath, System.Text.Encoding.Default);
                    string readl = sr3.ReadLine();
                    while (readl != null)
                    {
                        if (readl.IndexOf("<L>") > -1)
                        {
                            readl = readl.Replace("<L>", "").Trim();
                            string[] lineL = readl.Split(sp1, StringSplitOptions.RemoveEmptyEntries);
                            for (int m = 0; m < lineL.Length; m++)
                            {
                                lineList.Add(lineL[m].Trim().ToString());
                            }
                        }
                        else if (readl.IndexOf("<G>") > -1)
                        {
                            readl = readl.Replace("<G>", "");
                            string[] groupL = readl.Split(sp0, StringSplitOptions.RemoveEmptyEntries);
                            for (int m = 0; m < groupL.Length; m++)
                            {
                                groupList.Add(groupL[m].Trim().ToString());
                            }
                        }
                        else if (readl.IndexOf("<K>") > -1)
                        {
                            readl = readl.Replace("<K>", "");
                            string[] keyL = readl.Split(sp1, StringSplitOptions.RemoveEmptyEntries);
                            for (int m = 0; m < keyL.Length; m++)
                            {
                                keyList.Add(keyL[m].Trim().ToString());
                            }
                        }
                        else if (readl.IndexOf("<B>") > -1)
                        {
                            readl = readl.Replace("<B>", "");
                            string[] keyL = readl.Split(sp1, StringSplitOptions.RemoveEmptyEntries);
                            for (int m = 0; m < keyL.Length; m++)
                            {
                                blockList.Add(keyL[m].Trim().ToString());
                            }
                        }
                        readl = sr3.ReadLine();
                    }
                    sr3.Close();
                }

                Collection<Function.fileContent> col_1 = new Collection<Function.fileContent>();
                Collection<Function.fileContent> col_2 = new Collection<Function.fileContent>();



                StreamReader sr4 = new StreamReader(strFile1Path, System.Text.Encoding.Default);
                StreamReader sr5 = new StreamReader(strFile2Path, System.Text.Encoding.Default);

                i = 0;

                if (args.Length != 6)
                {
                    while (!sr4.EndOfStream)
                    {
                        i++;
                        temp1 = sr4.ReadLine().ToString().Trim();

                        if (temp1 != "")
                        {
                            Function.fileContent f1 = new Function.fileContent();
                            f1.fContent = temp1;
                            f1.lineNo = i.ToString();
                            col_1.Add(f1);
                        }
                    }
                    sr4.Close();
                    i = 0;
                    while (!sr5.EndOfStream)
                    {
                        i++;
                        temp1 = sr5.ReadLine().ToString().Trim();

                        if (temp1 != "")
                        {
                            Function.fileContent f2 = new Function.fileContent();
                            f2.fContent = temp1;
                            f2.lineNo = i.ToString();
                            col_2.Add(f2);
                        }
                    }
                    sr5.Close();
                }
                else
                {
                    bool isBlock = false;
                    int line_start_num = 0;
                    int line_end_num = 0;
                    i = 0;
                    while (!sr4.EndOfStream)
                    {
                        isBlock = false;
                        temp1 = sr4.ReadLine().ToString().Trim();
                        i++;
                        if (!lineList.Contains(i.ToString()) && !Function.linecontain(i, groupList) && !Function.ifcontain(temp1, keyList))
                        {
                            if (temp1 == "")
                            {
                                temp1 = null;
                                continue;
                            }
                            else
                            {
                                for (int n = 0; n < blockList.Count; n++)
                                {
                                    if (temp1.Contains(blockList[n].ToString()))
                                    {
                                        isBlock = true;
                                        if (temp2 == null)
                                        {
                                            temp2 = temp1;
                                            temp1 = null;
                                            line_start_num = i;
                                            line_end_num = i;
                                            break;
                                        }
                                        else
                                        {
                                            Function.fileContent f1 = new Function.fileContent();
                                            f1.fContent = temp2;
                                            if (line_start_num < line_end_num)
                                                f1.lineNo = line_start_num.ToString() + "-" + line_end_num.ToString();
                                            else
                                                f1.lineNo = line_start_num.ToString();
                                            col_1.Add(f1);
                                            temp2 = temp1;
                                            line_start_num = i;
                                            temp1 = null;
                                            break;
                                        }
                                    }
                                }
                                if (isBlock)
                                    continue;
                                else
                                {
                                    if (temp2 != null)
                                    {
                                        temp2 = temp2 + "," + temp1;
                                        line_end_num = i;
                                        temp1 = null;
                                        continue;
                                    }
                                    else
                                    {
                                        Function.fileContent f1 = new Function.fileContent();
                                        f1.fContent = temp1;
                                        f1.lineNo = i.ToString();
                                        col_1.Add(f1);
                                        temp1 = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            temp1 = null;
                            continue;
                        }
                    }
                    sr4.Close();
                    if (temp2 != null)
                    {
                        Function.fileContent f1 = new Function.fileContent();
                        f1.fContent = temp2;
                        if (line_start_num <= line_end_num)
                            f1.lineNo = line_start_num.ToString() + "-" + line_end_num.ToString();
                        else
                            f1.lineNo = line_start_num.ToString();
                        col_1.Add(f1);
                    }
                    line_start_num = 0;
                    line_end_num = 0;
                    i = 0;
                    temp1 = null;
                    temp2 = null;
                    while (!sr5.EndOfStream)
                    {
                        isBlock = false;
                        temp1 = sr5.ReadLine().ToString().Trim();
                        i++;
                        if (!lineList.Contains(i.ToString()) && !Function.linecontain(i, groupList) && !Function.ifcontain(temp1, keyList))
                        {
                            if (temp1 == "")
                            {
                                temp1 = null;
                                continue;
                            }
                            else
                            {

                                for (int n = 0; n < blockList.Count; n++)
                                {
                                    if (temp1.Contains(blockList[n].ToString()))
                                    {
                                        isBlock = true;
                                        if (temp2 == null)
                                        {
                                            temp2 = temp1;
                                            temp1 = null;
                                            line_start_num = i;
                                            line_end_num = i;
                                            break;
                                        }
                                        else
                                        {
                                            //line_end_num = i - 1;
                                            Function.fileContent f2 = new Function.fileContent();
                                            f2.fContent = temp2;
                                            if (line_start_num < line_end_num)
                                                f2.lineNo = line_start_num.ToString() + "-" + line_end_num.ToString();
                                            else
                                                f2.lineNo = line_start_num.ToString();
                                            col_2.Add(f2);
                                            temp2 = temp1;
                                            line_start_num = i;
                                            temp1 = null;
                                            break;
                                        }
                                    }
                                }
                                if (isBlock)
                                    continue;
                                else
                                {
                                    if (temp2 != null)
                                    {
                                        temp2 = temp2 + "," + temp1;
                                        line_end_num = i;
                                        temp1 = null;
                                        continue;
                                    }
                                    else
                                    {
                                        Function.fileContent f2 = new Function.fileContent();
                                        f2.fContent = temp1;
                                        f2.lineNo = i.ToString();
                                        col_2.Add(f2);
                                        temp1 = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            temp1 = null;
                            continue;
                        }
                    }
                    sr5.Close();
                    if (temp2 != null)
                    {
                        Function.fileContent f2 = new Function.fileContent();
                        f2.fContent = temp2;
                        if (line_start_num <= line_end_num)
                            f2.lineNo = line_start_num.ToString() + "-" + line_end_num.ToString();
                        else
                            f2.lineNo = line_start_num.ToString();
                        col_2.Add(f2);
                    }
                }

                strFile1Path = Path.GetFileName(strFile1Path);
                strFile2Path = Path.GetFileName(strFile2Path);

                int lenFile1 = col_1.Count;
                int lenFile2 = col_2.Count;

                if (strLaunch == "n" && lenFile1 != lenFile2)
                {
                    System.Console.WriteLine("Failed: Two files are not match!");
                    return 1;
                }

                if (lenFile1 == lenFile2)
                {
                    bool tm = true;
                    for (int a = 0; a < lenFile1; a++)
                    {
                        if (col_1[a].fContent != col_2[a].fContent)
                        {
                            tm = false;
                            if (strLaunch == "n" && strExac == "y")
                            {
                                System.Console.WriteLine("Failed: Two files are not match!");
                                return 1;
                            }
                            break;
                        }
                    }
                    if (tm)
                    {
                        System.Console.WriteLine("Two files are totally match!");
                        return 0;
                    }
                }

                if (lenFile1 > 100000 || lenFile2 > 100000)
                    System.Console.WriteLine(" It will take some minutes. Please wait...");

                bool errorFlag = false;
                bool matchFlag = false;
                int c = 0;
                string tempStr = "";

                if (strExac == "n")
                {
                    for (i = 0; i < lenFile1; i++)
                    //while(col_1.Count>0)
                    {
                        matchFlag = false;
                        tempStr = col_1[i].fContent;
                        for (j = 0; j < col_2.Count; j++)
                        {
                            if (tempStr == col_2[j].fContent)
                            {
                                matchFlag = true;
                                c++;
                                col_2.RemoveAt(j);
                                break;
                            }
                        }
                        if (matchFlag != true)
                        {
                            if (strLaunch == "y")
                            {
                                if (!errorFlag)
                                {
                                    System.Console.WriteLine("Failed: ");
                                    System.Console.WriteLine("{0,-20}{1,-50}{2,-50}", "Line NO.", strFile1Path, strFile2Path);
                                    errorFlag = true;
                                }
                                if (tempStr.Length > 40)
                                    System.Console.WriteLine("{0,-20}{1,-50}{2,-50}", "Line " + col_1[i].lineNo.ToString(), tempStr.Substring(0, 39) + "...", "missing");
                                else
                                    System.Console.WriteLine("{0,-20}{1,-50}{2,-50}", "Line " + col_1[i].lineNo.ToString(), tempStr, "missing");
                            }
                            else
                            {
                                System.Console.WriteLine("Failed: Two files are not match!");
                                return 1;
                            }
                        }
                        //col_1.RemoveAt(0);
                    }
                    //DateTime dtEnd = DateTime.Now;
                    //Console.WriteLine(dtEnd - dtStart);

                    if (c == lenFile2 && c == lenFile1)
                    {
                        System.Console.WriteLine("Two Files have the Same Content.");
                        System.Console.WriteLine("Info: There are " + lenFile1 + " lines in " + strFile1Path + " and " + strFile2Path + ".");
                        System.Console.WriteLine("Info: There are " + c + " lines matched.");
                        System.Console.WriteLine("Info: Comparison passed!");
                        return 0;
                    }

                    int disMatchCount = lenFile1 - c;
                    if (strOption == "-s")
                    {
                        if (disMatchCount != 0)
                        {
                            System.Console.WriteLine("Failed for fuzzing match: Two files are not match!");
                            System.Console.WriteLine("Info: There are " + lenFile1 + " lines in " + strFile1Path + ".");
                            System.Console.WriteLine("Info: There are " + c + " lines matched.");
                            System.Console.WriteLine("Info: There are " + disMatchCount + " lines not matched.");
                            return 1;
                        }
                        else
                        {
                            System.Console.WriteLine("Info: There are " + lenFile1 + " lines in " + strFile1Path + ".");
                            System.Console.WriteLine("Info: There are " + c + " lines matched.");
                            System.Console.WriteLine("Info: There are " + disMatchCount + " lines not matched.");
                            System.Console.WriteLine("Info: Comparison passed!");
                            return 0;
                        }
                    }

                    //c = 0;
                    //j = 0;
                    if (col_2.Count == 0)
                    {
                        System.Console.WriteLine("Failed for fuzzing match: Two files are not match!");
                        System.Console.WriteLine("Info: There are " + lenFile1 + " lines in " + strFile1Path + ".");
                        System.Console.WriteLine("Info: There are " + c + " lines matched.");
                        System.Console.WriteLine("Info: There are " + disMatchCount + " lines not matched.");
                        System.Console.WriteLine("Info: There are " + lenFile2 + " lines in " + strFile2Path + " and all matched in " + strFile1Path + ".");
                        return 1;
                    }
                    else
                    {
                        if (!errorFlag)
                        {
                            System.Console.WriteLine("Failed: ");
                            System.Console.WriteLine("{0,-20}{1,-50}{2,-50}", "Line NO.", strFile1Path, strFile2Path);
                            errorFlag = true;
                        }
                        foreach (Function.fileContent _f in col_2)
                        {
                            if (_f.fContent.Length > 40)
                                System.Console.WriteLine("{0,-20}{1,-50}{2,-50}", "Line " + _f.lineNo.ToString(), "missing", _f.fContent.Substring(0, 39) + "...");
                            else
                                System.Console.WriteLine("{0,-20}{1,-50}{2,-50}", "Line " + _f.lineNo.ToString(), "missing", _f.fContent);
                        }
                    }
                    System.Console.WriteLine("Failed for fuzzy match: Two files are not match!");
                    System.Console.WriteLine("Info: There are " + lenFile1 + " lines in " + strFile1Path + ".");
                    System.Console.WriteLine("Info: There are " + c + " lines matched.");
                    System.Console.WriteLine("Info: There are " + disMatchCount + " lines not matched.");
                    System.Console.WriteLine("Info: There are " + lenFile2 + " lines in " + strFile2Path + ".");
                    System.Console.WriteLine("Info: There are " + col_2.Count + " lines not matched.");
                    return 1;
                }
                else
                {

                    bool order = true;
                    if (lenFile1 < lenFile2)
                        order = false;
                    System.Console.WriteLine("Failed: ");
                    System.Console.WriteLine("{0,-20}{1,-50}{2,-50}", "Line NO.", strFile1Path, strFile2Path);
                    ExactlyMatch.Exactly_Match(col_1, col_2, order);
                    System.Console.WriteLine("Failed for exactly match: Two files are not match! ");

                    return 1;
                }
            }

            catch (Exception ex)
            {
                System.Console.WriteLine("Error: " + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.Source);
                return -1;
            }
            finally
            {
            }
        }
    }
}
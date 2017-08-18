using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace changeVer
{
    class Program
    {
        static int Main(string[] args)
        {
            string fileVersion="", productVersion = "", configFilename = "";

            // 0. get file/product version
            if (parsingParam(args, ref fileVersion, ref productVersion, ref configFilename) == false)
            {
                displayHelp();
                return -1;
            }

            // 1. load ini
            string[] lines = File.ReadAllLines(configFilename);

            int exitCode = 0;

            foreach(string line in lines)
            {
                if (Regex.IsMatch(line, "^#", RegexOptions.IgnoreCase) || Regex.IsMatch(line, "^//", RegexOptions.IgnoreCase))
                {
                    // is comment
                    continue;
                }

                string result = replaceVersion(line, fileVersion, productVersion);

                Console.WriteLine(string.Format("{0}:\t{1}", result, line));

                if (result.CompareTo("OK") != 0)
                {
                    exitCode = -1;
                }
            }

            return exitCode;
        }

        /// <summary>
        /// set fileversion and product version to resource file
        /// </summary>
        /// <param name="rcfile">resource filename</param>
        /// <param name="fileVersion">file version</param>
        /// <param name="productVersion">product version</param>
        /// <returns></returns>
        private static string replaceVersion(string rcfile, string fileVersion, string productVersion)
        {
            string ret = "OK";
            string backfile = rcfile + ".bak";
            FileStream rfs = null, wfs = null;
            StreamWriter sw = null;
            StreamReader sr = null;

            try { 
                // backup
                File.Delete(backfile);
                File.Move(rcfile, backfile);

                // 1.2 open file
                rfs = File.Open(backfile, FileMode.Open, FileAccess.Read);
                wfs = File.Open(rcfile, FileMode.CreateNew, FileAccess.Write);
                sr = new StreamReader(rfs, Encoding.Default);
                sw = new StreamWriter(wfs, Encoding.Default);
            }
            catch (Exception e)
            {
                ret = e.Message;
                return ret;
            }

            string line;
            int ir = 0, iw = 0;
            while ((line = sr.ReadLine()) != null)
            {
                ir++;

                if (line.CompareTo("VS_VERSION_INFO VERSIONINFO") == 0)
                {
                    sw.WriteLine("VS_VERSION_INFO VERSIONINFO");
                    sw.WriteLine(string.Format(" FILEVERSION {0}", fileVersion.Replace(".", ",")));
                    sw.WriteLine(string.Format(" PRODUCTVERSION {0}", productVersion.Replace(".", ",")));

                    iw += 3;

                    line = sr.ReadLine(); // FILEVERSION
                    line = sr.ReadLine(); // PRODUCTVERSION

                    ir += 2;
                }
                else if (Regex.IsMatch(line, "\\s*VALUE \"FileVersion\",", RegexOptions.IgnoreCase))
                {
                    char[] delimiter = { '"' };
                    string[] values = line.Split(delimiter);

                    sw.WriteLine(string.Format("{0}\"FileVersion\", \"{1}\"", values[0], fileVersion.Replace(".", ", ")));

                    iw++;
                }
                else if (Regex.IsMatch(line, "\\s*VALUE \"ProductVersion\",", RegexOptions.IgnoreCase))
                {
                    char[] delimiter = { '"' };
                    string[] values = line.Split(delimiter);

                    sw.WriteLine(string.Format("{0}\"ProductVersion\", \"{1}\"", values[0], productVersion.Replace(".", ", ")));
                    iw++;
                }
                else
                {
                    sw.WriteLine(line);
                    iw++;
                }
            }

            sr.Close();
            sw.Close();

            rfs.Close();
            wfs.Close();

            return ret;
        }

        /// <summary>
        /// return Application filename
        /// </summary>
        /// <returns>Application filename</returns>
        static string getAppName()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            string name = Path.GetFileName(codeBase);
            return name;
        }

        /// <summary>
        /// parsing commmand-line arguement 
        /// </summary>
        /// <param name="args">commmand-line arguements</param>
        /// <param name="fileVersion">[out] file version</param>
        /// <param name="productVersion">[out] product version</param>
        /// <param name="configFilename">[out] config filename</param>
        /// <returns></returns>
        static bool parsingParam(string[]  args, ref string fileVersion, ref string productVersion, ref string configFilename)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToUpper().CompareTo("-F") == 0)
                {
                    if (args.Length > i + 1) fileVersion = args[++i];
                }
                else if (args[i].ToUpper().CompareTo("-P") == 0)
                {
                    if (args.Length > i + 1) productVersion = args[++i];
                }
                else if (args[i].ToUpper().CompareTo("-C") == 0)
                {
                    if (args.Length > i + 1) configFilename = args[++i];
                }
            }

            if (fileVersion.Length == 0 || productVersion.Length == 0 || configFilename.Length == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// display help
        /// </summary>
        static void displayHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("Usage>");
            Console.WriteLine(string.Format("$ {0} -f FileVersion -p ProductVersion -c ConfigFilename", getAppName()));
            Console.WriteLine(string.Format("ex) $ {0} -f 2.1.6.33 -p 2.1.0.0 -c acm_resources.txt", getAppName()));
            Console.WriteLine("");
        }

    }
}

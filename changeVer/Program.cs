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
        static string _fileVersion = "";
        static string _productVersion = "";
        static string _configFilename = "";
        static bool _isShow = false;

        static int Main(string[] args)
        {
            // 0. get file/product version
            if (parsingParam(args) == false)
            {
                if (_configFilename.Length == 0)
                {
                    Console.WriteLine("ERROR! Not Exist Config-File");
                }

                if (!_isShow && _fileVersion.Length ==0 )
                {
                    Console.WriteLine("ERROR! Not Exist File-Version");
                }

                if (!_isShow && _productVersion.Length == 0)
                {
                    Console.WriteLine("ERROR! Not Exist Product-Version");
                }

                displayHelp();
                return -1;
            }

            // 1. load ini
            string[] lines = File.ReadAllLines(_configFilename);

            int exitCode = 0;

            foreach(string line in lines)
            {
                if (Regex.IsMatch(line, "^#", RegexOptions.IgnoreCase) || Regex.IsMatch(line, "^//", RegexOptions.IgnoreCase))
                {
                    // is comment
                    continue;
                }

                string rcPath = makeAbsolatePath(line);

                if (!File.Exists(rcPath))
                {
                    Console.WriteLine(string.Format("ERROR! Not Exist Resource-File: {0}", rcPath));
                    continue;
                }

                if (_isShow)
                {
                    // for show 

                    Console.WriteLine(string.Format("{0}:", rcPath));
                    showCurrentVersion(rcPath);
                }
                else
                {
                    // for change

                    string result = replaceVersion(rcPath);

                    // if show mode then does not print result
                    if (!_isShow)
                    {
                        Console.WriteLine(string.Format("{0}:\t{1}", result, rcPath));
                    }

                    if (result.CompareTo("OK") != 0)
                    {
                        exitCode = -1;
                    }
                }
            }

            return exitCode;
        }

        /// <summary>
        /// make absolate path 
        /// </summary>
        /// <param name="path">path </param>
        /// <returns>absolate path</returns>
        private static string makeAbsolatePath(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                string currentPath = Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath);

                path = currentPath + Path.DirectorySeparatorChar + path;
            }

            return path;
        }

        /// <summary>
        /// print current version
        /// </summary>
        /// <param name="rcfile">resource filename</param>
        private static void showCurrentVersion(string rcfile)
        {
            FileStream rfs = null;
            StreamReader sr = null;

            try
            {
                // 1.2 open file
                rfs = File.Open(rcfile, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(rfs, Encoding.Default);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            string line;
            int ir = 0;
            while ((line = sr.ReadLine()) != null)
            {
                ir++;

                if (line.CompareTo("VS_VERSION_INFO VERSIONINFO") == 0)
                {
                    line = sr.ReadLine(); // FILEVERSION
                    Console.WriteLine(string.Format("\t{0}", line));
                    line = sr.ReadLine(); // PRODUCTVERSION
                    Console.WriteLine(string.Format("\t{0}", line));

                    ir += 2;
                }
                else if (Regex.IsMatch(line, "\\s*VALUE \"FileVersion\",", RegexOptions.IgnoreCase))
                {
                    Console.WriteLine(string.Format("\t{0}", line));
                }
                else if (Regex.IsMatch(line, "\\s*VALUE \"ProductVersion\",", RegexOptions.IgnoreCase))
                {
                    Console.WriteLine(string.Format("\t{0}", line));
                }
            }

            sr.Close();
            rfs.Close();

            return;
        }

        /// <summary>
        /// set fileversion and product version to resource file
        /// </summary>
        /// <param name="rcfile">resource filename</param>
        /// <returns></returns>
        private static string replaceVersion(string rcfile)
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
                    sw.WriteLine(string.Format(" FILEVERSION {0}", _fileVersion.Replace(".", ",")));
                    sw.WriteLine(string.Format(" PRODUCTVERSION {0}", _productVersion.Replace(".", ",")));

                    iw += 3;

                    line = sr.ReadLine(); // FILEVERSION
                    line = sr.ReadLine(); // PRODUCTVERSION

                    ir += 2;
                }
                else if (Regex.IsMatch(line, "\\s*VALUE \"FileVersion\",", RegexOptions.IgnoreCase))
                {
                    char[] delimiter = { '"' };
                    string[] values = line.Split(delimiter);

                    sw.WriteLine(string.Format("{0}\"FileVersion\", \"{1}\"", values[0], _fileVersion.Replace(".", ", ")));

                    iw++;
                }
                else if (Regex.IsMatch(line, "\\s*VALUE \"ProductVersion\",", RegexOptions.IgnoreCase))
                {
                    char[] delimiter = { '"' };
                    string[] values = line.Split(delimiter);

                    sw.WriteLine(string.Format("{0}\"ProductVersion\", \"{1}\"", values[0], _productVersion.Replace(".", ", ")));
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
        /// <returns></returns>
        static bool parsingParam(string[]  args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToUpper().CompareTo("-F") == 0)
                {
                    if (args.Length > i + 1) _fileVersion = args[++i];
                }
                else if (args[i].ToUpper().CompareTo("-P") == 0)
                {
                    if (args.Length > i + 1) _productVersion = args[++i];
                }
                else if (args[i].ToUpper().CompareTo("-C") == 0)
                {
                    if (args.Length > i + 1)
                    {
                        _configFilename = makeAbsolatePath(args[++i]);
                        if (!File.Exists(_configFilename))
                        {
                            _configFilename = "";
                        }
                    }
                }
                else if (args[i].ToUpper().CompareTo("-S") == 0)
                {
                    _isShow = true;
                }
            }

            if (_configFilename.Length == 0) return false;
            if (_isShow == true) return true;
            if (_fileVersion.Length == 0 || _productVersion.Length == 0 ) return false;

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
            Console.WriteLine(string.Format("ex) $ {0} -s -c acm_resources.txt", getAppName()));
            Console.WriteLine("");
            Console.WriteLine("Options>");
            Console.WriteLine("\t-f[F]: After File version");
            Console.WriteLine("\t-p[P]: After Product version");
            Console.WriteLine("\t-c[C]: Config filename");
            Console.WriteLine("\t-s[S]: Show Current version");
            Console.WriteLine("");
        }

    }
}

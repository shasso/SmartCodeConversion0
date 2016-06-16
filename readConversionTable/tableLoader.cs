using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommandLine;

namespace readConversionTable
{
    public class tableLoader
    {
        class Options
        {
            [Option("t", "source", Required = true, HelpText = "file that conatins lookup values.")]
            public string source = null;

            [Option("e", "encoding", Required = false, HelpText = "encoding")]
            public string encoding = "u8";

            [HelpOption(HelpText = "Dispaly this help screen.")]
            public string GetUsage()
            {
                string str = string.Format("usage: {0} -t  file that conatins lookup values.", Environment.GetCommandLineArgs()[0]);
                return (str);
            }
        }
        static void Main(string[] args)
        {
            System.Text.Encoding encoding = Encoding.UTF8;
            var options = new Options();
            ICommandLineParser parser = new CommandLineParser();

            if (!parser.ParseArguments(args, options, Console.Error))
            {
                options.GetUsage();
                return;
            }
            // default encoding is set to UT8
            if (options.encoding.ToString().CompareTo("u") == 0)
                encoding = Encoding.Unicode;
            if (options.encoding.ToString().CompareTo("u32") == 0)
                encoding = Encoding.UTF32;
            if (options.encoding.ToString().CompareTo("a") == 0)
                encoding = Encoding.ASCII;

            //string[] codes = new string[] {"0710", "072c", "064d", "a0"};
            //byte[] bb = System.Text.Encoding.Unicode.GetBytes(codes[0]);
            //foreach (string ss in codes)
            //{
            //    int num = Convert.ToInt32(ss, 16);
            //    char cc = (char)num;
            //    Console.WriteLine("{0:x4}", cc);
            //}

            Dictionary<char, List<char>> ps;
            int rc = loadTable(options.source, out ps, encoding);
            

            Console.WriteLine("{0}", ps.Count());
        }

        public static int loadTable(string lookUpValuesFileName, out Dictionary<char, List<char>> lookuptable, Encoding encoding)
        {
            lookuptable = new Dictionary<char, List<char>>();

            using (TextReader input = new StreamReader
                     (new FileStream(lookUpValuesFileName, FileMode.Open), encoding))
            {
                string buffer = string.Empty;
                int lineno = 1;
                int num;
                char cc, index;
                try
                {
                    while ((buffer = input.ReadLine()) != null)
                    {
                        // parse comma separated lines
                        string[] codes = buffer.Split(new char[] { ',' });
                        num = Convert.ToInt32(codes[0], 16); // index
                        index = (char)num;
                        List<char> target = new List<char>();
                        for (int ii = 1; ii < codes.Length; ++ii )
                        {
                            num = Convert.ToInt32(codes[ii], 16);
                            cc = (char)num;
                            target.Add(cc);
                            //Console.Write("{0:x4} ", num);

                        }
                        
                        lookuptable[index] = target;
                        //Console.WriteLine();
                        ++lineno;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("error at line: {0} processing: {1}", lineno, buffer); 
                }

            }
            return 0;
        }
    }
}

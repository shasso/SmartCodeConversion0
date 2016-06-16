using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using CommandLine;
using readConversionTable;

namespace Pshitta
{
    class Options
    {
        [Option("t", "source", Required = true, HelpText = "file that conatins lookup values.")]
        public string source = null;

        [Option("i", "input", Required = true, HelpText = "source file")]
        public string ifile = null;

        [Option("o", "output", Required = true, HelpText = "output file")]
        public string ofile = null;

        [Option("e", "encoding", Required = false, HelpText = "encoding")]
        public string encoding = "u8";

        [HelpOption(HelpText = "Dispaly this help screen.")]
        public string GetUsage()
        {
            string str = string.Format("usage: {0} -t  file that conatins lookup values -i input file -o output file", Environment.GetCommandLineArgs()[0]);
            return (str);
        }
    }

    class converter
    {

        const int BufferSize = 1;
        static private void usage()
        {
            Console.WriteLine("usage: {0} -i input File -o output File, [-e u|u8|a]", Environment.GetCommandLineArgs()[0]);
        }

        static void Main(string[] args)
        {         
            HashSet<char> todelete = new HashSet<char>() { '\x0060' };
            // frequency count
            Dictionary<char, int> cfq = new Dictionary<char, int>();

            string ifile = null, ofile = null;
            System.Text.Encoding encoding = Encoding.UTF8;

            var options = new Options();
            ICommandLineParser parser = new CommandLineParser();
            if (!parser.ParseArguments(args, options, Console.Error))
            {
                options.GetUsage();
                return;
            }

            ifile = options.ifile;
            ofile = options.ofile;

           // default encoding is set to UTF8
            if (options.encoding.ToString().CompareTo("u") == 0)
                encoding = Encoding.Unicode;
            if (options.encoding.ToString().CompareTo("u32") == 0)
                encoding = Encoding.UTF32;
            if (options.encoding.ToString().CompareTo("a") == 0)
                encoding = Encoding.ASCII;
           
           
            Dictionary<char, List<char>> ps;
            int rc = tableLoader.loadTable(options.source, out ps, encoding);

            int ccount = 0;
            try
            {

                // Open a TextReader for the appropriate file
                using (TextReader input = new StreamReader
                      (new FileStream(ifile, FileMode.Open), encoding))
                {
                    // Open a TextWriter for the appropriate file
                    using (TextWriter output = new StreamWriter
                          (new FileStream(ofile, FileMode.Create), Encoding.UTF8))
                    {

                        // write a signature
                        //char signature = '\ufeff';
                        //output.Write("{0}", signature);

                        // Encode the string.
                        char[] buffer = new char[BufferSize];
                        int len;

                        // Repeatedly copy data until we've finished
                        while ((len = input.Read(buffer, 0, BufferSize)) > 0)
                        {
                            try
                            {
                                // count character frequency
                                char sc = buffer[0];
                                if (!cfq.ContainsKey(sc))
                                    cfq.Add(sc, 0);

                                cfq[sc]++;

                                if (!todelete.Contains(buffer[0]))
                                {
                                    // Console.WriteLine("{0:u} is decomposable", buffer[0]);
                                    // decompose and then convert each character
                                    List<char> codes = ps[buffer[0]];
                                    foreach (char cc in codes)
                                    {
                                        output.Write(cc);
                                        ++ccount;
                                    }
                                }
                                else
                                {
                                    // consume the character and don't do anything
                                    ++ccount;
                                }
                            }
                            catch (KeyNotFoundException)
                            {
                                // if key not found, don't convert; 
                                // these are characters that designated to map to void
                                output.Write("{0}", buffer[0]);
                                string ff = String.Format(@"\x{0:x4}", buffer[0]);
                                // Console.WriteLine("{0}, 0x{1:X4}", ccount, Convert.ToInt32(buffer[0]));
                                ++ccount;
                            }

                            // Console.Write(buffer, 0, len);
                            // output.Write(buffer, 0, len);

                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read at line {0}: ", ccount);
                Console.WriteLine(e.Message);
            }

            //Console.WriteLine("press any key to continue...");
            //Console.ReadKey();

            // sort character frequency table
            var qq = from mm in cfq
                     orderby mm.Key
                     select mm;

            foreach (var cc in qq)
            {
                Console.WriteLine("0x{0:X4}\t{1}", Convert.ToInt32(cc.Key), cc.Value);
            }
            // Console.ReadKey();
        }
    }
}

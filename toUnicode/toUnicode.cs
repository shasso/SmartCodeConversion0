using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using CommandLine;
using readConversionTable;
using Matching;

namespace toUnicode
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

    class toUnicode
    {
        const int BufferSize = 1;
        static Dictionary<char, List<char>> ps;
        static private void usage()
        {
            Console.WriteLine("usage: {0} -i input File -o output File, [-e u|u8|a]", Environment.GetCommandLineArgs()[0]);
        }

        // input: a source line of bracketed words (between < and >) of non-unicode syriac characters
        // output: the same source line with <words> transormed to unicode syriac characters
        static string transform(string input, List<Match> source, Delimiter dm)
        {
            string transformed_string = a2s(input, dm, true);
            //foreach (Match mm in source)
            //{
            //    string syriac = a2s(mm.value);
            //    transformed_string = transformed_string.Replace(mm.value, syriac);
            //}
            return (String.IsNullOrEmpty(transformed_string) ? input : transformed_string);
        }

        static string a2s(string word)
        {
            // walk though each character and convert
            // if no conversion possible, leave as is (the exception part)
            StringBuilder output = new StringBuilder();
            foreach (var cc in word)
            {
                try
                {
                    List<char> codes = ps[cc];
                    foreach (char ii in codes)
                    {
                        output.Append(ii);
                    }
                }
                catch (KeyNotFoundException ex) 
                {
                    output.Append(cc);
                }
            }
            return (output.ToString());
        }

        static string a2s(string line, Delimiter dm, bool reverse)
        {
            StringBuilder output = new StringBuilder();
            ProcessWord token = new ProcessWord(line);
            bool stillProcessing = true;
            char nextChar = '\u0000';
            int state = 0;
            Stack<char> tempWord = new Stack<char>();

            while (stillProcessing)
            {
                try
                {
                    switch (state)
                    {
                        case 0:
                            nextChar = token.getNextChar();
                            if (nextChar == dm.START)
                            {
                                output.Append(nextChar);
                                state = 1;
                            }
                            else
                            {
                                token.retract(1);
                                state = 3;
                            }
                            break;

                        case 1: // accepting state for <...>
                            nextChar = token.getNextChar();
                            if (nextChar == dm.END)
                            {
                                output.Append(nextChar);
                                state = 0;
                            }
                            else
                            {
                                output.Append(nextChar);
                            }
                            break;
                        case 3: // accepting state for anything but <...>
                            nextChar = token.getNextChar();
                            if (nextChar == dm.START)
                            {
                                // pop anything stored so far and append them to the output buffer
                                while (true)
                                {
                                    try
                                    {
                                        output.Append(tempWord.Pop());
                                    }
                                    catch (InvalidOperationException ex)
                                    {
                                        break; // done popping
                                    }

                                }
                            
                                output.Append(nextChar);       // for dm.START                        
                                state = 1;
                            }
                            else
                            {
                                // convert and add
                                try
                                {
                                    List<char> codes = ps[nextChar];
                                    foreach (char ii in codes)
                                    {
                                        // output.Append(ii);
                                        tempWord.Push(ii);
                                    }
                                }
                                catch (KeyNotFoundException ex)
                                {
                                    // no conversion, just leave alone
                                    // output.Append(nextChar);
                                    tempWord.Push(nextChar);

                                }
                            }
                            break;
                        default: // non-reaching statement
                            break;          
                    }
                }
                catch (ConversionException ex)
                {
                    // if there are still stored chars, append them to the output buffer
                    while (true)
                    {
                        try
                        {
                            output.Append(tempWord.Pop());
                        }
                        catch (InvalidOperationException emptyEx)
                        {
                            break; // done popping
                        }

                    }
                    stillProcessing = false;
                }
            }
            return (output.ToString());
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

            // default encoding is set to UT8
            if (options.encoding.ToString().CompareTo("u") == 0)
                encoding = Encoding.Unicode;
            if (options.encoding.ToString().CompareTo("u32") == 0)
                encoding = Encoding.UTF32;
            if (options.encoding.ToString().CompareTo("a") == 0)
                encoding = Encoding.ASCII;

            Delimiter wordDelimiter = new Delimiter('<', '>');

          
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
                        string oneLine;
                        while ( (oneLine = input.ReadLine()) != null)
                        {
                            try
                            {                              
                                List<Match> matches = Match.MatchCollection(oneLine, wordDelimiter);
                                string ts = transform(oneLine, matches, wordDelimiter);
                                output.WriteLine(ts);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }                       
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

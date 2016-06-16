using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using CommandLine.Utility;

namespace codeConversion
{
    public class ConversionException : Exception
    {
        public ConversionException() { }
        public ConversionException(string msg)
            : base(msg)
        {}
        public ConversionException(string msg, Exception inner)
            : base(msg, inner) { }
    }

    public class Match
    {
        public string value { get; set; }
        public int position { get; set; }
        public Match(string word, int pos)
        {
            value = word;
            position = pos;
        }
    }

    public class ProcessWord
    {
        string _buffer;
        int currentPosition;
        
        public ProcessWord(string word)
        {
            _buffer = word;
            currentPosition = 0;
        }
        
        // return the next character, '\0' if done processing
        public char getNextChar()
        {
            char cc;
            if (currentPosition == _buffer.Length)
                throw new ConversionException("end of word enumeration");

            cc = _buffer[currentPosition++];
            return (cc);
        }

        public void retract(int count)
        {
            while ( count-- > 0 )
                if (currentPosition > 0) {
                    --currentPosition;
                } else
                    throw new ConversionException("invalid character retraction");
        }

        public char discard()
        {
            return _buffer[currentPosition++];
        }
    }

    public class Delimiter
    {
        public char START { set; get; }
        public char END { get; set; }

        public Delimiter(char begin, char end)
        {
            this.START = begin;
            this.END = end;

        }
    }
    
        class Program
    {
        static int state = 0;
        static int start = 0;

        static Dictionary<char, char> bb;
        static Queue<char> sm;

        // define a dictionary to decompose compound codes, i.e. one to many
        // use 'char' for key, and a list for value
        static Dictionary<char, List<char>> decompose;

        // create lists first to be added to the decompose Dictionary
        static List<char> resh_syame;
        static List<char> sheen_maj;
        static List<char> kap_maj;
        static List<char> beth_rukakha;
        static List<char> gamal_rukakha;
        static List<char> kap_rukakha;
        static List<char> gamal_maj;
        static List<char> youdh_kbasa;
        static List<char> peh_rukakha;
        static List<char> tau_alaph;
        static List<char> he_qanona;
        static List<char> meem_noon;
        static List<char> syame_zlame;


        // create some convenient category classes using sets for quick searches
        static HashSet<char> canonicals;
        static HashSet<char> decomposable;
        static HashSet<char> many2many;
        static HashSet<char> lam_alaph;
        static HashSet<char> rukakha_zqapa;


        static void initConverterData()
        {
            // use this dictionary as a look up table
            bb = new Dictionary<char, char>();
            bb.Add('\u0009', '\u0009');
            bb.Add('\u000a', '\u000a');
            bb.Add('\u000d', '\u000d');
            bb.Add('\u0020', '\u0020');
            bb.Add('\u0024', '\u0024');
            bb.Add('\u0028', '\u0028');
            bb.Add('\u0029', '\u0029');
            bb.Add('\u002a', '\u002a');
            bb.Add('\u002b', '\u002b');
            bb.Add('\u002c', '\u002c');
            bb.Add('\u002d', '\u002d');
            bb.Add('\u002e', '\u002e');
            bb.Add('\u002f', '\u002f');
            bb.Add('\u0030', '\u0030');
            bb.Add('\u0031', '\u0031');
            bb.Add('\u0032', '\u0032');
            bb.Add('\u0033', '\u0033');
            bb.Add('\u0034', '\u0034');
            bb.Add('\u0035', '\u0035');
            bb.Add('\u0036', '\u0036');
            bb.Add('\u0037', '\u0037');
            bb.Add('\u0038', '\u0038');
            bb.Add('\u0039', '\u0039');
            bb.Add('\u003a', '\u003a');
            bb.Add('\u0041', '\u0041');
            bb.Add('\u0042', '\u0042');
            bb.Add('\u0043', '\u0043');
            bb.Add('\u0044', '\u0044');
            bb.Add('\u0045', '\u0045');
            bb.Add('\u0046', '\u0046');
            bb.Add('\u0047', '\u0047');
            bb.Add('\u0048', '\u0048');
            bb.Add('\u0049', '\u0049');
            bb.Add('\u004c', '\u004c');
            bb.Add('\u004d', '\u004d');
            bb.Add('\u004e', '\u004e');
            bb.Add('\u004f', '\u004f');
            bb.Add('\u0050', '\u0050');
            bb.Add('\u0052', '\u0052');
            bb.Add('\u0053', '\u0053');
            bb.Add('\u0054', '\u0054');
            bb.Add('\u0055', '\u0055');
            bb.Add('\u0056', '\u0056');
            bb.Add('\u0057', '\u0057');
            bb.Add('\u0059', '\u0059');
            bb.Add('\u0061', '\u0061');
            bb.Add('\u0062', '\u0062');
            bb.Add('\u0063', '\u0063');
            bb.Add('\u0064', '\u0064');
            bb.Add('\u0065', '\u0065');
            bb.Add('\u0066', '\u0066');
            bb.Add('\u0067', '\u0067');
            bb.Add('\u0068', '\u0068');
            bb.Add('\u0069', '\u0069');
            bb.Add('\u006b', '\u006b');
            bb.Add('\u006c', '\u006c');
            bb.Add('\u006d', '\u006d');
            bb.Add('\u006e', '\u006e');
            bb.Add('\u006f', '\u006f');
            bb.Add('\u0070', '\u0070');
            bb.Add('\u0071', '\u0071');
            bb.Add('\u0072', '\u0072');
            bb.Add('\u0073', '\u0073');
            bb.Add('\u0074', '\u0074');
            bb.Add('\u0075', '\u0075');
            bb.Add('\u0076', '\u0076');
            bb.Add('\u0077', '\u0077');
            bb.Add('\u0078', '\u0078');
            bb.Add('\u0079', '\u0079');
            bb.Add('\u007a', '\u007a');
            bb.Add('\u00b7', '\u00b7');
            bb.Add('\u060c', '\u060c');
            bb.Add('\u061f', '\u061f');
            bb.Add('\u0622', '\u0622');
            bb.Add('\u0623', '\u0710'); // alaph
            bb.Add('\u0624', '\u0728'); // sadeh
            bb.Add('\u0625', '\u0710'); // alaph
            bb.Add('\u0626', '\u0626');
            bb.Add('\u0627', '\u0710'); // alaph
            bb.Add('\u0628', '\u0712'); // beth
            bb.Add('\u0629', '\u072c'); // tau
            bb.Add('\u062a', '\u073c'); // kbasa
            bb.Add('\u062b', '\u062b');
            bb.Add('\u062c', '\u062c');
            bb.Add('\u062d', '\u071a'); // kheth
            bb.Add('\u062e', '\u062e');
            bb.Add('\u062f', '\u0715'); // daladh
            bb.Add('\u0630', '\u0630');
            bb.Add('\u0631', '\u072a'); // resh
            bb.Add('\u0632', '\u0719'); // zain
            bb.Add('\u0633', '\u0723'); // simkath
            bb.Add('\u0634', '\u072b'); // sheen
            bb.Add('\u0635', '\u0635');
            bb.Add('\u0636', '\u0636');
            bb.Add('\u0637', '\u071b'); // deth
            bb.Add('\u0638', '\u072c'); // SECOND tau
            bb.Add('\u0639', '\u0725'); // ain
            bb.Add('\u063a', '\u063a');
            bb.Add('\u0640', '\u0640');
            bb.Add('\u0641', '\u0726'); // peh
            bb.Add('\u0642', '\u0729'); // qop
            bb.Add('\u0643', '\u071f'); // kap
            bb.Add('\u0644', '\u0720'); // lamad
            bb.Add('\u0645', '\u0721'); // meem
            bb.Add('\u0646', '\u0722'); // noon
            bb.Add('\u0647', '\u0713'); // gamal
            bb.Add('\u0648', '\u0718'); // waw
            bb.Add('\u0649', '\u0717'); // he
            bb.Add('\u064a', '\u071d'); // yudh
            bb.Add('\u064b', '\u0739'); // zlame angular
            // bb.Add('\u064c', '\u064c');
            bb.Add('\u064d', '\u0732'); // ptakha
            bb.Add('\u064e', '\u0738'); // zlame horizontal
            // bb.Add('\u064f', '\u0308'); // syame + zlame
            bb.Add('\u0650', '\u0735'); // zqapa
            bb.Add('\u0651', '\u0742'); // rukakha
            bb.Add('\u0652', '\u0747'); // tlaqa
            bb.Add('\u2013', '\u2013');
            bb.Add('\u2018', '\u2018');
            bb.Add('\u2019', '\u2019');
            bb.Add('\u2122', '\u2122');
            bb.Add('\u0621', '\u0735'); // hamza --> zqapa

            // define a queue for state machine
            sm = new Queue<char>();

            decompose = new Dictionary<char, List<char>>();

            // create lists first to be added to the decompose Dictionary
            resh_syame = new List<char>();
            sheen_maj = new List<char>();
            kap_maj = new List<char>();
            beth_rukakha = new List<char>();
            gamal_rukakha = new List<char>();
            kap_rukakha = new List<char>();
            gamal_maj = new List<char>();
            youdh_kbasa = new List<char>();
            peh_rukakha = new List<char>();
            tau_alaph = new List<char>();
            he_qanona = new List<char>();
            meem_noon = new List<char>();
            syame_zlame = new List<char>();


            // now populate these these lists
            resh_syame.Add('\u072a');
            resh_syame.Add('\u0308');

            sheen_maj.Add('\u072b');
            sheen_maj.Add('\u0303');

            kap_maj.Add('\u071f');
            kap_maj.Add('\u0330');

            beth_rukakha.Add('\u0712');
            beth_rukakha.Add('\u0742');

            gamal_rukakha.Add('\u0713');
            gamal_rukakha.Add('\u0742');

            kap_rukakha.Add('\u071f');  // non-decomposable
            // kap_rukakha.Add('\u0742');

            gamal_maj.Add('\u0713');
            gamal_maj.Add('\u0330');

            youdh_kbasa.Add('\u071d'); // non-decomposable
            youdh_kbasa.Add('\u073c');

            peh_rukakha.Add('\u0726');
            peh_rukakha.Add('\u032e');

            tau_alaph.Add('\u072c');
            tau_alaph.Add('\u0710');

            he_qanona.Add('\u0717');
            he_qanona.Add('\u0307');

            meem_noon.Add('\u0721');
            meem_noon.Add('\u0323');
            meem_noon.Add('\u0722');

            syame_zlame.Add('\u0308');
            syame_zlame.Add('\u0739');



            // now load the decompose dictionary
            decompose.Add('\u0630', resh_syame);
            decompose.Add('\u0636', sheen_maj);
            decompose.Add('\u0635', kap_maj);
            decompose.Add('\u062b', beth_rukakha);
            decompose.Add('\u063a', gamal_rukakha);
            decompose.Add('\u062e', kap_rukakha);
            decompose.Add('\u062c', gamal_maj);
            decompose.Add('\u062a', youdh_kbasa);
            decompose.Add('\u0626', peh_rukakha);
            //decompose.Add('\u0638', tau_alaph);
            decompose.Add('\u0622', he_qanona);
            decompose.Add('\u002f', meem_noon);
            decompose.Add('\u064f', syame_zlame);
            decompose.Add('\u064c', syame_zlame);    // there are two of these


            // create some convenient category classes using sets for quick searches
            canonicals = new HashSet<char>();
            decomposable = new HashSet<char>();
            many2many = new HashSet<char>();
            lam_alaph = new HashSet<char>();
            rukakha_zqapa = new HashSet<char>();

            // load them
            canonicals.Add('\u0627');
            canonicals.Add('\u0623');
            canonicals.Add('\u0625');
            canonicals.Add('\u0628');
            canonicals.Add('\u0647');
            canonicals.Add('\u062f');
            canonicals.Add('\u0649');
            canonicals.Add('\u0648');
            canonicals.Add('\u0632');
            canonicals.Add('\u062d');
            canonicals.Add('\u0637');
            canonicals.Add('\u064a');
            canonicals.Add('\u0643');
            canonicals.Add('\u0645');
            canonicals.Add('\u0646');
            canonicals.Add('\u0633');
            canonicals.Add('\u0639');
            canonicals.Add('\u0641');
            canonicals.Add('\u0624');
            canonicals.Add('\u0642');
            canonicals.Add('\u0631');
            canonicals.Add('\u0634');
            canonicals.Add('\u0629');
            // vowles
            canonicals.Add('\u064b');
            canonicals.Add('\u064e');
            canonicals.Add('\u064d');
            canonicals.Add('\u0650');
            // canonicals.Add('\u0651');
            canonicals.Add('\u0652');

            // the rest of characters; leave them alone
            canonicals.Add('\u0009');
            canonicals.Add('\u000a');
            canonicals.Add('\u000d');
            canonicals.Add('\u0020');
            canonicals.Add('\u0024');
            canonicals.Add('\u0028');
            canonicals.Add('\u0029');
            canonicals.Add('\u002a');
            canonicals.Add('\u002b');
            canonicals.Add('\u002c');
            canonicals.Add('\u002d');
            canonicals.Add('\u002e');

            canonicals.Add('\u0030');
            canonicals.Add('\u0031');
            canonicals.Add('\u0032');
            canonicals.Add('\u0033');
            canonicals.Add('\u0034');
            canonicals.Add('\u0035');
            canonicals.Add('\u0036');
            canonicals.Add('\u0037');
            canonicals.Add('\u0038');
            canonicals.Add('\u0039');
            canonicals.Add('\u003a');
            canonicals.Add('\u0041');
            canonicals.Add('\u0042');
            canonicals.Add('\u0043');
            canonicals.Add('\u0044');
            canonicals.Add('\u0045');
            canonicals.Add('\u0046');
            canonicals.Add('\u0047');
            canonicals.Add('\u0048');
            canonicals.Add('\u0049');
            canonicals.Add('\u004c');
            canonicals.Add('\u004d');
            canonicals.Add('\u004e');
            canonicals.Add('\u004f');
            canonicals.Add('\u0050');
            canonicals.Add('\u0052');
            canonicals.Add('\u0053');
            canonicals.Add('\u0054');
            canonicals.Add('\u0055');
            canonicals.Add('\u0056');
            canonicals.Add('\u0057');
            canonicals.Add('\u0059');
            canonicals.Add('\u0061');
            canonicals.Add('\u0062');
            canonicals.Add('\u0063');
            canonicals.Add('\u0064');
            canonicals.Add('\u0065');
            canonicals.Add('\u0066');
            canonicals.Add('\u0067');
            canonicals.Add('\u0068');
            canonicals.Add('\u0069');
            canonicals.Add('\u006b');
            canonicals.Add('\u006c');
            canonicals.Add('\u006d');
            canonicals.Add('\u006e');
            canonicals.Add('\u006f');
            canonicals.Add('\u0070');
            canonicals.Add('\u0071');
            canonicals.Add('\u0072');
            canonicals.Add('\u0073');
            canonicals.Add('\u0074');
            canonicals.Add('\u0075');
            canonicals.Add('\u0076');
            canonicals.Add('\u0077');
            canonicals.Add('\u0078');
            canonicals.Add('\u0079');
            canonicals.Add('\u007a');
            canonicals.Add('\u00b7');
            canonicals.Add('\u060c');
            canonicals.Add('\u061f');
            canonicals.Add('\u0640');
            // canonicals.Add('\u064c');    // this is decomposable: syame + zlame
            // canonicals.Add('\u064f'); // this is decomposable: syame + zlame
            canonicals.Add('\u2013');
            canonicals.Add('\u2018');
            canonicals.Add('\u2019');
            canonicals.Add('\u2122');
            canonicals.Add('\u0621');
            //canonicals.Add('\u0638');   // second tau has special meaning

            decomposable.Add('\u0630');
            decomposable.Add('\u0636');
            decomposable.Add('\u0635');
            decomposable.Add('\u062b');
            decomposable.Add('\u063a');
            decomposable.Add('\u062e');
            decomposable.Add('\u062c');
            decomposable.Add('\u062a');
            decomposable.Add('\u0626');
            //decomposable.Add('\u0638');
            decomposable.Add('\u0622');
            decomposable.Add('\u002f');
            decomposable.Add('\u064f');
            decomposable.Add('\u064c');

            many2many.Add('\u0644');        // for lamad
            many2many.Add('\u0651');        // for rukakha

            // these are the second character combinations in addition to \u0644 
            lam_alaph.Add('\u0627');
            lam_alaph.Add('\u0625');
            lam_alaph.Add('\u0623');
            lam_alaph.Add('\u0622');
        
            rukakha_zqapa.Add('\u0650');
        }

        static bool isCanonical(char cc)
        {
            return canonicals.Contains(cc);
        }

        static bool isDecomposable(char cc)
        {
            return decomposable.Contains(cc);
        }

        static bool isLamad(char cc)
        {
            return cc == '\u0644';
        }

        static bool isQushaya(char cc)
        {
            return cc == '\u0651';
        }
        static bool isTau(char cc)
        {
            return cc == '\u0638';
        }

        static bool isLam_alaph(char cc)
        {
            return lam_alaph.Contains(cc);
        }

        static bool isRukakha_zqapa(char cc)
        {
            return rukakha_zqapa.Contains(cc);
        }

        static bool isMany2Many(char cc)
        {
            return many2many.Contains(cc);
        }

        static private void usage()
        {
            Console.WriteLine("usage: {0} -i input File -o output File", Environment.GetCommandLineArgs()[0]);
        }

        // input: a string of this format abc <word> cde <another>
        // output list of Match objects for <word>, <another>
        static List<Match> MatchCollection(string input, Delimiter dm)
        {
            List<Match> matches = new List<Match>();
            int start = input.IndexOf(dm.START, 0);
            while (true) {
                if (start == -1) break;
                int begin = start;
                int end = input.IndexOf(dm.END, start);
                int size = end - begin + 1;                      // count anything between start and end delimiters
                string w = input.Substring(begin, size);  // 
                matches.Add(new Match(w, start));
                start = input.IndexOf(dm.START, end);
            }
            return matches;
        }
        // input: a source line of bracketed words (between < and >) of non-unicode syriac characters
        // output: the same source line with <words> transormed to unicode syriac characters
        static string transform(string input, List<Match> source)
        {
            string transformed_string = input;
            foreach (Match mm in source) {
                string syriac = a2s(mm.value);
                transformed_string = transformed_string.Replace(mm.value, syriac);
            }
            return (String.IsNullOrEmpty(transformed_string) ? input : transformed_string);
        }

        static void Main(string[] args)
        {

            string ifile = null, ofile = null;
            // Command line parsing

            if (0 == args.Length) {
                usage();
                return;
            }

            Arguments CommandLine = new Arguments(args);
            if (CommandLine["i"] != null)
                ifile = CommandLine["i"];
            if (CommandLine["o"] != null)
                ofile = CommandLine["o"];

            // init data structures and create table lookups
            initConverterData();


            int lineno = 0;
            Delimiter wordDelimiter = new Delimiter('<', '>');
            
            try {

                // Open a TextReader for the appropriate file
                using (TextReader input = new StreamReader
                      (new FileStream(ifile, FileMode.Open), Encoding.Unicode)) {
                    // Open a TextWriter for the appropriate file
                    using (TextWriter output = new StreamWriter
                          (new FileStream(ofile, FileMode.Create), Encoding.Unicode)) {

                        // write a signature: unicode prefix (utf-8)
                        char signature = '\ufeff';
                       // output.Write("{0}", signature);



                        // Repeatedly copy data until we are finished
                        string oneLine;

                        bool fMoreToRead = true;
                        while (fMoreToRead) {
                            oneLine = input.ReadLine();
                            if (oneLine == null) {
                                fMoreToRead = false;
                                continue; // force to exit while loop; done reading all input
                            }
                            ++lineno;
                            List<Match> matches = MatchCollection(oneLine, wordDelimiter);
                            
                            // debug
                            //foreach (Match mm in matches) {
                            //    output.Write("({0},{1}) ", mm.value,mm.position);
                            //}
                            //output.WriteLine();

                            string ts = transform(oneLine, matches);
                            output.WriteLine(ts);
                            // debug
                            // output.WriteLine("--------------------------------");
                        }
                    }
                  
                }
            } catch (Exception e) {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read at line {0}: ", lineno);
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("press any key to continue...");
            Console.ReadKey();
        }


        static int fail()
        {
            switch (state) {
                case 0: start = 10; break;
                case 10: start = 30; break;
                case 30: start = 40; break;
                case 40: start = 50; break;
                case 50: start = 100; break;
                case 100: start = 0; break;

                default: break;
            }
            return start;
        }

        static string a2s(string word)
        {
            // Encode the string.         
            int ccount = 0;
            // Repeatedly copy data until we've finished
            char[] output = new char[32];
         
            char nextChar = '\u0000';
            ProcessWord token = new ProcessWord(word);
            bool stillProcessing = true;

            while (stillProcessing) {
                try {
                    switch (state) {
                        case 0:
                            nextChar = token.getNextChar();
                            if (isCanonical(nextChar)) {
                                // Console.WriteLine("{0} is canonical", buffer[0]));
                                // direct conversion
                                output[ccount] = bb[nextChar];
                                ++ccount;
                            } else {
                                token.retract(1);
                                state = fail();
                            }
                            break;

                        case 10:
                            nextChar = token.getNextChar();
                            if (isDecomposable(nextChar)) {
                                // Console.WriteLine("{0:u} is decomposable", buffer[0]);
                                //  decompose and then convert each character
                                List<char> codes = decompose[nextChar];
                                foreach (char cc in codes) {
                                    // Arabic2Syriac = bb[cc];
                                    output[ccount] = cc;
                                    ++ccount;
                                }
                                state = 0;
                            } else {
                                token.retract(1);
                                state = fail();
                            }
                            break;

                        case 30:
                            nextChar = token.getNextChar();
                            if (isLamad(nextChar)) {
                                char more = token.getNextChar();
                                switch (more) {
                                    case '\u0627':
                                    case '\u0625':  //now pop and write out
                                        // first character
                                        output[ccount] = '\u0720';
                                        ++ccount;
                                       // second character
                                        output[ccount] = '\u0710';
                                        ++ccount;
                                        break;

                                    case '\u0623':  // waw rwakha
                                        output[ccount] = '\u073f';
                                        ++ccount;
                                        break;

                                    case '\u0622':  // waw rwasa
                                        output[ccount] = '\u073c';
                                        ++ccount;
                                        break;

                                    default: // emit lamad only 
                                        token.retract(1); // return 'more' characater back
                                        output[ccount] = bb[nextChar];
                                        ++ccount;
                                        break; 
                                }
                                state = 0;
                            } else {
                                token.retract(1);
                                state = fail();
                            }
                          
                            break;

                        case 40:
                            nextChar = token.getNextChar();
                            if (isQushaya(nextChar)) {
                                char more = token.getNextChar();
                                switch (more) {
                                    case '\u0650':  // qushaya                                 
                                        output[ccount] = '\u0741';
                                        ++ccount;
                                        break;

                                    case '\u064f':  // another syame
                                        output[ccount] = '\u0308';
                                        ++ccount;
                                        break;

                                    case '\u064b':  // anther zlame
                                        output[ccount] = '\u0739';
                                        ++ccount;
                                        break;


                                    default: // emit nextChar only 
                                        token.retract(1); // return 'more' characater back
                                        output[ccount] = bb[nextChar];
                                        ++ccount;
                                        break; 
                                }
                                state = 0;
                            } else {
                                token.retract(1);
                                state = fail();
                            }
                            break;
                        
                        case 50:    // tau + rukakha + syame + zqapa
                            nextChar = token.getNextChar();
                            if (isTau(nextChar))
                            {
                                char more = token.getNextChar();
                                switch (more) {
                                    case '\u0651':
                                        output[ccount] = bb[nextChar];  
                                        ++ccount;
                                        output[ccount] = '\u0742';
                                        ++ccount;
                                        output[ccount] = '\u0308';
                                        ++ccount;
                                        output[ccount] = '\u0735';
                                        ++ccount;
                                        break;
                                    
                                    default: // emit tau only; it should never occur
                                        token.retract(1);  // return 'more' characater back
                                        output[ccount] = bb[nextChar];
                                        ++ccount;
                                        break;
                                }
                                state = 0;
                            } else {
                                token.retract(1);
                                state = fail();
                            }
                            break;

                        case 100: // non-convertable characters, leave them alone
                            nextChar = token.getNextChar();
                            Console.WriteLine("{0:u} is unknown", nextChar);
                            output[ccount] = bb[nextChar];
                            ++ccount;

                            state = fail();
                            break;

                        default:  // should be non-reachable state!
                            break;
                    }
                } catch (KeyNotFoundException) {
                    //if key not found, don't convert 
                    output[ccount] = nextChar;
                    ++ccount;
                    state = 0;
                } catch (ConversionException ex) {
                   // basically, we are done parsing a word
                    stillProcessing = false;
                }
            }
            // just return the converted characters & trim the rest of the output buffer
            return (new string(output, 0, ccount)); 
        }
    }
}


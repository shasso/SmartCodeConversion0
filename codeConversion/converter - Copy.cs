using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

using CommandLine.Utility;

namespace codeConversion
{
    class Match
    {
        public string value { get; set; }
        public int position { get; set; }
        public Match(string word, int pos)
        {
            value = word;
            position = pos;
        }
    }

    class Program
    {
        const int BufferSize = 1;

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
            kap_maj.Add('\u0303');

            beth_rukakha.Add('\u0712');
            beth_rukakha.Add('\u0742');

            gamal_rukakha.Add('\u0713');
            gamal_rukakha.Add('\u0742');

            kap_rukakha.Add('\u071f');  // non-decomposable
            // kap_rukakha.Add('\u0742');

            gamal_maj.Add('\u0713');
            gamal_maj.Add('\u0303');

            youdh_kbasa.Add('\u071d'); // non-decomposable
            youdh_kbasa.Add('\u073c');

            peh_rukakha.Add('\u0726');
            peh_rukakha.Add('\u032e');

            tau_alaph.Add('\u072c');
            tau_alaph.Add('\u0710');

            he_qanona.Add('\u0717');
            he_qanona.Add('\u0307');

            meem_noon.Add('\u0721');
            meem_noon.Add('\u0307');
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
            canonicals.Add('\u0638');   // second tau

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
            // many2many.Add('\u0651');        // for rukakha

            // these are the second character combinations in addition to \u0644 or \u0651
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
            return cc == '\u0644' || cc == '\u0651';
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
        static List<Match> MatchCollection(string input)
        {
            List<Match> matches = new List<Match>();
            int start = input.IndexOf('<', 0);
            while (true) {
                if (start == -1) break;
                int begin = start;
                int end = input.IndexOf('>', start);
                int size = end - begin + 1;                      // count anything between '<' and '>'
                string w = input.Substring(begin, size);  // 
                matches.Add(new Match(w, start));
                start = input.IndexOf('<', end);
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

        static string a2s(string word)
        {
            // Encode the string.
            char[] buffer = new char[BufferSize];
            char Arabic2Syriac;
            bool inStateMachine = false;
            int ccount = 0;
            // Repeatedly copy data until we've finished
            char[] output = new char[32];

            foreach (char ii in word) {
                buffer[0] = ii;

                try {
                    if (isCanonical(buffer[0]) && !inStateMachine) {
                        // Console.WriteLine("{0} is canonical", buffer[0]));
                        // direct conversion
                        Arabic2Syriac = bb[buffer[0]];
                        output[ccount] = Arabic2Syriac;
                        ++ccount;
                    } else if (isDecomposable(buffer[0]) && !inStateMachine) {
                        // Console.WriteLine("{0:u} is decomposable", buffer[0]);
                        //  decompose and then convert each character
                        List<char> codes = decompose[buffer[0]];
                        foreach (char cc in codes) {
                            // Arabic2Syriac = bb[cc];
                            output[ccount] = cc;
                            ++ccount;
                        }
                    } else if (isMany2Many(buffer[0]) || inStateMachine) {
                        // Console.WriteLine("{0:} is man2many", buffer[0]);
                        if (!inStateMachine) {
                            inStateMachine = true; // we are now in state machine
                            sm.Enqueue(buffer[0]); // store the current character UNCONVERTED
                            continue;               // force another read
                        }
                        //we are here because of the second read after being in state machine
                        //check to see if the next character read is not in lam_alaph group
                        //which means it is just lam (\u0644) or just rukakha (\u0651)
                        // ALSO, check to see if the next character is decomposable, if so decompose and add!
                        //
                        if (!isLam_alaph(buffer[0]) )
                        {
                            char cc = sm.Dequeue();
                            output[ccount] = bb[cc]; // translate
                            ++ccount;
                            // next, check to see if next character is decomposable, then decompose and then convert each character
                            if (!isDecomposable(buffer[0]))
                            {
                                output[ccount] = bb[buffer[0]];  // translate
                                ++ccount;
                            }
                            else {
                                List<char> codes = decompose[buffer[0]];
                                foreach (char jj in codes)
                                {
                                    // Arabic2Syriac = bb[jj];
                                    output[ccount] = jj;
                                    ++ccount;
                                }
                            }
                        } else {
                            char cc;
                            switch (buffer[0]) {
                                case '\u0627':
                                case '\u0625':
                                    cc = sm.Dequeue(); // pop
                                    sm.Enqueue('\u0720'); // translate & re-insert
                                    sm.Enqueue('\u0710');
                                    //now pop and write out
                                    cc = sm.Dequeue(); // first character
                                    output[ccount] = cc;
                                    ++ccount;
                                    cc = sm.Dequeue(); // second character
                                    output[ccount] = cc;
                                    ++ccount;
                                    break;

                                case '\u0623':  // waw rwakha
                                    cc = sm.Dequeue(); // pop
                                    // sm.Enqueue('\u0718');   // conume
                                    sm.Enqueue('\u073f'); // translate and re-insert
                                    cc = sm.Dequeue(); // first character
                                    output[ccount] = cc;
                                    ++ccount;
                                    break;

                                case '\u0622':  // waw rwasa
                                    cc = sm.Dequeue(); // pop
                                    // sm.Enqueue('\u0718'); // consume
                                    sm.Enqueue('\u073c');   // translate and re-insert
                                    cc = sm.Dequeue(); // first character
                                    output[ccount] = cc;
                                    ++ccount;
                                    break;

                                //case '\u0650':  // qushaya
                                //    cc = sm.Dequeue(); // pop
                                   
                                //    sm.Enqueue('\u0741');   // translate and re-insert
                                //    cc = sm.Dequeue(); // first character
                                //    output[ccount] = cc;
                                //    ++ccount;
                                //    break;

                            }
                            ////now pop and write out
                            //cc = sm.Dequeue(); // first character
                            //output[ccount] = cc;
                            //++ccount;
                            //cc = sm.Dequeue(); // second character
                            //output[ccount] = cc;
                            //++ccount;
                        }
                        inStateMachine = false; // done processing
                    } else {
                        // non-convertable characters, leave them alone
                        Console.WriteLine("{0:u} is unknown", buffer[0]); ;
                        Arabic2Syriac = buffer[0];
                        output[ccount] = Arabic2Syriac;
                        ++ccount;
                    }


                } catch (KeyNotFoundException) {
                    //if key not found, don't convert 
                    Arabic2Syriac = buffer[0];
                    output[ccount] = Arabic2Syriac;
                    ++ccount;
                }
            }
            // just return the converted characters & trim the rest of the output buffer
            return (new string(output, 0, ccount)); 
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
                            // debug
                            // output.WriteLine("{0}: {1}", lineno, oneLine); 

                            // extract bracketed words
                            //string pattern = @"<\p{IsArabic}>";
                            //MatchCollection Matches = Regex.Matches(oneLine, pattern, RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                            //// write out all matches
                            //foreach (Match m in Matches) {
                            //    string w = m.Value;
                            //    // output.Write("{0}", m.Value);
                            //

                            List<Match> matches = MatchCollection(oneLine);
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
                    #region  Begin Processing one character at a time
                    //// Repeatedly copy data until we've finished
                    //while ((len = input.Read(buffer, 0, BufferSize)) > 0)
                    //{
                    //    try
                    //    {
                    //        if (canonicals.Contains(buffer[0]) && !inStateMachine)
                    //        {
                    //            // Console.WriteLine("{0} is canonical", buffer[0]));
                    //            // direct conversion
                    //            Arabic2Syriac = bb[buffer[0]];
                    //            output.Write(Arabic2Syriac);
                    //            ++ccount;
                    //        }
                    //        else if (decomposable.Contains(buffer[0]) && !inStateMachine)
                    //        {
                    //            // Console.WriteLine("{0:u} is decomposable", buffer[0]);
                    //            // decompose and then convert each character
                    //            List<char> codes = decompose[buffer[0]];
                    //            foreach (char cc in codes)
                    //            {
                    //                // Arabic2Syriac = bb[cc];
                    //                output.Write(cc);
                    //                ++ccount;
                    //            }
                    //        }

                    //        else if (many2many.Contains(buffer[0]) || inStateMachine)
                    //        {
                    //            // Console.WriteLine("{0:} is man2many", buffer[0]);
                    //            if (!inStateMachine)
                    //            {
                    //                inStateMachine = true; // we are now in state machine
                    //                sm.Enqueue(buffer[0]); // store the current character UNCONVERTED
                    //                continue;               // force another read
                    //            }
                    //            // we are here because of the second read after being in state machine
                    //            // check to see if the next character read is not in lam_alaph group
                    //            // which means it is just lam
                    //            if (!lam_alaph.Contains(buffer[0]))
                    //            {
                    //                char cc = sm.Dequeue();
                    //                output.Write(bb[cc]); // translate
                    //                ++ccount;
                    //                output.Write(bb[buffer[0]]);  // translate
                    //                ++ccount;
                    //            }
                    //            else
                    //            {
                    //                char cc;
                    //                switch (buffer[0])
                    //                {
                    //                    case '\u0627':
                    //                    case '\u0625':
                    //                        cc = sm.Dequeue(); // pop
                    //                        sm.Enqueue('\u0720'); // translate & re-insert
                    //                        sm.Enqueue('\u0710');
                    //                        break;

                    //                    case '\u0623':
                    //                        cc = sm.Dequeue(); // pop
                    //                        sm.Enqueue('\u0718'); // translate and re-insert
                    //                        sm.Enqueue('\u073f');
                    //                        break;

                    //                    case '\u0622':
                    //                        cc = sm.Dequeue(); // pop
                    //                        sm.Enqueue('\u0718'); // translate and re-insert
                    //                        sm.Enqueue('\u073c');
                    //                        break;
                    //                }
                    //                // now pop and write out
                    //                cc = sm.Dequeue(); // first character
                    //                output.Write(cc);
                    //                ++ccount;
                    //                cc = sm.Dequeue(); // second character
                    //                output.Write(cc);
                    //                ++ccount;
                    //            }
                    //            inStateMachine = false; // done processing
                    //        }
                    //        else
                    //        {
                    //            // non-convertable characters, leave them alone
                    //            // Console.WriteLine("{0:u} is unknown", buffer[0]); ;
                    //            Arabic2Syriac = buffer[0];
                    //            output.Write(Arabic2Syriac);
                    //            ++ccount;
                    //        }


                    //    }
                    //    catch (KeyNotFoundException)
                    //    {
                    //        // if key not found, don't convert 
                    //        Arabic2Syriac = buffer[0];
                    //        output.Write(Arabic2Syriac);
                    //        ++ccount;
                    //    }

                    //    // Console.Write(buffer, 0, len);
                    //    // output.Write(buffer, 0, len);

                    //}
                    #endregion Begin Processing one character at a time
                }
            } catch (Exception e) {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read at line {0}: ", lineno);
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("press any key to continue...");
            Console.ReadKey();
        }

    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matching
{
    public class Match
    {
        public string value { get; set; }
        public int position { get; set; }
        public Match(string word, int pos)
        {
            value = word;
            position = pos;
        }

        // input: a string of this format abc <word> cde <another>
        // output list of Match objects for <word>, <another>
        public static List<Match> MatchCollection(string input, Delimiter dm)
        {
            List<Match> matches = new List<Match>();
            int start = input.IndexOf(dm.START, 0);
            while (true)
            {
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
    }
}

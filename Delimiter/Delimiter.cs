using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matching
{
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
}

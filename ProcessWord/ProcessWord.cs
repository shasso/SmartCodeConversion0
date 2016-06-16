using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matching
{
    public class ConversionException : Exception
    {
        public ConversionException() { }
        public ConversionException(string msg)
            : base(msg)
        { }
        public ConversionException(string msg, Exception inner)
            : base(msg, inner) { }
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
            while (count-- > 0)
                if (currentPosition > 0)
                {
                    --currentPosition;
                }
                else
                    throw new ConversionException("invalid character retraction");
        }

        public char discard()
        {
            return _buffer[currentPosition++];
        }
    }
}

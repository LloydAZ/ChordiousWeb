// 
// ChordQuality.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2013, 2014 Jon Thysell <http://jonthysell.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Xml;

namespace com.jonthysell.Chordious.Core
{
    public class ChordQuality
    {
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException();
                }
                _name = value;
            }
        }
        private string _name;

        public string Abbreviation
        {
            get
            {
                return _abbreviation;
            }
            set
            {
                if (null == value)
                {
                    throw new ArgumentNullException();
                }
                _abbreviation = value;
            }
        }
        private string _abbreviation;

        public int[] Intervals
        {
            get
            {
                return _intervals;
            }
            set
            {
                if (null == value)
                {
                    throw new ArgumentNullException();
                }

                _intervals = value;
            }
        }
        private int[] _intervals;

        public ChordQuality(string name, string abbreviation, int[] intervals)
        {
            this.Name = name;
            this.Abbreviation = abbreviation;
            this.Intervals = intervals;
        }

        public ChordQuality(XmlNode qualityNode)
        {
            if (null == qualityNode)
            {
                throw new ArgumentNullException("qualityNode");
            }

            this.Name = qualityNode.Attributes["name"].Value;
            this.Abbreviation = qualityNode.Attributes["abbv"].Value;

            string steps = qualityNode.Attributes["steps"].Value;

            string[] s = steps.Split(';');

            int[] intervals = new int[s.Length];

            for (int i = 0; i < intervals.Length; i++)
            {
                intervals[i] = Int32.Parse(s[i]);
            }

            this.Intervals = intervals;
        }

        public InternalNote[] GetNotes(InternalNote root)
        {
            InternalNote[] notes = new InternalNote[Intervals.Length];

            for (int i = 0; i < notes.Length; i++)
            {
                notes[i] = NoteUtils.Shift(root, Intervals[i]);
            }

            return notes;
        }

        public string ToXml()
        {
            string intervals = "";

            for (int i = 0; i < Intervals.Length; i++)
            {
                intervals += Intervals[i] + ";";
            }

            intervals = intervals.TrimEnd(';');

            return String.Format("<quality name=\"{0}\" abbv=\"{1}\" steps=\"{2}\" />", Name, Abbreviation, intervals);
        }
    }
}
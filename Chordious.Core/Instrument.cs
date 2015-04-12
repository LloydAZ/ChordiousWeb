// 
// Instrument.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2013 Jon Thysell <http://jonthysell.com>
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
using System.Collections.Generic;
using System.Xml;

namespace com.jonthysell.Chordious.Core
{
    public class Instrument
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

        public int NumStrings
        {
            get
            {
                return _numStrings;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _numStrings = value;
            }
        }
        private int _numStrings;

        public IEnumerable<Tuning> Tunings
        {
            get
            {
                return _tunings;
            }
        }
        private List<Tuning> _tunings;

        public Instrument(string name, int numStrings)
        {
            this.Name = name;
            this.NumStrings = numStrings;
            _tunings = new List<Tuning>();
        }

        public Instrument(string instrumentLine)
        {
            if (String.IsNullOrEmpty(instrumentLine))
            {
                throw new ArgumentNullException("instrumentLine");
            }

            string[] s = instrumentLine.Trim().Split(';');

            this.Name = s[0];
            this.NumStrings = Int32.Parse(s[1]);
        }

        public Instrument(XmlNode instrumentNode)
        {
            if (null == instrumentNode)
            {
                throw new ArgumentNullException("instrumentNode");
            }

            this.Name = instrumentNode.Attributes["name"].Value;
            this.NumStrings = Int32.Parse(instrumentNode.Attributes["strings"].Value);
            _tunings = new List<Tuning>();

            XmlNodeList tuningNodes = instrumentNode.SelectNodes("./tuning");
            foreach (XmlNode tuningNode in tuningNodes)
            {
                _tunings.Add(new Tuning(this, tuningNode));
            }
        }

        public void AddTuning(string name, Note[] notes)
        {
            if (notes.Length != NumStrings)
            {
                throw new ArgumentOutOfRangeException("notes");
            }

            Tuning t = new Tuning(this, name, notes);

            _tunings.Add(t);
        }

        public void AddTuning(XmlNode tuningNode)
        {
            _tunings.Add(new Tuning(this, tuningNode));
        }

        public string ToXml()
        {
            string s = String.Format("<instrument name=\"{0}\" strings=\"{1}\" >", Name, NumStrings);

            if (_tunings.Count > 0)
            {
                foreach(Tuning t in Tunings)
                {
                    s += t.ToXml();
                }
            }

            s += "</instrument>";

            return s;
        }
    }
}
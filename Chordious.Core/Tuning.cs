// 
// Tuning.cs
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
using System.Xml;

namespace com.jonthysell.Chordious.Core
{
    public class Tuning
    {
        public Instrument Instrument { get; private set; }

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

        public string LongName
        {
            get
            {
                string notes = "";

                foreach (Note n in RootNotes)
                {
                    notes += NoteUtils.ToString(n);
                }

                return String.Format("{0} ({1})", Name, notes);
            }
        }

        public Note[] RootNotes
        {
            get
            {
                return _rootNotes;
            }
            set
            {
                if (null == value)
                {
                    throw new ArgumentNullException();
                }
                _rootNotes = value;
            }
        }
        private Note[] _rootNotes;

        public Tuning(Instrument instrument, string name, Note[] rootNotes)
        {
            if (null == instrument)
            {
                throw new ArgumentNullException("instrument");
            }

            this.Instrument = instrument;
            this.Name = name;
            this.RootNotes = rootNotes;
        }

        public Tuning(Instrument instrument, XmlNode tuningNode)
        {
            if (null == instrument)
            {
                throw new ArgumentNullException("instrument");
            }

            if (null == tuningNode)
            {
                throw new ArgumentNullException("tuningNode");
            }

            this.Instrument = instrument;

            this.Name = tuningNode.Attributes["name"].Value;

            string notes = tuningNode.Attributes["notes"].Value;

            string[] s = notes.Split(';');

            Note[] rootNotes = new Note[s.Length];

            if (rootNotes.Length != instrument.NumStrings)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < rootNotes.Length; i++)
            {
                rootNotes[i] = NoteUtils.ParseNote(s[i]);
            }
            this.RootNotes = rootNotes;
        }

        public InternalNote NoteAt(int str, int fret)
        {
            if (str < 0 || str >= RootNotes.Length)
            {
                throw new ArgumentOutOfRangeException("str");
            }

            if (fret < 0)
            {
                throw new ArgumentOutOfRangeException("fret");
            }

            return NoteUtils.Shift(NoteUtils.ToInternalNote(RootNotes[str]), fret);
        }

        public string ToXml()
        {
            string rootNotes = "";

            for (int i = 0; i < RootNotes.Length; i++)
            {
                rootNotes += NoteUtils.ToString(RootNotes[i]) + ";";
            }

            rootNotes = rootNotes.TrimEnd(';');

            return String.Format("<tuning name=\"{0}\" notes=\"{1}\" />", Name, rootNotes);
        }
    }
}
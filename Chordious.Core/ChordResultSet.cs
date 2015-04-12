// 
// ChordResultSet.cs
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

using System.Collections.Generic;

namespace com.jonthysell.Chordious.Core
{
    public class ChordResultSet
    {
        public Note Root { get; private set; }

        public ChordQuality ChordQuality { get; private set; }

        public ChordFinderOptions ChordFinderOptions { get; private set; }

        public int Count
        {
            get
            {
                return _results.Count;
            }
        }

        private List<ChordResult> _results;

        public ChordResultSet(Note root, ChordQuality chordQuality, ChordFinderOptions chordFinderOptions)
        {
            this.Root = root;
            this.ChordQuality = chordQuality;
            this.ChordFinderOptions = chordFinderOptions;
            this._results = new List<ChordResult>();
        }

        public void AddResult(int[] marks)
        {
            if (IsValid(marks))
            {
                if (ChordFinderOptions.MirrorResults)
                {
                    Array.Reverse(marks);
                }

                _results.Add(new ChordResult(marks));
                _results.Sort();
            }
        }

        public int[] ResultAt(int index)
        {
            ChordResult cr = _results[index];

            int[] marks = new int[cr.Marks.Length];
            Array.Copy(cr.Marks, marks, cr.Marks.Length);

            return marks;
        }

        public Chord ChordAt(int index)
        {
            int[] absoluteMarks = ResultAt(index);
            int baseLine;
            int[] marks = MarkUtils.AbsoluteToRelativeMarks(absoluteMarks, out baseLine, ChordFinderOptions.NumFrets);

            string title = NoteUtils.ToString(Root) + ChordQuality.Abbreviation;
            string fileName = title + index;

            int numStrings = marks.Length;

            int numFrets = ChordFinderOptions.NumFrets;
            int barre = ChordFinderOptions.AutoAddBarres ? -1 : 0;

            return new Chord(fileName, title, numStrings, numFrets, baseLine, barre, marks);
        }

        private bool IsValid(int[] marks)
        {
            bool reachPass = MarkUtils.Reach(marks) <= ChordFinderOptions.MaxReach;
            bool openPass = ChordFinderOptions.AllowOpenStrings ? true : !MarkUtils.HasOpenStrings(marks);
            bool mutePass = ChordFinderOptions.AllowMutedStrings ? true : !MarkUtils.HasMutedStrings(marks);

            return reachPass && openPass && mutePass;
        }

        protected class ChordResult : IComparable
        {
            public int[] Marks { get; private set; }

            public ChordResult(int[] marks)
            {
                if (null == marks)
                {
                    throw new ArgumentNullException("marks");
                }

                this.Marks = marks;
            }
    
            public int CompareTo(object obj)
            {
                if (null == obj)
                {
                    throw new ArgumentException();
                }
    
                ChordResult cfr = obj as ChordResult;
                if ((object)cfr == null)
                {
                    throw new ArgumentException();
                }

                return MarkUtils.Compare(this.Marks, cfr.Marks);
            }

            public override string ToString()
            {
                return MarkUtils.ToString(this.Marks);
            }
        }
    }
}
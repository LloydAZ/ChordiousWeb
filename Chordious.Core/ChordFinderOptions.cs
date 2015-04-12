// 
// ChordFinderOptions.cs
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

namespace com.jonthysell.Chordious.Core
{
    public class ChordFinderOptions
    {
        public int NumFrets
        {
            get
            {
                return _numFrets;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _numFrets = value;
            }
        }
        private int _numFrets;

        public int MaxFret
        {
            get
            {
                return _maxFret;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _maxFret = value;
            }
        }
        private int _maxFret;

        public int MaxReach
        {
            get
            {
                return _maxReach;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _maxReach = value;
            }
        }
        private int _maxReach;

        public bool AutoAddBarres { get; set; }

        public bool AllowOpenStrings { get; set; }

        public bool AllowMutedStrings { get; set; }

        public bool AllowRootlessChords { get; set; }

        public bool MirrorResults { get; set; }

        public ChordFinderOptions() : this(DefaultNumFrets, DefaultMaxFret, DefaultMaxReach, DefaultAutoAddBarres, DefaultAllowOpenStrings, DefaultAllowMutedStrings, DefaultAllowRootlessChords, DefaultMirrorResults) {}

        public ChordFinderOptions(int numFrets, int maxFret, int maxReach, bool autoAddBarres, bool allowOpenStrings, bool allowMutedStrings, bool allowRootlessChords, bool mirrorResults)
        {
            if (numFrets < 0)
            {
                throw new ArgumentOutOfRangeException("numFrets");
            }

            if (maxFret < 0)
            {
                throw new ArgumentOutOfRangeException("maxFret");
            }

            if (maxReach < 0)
            {
                throw new ArgumentOutOfRangeException("maxReach");
            }

            if (numFrets < maxReach)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.NumFrets = numFrets;
            this.MaxFret = maxFret;
            this.MaxReach = maxReach;
            this.AutoAddBarres = autoAddBarres;
            this.AllowOpenStrings = allowOpenStrings;
            this.AllowMutedStrings = allowMutedStrings;
            this.AllowRootlessChords = allowRootlessChords;
            this.MirrorResults = mirrorResults;
        }

        public static int DefaultNumFrets = 5;
        public static int DefaultMaxFret = 12;
        public static int DefaultMaxReach = 4;
        public static bool DefaultAutoAddBarres = true;
        public static bool DefaultAllowOpenStrings = true;
        public static bool DefaultAllowMutedStrings = true;
        public static bool DefaultAllowRootlessChords = false;
        public static bool DefaultMirrorResults = false;
    }
}


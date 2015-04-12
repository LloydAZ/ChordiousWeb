// 
// Chord.cs
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
using System.Text;

namespace com.jonthysell.Chordious.Core
{
    public class Chord : ILineItem
    {
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (null == value)
                {
                    throw new ArgumentNullException();
                }
                _title = value;
            }
        }
        private string _title;

        public string FileName
        {
            get
            {
                if (String.IsNullOrEmpty(this._fileName))
                {
                    return this.Title;
                }
                else
                {
                    return this._fileName;
                }
            }
            set
            {
                this._fileName = value;
            }
        }
        private string _fileName;

        public bool FileNameSet
        {
            get
            {
                return !String.IsNullOrEmpty(this._fileName);
            }
        }

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

                // Init _marks
                if (null == _marks)
                {
                    _marks = new int[_numStrings];
                }
                else if (_marks.Length != _numStrings)
                {
                    int[] temp = new int[_numStrings];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = i < _marks.Length ? _marks[i] : 0;
                    }
                    _marks = temp;
                }
            }
        }
        private int _numStrings;

        public int NumFrets
        {
            get
            {
                return _numFrets;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _numFrets = value;
            }
        }
        private int _numFrets;

        public int BaseLine
        {
            get
            {
                return _baseLine;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _baseLine = value;
            }
        }
        private int _baseLine;

        public int Barre
        {
            get
            {
                return _barre;
            }
            set
            {
                _barre = Math.Max(-1, value);
            }
        }
        private int _barre;

        public int[] Marks
        {
            get
            {
                return _marks;
            }
            set
            {
                if (null == value)
                {
                    throw new ArgumentNullException();
                }

                // Init _marks
                if (null == _marks)
                {
                    _marks = new int[_numStrings];
                }

                for (int i = 0; i < _marks.Length; i++)
                {
                    _marks[i] = i < value.Length ? value[i] : 0;
                }
            }
        }
        private int[] _marks;

        public Chord(string fileName, string title, int numStrings, int numFrets, int baseLine, int barre, int[] marks)
        {
            this.Title = title;
            this.FileName = fileName;
            this.NumStrings = numStrings;
            this.NumFrets = numFrets;
            this.BaseLine = baseLine;
            this.Barre = barre;
            this.Marks = marks;
        }

        public Chord(string chordLine)
        {
            if (String.IsNullOrEmpty(chordLine))
            {
                throw new ArgumentNullException("chordLine");
            }

            string[] s = chordLine.Trim().Split(';');

            string[] name_parts = s[0].Split(':');

            string fileName = name_parts.Length > 1 ? name_parts[0] : "";
            string title = name_parts.Length > 1 ? name_parts[1] : name_parts[0];

            this.Title = title;
            this.FileName = fileName;

            this.NumStrings = Int32.Parse(s[1]);
            this.NumFrets = Int32.Parse(s[2]);
            this.BaseLine = Int32.Parse(s[3]);
            this.Barre = Int32.Parse(s[4]);

            int[] marks = new int[(int)Math.Min(this.NumStrings, s.Length - 5)];
            for (int i = 0; i < marks.Length; i++)
            {
                marks[i] = Int32.Parse(s[5 + i]);
            }
            this.Marks = marks;
        }

        public void CopyTo(Chord c)
        {
            if (null == c)
            {
                throw new ArgumentNullException("c");
            }

            c.Title = this.Title;
            c.FileName = this.FileNameSet ? this._fileName : "";
            c.NumStrings = this.NumStrings;
            c.NumFrets = this.NumFrets;
            c.BaseLine = this.BaseLine;
            c.Barre = this.Barre;
            c.Marks = (int[])this.Marks.Clone();
        }

        public string ToSvg(ChordOptions chordOptions)
        {
            StringBuilder sb = new StringBuilder("");

            // Chart Title
            if (!String.IsNullOrEmpty(Title))
            {
                string chord = Title[0].ToString();
                string chordModifier = Title.Substring(1);
                double modifierFontSize = chordOptions.FontSize * 0.75;

                string title = chord + String.Format(TSPAN, modifierFontSize, chordModifier);

                string fontStyle = "";

                if (chordOptions.FontStyle == FontStyle.Bold ||
                    chordOptions.FontStyle == FontStyle.BoldItalic)
                {
                    fontStyle += "font-weight:bold;";
                }

                if (chordOptions.FontStyle == FontStyle.Italic ||
                    chordOptions.FontStyle == FontStyle.BoldItalic)
                {
                    fontStyle += "font-style:italic;";
                }

                double titleX = chordOptions.Width / 2.0;
                double titleY = chordOptions.Margin + (chordOptions.FontSize / 2.0);

                sb.AppendFormat(TEXT,
                                chordOptions.FontSize,
                                chordOptions.FontFamily,
                                fontStyle,
                                "middle",
                                titleX,
                                titleY,
                                title);
            }

            //TODO Add option to remove title spacing when title is absent
            double titleHeight = chordOptions.FontSize;

            // Rect
            double rectWidth = chordOptions.Width - (2.0 * chordOptions.Margin);
            double rectHeight = chordOptions.Height - titleHeight - (2.0 * chordOptions.Margin);
            double rectX = chordOptions.Margin;
            double rectY = chordOptions.Margin + titleHeight;
            sb.AppendFormat(BASE,
                            chordOptions.StrokeWidth,
                            rectWidth,
                            rectHeight,
                            rectX,
                            rectY);

            // Vertical Lines
            double vSpacing = rectWidth / (NumStrings - 1);
            for (int vLine = 1; vLine < NumStrings - 1; vLine++)
            {
                double x = rectX + (vLine * vSpacing);
                double y1 = rectY;
                double y2 = y1 + rectHeight;
                sb.AppendFormat(LINE,
                                chordOptions.StrokeWidth,
                                x,
                                y1,
                                x,
                                y2);
            }

            // Horizontal Lines
            double hSpacing = rectHeight / NumFrets;
            for (int hLine = 1; hLine < NumFrets; hLine++)
            {
                double x1 = rectX;
                double x2 = rectX + rectWidth;
                double y = rectY + (hLine * hSpacing);

                sb.AppendFormat(LINE,
                                chordOptions.StrokeWidth,
                                x1,
                                y,
                                x2,
                                y);
            }

            double strokeCorrection = chordOptions.StrokeWidth / 2.0;
            double markRadius = Math.Min(vSpacing, hSpacing) / 3.0;
            double muteRadius = markRadius / 2.0;

            // Baseline or Marker Number
            if (BaseLine == 0)
            {
                // Add thicker baseline
                double x1 = rectX - strokeCorrection;
                double x2 = rectX + rectWidth + strokeCorrection;
                double y = rectY - strokeCorrection;

                sb.AppendFormat(LINE,
                                chordOptions.StrokeWidth * 2.0,
                                x1,
                                y,
                                x2,
                                y);
            }
            else if (BaseLine > 1)
            {
                // Add marker number
                double nFontSize = hSpacing * 0.75;

                double nX = rectX + rectWidth + (chordOptions.Margin / 4.0);
                double nY = rectY - strokeCorrection + ((hSpacing + nFontSize) / 2.0);

                sb.AppendFormat(TEXT,
                                nFontSize,
                                chordOptions.FontFamily,
                                "",
                                "left",
                                nX,
                                nY,
                                BaseLine);
            }

            // Marks

            int firstMark = -1;
            int lastMark = -1;

            bool onBarre = false;

            for (int mark = 0; mark < Marks.Length; mark++)
            {
                double mX = rectX + (vSpacing * mark);
                double mY = rectY + (hSpacing / 2.0) + (hSpacing * (Marks[mark] - 1));

                if (Marks[mark] > 0 && Marks[mark] <= NumFrets) // Mark
                {
                    if (!onBarre)
                    {
                        firstMark = mark;
                        onBarre = true;
                    }

                    if (onBarre)
                    {
                        if (Marks[firstMark] > Marks[mark])
                        {
                            firstMark = mark;
                        }
                        lastMark = mark;
                    }

                    sb.AppendFormat(SOLID_CIRCLE,
                                    markRadius,
                                    mX,
                                    mY);
                }
                else
                {
                    onBarre = false; // Hit an unmarked string, restart the barre

                    mY = rectY - (hSpacing / 2.0); // Align above top line

                    if (Marks[mark] == 0) // Open string
                    {
                        if (chordOptions.OpenStringType == OpenStringType.Circle)
                        {
                            sb.AppendFormat(OPEN_CIRCLE,
                                            chordOptions.StrokeWidth,
                                            muteRadius,
                                            mX,
                                            mY);
                        }
                    }
                    else if (Marks[mark] < 0) // Muted string
                    {
                        sb.AppendFormat(LINE,
                                        chordOptions.StrokeWidth,
                                        mX - muteRadius, // top left
                                        mY - muteRadius, // top left
                                        mX + muteRadius, // bottom right
                                        mY + muteRadius); // bottom right
    
                        sb.AppendFormat(LINE,
                                        chordOptions.StrokeWidth,
                                        mX - muteRadius, // bottom left
                                        mY + muteRadius, // bottom left
                                        mX + muteRadius, // top right
                                        mY - muteRadius); // top right
                    }
                }
            }

            // Barre
            int barreFret = Barre;

            if (barreFret < 0) // Calculate auto-barre
            {
                if (firstMark >= 0 && lastMark >= 0 && firstMark != lastMark)
                {
                    barreFret = Math.Min(Marks[firstMark], Marks[lastMark]);
                }
                else
                {
                    barreFret = 0;
                }
            }

            if (barreFret != 0 && barreFret <= NumFrets)
            {
                double bStartX = rectX + (chordOptions.FullBarres ? 0 : (firstMark * vSpacing));
                double bEndX = rectX + (chordOptions.FullBarres ? rectWidth : (lastMark * vSpacing));

                double bY = rectY + (hSpacing / 2.0) + (hSpacing * (barreFret - 1));

                if (chordOptions.BarreType == BarreType.Arc)
                {
                    double bRadiusX = rectWidth;
                    double bRadiusY = hSpacing;

                    sb.AppendFormat(ARC,
                                    chordOptions.StrokeWidth,
                                    bStartX,
                                    bY,
                                    bRadiusX,
                                    bRadiusY,
                                    bEndX,
                                    bY);
                }
                else if (chordOptions.BarreType == BarreType.Straight)
                {
                    sb.AppendFormat(LINE,
                                chordOptions.StrokeWidth,
                                bStartX,
                                bY,
                                bEndX,
                                bY);
                }
            }

            return String.Format(SVG,
                                 chordOptions.Width,
                                 chordOptions.Height,
                                 sb.ToString().Trim());
        }

        public override string ToString()
        {
            string s = "";

            if (!String.IsNullOrEmpty(_fileName))
            {
                s += _fileName + ":";
            }

            s += Title + ";";

            s += String.Format("{0};{1};{2};{3}", NumStrings, NumFrets, BaseLine, Barre);

            for (int i = 0; i < this.Marks.Length; i++)
            {
                s+= ";" + this.Marks[i];
            }

            return s;
        }

        private const string SVG = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<svg xmlns=""http://www.w3.org/2000/svg"" version=""1.1"" width=""{0}"" height=""{1}"">
{2}
</svg>";

        private const string TEXT = @"<text style=""font-size:{0}px;fill:#000000;opacity:1;font-family:{1};{2}text-anchor:{3}"" x=""{4}"" y=""{5}"">{6}</text>";

        private const string TSPAN = @"<tspan style=""font-size:{0}px;"">{1}</tspan>";

        private const string BASE = @"<rect style=""fill-opacity:0;stroke:#000000;stroke-width:{0}"" width=""{1}"" height=""{2}"" x=""{3}"" y=""{4}"" />";

        private const string LINE = @"<line style=""stroke:#000000;stroke-width:{0}"" x1=""{1}"" y1=""{2}"" x2=""{3}"" y2=""{4}"" />";

        // Red Solid Circle.
        //private const string SOLID_CIRCLE = @"<circle style=""fill:#FF0000;opacity:1"" r=""{0}"" cx=""{1}"" cy=""{2}"" />";

        // Black Solid Circle.
        private const string SOLID_CIRCLE = @"<circle style=""fill:#000000;opacity:1"" r=""{0}"" cx=""{1}"" cy=""{2}"" />";

        private const string OPEN_CIRCLE = @"<circle style=""fill-opacity:0;stroke:#000000;stroke-width:{0}"" r=""{1}"" cx=""{2}"" cy=""{3}"" />";

        private const string ARC = @"<path style=""fill-opacity:0;stroke:#000000;stroke-width:{0};opacity:1"" d=""M {1} {2} A {3} {4} 0 0 1 {5} {6}"" />";
    }
}

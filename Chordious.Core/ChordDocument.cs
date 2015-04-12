//
// ChordDocument.cs
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
using System.IO;

namespace com.jonthysell.Chordious.Core
{
    public class ChordDocument
    {
        public string FileName
        {
            get
            {
                if (String.IsNullOrEmpty(_filePath))
                {
                    return "";
                }
                else
                {
                    return Path.GetFileName(_filePath);
                }
            }
            private set
            {
                _filePath = String.IsNullOrEmpty(value) ? "" : value;
            }
        }
        private string _filePath;

        public bool IsDirty { get; private set; }

        public int Count
        {
            get
            {
                return _lineItems.Count;
            }
        }

        public ChordDocument()
        {
            _filePath = "";

            _lineItems = new List<ILineItem>();

            IsDirty = false;
        }

        public ChordDocument(string inputFile)
        {
            if (String.IsNullOrEmpty(inputFile))
            {
                throw new ArgumentNullException("inputFile");
            }

            _filePath = inputFile;

            _lineItems = new List<ILineItem>();

            IsDirty = false;

            using (StreamReader sr = new StreamReader(inputFile))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    try
                    {
                        if (line.StartsWith(Comment.CommentPrefix)) // line is a comment
                        {
                            _lineItems.Add(new Comment(line));
                        }
                        else if (line.StartsWith(ChordOptions.OptionsPrefix)) // line modifies the options
                        {
                            _lineItems.Add(new ChordOptions(line));
                        }
                        else if (!String.IsNullOrEmpty(line)) // treat line as a chord
                        {
                            _lineItems.Add(new Chord(line));
                        }
                    }
                    catch (Exception)
                    {
                        _lineItems.Add(new Comment(line));
                        IsDirty = true;
                    }
                }
            }
        }

        public void Save()
        {
            if (String.IsNullOrEmpty(_filePath))
            {
                throw new FileNotFoundException();
            }

            SaveAs(_filePath);
        }

        public void SaveAs(string outputFile)
        {
            if (String.IsNullOrEmpty(outputFile))
            {
                throw new ArgumentNullException("outputFile");
            }
            _filePath = outputFile;

            using (StreamWriter sw = new StreamWriter(_filePath))
            {
                foreach (ILineItem lineItem in _lineItems)
                {
                    sw.WriteLine(lineItem.ToString());
                }
                IsDirty = false;
            }
        }

        public void ExportChords(string outputDir)
        {
            ExportChords(outputDir, DefaultSvgWriter);
        }

        public void ExportChords(string outputDir, SvgWriter svgWriter)
        {
            if (null == svgWriter)
            {
                throw new ArgumentNullException("svgWriter");
            }

            for (int i = 0; i < Count; i++)
            {
                ExportChord(i, outputDir, svgWriter);
            }
        }

        public void ExportChord(int index, string outputDir)
        {
            ExportChord(index, outputDir, DefaultSvgWriter);
        }

        public void ExportChord(int index, string outputDir, SvgWriter svgWriter)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (null == svgWriter)
            {
                throw new ArgumentNullException("svgWriter");
            }

            if (String.IsNullOrEmpty(outputDir))
            {
                outputDir = "./";
            }

            if (_lineItems[index] is Chord)
            {
                Chord c = (Chord)_lineItems[index];
                string svgText = c.ToSvg(GetOptionsForLine(index));

                string fileName = Path.Combine(outputDir, c.FileName + ".svg");

                svgWriter(svgText, fileName);
            }
        }

        public ChordOptions GetOptionsForLine(int index)
        {
            ChordOptions co = new ChordOptions();
            if (index >= 0 && index < Count)
            {
                for(int i = 0; i <= index; i++)
                {
                    ILineItem lineItem = GetLine(i);
                    if (lineItem is ChordOptions)
                    {
                        co = (ChordOptions)lineItem;
                    }
                }
            }
            return co;
        }

        public ILineItem GetLine(int index)
        {
            return _lineItems[index];
        }

        public void MoveLineUp(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (index > 0)
            {
                ILineItem temp = _lineItems[index - 1];
                _lineItems[index - 1] = _lineItems[index];
                _lineItems[index] = temp;
                IsDirty = true;
            }
        }

        public void MoveLineDown(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (index < Count - 1)
            {
                ILineItem temp = _lineItems[index + 1];
                _lineItems[index + 1] = _lineItems[index];
                _lineItems[index] = temp;
                IsDirty = true;
            }
        }

        public void ToggleComment(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (_lineItems[index] is Comment) // Uncomment it
            {
                _lineItems[index] = ((Comment)_lineItems[index]).GetUncommented();
            }
            else // Comment it
            {
                _lineItems[index] = new Comment(_lineItems[index].ToString());
            }
            IsDirty = true;
        }

        public void AddLine(ILineItem lineItem)
        {
            _lineItems.Add(lineItem);
            IsDirty = true;
        }

        public void DeleteLine(int index)
        {
            _lineItems.RemoveAt(index);
            IsDirty = true;
        }

        public void UpdateLine(int index, ILineItem lineItem)
        {
            _lineItems[index] = lineItem;
            IsDirty = true;
        }

        public delegate void SvgWriter(string svgText, string fileName);

        public static void DefaultSvgWriter(string svgText, string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(svgText);
            }
        }

        private List<ILineItem> _lineItems;
    }
}
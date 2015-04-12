// 
// Comment.cs
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

namespace com.jonthysell.Chordious.Core
{
    public class Comment : ILineItem
    {
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = String.IsNullOrEmpty(value) ? "" : value;
            }
        }
        private string _text;

        public Comment(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                Text = text.TrimStart(CommentPrefix[0]).TrimEnd();
            }
        }

        public Comment() : this("") { }

        public ILineItem GetUncommented()
        {
            if (String.IsNullOrEmpty(Text.Trim()))
            {
                throw new ArgumentNullException("Text");
            }

            if (Text.StartsWith(ChordOptions.OptionsPrefix)) // Presume is ChordOptions
            {
                return new ChordOptions(Text);
            }
            else // Presume is Chord
            {
                return new Chord(Text);
            }
        }

        public override string ToString()
        {
            string s = "";

            s += CommentPrefix;

            if (!String.IsNullOrEmpty(Text))
            {
                s+= Text;
            }

            return s;
        }

        public static string CommentPrefix = "#";
    }
}
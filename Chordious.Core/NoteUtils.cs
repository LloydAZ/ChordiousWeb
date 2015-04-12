// 
// NoteUtils.cs
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
    public class NoteUtils
    {
        public static Note ParseNote(string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException("s");
            }

            return ((Note)Enum.Parse(typeof(Note), s.Replace("#", "s")));
        }

        public static string ToString(Note note)
        {
            return note.ToString().Replace("s","#");
        }

        public static InternalNote ToInternalNote(Note note)
        {
            switch (note)
            {
                case Note.C:
                    return InternalNote.C;
                case Note.Cs:
                case Note.Db:
                    return InternalNote.Cs_Db;
                case Note.D:
                    return InternalNote.D;
                case Note.Ds:
                case Note.Eb:
                    return InternalNote.Ds_Eb;
                case Note.E:
                    return InternalNote.E;
                case Note.F:
                    return InternalNote.F;
                case Note.Fs:
                case Note.Gb:
                    return InternalNote.Fs_Gb;
                case Note.G:
                    return InternalNote.G;
                case Note.Gs:
                case Note.Ab:
                    return InternalNote.Gs_Ab;
                case Note.A:
                    return InternalNote.A;
                case Note.As:
                case Note.Bb:
                    return InternalNote.As_Bb;
                case Note.B:
                    return InternalNote.B;
            }

            throw new ArgumentException();
        }

        public static InternalNote Shift(InternalNote root, int steps)
        {
            int newNote = ((int)root + steps);

            while (newNote < 0)
            {
                newNote += 12;
            }

            return (InternalNote)(newNote % 12);
        }
    }

    public enum Note
    {
        C = 0,
        Cs,
        Db,
        D,
        Ds,
        Eb,
        E,
        F,
        Fs,
        Gb,
        G,
        Gs,
        Ab,
        A,
        As,
        Bb,
        B
    }

    public enum InternalNote
    {
        C = 0,
        Cs_Db,
        D,
        Ds_Eb,
        E,
        F,
        Fs_Gb,
        G,
        Gs_Ab,
        A,
        As_Bb,
        B
    }
}
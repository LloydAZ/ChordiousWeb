// 
// ConfigFile.cs
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
    public class ConfigFile
    {

        public string this[string name]
        {
            get
            {
                return Get(name);
            }
            set
            {
                Set(name, value);
            }
        }
        private Dictionary<string, string> _settings;

        public IDictionary<string, Instrument> Instruments
        {
            get
            {
                return _instruments;
            }
        }
        private Dictionary<string, Instrument> _instruments;

        public IDictionary<string, ChordQuality> ChordQualities
        {
            get
            {
                return _chordQualities;
            }
        }
        private Dictionary<string, ChordQuality> _chordQualities;

        public ConfigFile()
        {
            this._settings = new Dictionary<string, string>();
            this._instruments = new Dictionary<string, Instrument>();
            this._chordQualities = new Dictionary<string, ChordQuality>();
        }

        public ConfigFile(string fileName) : this()
        {
            LoadFile(fileName);
        }

        public void LoadFile(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName");
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            XmlNode root = doc.DocumentElement;

            // Load settings
            XmlNodeList xmlNodes = root.SelectNodes("./settings/setting");

            foreach (XmlNode xmlNode in xmlNodes)
            {
                string key = xmlNode.Attributes["name"].Value.ToLower();
                string val = xmlNode.Attributes["value"].Value;

                if (_settings.ContainsKey(key))
                {
                    _settings[key] = val;
                }
                else
                {
                    _settings.Add(key, val);
                }
            }

            // Load instruments
            xmlNodes = root.SelectNodes("./instruments/instrument");

            foreach (XmlNode node in xmlNodes)
            {
                Instrument instrument = new Instrument(node);
                AddInstrument(instrument);
            }

            // Load chord qualities
            xmlNodes = root.SelectNodes("./qualities/quality");

            foreach (XmlNode node in xmlNodes)
            {
                ChordQuality quality = new ChordQuality(node);
                _chordQualities.Add(quality.Name.ToLower().Trim(), quality);
            }
        }

        public void Set(string name, string @value)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (String.IsNullOrEmpty(@value))
            {
                throw new ArgumentNullException("value");
            }

            name = name.ToLower();

            if (_settings.ContainsKey(name))
            {
                _settings[name] = @value;
            }
            else
            {
                _settings.Add(name, @value);
            }
        }

        public string Get(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            name = name.ToLower();

            if (!_settings.ContainsKey(name))

            if (_settings.ContainsKey(name))
            {
                return _settings[name];
            }

            return null;
        }

        public void AddInstrument(Instrument instrument)
        {
            if (null == instrument)
            {
                throw new ArgumentNullException("instrument");
            }

            _instruments.Add(instrument.Name.ToLower().Trim(), instrument);
        }
    }
}
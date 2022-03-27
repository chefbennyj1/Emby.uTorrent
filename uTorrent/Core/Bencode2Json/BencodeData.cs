//MIT License

//Copyright(c) 2017 Vijayshinva Karnure

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace uTorrent.Core.Bencode2Json
{
    public class BencodedData
    {
        private BinaryReader reader;
        private Stream stream;

        public BencodedData(Stream stream)
        {
            this.stream = stream;
            reader = new BinaryReader(stream, Encoding.UTF8);
        }

        public string ConvertToJson()
        {
            var o = ParseBencodedObject();
            reader.BaseStream.Position = 0;
            return o.ToJson();
        }

        IBencodedObject ParseBencodedObject()
        {
            switch (reader.PeekChar())
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return ParseBencodedString();
                case 'i':
                    return ParseBencodedInteger();
                case 'l':
                    return ParseBencodedList();
                case 'd':
                    return ParseBencodedDictionary();
                default:
                    return null;
            }
        }

        int ParseBencodedStringLength()
        {
            StringBuilder lengthBuilder = new StringBuilder();
            do
            {
                lengthBuilder.Append(reader.ReadChar());
            } while (reader.PeekChar() > 0 && reader.PeekChar() != ':');
            reader.ReadChar();
            return int.Parse(lengthBuilder.ToString());
        }

        BencodedString ParseBencodedString()
        {
            var length = ParseBencodedStringLength();
            byte[] buffer = new byte[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = reader.ReadByte();
            }
            return new BencodedString(buffer);
            //return new BencodedString(reader.ReadChars(length)); TODO : ReadChars doesnt work for some reason.
        }

        BencodedDictionary ParseBencodedDictionary()
        {
            if (reader.ReadChar() != 'd')
            {
                throw new Exception();
            }
            Dictionary<BencodedString, IBencodedObject> dictBuilder = new Dictionary<BencodedString, IBencodedObject>();
            do
            {
                var key = ParseBencodedString();
                var value = ParseBencodedObject();
                dictBuilder.Add(key, value);
            } while (reader.PeekChar() > 0 && reader.PeekChar() != 'e');
            reader.ReadChar();
            return new BencodedDictionary(dictBuilder);
        }

        BencodedInteger ParseBencodedInteger()
        {
            if (reader.ReadChar() != 'i')
            {
                throw new Exception();
            }
            StringBuilder integerBuilder = new StringBuilder();
            do
            {
                integerBuilder.Append(reader.ReadChar());
            } while (reader.PeekChar() > 0 && reader.PeekChar() != 'e');
            reader.ReadChar();
            return new BencodedInteger(integerBuilder.ToString());
        }
        BencodedList ParseBencodedList()
        {
            if (reader.ReadChar() != 'l')
            {
                throw new Exception();
            }
            List<IBencodedObject> listBuilder = new List<IBencodedObject>();
            do
            {
                listBuilder.Add(ParseBencodedObject());
            } while (reader.PeekChar() > 0 && reader.PeekChar() != 'e');
            reader.ReadChar();
            return new BencodedList(listBuilder);
        }
    }
}
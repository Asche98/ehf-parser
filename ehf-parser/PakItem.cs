using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ehf_parser
{
    class PakItem
    {
        public PakItem()
        {

        }

        public PakItem(byte[] data)
        {
            _data = data;
        }

        public bool Parse(BinaryReader br)
        {
            long old_seek = br.BaseStream.Position;

            _name = new string(br.ReadChars(HeaderSize - 8));
            _name = _name.Remove(_name.IndexOf('\0'));
            _seek = br.ReadUInt32();
            _size = br.ReadUInt32();

            br.BaseStream.Seek((int)_seek, SeekOrigin.Begin);
            _data = br.ReadBytes((int)_size);

            br.BaseStream.Seek((int)old_seek, SeekOrigin.Begin);

            return true;
        }

        public void Dump(string fname)
        {
            if (string.IsNullOrEmpty(fname))
                fname = _name.Substring(_name.LastIndexOf('/') + 1);
            using (Stream stm = File.OpenWrite(fname))
                stm.Write(_data, 0, (int)_size);
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name) && !string.IsNullOrEmpty(_file_name))
                {
                    _name = _file_name.Replace(Directory.GetCurrentDirectory() + '\\', "").Replace('\\', '/');
                }
                return _name;
            }
            set { _name = value; }
        }

        public string FileName
        {
            get { return _file_name; }
            set { _file_name = value; }
        }

        public UInt32 Seek
        {
            get { return _seek; }
            set { _seek = value; }
        }

        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public UInt32 Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public const int HeaderSize = 64;
        string _name;
        UInt32 _seek;
        UInt32 _size;
        byte[] _data;
        string _file_name;
    }
}

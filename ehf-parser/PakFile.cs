using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace ehf_parser
{
    class PakFile:IEnumerable
    {
        const int HeaderSize = 64;

        byte[] data;
        List<PakItem> _items = new List<PakItem>();


        public void load(string fname)
        {
            using (Stream stm = File.OpenRead(fname))
            {
                data = new byte[stm.Length];
                stm.Read(data, 0, (int)stm.Length);
            }
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (var br = new BinaryReader(stream))
                {
                    string pack = new string(br.ReadChars(4));
                    if (pack != "PACK")
                        throw new ApplicationException("Invalid pak file: 'PACK' signature not found");
                    UInt32 seek = br.ReadUInt32();
                    UInt32 size = br.ReadUInt32() / PakItem.HeaderSize;
                    br.BaseStream.Seek(seek, SeekOrigin.Begin);
                    for (int i = 0; i < size; i++)
                    {
                        PakItem item = new PakItem();
                        if (item.Parse(br))
                        {
                            _items.Add(item);
                            br.BaseStream.Seek(PakItem.HeaderSize, SeekOrigin.Current);
                        }
                    }
                }
            }
        }

        public void save(string fname)
        {
            using (var stm = File.OpenWrite(fname))
            {
                UInt32 seek = 12;
                using (var bw = new BinaryWriter(stm))
                {
                    bw.Write("PACK".ToCharArray());
                    bw.Write((int)0);
                    bw.Write(_items.Count * HeaderSize);
                    foreach (var pak in _items)
                    {
                        byte[] buf = null;
                        if (pak.FileName != null)
                        {
                            using (var rstm = File.OpenRead(pak.FileName))
                            {
                                buf = new byte[rstm.Length];
                                rstm.Read(buf, 0, (int)rstm.Length);
                            }
                        }
                        else
                        {
                            buf = pak.Data;
                        }
                        bw.Write(buf);
                        pak.Seek = seek;
                        pak.Size = (UInt32)buf.Length;
                        seek += (UInt32)buf.Length;
                    }
                    foreach (var pak in _items)
                    {
                        string name = pak.Name.Replace(Directory.GetCurrentDirectory() + '\\', "").Replace('\\', '/');
                        byte[] name_buf = new byte[HeaderSize - 8];
                        for (int i = 0; i < name.Length; i++)
                            name_buf[i] = (byte)name[i];
                        bw.Write(name_buf);
                        bw.Write(pak.Seek);
                        bw.Write(pak.Size);
                    }
                    bw.Seek(4, SeekOrigin.Begin);
                    bw.Write(seek);
                }
            }
        }

        public PakItem this[string name]
        {
            get
            {
                foreach (var item in _items)
                {
                    if (item.Name == name)
                        return item;
                }
                return null;
            }
        }

        public void append(PakItem item)
        {
            _items.Add(item);
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new PakEnum(_items);
        }

        #endregion
    }

    class PakEnum : IEnumerator
    {
        int position = -1;
        List<PakItem> _items;

        public PakEnum(List<PakItem> items)
        {
            _items = items;
        }

        #region IEnumerator Members

        public object Current
        {
            get { return _items[position]; }
        }

        public bool MoveNext()
        {
            position++;
            return (position < _items.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        #endregion
    }
}

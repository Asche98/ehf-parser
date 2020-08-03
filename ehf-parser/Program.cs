using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ehf_parser
{
    class Program
    {
        static void Main(string[] args)
        {
            string src = "";
            while (!File.Exists(src))
            {
                Console.WriteLine("Введите адрес pak-файла: ");
                src = Console.ReadLine();
                if (!File.Exists(src)) Console.WriteLine("Файл не существует, попробуйте ещё раз");
            }
            PakFile pk = new PakFile();
            pk.load(src);
            //int i = 0;
            foreach (PakItem f in pk)
            {
                Console.WriteLine(f.Name);
                /*
                string path = "j" + (++i) + ".bson";
                File.WriteAllBytes(path, f.Data);
                bsonTransformer.b2j(path, "j.json");*/
                f.Dump(null);
            }
            src = "";
            while (true)
            {
                Console.WriteLine("Для получения json введите адрес файла без пути (например,event_dscr.bson)");
                src = Console.ReadLine();
                if((!File.Exists(src))) Console.WriteLine("Файл не существует, попробуйте ещё раз");
                else
                {
                    string jname = src.Substring(0,src.IndexOf('.')) + "b.json";
                    bsonTransformer.b2j(src, jname);
                    Console.WriteLine("json сохранён в файле с именем " + jname);
                }
            }
            

            Console.ReadLine();
        }
    }
}

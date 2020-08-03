using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace ehf_parser
{
    static class bsonTransformer
    {
        static Dictionary<string, string> arguments = new Dictionary<string, string>();

        public static int b2j(string bfile, string jfile)
        {
            try
            {
                Stream textReader = File.OpenRead(bfile);

                bool need_root = false;

                if (!arguments.ContainsKey("noroot"))
                {
                    using (BsonReader reader = new BsonReader(textReader))
                    {
                        while (reader.Read())
                        {
                            if (reader.TokenType != JsonToken.PropertyName && reader.TokenType == JsonToken.StartObject)
                                continue;
                            if ((string)reader.Value != "root")
                                need_root = true;
                            break;
                        }
                    }
                }
                textReader.Close();
                textReader = File.OpenRead(bfile);


                using (BsonReader reader = new BsonReader(textReader))
                {
                    var outFile = jfile;
                    reader.Encoding = Encoding.GetEncoding(1251);
                    if (jfile == null)
                        outFile = bfile.Replace(".bson", ".json");
                    TextWriter stm = new StreamWriter(outFile, false, Encoding.GetEncoding(1251));

                    using (JsonTextWriter jsonWriter = new JsonTextWriter(stm))
                    {
                        jsonWriter.Formatting = Formatting.Indented;

                        if (need_root)
                        {
                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName("root");
                        }

                        while (reader.Read())
                        {
                            jsonWriter.WriteToken(reader);
                        }
                        if (need_root)
                        {
                            jsonWriter.WriteEndObject();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return 2;
            }
            return 0;
        }

    }
}

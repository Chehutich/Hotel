using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace Hotel.Utils 
{
    public static class ClassSerializare
    {
        public static void SerializeToXml<T>(ref T inObject, string inFileName)
        {
            try
            {
                XmlSerializer writer = new XmlSerializer(typeof(T));
                StreamWriter file = new StreamWriter(inFileName);
                writer.Serialize(file, inObject);
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
        }

        public static void DeserializeFromXml<T>(ref T inObject, string inFileName)
        {
            if (File.Exists(inFileName))
            {
                XmlSerializer reader = new XmlSerializer(typeof(T));
                StreamReader file = new StreamReader(inFileName);
                inObject = (T)reader.Deserialize(file);
                file.Close();
            }
        }
    }
}
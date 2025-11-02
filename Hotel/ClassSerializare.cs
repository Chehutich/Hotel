using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace Hotel 
{
    public static class ClassSerializare
    {
        public static void SerializeToXml<T>(ref T inObject, string inFileName)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                System.IO.StreamWriter file = new System.IO.StreamWriter(inFileName);
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
            if (System.IO.File.Exists(inFileName))
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(T));
                System.IO.StreamReader file = new System.IO.StreamReader(inFileName);
                inObject = (T)reader.Deserialize(file);
                file.Close();
            }
        }
    }
}
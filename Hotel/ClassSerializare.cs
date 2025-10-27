using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace Hotel // Переконайтесь, що простір імен тут правильний
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
                MessageBox.Show(ex.Message); // Залишаємо повідомлення про помилку тут
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
            // Можна додати else блок з повідомленням, якщо файл не знайдено
        }
    }
}
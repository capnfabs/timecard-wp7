using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.Xml;

namespace Timecard.Util
{
    public class IsolatedStorageHelper
    {
        public static Object FileToObj(Type type, String filename)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = file.OpenFile(filename, System.IO.FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(type);
            Object data = serializer.Deserialize(stream);
            stream.Close();
            return data;
        }
        public static void ObjToFile(Object obj, Type type, String filename)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = file.OpenFile(filename, System.IO.FileMode.Create);
            XmlWriter writer = XmlWriter.Create(stream);
            XmlSerializer serializer = new XmlSerializer(type);
            serializer.Serialize(writer, obj);
            writer.Close();
            stream.Close();
        }
        public static String LoadFile(String filename)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = file.OpenFile(filename, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(stream);
            String str = reader.ReadToEnd();
            reader.Close();
            return str;
        }

        public static void DeleteFile(String filename)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            file.DeleteFile(filename);
        }
    }
}

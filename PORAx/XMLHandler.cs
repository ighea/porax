using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace PORAx
{
    class XMLHandler
    {
        public static T LoadXML<T>(String filename)
        {
            Object data = null;

            if (!filename.EndsWith(".xml", true, System.Globalization.CultureInfo.CurrentCulture))
            {
                filename = filename + ".xml";
            }

            XmlReaderSettings xmlSettings = new XmlReaderSettings();
            xmlSettings.IgnoreComments = true;
            
            using (XmlReader xmlReader = XmlReader.Create(filename, xmlSettings))
            {
                data = IntermediateSerializer.Deserialize<T>(xmlReader, filename);
            }
            return (T)data;
        }

        public static void SaveXML(String filename, Object data)
        {
            if (!filename.EndsWith(".xml", true, System.Globalization.CultureInfo.CurrentCulture))
            {
                filename = filename + ".xml";
            }

            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;

            using (XmlWriter xmlWriter = XmlWriter.Create(filename, xmlSettings))
            {
                IntermediateSerializer.Serialize(xmlWriter, data, null);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SciSharp
{
    public static class DataUtils
    {

        public static void Serialize<T>(this object X, string filePath, bool useXML=false)
        {
            using (Stream stream = File.Open(filePath, FileMode.Create))
            {
                if (useXML)
                {
                    var frmx = new XmlSerializer(typeof(T));
                    frmx.Serialize(stream, X);
                }
                else
                {
                    var frmb = new BinaryFormatter();
                    frmb.Serialize(stream, X);
                }
            }
        }

        public static T Deserialize<T>(this string filePath, bool useXml = false)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                if (useXml)
                {
                    var frmx = new XmlSerializer(typeof(T));
                    return (T)frmx.Deserialize(stream);
                }
                else
                {
                    var frmb = new BinaryFormatter();
                    return (T)frmb.Deserialize(stream);
                }
            }
        }
    }
}

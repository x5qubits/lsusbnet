using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace lsusbnet
{
    class Program
    {
        static void Main(string[] args)
        {

            bool Json = false;
            bool Xml = false;
            bool Help = false;
            bool storage = false;

            if (args == null || args!=null && args.Length == 0)
            {
                storage = false;
            }
            else
            {
                if(args != null && args.Length > 0)
                {
                    foreach(string str in args)
                    {
                        if (str.Contains("-j") || str.Contains("--json"))
                        {
                            storage = true;
                            Json = true;
                            break;
                        }
                        else if (str.Contains("-h") || str.Contains("--help"))
                        {
                            storage = true;
                            Help = true;
                            break;
                        }
                        else if (str.Contains("-x") || str.Contains("--xml"))
                        {
                            storage = true;
                            Xml = true;
                            break;
                        }
                    }
                }
            }

            if (Help)
            {
                Console.WriteLine("Menu:");
                Console.WriteLine(" -j --json ");
                Console.WriteLine("   Output json formated info.");
                Console.WriteLine(" -x --xml ");
                Console.WriteLine("   Output xml formated info.");
                Console.WriteLine(" -h --help ");
                Console.WriteLine("   Show usage and help.");
                Console.WriteLine("");
            }
            else
            {
                DeviceParser parser = new DeviceParser();
                parser.GetDeviceList(storage);
                if (Json)
                {
                    var json = JsonConvert.SerializeObject(parser);
                    string jsonFormatted = JValue.Parse(json).ToString(Newtonsoft.Json.Formatting.Indented);
                    Console.WriteLine(jsonFormatted);
                }
                else if (Xml)
                {
                    XmlSerializer xsSubmit = new XmlSerializer(typeof(DeviceParser));
                    using (StringWriter sww = new StringWriter())
                    {
                        using (XmlWriter writer = XmlWriter.Create(sww))
                        {
                            xsSubmit.Serialize(writer, parser);
                            XDocument doc = XDocument.Parse(sww.ToString());
                            Console.WriteLine(ToIndentedString(doc));
                        }
                    }
                }
                else
                {
                    foreach (UsbDevice device in parser.devices)
                    {
                        Console.WriteLine("Bus " + device.busnum + " Device " + device.devnum + ": ID " + device.DeviceString + " " + device.iProduct);
                    }
                }
            }
        }

        private static string ToIndentedString(XDocument doc)
        {
            var stringWriter = new StringWriter(new StringBuilder());
            var xmlTextWriter = new XmlTextWriter(stringWriter) { Formatting = System.Xml.Formatting.Indented };
            doc.Save(xmlTextWriter);
            return stringWriter.ToString();
        }
    }
}

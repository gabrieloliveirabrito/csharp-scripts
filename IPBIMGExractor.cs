using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Text.Encodings;

namespace IPBoardXMLExtractor
{
    class IPBFile
    {
        public string Filename { get; set; }
        public string Content { get; set; }
        public string Path { get; set; }
    }

    class Program
    {
        static IPBFile ParseFileNode(XmlNode node)
        {
            IPBFile file = new IPBFile();
            file.Filename = node.SelectSingleNode("filename").InnerText;
            file.Content = node.SelectSingleNode("content").InnerXml.Replace("\r\n", "");
            file.Path = node.SelectSingleNode("path").InnerText;

            return file;
        }

        const string filename = "/Users/gabrieloliveirabrito/Downloads/Dashboard/Skin Files/replacements-dashboard.xml";
        static void Main(string[] args)
        {
            Console.Clear();
            if(!File.Exists(filename))
            {
                Console.WriteLine("File not exists!");
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);

                XmlNode filesetNode = doc.DocumentElement.SelectSingleNode("fileset");
                if(filesetNode == null)
                {
                    Console.WriteLine("No fileset node on root element!");
                }
                else
                {
                    string directory = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename));
                    if (Directory.Exists(directory))
                        Directory.Delete(directory, true);
                    Directory.CreateDirectory(directory);

                    XmlNodeList fileNodes = filesetNode.SelectNodes("file");
                    foreach(XmlNode fileNode in fileNodes)
                    {
                        IPBFile file = ParseFileNode(fileNode);
                        Console.WriteLine("File: {0} | Path: {1} | Content Length: {2}", file.Filename, file.Path, file.Content.Length);

                        string fullPath = Path.Combine(directory, file.Path, file.Filename);
                        Console.WriteLine(fullPath);

                        string fileDir = Path.GetDirectoryName(fullPath);
                        if (!Directory.Exists(fileDir))
                            Directory.CreateDirectory(fileDir);

                        using(FileStream stream = File.Create(fullPath))
                        {
                            byte[] buffer = Convert.FromBase64String(file.Content);
                            stream.Write(buffer, 0, buffer.Length);

                            stream.Flush();
                        }

                        Console.WriteLine();
                    }
                }
            }
            //Console.ReadLine();
        }
    }
}
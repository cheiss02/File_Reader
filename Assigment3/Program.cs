using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;

namespace Assigment3
{
    class Program
    {
        public static void Main(string[] args)
        {
            string toAnalyzePath;
            Console.Write("Please enter the extention to query the data ");
            toAnalyzePath = Console.ReadLine();

            string toSavePath;
            Console.Write("Enter the extention where to save the analyze information ");
            toSavePath = Console.ReadLine();
            CreateReport(EnumerateFilesRecursively(toAnalyzePath)).Save(@toSavePath);
        }
        static IEnumerable<string> EnumerateFilesRecursively(string path)
        {
            return Directory.EnumerateFiles(path).Union(Directory.EnumerateDirectories(path).SelectMany((dir) => EnumerateFilesRecursively(dir)));
        }
        static string FormatByteSize(long byteSize)
        {
            //source https://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
            string[] suffixes = { "B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB"};
            if (byteSize == 0)
                return "0" + suffixes[0];
            long bytes = Math.Abs(byteSize);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1000)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteSize) * num).ToString() + suffixes[place];
        }
        static XDocument CreateReport(IEnumerable<string> files)
        {
            var list4 = files.Select(file => new FileInfo(file))
                .GroupBy(ext => ext.Extension.ToLower())
                .OrderByDescending(t => t.Sum(x => x.Length))
                .Select(t => new { 
                    Extension = t.Key, 
                    extCounter = t.Count(), 
                    SumUp = FormatByteSize(t.Sum(x => x.Length)) });

            XDocument doc = new XDocument(new XComment("This is my table "),
                new XElement("table", new XAttribute("border", 1),
                new XElement("thead",
                        new XElement("tr",
                            new XElement("th", "  Type  "),
                            new XElement("th", "  Count  "),
                            new XElement("th", "  Size  ")),
                        new XElement("tBody",
                            from data in list4
                            select new XElement("tr",
                                new XElement("td", data.Extension),
                                new XElement("td", data.extCounter),
                                new XElement("td", data.SumUp))))));
                return doc;
        }
    }
}
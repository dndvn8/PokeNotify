using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using log4net.Util;
using PokeNotify.Core.Models;

namespace PokeNotify.Core
{
    public static class Utils
    {
        public static double ConvertToDouble(string str)
        {
            try
            {
                return double.Parse(str);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static void LoadSniper2Config(ISettings settings)
        {
            var path = Path.Combine(Path.GetDirectoryName(settings.Pokesnipe2Path), "user.xml");
            using (StreamReader sr = new StreamReader(path, true)) 
            {
                XDocument xml = XDocument.Load(sr);
                settings.Latitude =
                    ConvertToDouble(xml.Element("UserSettingsXml")?.Elements("DefaultLatitude").Single().Value);
                settings.Longitude =
                    ConvertToDouble(xml.Element("UserSettingsXml")?.Elements("DefaultLongitude").Single().Value);
            }
        }
        public static void SaveSniper2Config(ISettings settings)
        {
            var path = Path.Combine(Path.GetDirectoryName(settings.Pokesnipe2Path), "user.xml");
            XDocument xml = new XDocument();
            using (StreamReader sr = new StreamReader(path, true))
            {
                xml = XDocument.Load(sr);
                xml.Element("UserSettingsXml")?.Elements("DefaultLatitude").Single().SetValue(settings.Latitude);
                xml.Element("UserSettingsXml")?.Elements("DefaultLongitude").Single().SetValue(settings.Longitude);
            }
            xml.Save(path);
        }

        public static void RandomLocation(ISettings settings)
        {
            settings.Latitude += GetRandomNumber(0.00001, 0.00009);
            settings.Longitude += GetRandomNumber(0.00001, 0.00009);
            SaveSniper2Config(settings);
        }
        public static double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            var i = random.Next(0, 1);
            if (i%2 > 0) 
                return 0 - random.NextDouble() * (maximum - minimum) + minimum;
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
        public static string Pokesnipe2Builder(string str)
        {
            string builder = "pokesniper2://{0}/{1}";
            try
            {
                Regex regex = new Regex(@"^(\w*)\s-");
                Match match = regex.Match(str);
                var pokemonId = match.Groups[1].Value;
                regex = new Regex(@"Lat\/Lng:(.*)\s-");
                match = regex.Match(str);
                var location = match.Groups[1].Value;
                builder = string.Format(builder, pokemonId, location);
            }
            catch (Exception)
            {
                //
            }
            
            return builder;
        }
    }
}

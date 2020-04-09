using System;
using System.IO;
using System.Reflection;

namespace Sample.Presentation.Console.Resources
{
    public class DataGenerator
    {
        private readonly Random _random = new Random();
        private readonly string[] _firstNames;
        private readonly string[] _lastNames;
        private static int _sequence = 1;

        public DataGenerator()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("Sample.Presentation.Console.Resources.FirstNames.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                var text = reader.ReadToEnd();
                _firstNames = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            }

            using (Stream stream = assembly.GetManifestResourceStream("Sample.Presentation.Console.Resources.LastNames.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                var text = reader.ReadToEnd();
                _lastNames = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string RandomFirstName()
        {
            return _firstNames[_random.Next(_firstNames.Length - 1)].Trim();
        }

        public string RandomLastName()
        {
            return _lastNames[_random.Next(_lastNames.Length - 1)].Trim();
        }

        public int RandomInteger(int min, int max)
        {
            return _random.Next(min, max);
        }

        public int Sequence()
        {
            return _sequence++;
        }
    }
}

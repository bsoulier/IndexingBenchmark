using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bogus;

namespace IndexingBenchmarks
{
    class Program
    {
        private static List<Entity> _entitiesLists;
        private static List<string> _countryCodes;

        static void Main(string[] args)
        {
            BuildData();
            Test1();
            Test2();
            Console.ReadLine();
        }

        private static void BuildData()
        {
            Console.WriteLine("Data: building");

            var entityFaker = new Faker<Entity>()
                .RuleFor(e => e.Name, f => f.Name.FullName())
                .RuleFor(e => e.Address, f => f.Address.StreetAddress())
                .RuleFor(e => e.Age, f => f.Random.Number(18, 99))
                .RuleFor(e => e.Country, (f, u) => f.Address.CountryCode());

            _entitiesLists = entityFaker.Generate(1000000);

            _countryCodes = _entitiesLists.Select(e => e.Country).Distinct().ToList();
            Console.WriteLine("Data: done");
        }

        private static void Test1()
        {
            Console.WriteLine("Test1: start");
            var sw = new Stopwatch();
            sw.Start();

            foreach (var countryCode in _countryCodes)
            {
                var countriesPerCode = _entitiesLists.Where(e => e.Country == countryCode).ToList();
                var count = countriesPerCode.Count;
                // Code supposingly doing something with entities.
            }
            sw.Stop();
            Console.WriteLine("Test1: processed in {0} milliseconds", sw.Elapsed.Milliseconds);
        }

        private static void Test2()
        {
            Console.WriteLine("Test2: start");
            var sw = new Stopwatch();
            sw.Start();

            var countryLookup = _entitiesLists.ToLookup(e => e.Country);
            Console.WriteLine("Test2: lookup done in {0} milliseconds", sw.Elapsed.Milliseconds);

            // Using lookup instead of plain list here.
            foreach (var countryCode in _countryCodes)
            {
                var countriesPerCode = countryLookup[countryCode];
                var count = countriesPerCode.Count();
                // Code supposingly doing something with entities.
            }
            sw.Stop();
            Console.WriteLine("Test2: processed in {0} seconds", sw.Elapsed.Milliseconds);
        }
    }
}
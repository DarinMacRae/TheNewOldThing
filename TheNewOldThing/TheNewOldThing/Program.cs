using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheNewOldThing
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(args[0]);
            var summary = new Dictionary<string, int>();

            foreach (var line in lines)
            {
                var splits = line.Split(',');

                if (!summary.ContainsKey(splits[1]))
                    summary.Add(splits[1], 0);

                summary[splits[1]]++;
            }

            foreach (var item in summary)
            {
                Console.WriteLine("{0},{1}", item.Key, item.Value);
            }
            
            Console.ReadLine();
        }
    }
}

using Accord.MachineLearning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Accord
{
    public class Learn_KNN_Accord
    {
        class Digit
        {
            public int[] Image;
            public int Label;
        }

        public void Main()
        {
            WriteLine("Execution begins...");

            var fn = @"c:\DEMO\Data\train.csv";
            var f = File.ReadLines(fn);
            var data = from z in f.Skip(1)
                       let zz = z.Split(',').Select(int.Parse)
                       select new Digit
                       {
                           Label = zz.First(),
                           Image = zz.Skip(1).ToArray()
                       };
            var train = data.Take(10000).ToArray();
            var test = data.Skip(10000).Take(1000).ToArray();

            var classifier = new KNearestNeighbors(1);
            classifier.Learn(
                (from x in train select x.Image.Select(z => (double)z).ToArray()).ToArray(),
                (from x in train select x.Label).ToArray());

            int count = 0, correct = 0;

            foreach (var z in test)
            {
                var n = classifier.Decide(z.Image.Select(t => (double)t).ToArray());
                WriteLine("{0} => {1}", z.Label, n);
                if (z.Label == n) correct++;
                count++;
            }

            WriteLine("Done, {0} of {1} correct ({2}%)", correct, count, (double)correct / (double)count * 100);
            ReadKey();
        }

    }
}

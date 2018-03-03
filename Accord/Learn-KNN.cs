using System;
using static System.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Accord.Controls;
using SciSharp;

namespace Accord
{
    public class Learn_KNN
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

            for (int i = 0; i < 5; i++)
            {
                ImageBox.Show(train[i].Image.Select(x => x / 256.0).ToArray(), 28, 28);
            }

            Func<int[], int[], int> dist = (a, b) =>
                  a.Zip(b, (x, y) => { return (x - y) * (x - y); }).Sum();

            Func<int[], int> classify = (im) =>
                 train.MinBy(d => dist(d.Image, im)).Label;

            int count = 0, correct = 0;

            foreach (var z in test)
            {
                var n = classify(z.Image);
                WriteLine("{0} => {1}", z.Label, n);
                if (z.Label == n) correct++;
                count++;
            }

            WriteLine("Done, {0} of {1} correct ({2}%)", correct, count, (double)correct / (double)count * 100);
            ReadKey();
        }
    }
}

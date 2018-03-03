using Accord.Neuro;
using Accord.Neuro.Learning;
using SciSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Accord
{
    public class Learn_NN
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

            var nn = new ActivationNetwork(new SigmoidFunction(0.1), 784, 10);
            var learn = new BackPropagationLearning(nn);

            nn.Randomize();

            WriteLine("Starting learning");

            for (int ep = 0; ep < 150; ep++)
            {
                var err = learn.RunEpoch((from x in train select x.Image.Select(t => (double)t / 256.0).ToArray()).ToArray(),
                               (from x in train select x.Label.ToOneHot10(10).ToDoubleArray()).ToArray());
                WriteLine($"Epoch={ep}, Error={err}");
            }

            int count = 0, correct = 0;

            foreach (var z in test)
            {
                var t = nn.Compute(z.Image.Select(tt => (double)tt / 256.0).ToArray());
                var n = t.MaxIndex();
                WriteLine("{0} => {1}", z, z.Label);
                if (n == z.Label) correct++;
                count++;
            }

            WriteLine("Done, {0} of {1} correct ({2}%)", correct, count, (double)correct / (double)count * 100);
            ReadKey();

        }
    }
}

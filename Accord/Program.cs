using System;
using static System.Console;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Controls;
using System.Drawing;
using System.Windows.Forms;
using SciSharp;
using Accord.MachineLearning;
using Accord.MachineLearning.Bayes;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;
using Accord.Neuro;
using Accord.Neuro.Learning;

namespace Accord
{

    public class Digit
    {
        public int[] Image;
        public int Label;
    }

    public class Program
    {

        static void Main(string[] args)
        {
            WriteLine("Execution begins...");

            var fn = @"c:\DEMO\Data\train.csv";
            var f = File.ReadLines(fn);
            var data = from z in f.Skip(1)
                       let zz = z.Split(',').Select(int.Parse)
                       select new Digit { Label = zz.First(),
                                          Image = zz.Skip(1).ToArray() };

            var train = data.Take(10000).ToArray();
            var test = data.Skip(10000).Take(1000).ToArray();

            // КОД БУДЕТ ТУТ!

            ReadKey();
        }



    }
}

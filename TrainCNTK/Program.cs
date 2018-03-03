using CNTK;
using SciSharp;
using SciSharp.CNTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace TrainCNTK
{

    public class Digit
    {
        public float[] Image;
        public int Label;
    }

    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Execution begins");
            var fn = @"c:\DEMO\Data\train.csv";
            var f = File.ReadLines(fn);
            var data = from t in f.Skip(1)
                       let zz = t.Split(',').Select(float.Parse)
                       select new Digit
                       {
                           Label = (int)zz.First(),
                           Image = zz.Skip(1).Select(x => x / 256f).ToArray()
                       };

            var train = data.Take(40000).ToArray();
            var test = data.Skip(40000).Take(1000).ToArray();

            WriteLine("Creating network");
            DeviceDescriptor device = DeviceDescriptor.CPUDevice;

            int inputDim = 784;
            int outputDim = 10;

            var inputShape = new NDShape(1, inputDim);
            var outputShape = new NDShape(1, outputDim);

            Variable features = Variable.InputVariable(inputShape, DataType.Float);
            Variable label = Variable.InputVariable(outputShape, DataType.Float);

            var W = new Parameter(new [] { outputDim, inputDim }, DataType.Float, CNTKLib.GlorotUniformInitializer(), device, "w");
            var b = new Parameter(new [] { outputDim }, DataType.Float, 0, device, "b");

            var z = CNTKLib.Times(W,features) + b;

            var loss = CNTKLib.CrossEntropyWithSoftmax(z, label);
            var evalError = CNTKLib.ClassificationError(z, label);
            CNTK.TrainingParameterScheduleDouble learningRatePerSample = new CNTK.TrainingParameterScheduleDouble(0.02, 1);
            IList<Learner> parameterLearners =
                new List<Learner>() { Learner.SGDLearner(z.Parameters(), learningRatePerSample) };
            var trainer = Trainer.CreateTrainer(z, loss, evalError, parameterLearners);

            int minibatchSize = 64;
            int numMinibatchesToTrain = 500;

            var feat = new BatchSource<float[]>((from x in train select x.Image).ToArray(), minibatchSize);
            var labl = new BatchSource<float[]>((from x in train select x.Label.ToOneHot10(10).ToFloatArray()).ToArray(), minibatchSize);
            var gr = new List<float>();

            // train the model
            for (int ep = 0; ep < numMinibatchesToTrain; ep++)
            {
                Value ft, lb;

                feat.MoveNext(); labl.MoveNext();

                ft = Value.CreateBatchOfSequences<float>(inputShape, feat.Current, device);
                lb = Value.CreateBatchOfSequences<float>(outputShape, labl.Current, device);

                trainer.TrainMinibatch(
                    new Dictionary<Variable, Value>() { { features, ft }, { label, lb } }, device);

                if (ep % 50 == 0)
                {
                    var _loss = trainer.PreviousMinibatchLossAverage();
                    var _eval = trainer.PreviousMinibatchEvaluationAverage();
                    WriteLine($"Epoch={ep}, loss={_loss}, eval={_eval}");
                    gr.Add((float)_loss);
                }
            }

            var G = new GraphLib();
            G.Plot(gr,System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line);
            int count = 0, correct = 0;

            // Test the model
            foreach(var x in test)
            {
                var imap = new Dictionary<Variable, Value> { { features, Value.CreateBatch(inputShape,x.Image,device) } };
                var omap = new Dictionary<Variable, Value> { { z, null } };
                z.Evaluate(imap, omap, device);
                var o = omap[z].GetDenseData<float>(z).First();
                var res = o.MaxIndex();

                WriteLine("{0} => {1}", x.Label, res);
                if (x.Label == res) correct++;
                count++;
            }
            WriteLine("Done, {0} of {1} correct ({2}%)", correct, count, (double)correct / (double)count * 100);

            Console.ReadKey();
        }
    }
}

using CNTK;
using SciSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace TrainCNTK
{
    class SimpleRegression
    {
        public void Main()
        {
            WriteLine("Starting");

            var G = new GraphLib();

            WriteLine("Generating sample data");

            // Создаём случайные точки двух типов
            float cx1 = -1.0f, cy1 = -1.0f;
            float cx2 = 1.0f, cy2 = 1.0f;

            var Rnd = new Random();

            var x = new List<float>();
            var y = new List<float>();
            var lab = new List<float>();
            var lab_oh = new List<float[]>();

            for (int i = 0; i < 200; i++)
            {
                x.Add(cx1 + Rnd.Next(-2.0, 2.0)); y.Add(cy1 + Rnd.Next(-2.0, 2.0));
                lab.Add(-1.0f); lab_oh.Add(new float[] { 1.0f, 0.0f });
                x.Add(cx2 + Rnd.Next(-2.0, 2.0)); y.Add(cy2 + Rnd.Next(-2.0, 2.0));
                lab.Add(1.0f); lab_oh.Add(new float[] { 0.0f, 1.0f });
            }

            G.Plot(x, y);

            WriteLine("Doing data split");

            var x_train = x.Take(150).ToArray();
            var y_train = y.Take(150).ToArray();
            var l_train = lab.Take(150).ToArray();
            var l_oh_train = lab_oh.Take(150).ToArray();

            var x_test = x.Skip(150).ToArray();
            var y_test = y.Skip(150).ToArray();
            var l_test = lab.Skip(150).ToArray();
            var l_oh_test = lab_oh.Skip(150).ToArray();


            WriteLine("Creating network");

            DeviceDescriptor device = DeviceDescriptor.CPUDevice;

            // Create network model
            int inputDim = 2;
            int outputDim = 2;

            Variable features = Variable.InputVariable(inputDim.AsArray(), DataType.Float);
            Variable label = Variable.InputVariable(outputDim.AsArray(), DataType.Float);

            var W = new Parameter(new int[] { outputDim, inputDim }, DataType.Float, 1, device, "w");
            var b = new Parameter(new int[] { outputDim }, DataType.Float, 0, device, "b");
            var z = CNTKLib.Times(W, features) + b;

            var loss = CNTKLib.CrossEntropyWithSoftmax(z, label);
            var evalError = CNTKLib.ClassificationError(z, label);

            // prepare for training
            CNTK.TrainingParameterScheduleDouble learningRatePerSample = new CNTK.TrainingParameterScheduleDouble(0.02, 1);
            IList<Learner> parameterLearners =
                new List<Learner>() { Learner.SGDLearner(z.Parameters(), learningRatePerSample) };
            var trainer = Trainer.CreateTrainer(z, loss, evalError, parameterLearners);

            int minibatchSize = 64;
            int numMinibatchesToTrain = 1000;

            int k = 0; // current position in dataset

            // train the model
            for (int ep = 0; ep < numMinibatchesToTrain; ep++)
            {
                Value f, l;

                var fa = new float[minibatchSize * inputDim];
                var la = new float[minibatchSize * outputDim];

                for (int j = 0; j < minibatchSize; j++)
                {
                    fa[j * inputDim] = x_train[k];
                    fa[j * inputDim + 1] = y_train[k];
                    la[j * outputDim] = l_oh_train[k][0];
                    la[j * outputDim + 1] = l_oh_train[k][1];
                    k++;
                    if (k == x_train.Length) k = 0;
                }

                f = Value.CreateBatch<float>(inputDim.AsArray(), fa, device);
                l = Value.CreateBatch<float>(outputDim.AsArray(), la, device);

                trainer.TrainMinibatch(
                    new Dictionary<Variable, Value>() { { features, f }, { label, l } }, device);

                if (ep % 50 == 0)
                {
                    var _loss = trainer.PreviousMinibatchLossAverage();
                    var _eval = trainer.PreviousMinibatchEvaluationAverage();
                    WriteLine($"Epoch={ep}, loss={_loss}, eval={_eval}");
                }
            }


            WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}

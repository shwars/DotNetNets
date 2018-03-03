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
    public class CatsDogs
    {

        DeviceDescriptor device = DeviceDescriptor.CPUDevice;

        private int[] imageDim = { 32, 32, 3 };
        private int numClasses = 2;

        private string pr(NDShape X)
        {
            return X.Dimensions.Select(x => x.ToString()).Aggregate( (a, b) => a + "x"+b); 
        }

        public Function ConvolutionLayer(Variable inp, int wx, int wy, int c, int feat)
        {
            var ConvParam1 = new Parameter(new int[] { wx, wy, c, feat }, DataType.Float, CNTKLib.GlorotUniformInitializer(), device);
            var Layer1 = CNTKLib.ReLU(CNTKLib.Convolution(ConvParam1, inp, new int[] { 1, 1 }, new bool[] { true }, new bool[] { true }));
            var Layer1Pool = CNTKLib.Pooling(Layer1, PoolingType.Max, new int[] { 3, 3 }, new int[] { 2, 2 });
            WriteLine($" - Conv layer from {pr(inp.Shape)} to {pr(Layer1Pool.Output.Shape)}");
            return Layer1Pool;
        }

        public Function DenseLayer(Function inp, int num, bool useActivation = true)
        {
            if (inp.Output.Shape.Dimensions.Count > 1)
            {
                var x = inp.Output.Shape.Dimensions.Aggregate((u,v)=>u*v);
                inp = CNTKLib.Reshape(inp, x.AsArray());
            }
            var W = new Parameter(new int[] { num, inp.Output.Shape.Dimensions.First() }, DataType.Float, CNTKLib.GlorotUniformInitializer(), device);
            var b = new Parameter(num.AsArray(), DataType.Float, CNTKLib.GlorotUniformInitializer(), device);
            var z = CNTKLib.Times(W, inp) + b;
            WriteLine($" - Dense layer from ${pr(inp.Output.Shape)} to ${pr(z.Output.Shape)}");
            if (useActivation) return CNTKLib.ReLU(z);
            else return z;
        }


        public void Main()
        {

            // Create minibatch source
            var transforms = new List<CNTKDictionary>{
                CNTKLib.ReaderCrop("RandomSide",
                    new Tuple<int, int>(0, 0),
                    new Tuple<float, float>(0.8f, 1.0f),
                    new Tuple<float, float>(0.0f, 0.0f),
                    new Tuple<float, float>(1.0f, 1.0f),
                    "uniRatio"),
                CNTKLib.ReaderScale(imageDim[0], imageDim[1], imageDim[2])
            };

            var conf = CNTKLib.ImageDeserializer(@"D:\Microsoft\Presentations\FY18 AI\DotNext17\Data\Train.txt",
                               "labels", (uint)numClasses,
                               "features", transforms);

            MinibatchSourceConfig config = new MinibatchSourceConfig(new List<CNTKDictionary> { conf });

            var minibatchSource = CNTKLib.CreateCompositeMinibatchSource(config);

            var imageStreamInfo = minibatchSource.StreamInfo("features");
            var labelStreamInfo = minibatchSource.StreamInfo("labels");

            // build a model
            var imageInput = CNTKLib.InputVariable(imageDim, imageStreamInfo.m_elementType, "Images");
            var labelsVar = CNTKLib.InputVariable(new int[] { numClasses }, labelStreamInfo.m_elementType, "Labels");

            var C1 = ConvolutionLayer(imageInput, 5, 5, 3, 32);
            var C2 = ConvolutionLayer(C1, 5, 5, 32, 1);
            var C3 = ConvolutionLayer(C2, 5, 5, 64, 2);

            var C4 = DenseLayer(C3, 64, true);
            var C5 = DenseLayer(C4, 64, true);
            var z = DenseLayer(C5, numClasses, true);

            var loss = CNTKLib.CrossEntropyWithSoftmax(z, labelsVar);
            var evalError = CNTKLib.ClassificationError(z, labelsVar);

            // prepare for training
            CNTK.TrainingParameterScheduleDouble learningRatePerSample = new CNTK.TrainingParameterScheduleDouble(0.02, 1);
            IList<Learner> parameterLearners =
                new List<Learner>() { Learner.SGDLearner(z.Parameters(), learningRatePerSample) };
            var trainer = Trainer.CreateTrainer(z, loss, evalError, parameterLearners);

            uint minibatchSize = 64;

            // train the model
            for (int ep = 0; ep < 1000; ep++)
            {
                var data = minibatchSource.GetNextMinibatch(minibatchSize);
                trainer.TrainMinibatch(new Dictionary<Variable, MinibatchData>()
                {
                    { imageInput, data[imageStreamInfo] },
                    { labelsVar, data[labelStreamInfo] }
                }, device);

                if (ep % 50 == 0)
                {
                    var _loss = trainer.PreviousMinibatchLossAverage();
                    var _eval = trainer.PreviousMinibatchEvaluationAverage();
                    WriteLine($"Epoch={ep}, loss={_loss}, eval={_eval}");
                }
            }
        }
    }
}
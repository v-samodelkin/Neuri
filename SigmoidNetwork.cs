using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroS
{
    public class SigmoidNetwork
    {
        public readonly int LayersCount;
        // i-ый элемент - это матрица w
        // w[Б,А] - это вес ребра, идущего из нейрона А слоя w в нейрон Б слоя w+1
        protected readonly Matrix[] ConnectionWeights;
        protected readonly Line[] Bioses;
        protected readonly int[] LayersSizes;
        public SigmoidNetwork(IEnumerable<int> layersSizes)
        {
            LayersSizes = layersSizes.ToArray();
            LayersCount = LayersSizes.Count();

            ConnectionWeights = new Matrix[LayersCount - 1];
            for (int i = 0; i < LayersCount - 1; i++)
                ConnectionWeights[i] = new Matrix(LayersSizes[i + 1], LayersSizes[i]);

            Bioses = new Line[LayersCount - 1];
            for (int i = 0; i < LayersCount - 1; i++)
                Bioses[i] = new Line(LayersSizes[i]);

        }

        public double Sigmoid(double z)
        {
            return 1.0 / (1.0 + Math.Exp(-z));
        }

        public SigmoidNetwork Randomize()
        {
            var rand = new Random();

            foreach (var a in Bioses)
                for (int i = 0; i < a.Width; i++)
                    a[i] = (rand.NextDouble() - 0.5) * 2;

            foreach (var a in ConnectionWeights)
                for (int i = 0; i < a.Width; i++)
                    for (int j = 0; j < a.Height; j++)
                        a[i, j] = (rand.NextDouble() - 0.5) * 2;

            return this;
        }

        public SigmoidNetwork Randomize(Random rand)
        {
            return Randomize(rand, rand);
        }

        public SigmoidNetwork Randomize(Random biosesRand, Random weightsRand)
        {
            foreach (var a in Bioses)
                for (int i = 0; i < a.Width; i++)
                    a[i] = biosesRand.Next();

            foreach (var a in ConnectionWeights)
                for (int i = 0; i < a.Width; i++)
                    for (int j = 0; j < a.Height; j++)
                        a[i, j] = weightsRand.Next();

            return this;
        }

        public SigmoidNetwork Normalize()
        {
            foreach (var a in Bioses)
                for (int i = 0; i < a.Width; i++)
                    a[i] = Sigmoid(a[i]);

            foreach (var a in ConnectionWeights)
                for (int i = 0; i < a.Width; i++)
                    for (int j = 0; j < a.Height; j++)
                        a[i, j] = Sigmoid(a[i, j]);

            return this;
        }

        public Line NextLayer(int layerId, Line input)
        {
            if (input.Width != LayersSizes[layerId])
                throw new ArgumentOutOfRangeException(String.Format("Неверный вектор подан на слой {0}. Ожидается длина {1}, а пришло {2}.", layerId, LayersSizes[layerId], input.Width));
            var ans = input * ConnectionWeights[layerId];
            return ans.AsLine() - Bioses[layerId];
        }

        public Line Ask(Line input)
        {
            if (input.Width != LayersSizes[0])
                throw new ArgumentOutOfRangeException("Длина входного вектора не равна количеству нейронов на первом слое");

            return new Line(new double[4]);
        }
    }
}

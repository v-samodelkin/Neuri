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
        private Line[] deltas;
        protected readonly int[] LayersSizes;
        public SigmoidNetwork(IEnumerable<int> layersSizes)
        {
            LayersSizes = layersSizes.ToArray();
            LayersCount = LayersSizes.Count() - 1;

            ConnectionWeights = new Matrix[LayersCount];
            for (int i = 0; i < LayersCount; i++)
                ConnectionWeights[i] = new Matrix(LayersSizes[i + 1], LayersSizes[i]);

            Bioses = new Line[LayersCount];
            deltas = new Line[LayersCount];
            for (int i = 0; i < LayersCount; i++)
            {
                Bioses[i] = new Line(LayersSizes[i + 1]);
                deltas[i] = new Line(LayersSizes[i + 1]);
            }
                

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
                    a[i] = (rand.NextDouble() - 0.5) * 5;

            foreach (var a in ConnectionWeights)
                for (int i = 0; i < a.Width; i++)
                    for (int j = 0; j < a.Height; j++)
                        a[i, j] = (rand.NextDouble() - 0.5) * 5;

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
            return (ans.AsLine() - Bioses[layerId]).Select(x => Sigmoid(x));
        }

        public Line Ask(Line input)
        {
            if (input.Width != LayersSizes[0])
                throw new ArgumentOutOfRangeException("Длина входного вектора не равна количеству нейронов на первом слое");

            for (int i = 0; i < LayersCount; i++)
                input = NextLayer(i, input);

            return input;
        }

        public IEnumerable<Line> AskEveryLayer(Line input)
        {
            if (input.Width != LayersSizes[0])
                throw new ArgumentOutOfRangeException("Длина входного вектора не равна количеству нейронов на первом слое");
            yield return input;
            for (int i = 0; i < LayersCount; i++)
            {
                input = NextLayer(i, input);
                yield return input;
            }
        }

        public SigmoidNetwork Learning(double eta, int epochs, Line[] questions, Line[] answers)
        {
            if (questions.Count() != answers.Count())
                throw new ArgumentException("Число вопросов и ответов не совпадают");
            for (int i = 0; i < questions.Count(); i++)
                if (questions[i].Width != LayersSizes[0])
                    throw new ArgumentOutOfRangeException(String.Format("Тест {0} имеет длину {1}, а требуется {2}", i, questions[i].Width, LayersSizes[0]));
            for (int i = 0; i < answers.Count(); i++)
                if (answers[i].Width != LayersSizes[LayersCount])
                    throw new ArgumentOutOfRangeException(String.Format("Ответ {0} имеет длину {1}, а требуется {2}", i, answers[i].Width, LayersSizes[LayersCount]));

            for (int epoch = 0; epoch < epochs; epoch++)
            {
                LearnEpoch(eta, questions, answers);
            }

            return this;
        }

        private void LearnEpoch(double eta, Line[] questions, Line[] answers)
        {
            for (int qq = 0; qq < questions.Count(); qq++)
            {
                var question = questions[qq];
                var answer = answers[qq];

                var outputs = AskEveryLayer(question).ToList();

                for (int i = 0; i < LayersSizes[LayersCount]; i++)
                {
                    var o = outputs[LayersCount][i];
                    if (o > 1 || o < -1)
                        throw new ArgumentException("'o' вне границ (-1;1)");
                    var dl = o * (1 - o) * (answer[i] - o);
                    deltas[LayersCount - 1][i] = dl;
                }
                for (int layerId = LayersCount - 2; layerId >= 0; layerId--)
                {
                    for (int i = 0; i < LayersSizes[layerId + 1]; i++)
                    {
                        var o = outputs[layerId + 1][i];
                        if (o > 1 || o < -1)
                            throw new ArgumentException("'o' вне границ (-1;1)");
                        double sum = 0;
                        for (int j = 0; j < LayersSizes[layerId + 2]; j++)
                        {
                            sum += ConnectionWeights[layerId + 1][j, i] * deltas[layerId + 1][j];
                        }
                        deltas[layerId][i] = o * (1 - o) * sum;
                    }
                }

                for (int i = 0; i < LayersCount; i++)
                {
                    for (int x = 0; x < ConnectionWeights[i].Width; x++)
                    {
                        for (int y = 0; y < ConnectionWeights[i].Height; y++)
                        {
                            var dl = eta * deltas[i][x] * outputs[i][y];
                            ConnectionWeights[i][x, y] += dl;
                        }
                    }
                }

                for (int i = 0; i < LayersCount; i++)
                {
                    for (int bid = 0; bid < LayersSizes[i + 1]; bid++)
                    {
                        var dl = eta * deltas[i][bid];
                        Bioses[i][bid] += dl;
                    }
                }
            }
        }
    }
}

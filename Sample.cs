using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroS;
namespace NeutoTests
{
    class Program
    {
        public static int NUM = 4;
        static void Main(string[] args)
        {
            var network = new SigmoidNetwork(new List<int> { 20, 150, 1 })
                .Randomize();

            var rand = new Random();
            int bunch = 500;
            int tests = 100;
            int iterations = 10000;
            int epochs = 10;
            double eta = 2;
            double maxSuccess = 0;
            for (int i = 0; i < iterations; i++)
            {
                var questions = new Line[bunch];
                var answers = new Line[bunch];
                for (int i = 0; i < bunch; i++)
                {
                    var next = rand.Next(1023, 1024*1023);
                    questions[i] = ToBinQ(next);
                    answers[i] = ToBinA(next);
                }
                network = network.Learning(eta, epochs, questions, answers);


                double success = 0;
                double ones = 0;
                for (int i = 0; i < tests; i++)
                {
                    var ans = network.Ask(ToBinQ(i))[0];
                    if (ans > 0.5)
                        ans = 1;
                    else
                        ans = 0;
                    if (ans == 1)
                        ones += 1;
                    if (ans == 1 && (i % NUM == 0))
                        success++;
                    if (ans == 0 && (i % NUM > 0))
                        success++;
                }
                maxSuccess = Math.Max(maxSuccess, success);
                Console.WriteLine(String.Format("{0}   |   {1}     |    {2}", success, maxSuccess, ones));
            }
            Console.WriteLine("end");
            Console.ReadKey();
        }

        public static Line ToBinQ(int number)
        {
            var ans = new double[20];
            for (int i = 19; i >= 0; i--)
            {
                if (number % 2 == 1)
                    ans[i] = 1;
                number /= 2;
            }
            return new Line(ans);
        }

        public static Line ToBinA(int number)
        {
            var ans = 0.0;
            if (number % NUM == 0)
                ans = 1;
            return new Line(new double[] { ans });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroS
{
    public class Line : Matrix
    {
        public Line(double[] line)
        {
            data = new double[line.Count(), 1];
            for (int i = 0; i < line.Count(); i++)
                data[i, 0] = line[i];
        }

        public Line(int width) : base(width, 1) { }

        public Line(Column column)
        {
            data = new double[column.Height, 1];
            for (int i = 0; i < column.Width; i++)
                data[i, 0] = column[i];
        }

        public Line(double[,] _data)
        {
            int width = _data.GetLength(0);
            int height = _data.GetLength(1);
            if (height == 1)
            {
                data = new double[width, 1];
                for (int i = 0; i < width; i++)
                    data[i, 0] = _data[i, 0];
            }
            else if (width == 1)
            {
                data = new double[height, 1];
                for (int i = 0; i < height; i++)
                    data[i, 0] = _data[0, i];
            }
            else
                throw new ArgumentException("Это настоящий двумерный массив, он не линия");
        }

        public static Line operator -(Line m1, Line m2)
        {
            if (m1.Width != m2.Width)
                throw new ArgumentException("Разная длина векторов");

            var ans = new Line(m1.Width);

            for (int k = 0; k < m1.Width; k++)
                ans[k] = m1[k] - m2[k];

            return ans;
        }

        public Line Select(Func<double, double> func)
        {
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    data[i, j] = func(data[i, j]);
            return this;
        }

        public double this[int x]
        {
            get
            {
                return data[x, 0];
            }

            set
            {
                data[x, 0] = value;
            }
        }
    }
}

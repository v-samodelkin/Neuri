using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroS
{
    public class Column : Matrix
    {
        public Column(double[] column)
        {
            data = new double[1, column.Count()];
            for (int i = 0; i < column.Count(); i++)
                data[0, i] = column[i];
        }

        public Column(int height) : base(1, height) { }

        public Column(Line line)
        {
            data = new double[1, line.Width];
            for (int i = 0; i < line.Width; i++)
                data[0, i] = line[i];
        }

        public Column(double[,] _data)
        {
            int width = _data.GetLength(0);
            int height = _data.GetLength(1);
            if (height == 1)
            {
                data = new double[1, width];
                for (int i = 0; i < width; i++)
                    data[0, i] = _data[i, 0];
            }
            else if (width == 1)
            {
                data = new double[1, height];
                for (int i = 0; i < height; i++)
                    data[0, i] = _data[0, i];
            }
            else
                throw new ArgumentException("Это настоящий двумерный массив, он не столбик");
        }

        public Column Select(Func<double, double> func)
        {
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    data[i, j] = func(data[i, j]);
            return this;
        }
        public double this[int y]
        {
            get
            {
                return data[0, y];
            }

            set
            {
                data[0, y] = value;
            }
        }
    }
}

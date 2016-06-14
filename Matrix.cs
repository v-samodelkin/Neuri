using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroS
{
    public class Matrix
    {
        protected double[,] data;
        public int Width
        {
            get
            {
                return data.GetLength(0);
            }
        }
        public int Height
        {
            get
            {
                return data.GetLength(1);
            }
        }

        public bool IsLine {
            get {
                return (Height == 1);
            }
        }

        public bool IsColumn {
            get {
                return (Width == 1);
            }
        }

        protected Matrix()
        {

        }

        public Matrix(double[,] _data)
        {
            data = _data;
        }

        public Matrix(int width, int height)
        {
            data = new double[width, height];
        }

        public double this[int x, int y]
        {
            get
            {
                return data[x, y];
            }

            set
            {
                data[x, y] = value;
            }
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.Width != m2.Height)
                throw new ArgumentException("Неперемножаемые матрицы");

            var ans = new Matrix(m1.Height, m2.Width);

            for (int ansx = 0; ansx < m1.Height; ansx++)
                for (int ansy = 0; ansy < m2.Width; ansy++)
                    for (int k = 0; k < m1.Width; k++)
                        ans[ansx, ansy] += m1[k, ansx] * m2[ansy, k];

            return ans;
        }



        public double[,] GetData()
        {
            return data;
        }

        public Line AsLine()
        {
            return new Line(data);
        }

        public Column AsColumn()
        {
            return new Column(data);
        }
    }
}

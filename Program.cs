using System;
using System.IO;
namespace Tools
{
	public class Program
	{
		public static void Main()
		{
			Vector2 pointer = Vector2.Unit;
			for (int i = -100; i < 100; i++) Console.WriteLine(FastMath.RadianAngleClamp(i / 180.0 * Math.PI));
			for (int i = 0; i < 720; i++)
				Console.WriteLine($"{i} {FastMath.Cos(i * (float)Math.PI / 180)} {FastMath.Sin(i * (float)Math.PI / 180)} {Vector2.VectorFromAngleAndSize(i).ToString()} {Math.Cos(i * Math.PI / 180)} {Math.Sin(i * Math.PI / 180)}");
		}
	}

	public static class EasyIO
	{
		public static void Write(string[] lines, string location)
		{
			if (location == null) throw new ArgumentNullException("path");
			if (lines == null) throw new ArgumentNullException("lines");

			if (lines.Length == 0)
			{
				using (File.Create(location)) { } // Create an empty file
				return;
			}
			using (StreamWriter stream = new StreamWriter(location))
			{
				for (int i = 0; i < lines.Length - 1; i++)
					stream.WriteLine(lines[i]);

				stream.Write(lines[lines.Length - 1]); // Writes last line without a closing newline character
			}
		}
		public static void Write(byte[] bytes, string location)
		{
			File.WriteAllBytes(location, bytes);
		}
		public static void Append(string[] lines, string location)
		{
			using (StreamWriter stream = new StreamWriter(location, true))
			{
				foreach (string line in lines)
				{
					stream.WriteLine(line);
				}
			}
		}
		public static string[] Read(string location)
		{
			string gluedLines = "";
			using (StreamReader stream = new StreamReader(location))
			{
				gluedLines = stream.ReadToEnd();
			}
			string[] lines = gluedLines.Split('\n');
			return lines;
		}
	}

	public static class FastMath // Math that is often slow or not included in the standard Math library
	{
		public static float Sinc(float x) => x == 0 ? 1 : Sin(x) / x;

		public static float Sin(float x) // Uses taylor series
		{
			//a = (float)(a*Math.PI/2+1)%2-1;
			//float a = (float)(2 * Math.PI * RadianAngleClamp(x) - Math.PI);
			float a = RadianAngleClamp(x);
			a = a > Math.PI ? 2 * (float)Math.PI - a : a;
			a = a > Math.PI / 2 ? (float)Math.PI - a : a;
			float a2 = a * a;
			return (RadianAngleClamp(x) > Math.PI ? -1 : 1) *
			/*  a*(1+a2*( //x
                (-1/6)+a2*( //-x^3/3!
                (1/120)+a2*( //x^5/5!
                (-1/5040)+a2*( //-x^7/7!
                (1/362880)+a2*( //x^9/9!
                (-1/39916800)+a2*( //-x^11/11!
                (1/6227020800) //x^13/13!
                )))))));*/
			a * (1 + a2 * ( //x
				-0.16666666666666666666666666666667f + a2 * ( //-x^3/3!
					0.00833333333333333333333333333333f + a2 * ( //x^5/5!
						-0.0001984126984126984126984126984127f + a2 * ( //-x^7/7!
							0.0000027557319223985890652557319223986f + a2 * ( //x^9/9!
								-0.000000025052108385441718775052108385442f + a2 * ( //-x^11/11!
									0.00000000016059043836821614599392377170155f //x^13/13!
								)))))));
		}
		public static float Cos(float a) => Sin(a + (float)Math.PI / 2);
		public static float DegreeAngleClamp(float x) => Mod(x, 360f);
		public static double DegreeAngleClamp(double x) => Mod(x, 360.0);
		public static float RadianAngleClamp(float x) => Mod(x, 2 * (float)Math.PI);
		public static double RadianAngleClamp(double x) => Mod(x, 2 * Math.PI);
		public static bool IsEven(long n) => (n & 1) == 0;
		public static bool IsOdd(long n) => (n & 1) == 1;
		public static double Abs(double a) => a < 0 ? -a : a;
		public static float Abs(float a) => a < 0 ? -a : a;
		public static double Mod(double a, double b) => a - Abs(b) * Floor(a / Abs(b)); // The floored type modulo in default c# is annoying
		public static float Mod(float a, float b) => a - Abs(b) * Floor(a / Abs(b));
		public static long Floor(double a) => a >= 0 || a % 1 == 0 ? (long)a : (long)a - 1; // Why are rounded/floored/ceiled(?) values doubles in default math? They are whole numbers!
		public static long Ceiling(double a) => a <= 0 || a % 1 == 0 ? (long)a : (long)a + 1;
		public static long Round(double a) => Floor(a + 0.5f);
	}

	public sealed class Vector2 : VectorN
	{
		public double x { get => this[0]; set => this[0] = value; }
		public double y { get => this[1]; set => this[1] = value; }
		public double size { get { return Math.Sqrt(this.x * this.x + this.y * this.y); } }
		public double sqrSize { get { return this.x * this.x + this.y * this.y; } }
		public Vector2 normalised { get { return this / this.size; } }
		public Vector2(double x = 0, double y = 0) : base(x, y) { }
		public Vector2(Vector2 v) : base(v.x, v.y) { }
		public static Vector2 VectorFromAngleAndSize(double angle, double m = 1)
		{
			return Vector2.Unit.RotateDegrees(angle) * m;
		}

		public static double DotProduct(Vector2 a, Vector2 b)
		{
			return a.x * b.x + a.y * b.y;
		}

		public bool SafeNormalise()
		{
			double l = this.sqrSize;
			if (l == 0) return false;
			this.x /= l;
			this.y /= l;
			return true;
		}
		public void Normalise()
		{
			double l = this.size;
			this.x /= l;
			this.y /= l;
		}
		public Vector2 AssignVector(Vector2 v)
		{
			this.x = v.x;
			this.y = v.y;
			return this;
		}
		public Vector2 RotateDegrees(double angle)
		{
			return this.RotateRadians(angle * Math.PI / 180);
		}
		public Vector2 RotateRadians(double angle)
		{
			this.AssignVector(this * new Vector2(Math.Cos(angle), Math.Sin(angle)));
			return this;
		}
		public Vector2 SetSize(double m)
		{
			this.Normalise();
			this.x *= m;
			this.y *= m;
			return this;
		}
		public double AngleInRadians()
		{
			if (this.x < 0)
			{
				if (this.y < 0) return Math.PI + Math.Acos(-this.x); // Quadrant 3
				else return Math.PI - Math.Acos(-this.x); // Quadrant 2
			}
			else
			{
				if (this.y < 0) return 2 * Math.PI - Math.Acos(this.x); // Quadrant 4
				return Math.Acos(this.x); // Quadrant 1
			}
		}
		public double AngleInDegrees()
		{
			return this.AngleInRadians() / Math.PI * 180;
		}
		public string CoordString()
		{
			return "(" + this.x + ", " + this.y + ")";
		}
		public override string ToString()
		{
			return this.x + " " + this.y;
		}
		public double DistanceTo(Vector2 v)
		{
			return (this - v).size;
		}

		public static Vector2 One { get { return new Vector2(1, 1); } }
		public static Vector2 Unit { get { return new Vector2(1, 0); } }
		public static Vector2 Zero { get { return new Vector2(0, 0); } }

		public override bool Equals(object o)
		{
			if (o.GetType() != typeof(Vector2)) return false;
			Vector2 oV = (Vector2)o;
			return oV.x == this.x && oV.y == this.y;
		}

		public static Vector2 operator +(Vector2 v1, Vector2 v2)
		{
			return new Vector2(v1.x + v2.x, v1.y + v2.y);
		}
		public static Vector2 operator -(Vector2 v1, Vector2 v2)
		{
			return new Vector2(v1.x + v2.x, v1.y + v2.y);
		}
		public static Vector2 operator *(Vector2 v1, Vector2 v2)
		{
			return new Vector2(v1.x * v2.x - v1.y * v2.y, v1.x * v2.y + v1.y * v2.x); //(a+bi)(c+di) = ac-bd + (ad+bc)i
		}
		public static Vector2 operator *(Vector2 v, double m)
		{
			return new Vector2(v.x * m, v.y * m);
		}
		public static Vector2 operator *(double m, Vector2 v)
		{
			return new Vector2(v.x * m, v.y * m);
		}
		public static Vector2 operator /(Vector2 v, double m)
		{
			return new Vector2(v.x / m, v.y / m);
		}
		public static Vector2 operator %(Vector2 v, double m)
		{
			return new Vector2(v.x % m, v.y % m);
		}

		public static Vector2 operator -(Vector2 v)
		{
			return new Vector2(-v.x, -v.y);
		}

		public static bool operator ==(Vector2 v1, Vector2 v2)
		{
			return v1.x == v2.x && v1.y == v2.y;
		}
		public static bool operator !=(Vector2 v1, Vector2 v2)
		{
			return v1.x != v2.x || v1.y != v2.y;
		}
		public static bool operator >(Vector2 v, double m)
		{
			return v.sqrSize > m * m;
		}
		public static bool operator <(Vector2 v, double m)
		{
			return v.sqrSize < m * m;
		}
		public static bool operator >=(Vector2 v, double m)
		{
			return v.sqrSize >= m * m;
		}
		public static bool operator <=(Vector2 v, double m)
		{
			return v.sqrSize <= m * m;
		}

	}

	public class VectorN
	{
		private double[] data;
		public readonly int dimensions;

		public double this[int index]
		{
			get => data[index];
			set => data[index] = value;
		}

		public VectorN(params double[] values)
		{
			this.data = values;
			this.dimensions = values.Length;
		}
		public VectorN(int dimensions) : this(new double[dimensions]) { }
		public VectorN(VectorN vector) : this((double[])vector) { }

		public static explicit operator double[](VectorN vector)
		{
			double[] data = new double[vector.dimensions];
			for (int i = 0; i < vector.dimensions; i++)
				data[i] = vector[i];
			return data;

		}
	}

	// Adapted from coding train code: https://thecodingtrain.com/CodingChallenges/112-3d-rendering.html
	public sealed class Matrix
	{
		private double[,] matrix;
		public readonly int width;
		public readonly int height;

		public double this[int x, int y]
		{
			get => matrix[x, y];
			set => matrix[x, y] = value;
		}

		public Matrix(double[] array)
		{
			this.matrix = new double[array.Length, 1];
			for (int i = 0; i < array.Length; i++)
				this.matrix[i, 0] = array[i];
			this.width = 1;
			this.height = matrix.GetLength(0);
		}
		public Matrix(double[,] matrix)
		{
			this.matrix = matrix;
			this.width = matrix.GetLength(1); // I hate that this has to be backwards, ugh. Come on math, just use standard cartesian coordinates smh
			this.height = matrix.GetLength(0);
		}
		public Matrix(VectorN vector) : this((double[])vector) { }

		public static VectorN matrixToVec(Matrix m)
		{
			double[] data = new double[m.height];
			return new VectorN(m[0, 0], m[1, 0], m.height > 2 ? m[2, 0] : 0);
		}

		public void Print()
		{
			int cols = this.width;
			int rows = this.height;
			Console.WriteLine($"{rows}x{cols}");
			Console.WriteLine("----------------");
			for (int i = 0; i < rows; i++)
			{
				string line = "";
				for (int j = 0; j < cols; j++)
				{
					line += (this[i, j] + " ");
				}
				Console.WriteLine(line);
			}
			Console.WriteLine();
		}
		public static VectorN matmulvec(Matrix a, VectorN vec)
		{
			Matrix m = new Matrix(vec);
			Matrix r = matmul(a, m);
			return matrixToVec(r);
		}
		public static Matrix matmul(Matrix a, Matrix b)
		{
			int colsA = a.width;
			int rowsA = a.height;
			int colsB = b.width;
			int rowsB = b.height;

			if (colsA != rowsB) throw new ArgumentException("Columns of A must match rows of B");

			double[,] result = new double[rowsA, colsB];
			for (int j = 0; j < rowsA; j++)
			{
				for (int i = 0; i < colsB; i++)
				{
					double sum = 0;
					for (int n = 0; n < colsA; n++)
					{
						sum += a[j, n] * b[n, i];
					}
					result[j, i] = sum;
				}
			}
			return new Matrix(result);
		}
	}
}

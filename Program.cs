using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
namespace Tools
{
	public static class EasyBinary
	{
		/// <summary>
		/// Converts a byte to 8 bools.
		/// </summary>
		/// <param name="dat">The bool to parse.</param>
		/// <param name="MSBo">Most significant bit order: Determines if 1 is at the most significant bit (leftmost).</param>
		/// <returns></returns>
		public static bool[] SplitByte(byte binary, bool MSBo = false)
		{
			// MSB
			if (MSBo)
				return new bool[] {
					(binary & 1  ) == 1,
					(binary & 2  ) == 2,
					(binary & 4  ) == 4,
					(binary & 8  ) == 8,
					(binary & 16 ) == 16,
					(binary & 32 ) == 32,
					(binary & 64 ) == 64,
					(binary & 128) == 128
				};

			// LSB
			return new bool[] {
				(binary & 128) == 128,
				(binary & 64 ) == 64,
				(binary & 32 ) == 32,
				(binary & 16 ) == 16,
				(binary & 8  ) == 8,
				(binary & 4  ) == 4,
				(binary & 2  ) == 2,
				(binary & 1  ) == 1
			};
		}

		public static byte[] MakeBytes(bool[] bits, bool strict = false, bool MSBo = false)
		{
			int dataLen = bits.Length >> 3;
			byte[] data;

			if ((bits.Length & 7) != 0)
			{
				if (strict)
					throw new System.ArgumentException($"In strict mode the length of the passed bit array must be a multiple of 8 to construct a byte array. {bits.Length} bits were passed.");
				else
					data = new byte[dataLen + 1];
			}
			else
			{
				data = new byte[dataLen];
			}

			if (MSBo)
			{
				for (int i = 0; i < dataLen; i++)
					data[i] = MakeByte(EasyArray.SubArray(bits, bits.Length - 9 - i << 3, 8), strict, true);
				if ((bits.Length & 7) != 0)
					data[dataLen] = MakeByte(EasyArray.SubArray(bits, 0, bits.Length & 7), strict, true);
			}
			else
			{
				for (int i = 0; i < dataLen; i++)
					data[i] = MakeByte(EasyArray.SubArray(bits, i << 3, 8), strict, false);
				if ((bits.Length & 7) != 0)
					data[dataLen] = MakeByte(EasyArray.SubArray(bits, dataLen << 3, bits.Length & 7), strict, false);
			}

			return data;
		}

		public static byte MakeByte(bool[] bits, bool strict = false, bool MSBo = false)
		{
			if (bits.Length != 8)
			{
				if (strict)
					throw new System.ArgumentException($"In strict mode a full 8 bits must be passed to construct a byte. {bits.Length} bits were passed.");

				if (bits.Length > 8)
					bits = bits.SubArray(bits.Length - 8, 8);

				else
					Array.Resize(ref bits, 8);
			}
			if (MSBo)
			{
				return (byte)(
					(bits[7] ? 128 : 0) |
					(bits[6] ? 64 : 0) |
					(bits[5] ? 32 : 0) |
					(bits[4] ? 16 : 0) |
					(bits[3] ? 8 : 0) |
					(bits[2] ? 4 : 0) |
					(bits[1] ? 2 : 0) |
					(bits[0] ? 1 : 0)
				);
			}
			return (byte)(
				(bits[0] ? 128 : 0) |
				(bits[1] ? 64 : 0) |
				(bits[2] ? 32 : 0) |
				(bits[3] ? 16 : 0) |
				(bits[4] ? 8 : 0) |
				(bits[5] ? 4 : 0) |
				(bits[6] ? 2 : 0) |
				(bits[7] ? 1 : 0)
			);
		}

		public static string ToString(byte binary)
		{
			string strOut = "";

			foreach (bool bit in SplitByte(binary))
				strOut += bit ? "1" : "0";

			return strOut;
		}

		public static string ToString(byte[] binary)
		{
			string strOut = "";

			foreach (byte data in binary)
				foreach (bool bit in SplitByte(data))
					strOut += bit ? "1" : "0";

			return strOut;
		}

		public static string ToString(bool[] bits)
		{
			string strOut = "";

			foreach (bool bit in bits)
				strOut += bit ? "1" : "0";

			return strOut;
		}


		/// <summary>Shifts a byte left, except bits are wrapped and not truncated</summary>
		public static byte CyclicLeftShift(byte data, int shift)
		{
			shift &= 0x7;
			byte wrap = (byte)(data >> (8 - shift));
			return (byte)((data << shift) | wrap);
		}
		/// <summary>Shifts an sbyte left, except bits are wrapped and not truncated</summary>
		public static sbyte CyclicLeftShift(sbyte data, int shift)
		{
			shift &= 0x7;
			byte num = (byte)data;
			byte wrap = (byte)(num >> (8 - shift));
			return (sbyte)((num << shift) | wrap);
		}
		/// <summary>Shifts an uint left, except bits are wrapped and not truncated</summary>
		public static uint CyclicLeftShift(uint data, int shift)
		{
			shift &= 0x1F;
			uint wrap = data >> (32 - shift);
			return (data << shift) | wrap;
		}
		/// <summary>Shifts an int left, except bits are wrapped and not truncated</summary>
		public static int CyclicLeftShift(int data, int shift)
		{
			shift &= 0x1F;
			uint num = (uint)data;
			uint wrap = num >> (32 - shift);
			return (int)((num << shift) | wrap);
		}
		/// <summary>Shifts an ulong left, except bits are wrapped and not truncated</summary>
		public static ulong CyclicLeftShift(ulong data, int shift)
		{
			shift &= 0x3F;
			ulong wrap = data >> (64 - shift);
			return (data << shift) | wrap;
		}
		/// <summary>Shifts a long left, except bits are wrapped and not truncated</summary>
		public static long CyclicLeftShift(long data, int shift)
		{
			shift &= 0x3F;
			ulong num = (ulong)data;
			ulong wrap = num >> (64 - shift);
			return (long)((num << shift) | wrap);
		}

		/// <summary>Shifts a byte right, except bits are wrapped and not truncated</summary>
		public static byte CyclicRightShift(byte data, int shift)
		{
			shift &= 0x7;
			byte wrap = (byte)(data << (8 - shift));
			return (byte)((data >> shift) | wrap);
		}
		/// <summary>Shifts an sbyte right, except bits are wrapped and not truncated</summary>
		public static sbyte CyclicRightShift(sbyte data, int shift)
		{
			shift &= 0x7;
			byte num = (byte)data;
			byte wrap = (byte)(num << (8 - shift));
			return (sbyte)((num >> shift) | wrap);
		}
		/// <summary>Shifts an uint right, except bits are wrapped and not truncated</summary>
		public static uint CyclicRightShift(uint data, int shift)
		{
			shift &= 0x1F;
			uint wrap = data << (32 - shift);
			return (data >> shift) | wrap;
		}
		/// <summary>Shifts an int right, except bits are wrapped and not truncated</summary>
		public static int CyclicRightShift(int data, int shift)
		{
			shift &= 0x1F;
			uint num = (uint)data;
			uint wrap = num << (32 - shift);
			return (int)((num >> shift) | wrap);
		}
		/// <summary>Shifts an ulong right, except bits are wrapped and not truncated</summary>
		public static ulong CyclicRightShift(ulong data, int shift)
		{
			shift &= 0x3F;
			ulong wrap = data << (64 - shift);
			return (data >> shift) | wrap;
		}
		/// <summary>Shifts a long right, except bits are wrapped and not truncated</summary>
		public static long CyclicRightShift(long data, int shift)
		{
			shift &= 0x3F;
			ulong num = (ulong)data;
			ulong wrap = num << (64 - shift);
			return (long)((num >> shift) | wrap);
		}
	}

	public static class EasyArray
	{
		///	<summary>
		/// This method takes a specified subsection of an array.
		/// </summary>
		/// <example>
		/// Here is an example use of the SubArray method:
		/// <code>
		/// int[] array = { 0, 1, 2, 3, 4, 5 };
		/// int[] subArray = array.SubArray( 2, 3 ); // { 2, 3, 4 }
		/// </code>
		/// </example>
		public static T[] SubArray<T>(this T[] data, int index, int length)
		{
			T[] result = new T[length];
			Array.Copy(data, index, result, 0, length);
			return result;
		}

		/// <summary>
		/// Searches an array for all occurrances of selected targets.
		/// </summary>
		/// <example>
		/// Here is an example use of the SubArray method:
		/// <code>
		/// string[] array = { "lemon", "apple", "orange", "peach"
		/// int[] subArray = array.SubArray( 2, 3 ); // { 2, 3, 4 }
		/// </code>
		/// </example>
		public static Dictionary<int, T> SearchFor<T>(this T[] data, params T[] targets)
		{
			Dictionary<int, T> hits = new Dictionary<int, T>();
			for (int i = 0; i < data.Length; i++)
				foreach (T target in targets)
					if (data[i].Equals(target))
					{
						hits[i] = data[i];
						continue;
					}
			return hits;
		}
	}

	public static class EasyRegex
	{
		public static MatchCollection[] CSVParseByLine(string[] lines)
		{
			List<MatchCollection> lineMatches = new List<MatchCollection>();
			Regex splitter = new Regex(@"\s*([^,]*[^\s,])\s*,?");

			foreach (string line in lines)
				lineMatches.Add(splitter.Matches(line));

			return lineMatches.ToArray();
		}

		public static Dictionary<string, string> CSVParseDictionary(string data)
		{
			Regex splitter = new Regex(@"^[\t ]*(?<key>[^\n,]*[^\s,])[\t ]*,[\t ]*(?<pair>[^\n,]*[^\s,])", RegexOptions.Multiline);
			MatchCollection matches = splitter.Matches(data);
			Dictionary<string, string> dictOut = new Dictionary<string, string>();

			foreach (Match m in matches)
				if (m.Groups["key"].Success && m.Groups["pair"].Success)
					dictOut[m.Groups["key"].Value] = m.Groups["pair"].Value;

			return dictOut;
		}
	}

	public static class EasyIO
	{
		public static void Write(this string location, string[] lines)
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
		public static void Write(this string location, byte[] bytes)
		{
			File.WriteAllBytes(location, bytes);
		}
		public static void Append(this string location, string[] lines)
		{
			using (StreamWriter stream = new StreamWriter(location, true))
			{
				foreach (string line in lines)
				{
					stream.WriteLine(line);
				}
			}
		}
		public static IEnumerable<string> Read(this string location)
		{
			// Adapted from https://stackoverflow.com/a/23408020/11335381
			using (StreamReader stream = new StreamReader(location))
			{
				string line;
				while ((line = stream.ReadLine()) != null)
					yield return line;
			}
		}
		public static string ReadAll(this string location)
		{
			string lines = "";
			using (StreamReader stream = new StreamReader(location))
			{
				lines = stream.ReadToEnd();
			}
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
		public override int GetHashCode()
		{
			return EasyBinary.CyclicLeftShift(x.GetHashCode(), 6) ^ y.GetHashCode();
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

		// To prevent somebody changing the size of the array, but not the corresponding dimension
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

		public double[,] data
		{
			get => matrix;
		}
		public readonly int width;
		public readonly int height;

		public double this[int y, int x]
		{
			get => matrix[y, x];
			set => matrix[y, x] = value;
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
			this.width = matrix.GetLength(1);
			this.height = matrix.GetLength(0);
		}
		public Matrix(Matrix matrix)
		{
			this.matrix = matrix.data;
			this.width = matrix.width;
			this.height = matrix.height;
		}
		public Matrix(VectorN vector) : this((double[])vector) { }

		public static VectorN MatrixToVec(Matrix m)
		{
			double[] data = new double[m.height];

			for (int i = 0; i < m.height; i++) data[i] = m[i, 0];

			return new VectorN(data);
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
		public static VectorN MatmulVec(Matrix a, VectorN vec)
		{
			Matrix m = new Matrix(vec);
			Matrix r = Matmul(a, m);
			return MatrixToVec(r);
		}
		public static Matrix Matmul(Matrix a, Matrix b)
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
		public static Matrix operator *(Matrix a, double m)
		{
			Matrix b = new Matrix(a);
			for (int i = 0; i < a.height; i++)
				for (int j = 0; j < a.width; j++)
					b[i, j] *= m;
			return b;
		}
		public static Matrix operator *(double m, Matrix a)
		{
			return a * m;
		}
		public static explicit operator VectorN(Matrix matrix)
		{
			double[] data = new double[matrix.height];
			for (int i = 0; i < matrix.height; i++)
				data[i] = matrix[i, 0];
			return new VectorN(data);

		}
	}
}

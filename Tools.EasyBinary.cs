namespace Tools.EasyBinary
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
					System.Array.Resize(ref bits, 8);
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
}

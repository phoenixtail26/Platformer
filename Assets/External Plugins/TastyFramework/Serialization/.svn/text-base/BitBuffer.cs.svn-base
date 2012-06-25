/*
 * BitBuffer class
 * For serializing on a per bit level to a buffer
 * Can be used for sending data over the network
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
	public class BitBuffer 
	{
		static Crc32 _crcHasher = new Crc32();
		
		public BitBuffer(byte[] data)
		{
			_buffer = new List<byte>(data);
			_bitLocation = 0;
			_bitCount = 0;
		}
		
		public BitBuffer()
		{
			_buffer = new List<byte>();
			_buffer.Add(0);
			_bitLocation = 0;
			_bitCount = 0;
		}
		
		public void Write(bool bit)
		{
			int byteLocation = (int)(_bitLocation >> 3);
			
			if(byteLocation >= _buffer.Count)
			{
				_buffer.Add(0);
			}
			
			if(bit)
			{
				int shift = (int) (_bitLocation & 0x7);
				_buffer[byteLocation] = (byte) (_buffer[byteLocation] | (0x1 << shift));
			}
			
			_bitLocation ++;
			
			if(_bitLocation > _bitCount)
			{
				_bitCount ++;
			}
		}
		
		public void Write(byte data, int numBits)
		{
			for(int n = 0; n<numBits; n++)
			{
				Write(((data >> n) & 0x1) != 0);
			}
		}
		
		public void Write(byte[] data, int numBits)
		{
			for(int i = 0; i<data.Length && numBits > 0; i++)
			{
				byte b = data[i];
				
				for(int n = 0; n<8 && numBits > 0; n++)
				{
					Write(((b >> n) & 0x1) != 0);
					numBits --;
				}
			}
		}

		public void Write(byte[] data)
		{
			for(int i = 0; i<data.Length; i++)
			{
				byte b = data[i];
				
				for(int n = 0; n<8; n++)
				{
					Write(((b >> n) & 0x1) != 0);
				}
			}
		}
		
		public void Write(BitBuffer data)
		{
			Write(data.ToArray(), (int) data.bitCount);
		}
		
		public bool Read()
		{
			int byteLocation = (int)(_bitLocation >> 3);
			
			if(byteLocation >= _buffer.Count)
			{
				Debug.LogError("Read past end of buffer: " + _bitLocation);
				Debug.LogError(StackTraceUtility.ExtractStackTrace());
				return false;
			}
			
			int shift = (int) (_bitLocation & 0x7);
			
			_bitLocation ++;
			return (_buffer[byteLocation] & (0x1 << shift)) != 0;
		}
		
		public byte[] Read(int numBits)
		{
			return Read(numBits, 0);
		}
			
		public byte[] Read(int numBits, int minBytes)
		{
			int bytesNeeded = (numBits >> 3) + ((numBits & 0x7) == 0 ? 0 : 1);
			if(bytesNeeded < minBytes)
				bytesNeeded = minBytes;
			byte[] output = new byte[bytesNeeded];
			
			for(int n = 0; n<numBits; n++)
			{
				int byteLocation = (n >> 3);
				
				if(Read())
				{
					int shift = (n & 0x7);
					output[byteLocation] |= (byte) ((0x1 << shift));
				}
			}
			
			return output;
		}
		
		public void Seek(long seekPosition)
		{
			if(_bitLocation > _bitCount)
			{
				Debug.LogError("Cannot seek past end of buffer!");
				return;
			}
			
			_bitLocation = seekPosition;
		}
		
		public byte[] ToArray()
		{
			return _buffer.ToArray();
		}
		
		public void Dump()
		{
			Debug.Log("Buffer length: " + _buffer.Count);
			Debug.Log("Bit location: " + _bitLocation);
			
			string dump = "";
			
			foreach(byte b in _buffer)
			{
				dump = dump + (int) b + " ";
			}
			Debug.Log("buf: " + dump);
			Debug.Log("Hash: " + hash);
		}
		
		public long seekPosition
		{
			get { return (_bitLocation >> 3); }
		}
		
		public long bitCount
		{
			get { return _bitCount; }
		}
		
		private List<byte> _buffer;
		private long _bitLocation;
		private long _bitCount;
		
		public uint hash
		{
			get
			{
				return _crcHasher.ComputeChecksum(_buffer.ToArray());
			}
		}
	}
}
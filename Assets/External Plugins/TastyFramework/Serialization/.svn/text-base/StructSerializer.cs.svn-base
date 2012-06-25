/*
 * StructSerializer class
 * 
 * -Takes an input struct and serializes it an array of bytes
 * e.g. byte[] rawBytes = StructSerializer.ToBytes<MyStruct>(myStruct);
 * -Takes an array of bytes and serializes to a struct
 * e.g. int bytesUsed = StructSerializer.ToStruct<MyStruct>(rawBytes, out myStruct);
 * 
 * Custom attributes can be used to control how struct members are encoded:
 * [BitPrecision(6)]	// Integer values are stored using 6 bits
 * [CompressedEnum()]	// Enum values are compressed to the fewest required bits
 * [SerializeAsFixed(8, min = -1.0f, max = 1.0f)	// Floating point values stored using 8 bits, range of -1.0f to 1.0f
 * 
 * NOTE: Class (objects) will not be serialized. Only primitive and structs will work.
 */

// Undefine this if you want remove the warnings when precision is lost
#define WARN_ON_TRUNCATION 

using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace Framework
{
#region Custom attributes
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class BitPrecision : System.Attribute
	{
	    public int numBits;
	
	    public BitPrecision(int bits)
	    {
	        this.numBits = bits;
	    }
	}

	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class CompressedEnum : System.Attribute
	{
	}

	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class SerializeAsFixed : System.Attribute
	{
		public float min;
		public float max;
		public int numBits;
		
		public SerializeAsFixed(int bits)
		{
			this.numBits = bits;
			min = (float) ((0x1 << bits) - 1);
			max = -max;
		}
	}

#endregion
	
	public static class StructSerializer
	{
		private static int GetBitsRequiredForSignedInt(int min, int max)
		{
			max = Mathf.Max(Mathf.Abs(min), Mathf.Abs(max));
			
			int bits = 0;
			while(max > 0)
			{
				max = max >> 1;
				bits ++;
			}
			
			return bits;
		}
		
		private static T GetCustomAttribute<T>(FieldInfo info) where T : System.Attribute
		{
			System.Object[] customAttributes = info.GetCustomAttributes(typeof(T), true);
			foreach(System.Object attribute in customAttributes)
			{
				return (T) attribute;
			}
			return null;
		}
		
		private static int GetBitPrecision(FieldInfo info, System.Object obj)
		{
			System.Object[] customAttributes = info.GetCustomAttributes(true);
			foreach(System.Object attribute in customAttributes)
			{
				if(attribute.GetType() == typeof(BitPrecision))
				{
					if(obj is float)
					{
						Debug.LogError("Cannot use attribute BitPrecision on float, consider using SerializeAsFixed");
					}
					else
					{
						return ((BitPrecision) attribute).numBits;
					}
				}
				else if(attribute.GetType() == typeof(SerializeAsFixed))
				{
					if(obj is float)
					{
						return ((SerializeAsFixed) attribute).numBits;
					}
					else
					{
						Debug.LogError("Cannot use SerializeAsFixed attribute on type " + obj.GetType());
					}
				}
				else if(attribute.GetType() == typeof(CompressedEnum) && obj.GetType().IsEnum)
				{
					System.Array values = System.Enum.GetValues(obj.GetType());
					int min = 0, max = 0;
					
					foreach(int val in values)
					{
						if(val < min)
							min = val;
						if(val > max)
							max = val;
					}
					
					return GetBitsRequiredForSignedInt(min, max);
				}
			}
			
			// No pretty way to do this :(
			if(obj is int)
				return sizeof(int) << 3;
			if(obj is long)
				return sizeof(long) << 3;
			if(obj is float)
				return sizeof(float) << 3;
			if(obj is short)
				return sizeof(short) << 3;
			if(obj.GetType().IsEnum)
				return 32;
			
			Debug.LogError("Don't know size of " + info.Name);
			return 0;
		}
		
#region Serializing to bytes
		public static byte[] ToBytes<T>(T obj) where T : struct
		{
			BitBuffer buffer = new BitBuffer();
			
			StructToBytes(obj, buffer);
			
			return buffer.ToArray();
		}
		
		public static void ToBuffer<T>(T obj, BitBuffer buffer) where T : struct
		{
			StructToBytes(obj, buffer);
		}
		
		private static void StructToBytes(System.Object obj, BitBuffer buffer) 
		{
			System.Type objType = obj.GetType();
			FieldInfo[] fi = objType.GetFields(BindingFlags.Public | BindingFlags.Instance);
			
			foreach(FieldInfo info in fi)
			{
				System.Object fieldValue = info.GetValue(obj);
				System.Type fieldType = fieldValue.GetType();
				
				if(!fieldType.IsClass && !fieldType.IsEnum && fieldType.IsValueType && !fieldType.IsPrimitive)
				{
					StructToBytes(fieldValue, buffer);
				}
				else
				{
					int numBits = GetBitPrecision(info, fieldValue);
					
					if(fieldValue is int)
					{
						buffer.Write(System.BitConverter.GetBytes((int) fieldValue), numBits);
					}
					else if(fieldValue is uint)
					{
						buffer.Write(System.BitConverter.GetBytes((uint) fieldValue), numBits);
					}
					else if(fieldValue is long)
					{
						buffer.Write(System.BitConverter.GetBytes((long) fieldValue), numBits);
					}
					else if(fieldValue is ulong)
					{
						buffer.Write(System.BitConverter.GetBytes((ulong) fieldValue), numBits);
					}
					else if(fieldValue is float)
					{
						if(numBits == (sizeof(float) << 3))
						{
							buffer.Write(System.BitConverter.GetBytes((float) fieldValue), numBits);
						}
						else
						{
							SerializeAsFixed attribute = GetCustomAttribute<SerializeAsFixed>(info);
							if(attribute == null)
							{
								Debug.LogError("Error calculating number of bits needed for " + info.Name);
							}
							else
							{
								float fieldFloat = (float) fieldValue;
								if(fieldFloat < attribute.min || fieldFloat > attribute.max)
								{
#if WARN_ON_TRUNCATION
									Debug.LogWarning("Truncation on floating point field " + info.Name + " when serialized as fixed point value! "
										+ " Value was: " + fieldFloat + " min: " + attribute.min + " max: " + attribute.max);
#endif
									fieldFloat = Mathf.Clamp(fieldFloat, attribute.min, attribute.max);
								}
								int maxRepresented = (0x1 << attribute.numBits) - 1;
								float normalized = ((fieldFloat) - attribute.min) / (attribute.max - attribute.min);
								uint fixedValue = (uint) (normalized * maxRepresented);
								buffer.Write(System.BitConverter.GetBytes(fixedValue), numBits);
							}
						}
					}
					else if(fieldType.IsEnum)
					{
						buffer.Write(System.BitConverter.GetBytes((int) fieldValue), numBits);
					}
					else
					{
						Debug.LogError("Unsupported type: " + fieldType.Name);
					}
				}
			}
		}
#endregion
		
#region Deserializing to struct
		
		public static void ToStruct<T>(BitBuffer buffer, out T objOut) where T : struct
		{
			System.Object obj = new T();
			
			BytesToStruct(ref obj, buffer);
			
			objOut = (T) obj;
		}
		
		public static long ToStruct<T>(byte[] data, out T objOut) where T : struct
		{
			System.Object obj = new T();
			BitBuffer buffer = new BitBuffer(data);
			
			BytesToStruct(ref obj, buffer);
			
			objOut = (T) obj;
			
			return buffer.seekPosition;
		}

		private static uint SignExtend(uint data, int numBits)
		{
			uint mask = (1U << (numBits - 1)); 
			
		//	data = (data & ((1U << numBits) - 1));		// Shouldn't need to as it should be zero
			
			data = (data ^ mask) - mask;
			return data;
		}
	
		private static ulong SignExtend(ulong data, int numBits)
		{
			ulong mask = (1U << (numBits - 1)); 
			
		///	data = (data & ((1U << numBits) - 1));		// Shouldn't need to as it should be zero
			
			data = (data ^ mask) - mask;
			return data;
		}

		private static void BytesToStruct(ref System.Object obj, BitBuffer buffer)
		{
			System.Type objType = obj.GetType();
			FieldInfo[] fi = objType.GetFields(BindingFlags.Public | BindingFlags.Instance);
			
			foreach(FieldInfo info in fi)
			{
				System.Object fieldValue = info.GetValue(obj);
				System.Type fieldType = fieldValue.GetType();

				if(!fieldType.IsClass && !fieldType.IsEnum && fieldType.IsValueType && !fieldType.IsPrimitive)
				{
					BytesToStruct(ref fieldValue, buffer);
					info.SetValue(obj, fieldValue);
				}
				else
				{
					int numBits = GetBitPrecision(info, fieldValue);
					if(fieldValue is int)
					{
						info.SetValue(obj, (int) SignExtend(System.BitConverter.ToUInt32(buffer.Read(numBits, sizeof(int)), 0), numBits));
					}
					else if(fieldValue is uint)
					{
						info.SetValue(obj, System.BitConverter.ToUInt32(buffer.Read(numBits, sizeof(uint)), 0));
					}
					else if(fieldValue is long)
					{
						info.SetValue(obj, (long) SignExtend(System.BitConverter.ToUInt64(buffer.Read(numBits, sizeof(long)), 0), numBits));
					}
					else if(fieldValue is ulong)
					{
						info.SetValue(obj, System.BitConverter.ToUInt64(buffer.Read(numBits, sizeof(ulong)), 0));
					}
					else if(fieldValue is float)
					{
						if(numBits == (sizeof(float) << 3))
						{
							info.SetValue(obj, System.BitConverter.ToSingle(buffer.Read(numBits, sizeof(float)), 0));
						}
						else
						{
							SerializeAsFixed attribute = GetCustomAttribute<SerializeAsFixed>(info);
							if(attribute == null)
							{
								Debug.LogError("Error calculating number of bits needed for " + info.Name);
							}
							else
							{
								int maxRepresented = (0x1 << attribute.numBits) - 1;
								uint fixedValue = System.BitConverter.ToUInt32(buffer.Read(numBits, sizeof(int)), 0);
								float normalized = ((float)(fixedValue)) / maxRepresented;
								float result = normalized * (attribute.max - attribute.min) + attribute.min;
								
								info.SetValue(obj, result);
							}
						}
						
					}
					else if(fieldType.IsEnum)
					{
						info.SetValue(obj, (int) SignExtend(System.BitConverter.ToUInt32(buffer.Read(numBits, sizeof(int)), 0), numBits));
					}
					else
					{
						Debug.LogError("Unsupported type: " + fieldType.Name);
					}
				}
			}
		}	
#endregion
	}
}
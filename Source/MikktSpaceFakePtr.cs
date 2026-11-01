using System;

namespace MikkTSpaceSharp
{
	public struct MikktSpaceFakePtr<T> where T : unmanaged
	{
		public static readonly MikktSpaceFakePtr<T> Null = new MikktSpaceFakePtr<T>();

		private T[] _data;
		private long _offset;

		public bool IsNull => _data == null;

		public T[] Data => _data;

		public long Offset
		{
			get => _offset;
			set => _offset = value;
		}

		public T this[long index]
		{
			get => _data[index + _offset];
			set => _data[index + _offset] = value;
		}

		public T FirstValue
		{
			get => _data[_offset];
			set => _data[_offset] = value;
		}

		public int Length => _data.Length;

		public MikktSpaceFakePtr(T[] data, long offset = 0)
		{
			_data = data;
			_offset = offset;
		}

		public MikktSpaceFakePtr(long size)
		{
			_data = new T[size];
			_offset = 0;
		}

		public MikktSpaceFakePtr<T> SubBuffer(long offset)
		{
			return new MikktSpaceFakePtr<T>(_data, _offset + offset);
		}

		public void Write(T value)
		{
			_data[_offset] = value;
			++_offset;
		}

		public T Read()
		{
			var result = _data[_offset];
			++_offset;

			return result;
		}

		public void Set(T value, long size)
		{
			for (long i = 0; i < size; i++)
			{
				_data[_offset + i] = value;
			}
		}

		public void Set(T value) => Set(value, _data.Length - _offset);

		public void Copy(long offset, MikktSpaceFakePtr<T> source, long sourceOffset, long size)
		{
			Array.Copy(source._data, source._offset + sourceOffset, _data, _offset + offset, size);
		}

		public void Copy(MikktSpaceFakePtr<T> source, long sourceOffset, long size) => Copy(0, source, sourceOffset, size);

		public void Copy(MikktSpaceFakePtr<T> source, long size) => Copy(0, source, 0, size);

		public void Copy(long offset, T[] source, long sourceOffset, long size)
		{
			Array.Copy(source, sourceOffset, _data, _offset + offset, size);
		}

		public MikktSpaceFakePtr<T> Expand(long newSize)
		{
			if (newSize <= _data.Length)
				return this;
			var newBuffer = new MikktSpaceFakePtr<T>(newSize);
			Array.Copy(_data, 0, newBuffer._data, 0, _data.Length);
			newBuffer._offset = _offset;
			return newBuffer;
		}

		public static MikktSpaceFakePtr<T> operator +(MikktSpaceFakePtr<T> buffer, long offset)
		{
			return new MikktSpaceFakePtr<T>(buffer._data, buffer._offset + offset);
		}

		public static MikktSpaceFakePtr<T> operator -(MikktSpaceFakePtr<T> buffer, long offset)
		{
			return new MikktSpaceFakePtr<T>(buffer._data, buffer._offset - offset);
		}
	}
}

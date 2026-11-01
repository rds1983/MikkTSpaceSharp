namespace MikkTSpaceSharp
{
	public struct MikktSpaceFakePtr<T> where T : unmanaged
	{
		private T[] _data;
		private long _offset;

		public T this[long index]
		{
			get => _data[index + _offset];
			set => _data[index + _offset] = value;
		}

		public MikktSpaceFakePtr(T[] data, long offset = 0)
		{
			_data = data;
			_offset = offset;
		}
	}
}

#pragma once

#include <memory>

#include "mikktspace.c"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace MikkTSpaceNativeWrapper {
	struct InternalVec3
	{
	public:
		float X, Y, Z;
	};

	struct InternalVec2
	{
	public:
		float X, Y;
	};


	struct InternalVData
	{
	public:
		InternalVec3 Position;
		InternalVec3 Normal;
		InternalVec2 UV;
		InternalVec3 Tangent;
		InternalVec3 BiTangent;
	};

	public value struct Vec3
	{
	public:
		float X, Y, Z;
	};

	public value struct Vec2
	{
	public:
		float X, Y;
	};


	public value struct VData
	{
	public:
		Vec3 Position;
		Vec3 Normal;
		Vec2 UV;
		Vec3 Tangent;
		Vec3 BiTangent;
	};

	class Internal
	{
		static InternalVData *_data;
		static unsigned int *_indices;
		static int _vertexCount, _indexCount;

		static int GetNumFaces(const SMikkTSpaceContext* pContext)
		{
			return _indexCount / 3;
		}
	public:

		static bool CalculateTangents(array<VData>^ data, array<unsigned int>^ indices)
		{
			_data = new InternalVData[data->Length];
			_vertexCount = data->Length;
			_indices = new unsigned int[indices->Length];
			_indexCount = indices->Length;

			{
				pin_ptr<VData> v = &data[0];
				for (int i = 0; i < data->Length; ++i)
				{
					_data[i] = ((InternalVData*)v)[i];
				}
			}

			{
				pin_ptr<unsigned int> idx = &indices[0];
				for (int i = 0; i < indices->Length; ++i)
				{
					_indices[i] = ((unsigned int*)idx)[i];
				}
			}

			SMikkTSpaceInterface inter;

			inter.m_getNumFaces = GetNumFaces;

			return true;
		}
	};

	public ref class Native
	{
	public:
		static bool CalculateTangents(array<VData>^ data, array<unsigned int>^ indices)
		{
			Internal::CalculateTangents(data, indices);

			return true;
		}
	};
}

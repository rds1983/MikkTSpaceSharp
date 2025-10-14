#pragma once

#include <memory>

#include "mikktspace.c"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace MikkTSpaceNativeWrapper {
	public value struct Vec2
	{
	public:
		float X, Y;

		Vec2(float x, float y)
		{
			X = x;
			Y = y;
		}
	};

	public value struct Vec3
	{
	public:
		float X, Y, Z;

		Vec3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	};

	public value struct Vec4
	{
	public:
		float X, Y, Z, W;

		Vec4(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}
	};


	public value struct VData
	{
	public:
		Vec3 Position;
		Vec3 Normal;
		Vec2 UV;
		Vec3 Tangent;
		Vec3 BiTangent;
		Vec4 TangentBasic;
	};

	class Internal
	{
	public:
		static bool CalculateTangents();
		static bool CalculateTangentsBasic();
	};

	public ref class Native
	{
		static array<VData>^ _data;
		static array<unsigned int>^ _indices;

	public:
		static int GetNumFaces(const SMikkTSpaceContext* pContext)
		{
			return _indices->Length / 3;
		}

		static int GetNumVerticesPerFace(const SMikkTSpaceContext* pContext, int face)
		{
			return 3;
		}

		static unsigned int GetIndex(const int iFace, const int iVert)
		{
			return _indices[iFace * 3 + iVert];
		}

		static void GetPosition(const SMikkTSpaceContext* pContext, float fvPosOut[], const int iFace, const int iVert)
		{

			auto v = _data[GetIndex(iFace, iVert)];

			fvPosOut[0] = v.Position.X;
			fvPosOut[1] = v.Position.Y;
			fvPosOut[2] = v.Position.Z;
		}

		static void GetNormal(const SMikkTSpaceContext* pContext, float fvPosOut[], const int iFace, const int iVert)
		{
			auto v = _data[GetIndex(iFace, iVert)];

			fvPosOut[0] = v.Normal.X;
			fvPosOut[1] = v.Normal.Y;
			fvPosOut[2] = v.Normal.Z;
		}

		static void GetUV(const SMikkTSpaceContext* pContext, float fvPosOut[], const int iFace, const int iVert)
		{
			auto v = _data[GetIndex(iFace, iVert)];

			fvPosOut[0] = v.UV.X;
			fvPosOut[1] = v.UV.Y;
		}

		static void SetTSpace(const SMikkTSpaceContext* pContext, const float fvTangent[], const float fvBiTangent[],
			const float fMagS, const float fMagT, const tbool bIsOrientationPreserving, const int iFace, const int iVert)
		{
			auto idx = GetIndex(iFace, iVert);
			auto v = _data[idx];

			v.Tangent = Vec3(fvTangent[0], fvTangent[1], fvTangent[2]);
			v.BiTangent = Vec3(fvBiTangent[0], fvBiTangent[1], fvBiTangent[2]);

			_data[idx] = v;
		}

		static void SetTSpaceBasic(const SMikkTSpaceContext* pContext, const float fvTangent[], const float fSign, const int iFace, const int iVert)
		{
			auto idx = GetIndex(iFace, iVert);
			auto v = _data[idx];

			v.TangentBasic = Vec4(fvTangent[0], fvTangent[1], fvTangent[2], fSign);

			_data[idx] = v;
		}

		static bool CalculateTangents(array<VData>^ data, array<unsigned int>^ indices)
		{
			_data = data;
			_indices = indices;

			// We need to use non-managed Internal class for proper function pointers
			return Internal::CalculateTangents();
		}

		static bool CalculateTangentsBasic(array<VData>^ data, array<unsigned int>^ indices)
		{
			_data = data;
			_indices = indices;

			// We need to use non-managed Internal class for proper function pointers
			return Internal::CalculateTangentsBasic();
		}
	};
}

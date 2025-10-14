#include "pch.h"

#include "MikkTSpaceSharp.NativeWrapper.h"

namespace MikkTSpaceNativeWrapper
{
	static int GetNumFaces(const SMikkTSpaceContext* pContext)
	{
		return Native::GetNumFaces(pContext);
	}

	static int GetNumVerticesPerFace(const SMikkTSpaceContext* pContext, int face)
	{
		return Native::GetNumVerticesPerFace(pContext, face);
	}

	static void GetPosition(const SMikkTSpaceContext* pContext, float fvPosOut[], const int iFace, const int iVert)
	{
		return Native::GetPosition(pContext, fvPosOut, iFace, iVert);
	}

	static void GetNormal(const SMikkTSpaceContext* pContext, float fvPosOut[], const int iFace, const int iVert)
	{
		return Native::GetNormal(pContext, fvPosOut, iFace, iVert);
	}

	static void GetUV(const SMikkTSpaceContext* pContext, float fvPosOut[], const int iFace, const int iVert)
	{
		return Native::GetUV(pContext, fvPosOut, iFace, iVert);
	}

	static void SetTSpace(const SMikkTSpaceContext* pContext, const float fvTangent[], const float fvBiTangent[],
		const float fMagS, const float fMagT, const tbool bIsOrientationPreserving, const int iFace, const int iVert)
	{
		return Native::SetTSpace(pContext, fvTangent, fvBiTangent, fMagS, fMagT, bIsOrientationPreserving, iFace, iVert);
	}

	static void SetTSpaceBasic(const SMikkTSpaceContext* pContext, const float fvTangent[], const float fSign,
		const int iFace, const int iVert)
	{
		return Native::SetTSpaceBasic(pContext, fvTangent, fSign, iFace, iVert);
	}

	bool Internal::CalculateTangents()
	{
		SMikkTSpaceInterface inter;

		inter.m_getNumFaces = GetNumFaces;
		inter.m_getNumVerticesOfFace = GetNumVerticesPerFace;
		inter.m_getPosition = GetPosition;
		inter.m_getNormal = GetNormal;
		inter.m_getTexCoord = GetUV;
		inter.m_setTSpace = SetTSpace;
		inter.m_setTSpaceBasic = nullptr;

		SMikkTSpaceContext ctx;
		ctx.m_pInterface = &inter;

		auto result = genTangSpaceDefault(&ctx);

		return result;
	}

	bool Internal::CalculateTangentsBasic()
	{
		SMikkTSpaceInterface inter;

		inter.m_getNumFaces = GetNumFaces;
		inter.m_getNumVerticesOfFace = GetNumVerticesPerFace;
		inter.m_getPosition = GetPosition;
		inter.m_getNormal = GetNormal;
		inter.m_getTexCoord = GetUV;
		inter.m_setTSpace = nullptr;
		inter.m_setTSpaceBasic = SetTSpaceBasic;

		SMikkTSpaceContext ctx;
		ctx.m_pInterface = &inter;

		auto result = genTangSpaceDefault(&ctx);

		return result;
	}
}
/* VHACD.cpp : Defines the exported functions for the DLL.
*/
#include <stdio.h>
#include <stdint.h>
#include <functional>
#define ENABLE_VHACD_IMPLEMENTATION 1
#define VHACD_DISABLE_THREADING 1
#include "VHACD.h"

/**
* This class provides the parameters controlling the convex decomposition operation
*/
class ModifiedParameters
{
public:
	uint32_t            m_resolution;//{ 400000 };
	double				m_concavity;//{ 1 };				//concavity replaced with minimumVolumePercentErrorAllowed
	uint32_t			m_planeDownsampling;//{ 4 };		//Removed Feature
	uint32_t			m_convexhullDownsampling;//{ 4 };	//Removed Feature
	double				m_alpha;//{ .05 };					//Removed Feature
	double				m_beta;//{ .05 };					//Removed Feature
	uint32_t			m_pca;//{ 0 };						//Removed Feature
	uint32_t			m_mode;//{ 0 };					//Removed Feature
	uint32_t            m_maxNumVerticesPerCH;//{ 64 };
	double				m_minVolumePerCH;//{ 0.0001 };		//Removed Feature
	VHACD::IVHACD::IUserCallback* m_callback;//{ nullptr };
	VHACD::IVHACD::IUserLogger* m_logger;//{ nullptr };
	uint32_t			m_convexhullApproximation;//{ 1 };	//Removed Feature
	uint32_t			m_oclAcceleration;//{ 0 };			//Removed Feature
	uint32_t            m_maxConvexHulls;//{ 64 };
	bool				m_projectHullVertices;//{ true };  //projectHullVertices replaced with shrinkWrap
};

VHACD::IVHACD::Parameters* ConvertParams(const ModifiedParameters& oldParams) {

	VHACD::IVHACD::Parameters p = VHACD::IVHACD::Parameters();
	
	//Default Values
	p.m_callback = nullptr;
	p.m_logger = nullptr;
	p.m_taskRunner = nullptr;
	p.m_maxConvexHulls = 1024;
	p.m_resolution = 100000;
	p.m_minimumVolumePercentErrorAllowed = 1;
	p.m_maxRecursionDepth = 7;
	p.m_shrinkWrap = true;
	p.m_fillMode = VHACD::FillMode::FLOOD_FILL;
	p.m_maxNumVerticesPerCH = 64;
	p.m_asyncACD = false;
	p.m_minEdgeLength = 2;
	p.m_findBestPlane = false;

	//Modified Values
	p.m_resolution = 50000;
	p.m_maxNumVerticesPerCH = 32;
	p.m_maxConvexHulls = 8;


	//WIP - Converting C# Variables to C++

	/*p.m_resolution = oldParams.m_resolution;
	p.m_minimumVolumePercentErrorAllowed = oldParams.m_concavity;
	p.m_maxNumVerticesPerCH = oldParams.m_maxNumVerticesPerCH;
	//p.m_callback = oldParams.m_callback;
	//p.m_logger = oldParams.m_logger;
	p.m_maxConvexHulls = oldParams.m_maxConvexHulls;
	p.m_shrinkWrap = oldParams.m_projectHullVertices;*/
   

	VHACD::IVHACD::Parameters* param = new VHACD::IVHACD::Parameters(p);

	return param;
}

extern "C" void* CreateVHACD()
{
	return VHACD::CreateVHACD();
}

extern "C" void DestroyVHACD(void* pVHACD)
{
	auto vhacd = (VHACD::IVHACD*)pVHACD;
	vhacd->Clean();
	vhacd->Release();
}

extern "C" bool ComputeFloat(
	void* pVHACD,
	const float* const points,
	const uint32_t countPoints,
	const uint32_t* const triangles,
	const uint32_t countTriangles,
	const void* params)
{
	auto vhacd = (VHACD::IVHACD*)pVHACD;
	auto modifiedParams = *(ModifiedParameters const*)params;
	return vhacd->Compute(points, countPoints, triangles, countTriangles, *(VHACD::IVHACD::Parameters const*)ConvertParams(modifiedParams));
}

extern "C" bool ComputeDouble(
	void* pVHACD,
	const double* const points,
	const uint32_t countPoints,
	const uint32_t* const triangles,
	const uint32_t countTriangles,
	const void* params)
{
	auto vhacd = (VHACD::IVHACD*)pVHACD;
	auto modifiedParams = *(ModifiedParameters const*)params;
	return vhacd->Compute(points, countPoints, triangles, countTriangles, *(VHACD::IVHACD::Parameters const*)ConvertParams(modifiedParams));
}

extern "C" uint32_t GetNConvexHulls(
	void* pVHACD
)
{
	auto vhacd = (VHACD::IVHACD*)pVHACD;
	return vhacd->GetNConvexHulls();
}

extern "C" void GetConvexHull(
	void* pVHACD,
	const uint32_t index,
	void* ch)
{
	auto vhacd = (VHACD::IVHACD*)pVHACD;
	vhacd->GetConvexHull(index, *(VHACD::IVHACD::ConvexHull*)ch);
	return;
}
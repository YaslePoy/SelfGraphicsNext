#pragma once
#include "Tools.h"
class Polygon
{
public:
	ColisionResult Colide(Ray3 ray);
	Point3 Normal, p1, p2, p3;
	double DRatio;
};

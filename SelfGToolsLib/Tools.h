#pragma once
#include "pch.h"

class Point3C {
public:
	Point3C() {
		X = 0;
		Y = 0;
		Z = 0;
	}
	Point3C(double x, double y, double z) {
		X = x;
		Y = y;
		Z = z;
	}
	Point3C(double x, double y) {
		X = x;
		Y = y;
		Z = 0;
	}
	double X, Y, Z;
	Point3C Mul(double k);
	Point3C Plus(Point3C k);
};
class ColisionResultC {
public:
	bool isColided;
	Point3C colPoint;
};
class Ray3C {
public:
	Point3C Direction;
	Point3C Position;
};

class Tools
{
public:
	static bool PoinInTringle(Point3C p1, Point3C p2, Point3C p3, Point3C p0);
};
class Polygon3C
{
public:
    Point3C Normal;
    Point3C p1;
    Point3C p2;
    Point3C p3;
    double DRatio;
    ColisionResultC Colide(Ray3C ray) {
        ColisionResultC res;
        Point3C mpl = ray.Direction;
        Point3C xyz = ray.Position;
        Point3C abc = Normal;
        double upper = DRatio + abc.X * xyz.X + abc.Y * xyz.Y + abc.Z * xyz.Z;
        double lower = abc.X * mpl.X + abc.Y * mpl.Y + abc.Z * mpl.Z;
        if (lower == 0 && upper == 0) {
            res.isColided = false;
            return res;
        }
        if (upper > 0 && lower == 0) {
            res.isColided = false;
            return res;
        }
        double tRatio = -(upper / lower);
        if (tRatio < 0) {
            res.isColided = false;
            return res;
        }

        res.colPoint = xyz.Plus(mpl.Mul(tRatio));

        if (Normal.X != 0)
            res.isColided = Tools::PoinInTringle(Point3C(p1.Y, p1.Z), Point3C(p2.Y, p2.Z), Point3C(p3.Y, p3.Z), Point3C(res.colPoint.Y, res.colPoint.Z));
        else
            if (Normal.Y != 0)
                res.isColided = Tools::PoinInTringle(Point3C(p1.X, p1.Z), Point3C(p2.X, p2.Z), Point3C(p3.X, p3.Z), Point3C(res.colPoint.X, res.colPoint.Z));
            else
                res.isColided = Tools::PoinInTringle(Point3C(p1.X, p1.Y), Point3C(p2.X, p2.Y), Point3C(p3.X, p3.Y), Point3C(res.colPoint.X, res.colPoint.Y));
        return res;
    };
};

extern "C" __declspec(dllexport) ColisionResultC CalcColision(Polygon3C  p, Ray3C  r) { return p.Colide(r); }


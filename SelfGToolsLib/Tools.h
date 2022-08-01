#pragma once
#include "pch.h"

class Point3 {
public:
	Point3() {
		X = 0;
		Y = 0;
		Z = 0;
	}
	Point3(double x, double y, double z) {
		X = x;
		Y = y;
		Z = z;
	}
	Point3(double x, double y) {
		X = x;
		Y = y;
		Z = 0;
	}
	double X, Y, Z;
	Point3 Mul(double k);
	Point3 Plus(Point3 k);
};
class ColisionResult {
public:
	bool isColided;
	Point3 colPoint;
};
class Ray3 {
public:
	Point3 Direction;
	Point3 Position;
};

class Tools
{
public:
	static bool PoinInTringle(Point3 p1, Point3 p2, Point3 p3, Point3 p0);
};
class Polygon
{
public:
    Point3 Normal;
    Point3 p1;
    Point3 p2;
    Point3 p3;
    double DRatio;
    ColisionResult Colide(Ray3 ray) {
        ColisionResult res;
        Point3 mpl = ray.Direction;
        Point3 xyz = ray.Position;
        Point3 abc = Normal;
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
            res.isColided = Tools::PoinInTringle(Point3(p1.Y, p1.Z), Point3(p2.Y, p2.Z), Point3(p3.Y, p3.Z), Point3(res.colPoint.Y, res.colPoint.Z));
        else
            if (Normal.Y != 0)
                res.isColided = Tools::PoinInTringle(Point3(p1.X, p1.Z), Point3(p2.X, p2.Z), Point3(p3.X, p3.Z), Point3(res.colPoint.X, res.colPoint.Z));
            else
                res.isColided = Tools::PoinInTringle(Point3(p1.X, p1.Y), Point3(p2.X, p2.Y), Point3(p3.X, p3.Y), Point3(res.colPoint.X, res.colPoint.Y));
        return res;
    };;

};


#pragma once
class ColisionResult {
public:
	bool isColided;
	Point3 colPoint;
};
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


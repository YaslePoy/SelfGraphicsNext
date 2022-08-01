#include "pch.h"
#include "Tools.h"

Point3 Point3::Mul(double k)
{
    Point3 out;
    out.X = this->X * k;
    out.Y = this->Y * k;
    out.Z = this->Z * k;
    return out;
};

Point3 Point3::Plus(Point3 k)
{
    Point3 out;
    out.X = this->X + k.X;
    out.Y = this->Y + k.Y;
    out.Z = this->Z + k.Z;
    return out;
};

bool Tools::PoinInTringle(Point3 p1, Point3 p2, Point3 p3, Point3 p0)
{
    int a = (p1.X - p0.X) * (p2.Y - p1.Y) * (p1.Y - p0.Y),
        b = (p2.X - p0.X) * (p3.Y - p2.Y) * (p2.Y - p0.Y),
        c = (p3.X - p0.X) * (p1.Y - p2.Y) * (p3.Y - p0.Y);

    return (a >= 0 && b >= 0 && c >= 0) ||
        (a <= 0 && b <= 0 && c <= 0) ? 1 : 0;
};

#include "pch.h"
#include "Tools.h"

Point3C Point3C::Mul(double k)
{
    Point3C out;
    out.X = this->X * k;
    out.Y = this->Y * k;
    out.Z = this->Z * k;
    return out;
};

Point3C Point3C::Plus(Point3C k)
{
    Point3C out;
    out.X = this->X + k.X;
    out.Y = this->Y + k.Y;
    out.Z = this->Z + k.Z;
    return out;
};

bool Tools::PoinInTringle(Point3C p1, Point3C p2, Point3C p3, Point3C p0)
{
    int a = (p1.X - p0.X) * (p2.Y - p1.Y) * (p1.Y - p0.Y),
        b = (p2.X - p0.X) * (p3.Y - p2.Y) * (p2.Y - p0.Y),
        c = (p3.X - p0.X) * (p1.Y - p2.Y) * (p3.Y - p0.Y);

    return (a >= 0 && b >= 0 && c >= 0) ||
        (a <= 0 && b <= 0 && c <= 0) ? 1 : 0;
};

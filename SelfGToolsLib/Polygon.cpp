#include "pch.h"
#include "Polygon.h"
#include "Tools.h"
#include "Tools.cpp"

ColisionResult Polygon::Colide(Ray3 ray)
{
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
    if (upper > 0 && lower == 0){
        res.isColided = false;
        return res;
    }
    double tRatio = -(upper / lower);
    if (tRatio < 0){
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
}



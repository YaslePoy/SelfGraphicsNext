namespace SelfGraphicsNext.BaseGraphics
{
    public struct Direction
    {
        public double AngleGrads = 0;
        public Direction(double degrees)
        {
            degrees = degrees % 360;
            if (degrees < 0)
                degrees = 360 + degrees;
            degrees = degrees % 360;
            AngleGrads = degrees;
        }

        public static bool operator ==(Direction dir1, Direction dir2) => dir1.AngleGrads == dir2.AngleGrads;

        public static bool operator !=(Direction dir1, Direction dir2) => dir1.AngleGrads != dir2.AngleGrads;

        public static bool operator <(Direction dir1, Direction dir2) => dir1.AngleGrads < dir2.AngleGrads;

        public static bool operator >(Direction dir1, Direction dir2) => dir1.AngleGrads > dir2.AngleGrads;

        public static bool operator <=(Direction dir1, Direction dir2) => dir1.AngleGrads <= dir2.AngleGrads;

        public static bool operator >=(Direction dir1, Direction dir2) => dir1.AngleGrads >= dir2.AngleGrads;

        public static bool operator ==(Direction dir1, double dir2) => dir1.AngleGrads == dir2;

        public static bool operator !=(Direction dir1, double dir2) => dir1.AngleGrads != dir2;

        public static bool operator <(Direction dir1, double dir2) => dir1.AngleGrads < dir2;

        public static bool operator >(Direction dir1, double dir2) => dir1.AngleGrads > dir2;

        public static bool operator <=(Direction dir1, double dir2) => dir1.AngleGrads <= dir2;

        public static bool operator >=(Direction dir1, double dir2) => dir1.AngleGrads >= dir2;

        public static Direction operator *(Direction dir, double k)
        {
            return new Direction(dir.AngleGrads * k);
        }

        public static Direction operator /(Direction dir, double k)
        {
            return new Direction(dir.AngleGrads / k);
        }

        public static Direction operator %(Direction dir, double k)
        {
            return new Direction(dir.AngleGrads * k);
        }
        public static Direction operator +(Direction dir, double degrees)
        {
            dir.AddВegrees(degrees);
            return dir;
        }

        public static Direction operator -(Direction dir, double degrees)
        {
            dir.AddВegrees(-degrees);
            return dir;
        }

        public static Direction operator +(Direction dir, Direction degrees)
        {
            dir.AddВegrees(degrees.AngleGrads);
            return dir;
        }

        public static Direction operator -(Direction dir, Direction degrees)
        {
            dir.AddВegrees(-degrees.AngleGrads);
            return dir;
        }

        public void AddВegrees(double degrees)
        {
            AngleGrads += degrees;
            AngleGrads %= 360;
            if (AngleGrads < 0)
                AngleGrads = 360 + AngleGrads;
        }

        public override string ToString()
        {
            return $"{AngleGrads.Round(4)}°";
        }
        public double Sin => AngleGrads.Sin();
        public double Cos => AngleGrads.Cos();
        public double Tan => AngleGrads.Tan();

        public double Radians => AngleGrads.ToRadians();

        public Direction Rounded => new Direction(AngleGrads.Round(1));

        public int Quater()
        {
            if (AngleGrads is (> 0 and <= 90))
                return 1;
            if (AngleGrads is (> 90 and <= 180))
                return 2;
            if (AngleGrads is (> 180 and <= 270))
                return 3;
            if (AngleGrads is (> 270 and <= 360))
                return 4;
            return 0;
        }

    }
}

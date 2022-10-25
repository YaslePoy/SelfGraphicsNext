using System.Text.Json.Serialization;

namespace SelfGraphicsNext.BaseGraphics
{
    public struct Direction
    {
        public float AngleGradsF = 0;
        public double AngleGrads = 0;
        public Direction(double degrees)
        {
            degrees = degrees % 360;
            if (degrees < 0)
                degrees = 360 + degrees;
            degrees = degrees % 360;
            AngleGrads = degrees;
            AngleGradsF = (float)degrees;
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
            dir.AddDegrees(degrees);
            return dir;
        }

        public static Direction operator -(Direction dir, double degrees)
        {
            dir.AddDegrees(-degrees);
            return dir;
        }

        public static Direction operator +(Direction dir, Direction degrees)
        {
            dir.AddDegrees(degrees.AngleGrads);
            return dir;
        }

        public static Direction operator -(Direction dir, Direction degrees)
        {
            dir.AddDegrees(-degrees.AngleGrads);
            return dir;
        }

        public static implicit operator Direction(double v)
        {
            return new Direction(v);
        }

        public void AddDegrees(double degrees)
        {
            AngleGrads += degrees;
            AngleGrads %= 360;
            if (AngleGrads < 0)
                AngleGrads = 360 + AngleGrads;
            AngleGradsF = (float)degrees;
        }
        public override string ToString()
        {
            return $"{AngleGrads.Round(4)}°";
        }
        [JsonIgnore]
        public double Sin => Math.Sin(AngleGrads * Utils.ToRad);
        [JsonIgnore]
        public double Cos => Math.Cos(AngleGrads * Utils.ToRad);
        [JsonIgnore]
        public double Tan => Math.Tan(AngleGrads * Utils.ToRad);
        [JsonIgnore]
        public double Radians => AngleGrads.ToRadians();
        [JsonIgnore]
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
        public List<Direction> GetDirectionsByCount(int count, double fow)
        {
            var halfFow = fow / 2;
            var len = halfFow.Sin();
            len *= 2;
            var step = len / count;
            var stepped = Utils.Range(-len / 2, len / 2, step);
            List<Direction> result = new List<Direction>();
            foreach (var current in stepped)
            {
                result.Add(this + new Direction(Math.Asin(current).ToDegrees()));
            }
            return result;
        }
    }
}

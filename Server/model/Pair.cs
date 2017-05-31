using System;
using System.Windows.Media.Media3D;

namespace HexBall
{
    [Serializable]
    public class Pair
    {
        public double First { get; set; }
        public double Second { get; set; }

        public Pair()
        {

        }
        public Pair(double a, double b)
        {
            First = a;
            Second = b;
        }
        public Pair(Pair a)
        {
            First = a.First;
            Second = a.Second;
        }

        public static Pair operator +(Pair a, Pair b)
        {
            return new Pair(a.First + b.First, a.Second + b.Second);
        }

        public static Pair operator -(Pair a, Pair b)
        {
            return new Pair(a.First - b.First, a.Second - b.Second);
        }

        public static Pair operator *(Pair a, double b)
        {
            return new Pair(a.First * b, a.Second * b);
        }

        public static bool operator >(Pair a, Pair b)
        {
            return a.First > b.First && a.Second > b.Second;
        }
        public static bool operator <(Pair a, Pair b)
        {
            return a.First < b.First && a.Second < b.Second;
        }

        public void Set(double a, double b)
        {
            First = a;
            Second = b;
        }

        public static double Distance(Pair a, Pair b)
        {
            return Math.Sqrt(Math.Pow(b.First - a.First, 2) + Math.Pow(b.Second - a.Second, 2));
        }

        public bool IsBetween(double size, Tuple<Pair, Pair> pairs)
        {
            var points=new Pair[4];
            points[0]=new Pair(this);
            points[0].Second -= size;

            points[1] = new Pair(this);
            points[1].First += size;

            points[2] = new Pair(this);
            points[2].Second += size;

            points[3] = new Pair(this);
            points[3].First -= size;

            foreach (var point in points)
            {
                if (point.IsBetween(pairs))
                    return true;
            }
            return false;
        }
        public bool IsBetween(Tuple<Pair, Pair> pairs)
        {
            var begin = pairs.Item1;
            var end = pairs.Item2;
            return this > begin && this < end;
        }

        public static Pair GetCenterOfPairs(Tuple<Pair, Pair> pairs)
        {
            var center = new Pair()
            {
                First = (pairs.Item1.First + pairs.Item1.First) / 2.0,
                Second = (pairs.Item1.Second + pairs.Item1.Second) / 2.0
            };
            return center;
        }

        public Pair GetCenter(int size)
        {
            var center=new Pair()
            {
                First = this.First+size/2.0,
                Second = this.Second+size/2.0
            };
            return center;
        }
    }
}

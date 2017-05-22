using System;

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

        public void Set(double a, double b)
        {
            First = a;
            Second = b;
        }

        public static double Distance(Pair a, Pair b)
        {
            return Math.Sqrt(Math.Pow(b.First - a.First, 2) + Math.Pow(b.Second - a.Second, 2));
        }
    }
}

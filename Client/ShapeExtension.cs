using System;
using System.Windows.Shapes;
using HexBall;

namespace Client
{
    public static class ShapeExtension
    {
        public static void Hide(this Shape shape)
        {
            shape.Width = 0;
            shape.Height = 0;
        }

        public static bool IsVisible(this Shape shape)
        {
            return shape.Width > 0;
        }

        public static void SetSize(this Shape shape, int size)
        {
            shape.Width = size;
            shape.Height = size;
        }

    }
}
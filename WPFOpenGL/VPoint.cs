using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using SharpGL;


namespace WPFOpenGL;

public class VPoint
{
    public float X { get; set; } = 0.0f;
    public float Y { get; set; } = 0.0f;

    public VPoint() {}
    public VPoint(float x = 0.0f, float y = 0.0f)
    {
        X = x; Y = y;
    }
    public VPoint(VPoint other): this(other.X, other.Y) {}
    public static VPoint operator +(VPoint a, VPoint b)
    {
        return new VPoint(a.X + b.X, a.Y + b.Y);
    }
    public static VPoint operator -(VPoint a, VPoint b)
    {
        return new VPoint(a.X - b.X, a.Y - b.Y);
    }
    public static VPoint operator *(VPoint a, float num)
    {
        return new VPoint(a.X * num, a.Y * num);
    }
    public static VPoint operator *(float num, VPoint a)
    {
        return new VPoint(a.X * num, a.Y * num);
    }
    public static explicit operator VPoint(Point point)
    {
        return new VPoint((float)point.X, (float)point.Y);
    }
    public static explicit operator Point(VPoint point)
    {
        return new Point(point.X, point.Y);
    }

}


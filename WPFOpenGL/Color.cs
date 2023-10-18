using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;

namespace WPFOpenGL;

public class Color
{
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }

    public Color(byte r, byte g, byte b)
    {
        R = r; G = g; B = b;
    }

    public Color(float r, float g, float b)
    {
        R = (byte)r; G = (byte)g; B = (byte)b;
    }

    //public Color(Color c): this(c.R, c.G, c.B) {}

    public static void SetColor(OpenGL gl, Color color)
    {
        gl.Color((float)color.R, (float)color.G, (float)color.B);
    }

    public override string ToString()
    {
        return $"({R},{G},{B})";
    }
    public override bool Equals(object obj)
    {
        if (obj is Color color)
        {
            return R == color.R && G == color.G && B == color.B;
        }
        else return false;
    }

    public static Color RandomColor()
    {
        var seed = 10;
        Random random = new Random(seed);
        int min = 1;
        int max = 255;
        return new Color(
            (byte)random.Next(min, max),
            (byte)random.Next(min, max),
            (byte)random.Next(min, max)
        );
    }
}
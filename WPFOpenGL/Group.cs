using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFOpenGL;

public class Group
{
    public List<VPoint> Points { get; set; } = new();
    public Color Color { get; set; } = new(0, 0, 0);
    public int Priority { get; set; } = 0;

    public bool IsPointInside(VPoint point)
    {
        int windingNumber = 0;
        var vertices = Points;
        int numVertices = vertices.Count;

        for (int i = 0; i < numVertices; i++)
        {
            VPoint currentVertex = vertices[i];
            VPoint nextVertex = vertices[(i + 1) % numVertices];

            // Check if the point coincides with a vertex
            if (point.X == currentVertex.X && point.Y == currentVertex.Y)
            {
                return true; // Point is on a vertex
            }

            // Check if the point is on the edge defined by currentVertex and nextVertex
            if (point.Y >= Math.Min(currentVertex.Y, nextVertex.Y) &&
                point.Y <= Math.Max(currentVertex.Y, nextVertex.Y))
            {
                if (point.X <= Math.Max(currentVertex.X, nextVertex.X) &&
                    currentVertex.Y != nextVertex.Y)
                {
                    double xIntersection = (point.Y - currentVertex.Y) *
                        (nextVertex.X - currentVertex.X) /
                        (nextVertex.Y - currentVertex.Y) + currentVertex.X;

                    if (currentVertex.X == nextVertex.X || point.X <= xIntersection)
                    {
                        windingNumber++;
                    }
                }
            }
        }

        return windingNumber % 2 == 1;
    }
}
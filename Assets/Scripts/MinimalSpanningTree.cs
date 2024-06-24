using DelaunatorSharp;
using System.Collections.Generic;
using UnityEngine;

public static class MinimalSpanningTree
{
    public static int LengthComparison(IEdge x, IEdge y)
    {
        float lx = Vector2.Distance(new Vector2((float)x.P.X, (float)x.P.Y), new Vector2((float)x.Q.X, (float)x.Q.Y));
        float ly = Vector2.Distance(new Vector2((float)y.P.X, (float)y.P.Y), new Vector2((float)y.Q.X, (float)y.Q.Y));

        if (Mathf.Approximately(lx, ly))
            return 0;
        else if (lx > ly)
            return 1;
        else
            return -1;
    }

    public static List<IEdge> Make(IEnumerable<IEdge> graph)
    {
        List<IEdge> ans = new();
        List<IEdge> edges = new(graph);
        edges.Sort(LengthComparison);

        HashSet<IPoint> points = new();
        foreach (var edge in edges)
        {
            points.Add(edge.P);
            points.Add(edge.Q);
        }

        Dictionary<IPoint, IPoint> parents = new();
        foreach (var point in points)
            parents[point] = point;

        IPoint UnionFind(IPoint x)
        {
            if (parents[x] != x)
                parents[x] = UnionFind(parents[x]);
            return parents[x];
        }

        foreach (var edge in edges)
        {
            var x = UnionFind(edge.P);
            var y = UnionFind(edge.Q);
            if (x != y)
            {
                ans.Add(edge);
                parents[x] = y;
            }
        }

        return ans;
    }
}

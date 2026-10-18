using System;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

/// <summary>
/// Stores vertex pairs that allows for rendering.
/// Also converts the vertex pair list into a hash to allow for easy pattern matching.
/// </summary>
[System.Serializable]
public class Symbol
{
    [System.Serializable]
    public struct VertexPair : IEquatable<VertexPair>, IEqualityComparer<VertexPair>
    {
        public Vector2 A, B;
        public float angle;
        public VertexPair(Vector2 A, Vector2 B)
        {
            this.A = A;
            this.B = B;
            angle = 0;
        }

        public bool Equals(VertexPair x, VertexPair y)
        {
            if ((x.A == y.A && x.B == y.B) || (x.B == y.A && x.A == y.B)) return true;
            return false;
        }

        public bool Equals(VertexPair other)
        {
            return Equals(this, other);
        }

        public int GetHashCode(VertexPair obj)
        {
            return A.GetHashCode() + B.GetHashCode();
        }

        public override string ToString()
        {
            return $"{A} | {B}";
        }
    };
    public List<VertexPair> vertexPairs = new();
    public List<ShapeRenderer> extraShapes = new();
    public List<VertexPair> comparisonPairs = new();
    public Vector2 offsetForced;
    public Bounds bounds = new Bounds();
    public bool updateComparer;
    public int sourceRuneUID;

    public void AddVertexPair(Vector2 A, Vector2 B)
    {
        vertexPairs.Add(new VertexPair(A, B));
    }

    public void UpdateBounds(Vector3 position, Vector3 deltaScale)
    {
        bounds.center = position;
        Vector3 extents = bounds.extents;
        extents.x = Mathf.Abs(extents.x * deltaScale.x);
        extents.y = Mathf.Abs(extents.y * deltaScale.y);
        extents.z = Mathf.Abs(extents.z * deltaScale.z);
        bounds.extents = extents;
    }

    public void ScaleSymbol(Vector3 scale)
    {
        for (int i = 0; i < vertexPairs.Count; i++)
        {
            VertexPair pair = vertexPairs[i];
            pair.A.x *= scale.x;
            pair.A.y *= scale.y;
            pair.B.x *= scale.x;
            pair.B.y *= scale.y;
            vertexPairs[i] = pair;
        }
        UpdateBounds(Vector3.zero, scale);
    }

    public Vector2 LocalizeAroundBoundsCenter()
    {
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);
        foreach (VertexPair p in vertexPairs)
        {
            VertexPair pair = p;
            // Get x min
            if (pair.A.x < min.x) min.x = pair.A.x;
            if (pair.B.x < min.x) min.x = pair.B.x;

            // Get y min 
            if (pair.A.y < min.y) min.y = pair.A.y;
            if (pair.B.y < min.y) min.y = pair.B.y;

            // Get x max
            if (pair.A.x > max.x) max.x = pair.A.x;
            if (pair.B.x > max.x) max.x = pair.B.x;

            // Get y max
            if (pair.A.y > max.y) max.y = pair.A.y;
            if (pair.B.y > max.y) max.y = pair.B.y;
        }

        Vector2 center = (max - min) / 2f;
        Vector2 offset = min + center;
        for (int i = 0; i < vertexPairs.Count; i++)
        {
            VertexPair pair = vertexPairs[i];
            pair.A = pair.A - offset + offsetForced;
            pair.B = pair.B - offset + offsetForced;
            vertexPairs[i] = pair;
        }
        bounds = new Bounds(new Vector3(offsetForced.x, 0, offsetForced.y), new Vector3(Mathf.Abs(center.x * 2), 0.02f, Mathf.Abs(center.y * 2)));
        return offset;
    }

    private const float minRangeLeewayPercent = 0.06f;
    private float minRangeLeeway => (new Vector2(bounds.max.x, bounds.max.z) - new Vector2(bounds.min.x, bounds.min.z)).magnitude * minRangeLeewayPercent;
    public void UpdateComparer()
    {
        updateComparer = false;
        LocalizeAroundBoundsCenter();
        List<Vector2> points = new();
        List<VertexPair> vertexPairsTemp = new();
        comparisonPairs.Clear();

        // 1. Fetch all points and "merge" any that are within the minimum range
        foreach(VertexPair pair in vertexPairs)
        {
            VertexPair pairTemp = pair;
            foreach (Vector3 point in points)
            {
                if (Vector3.Distance(pairTemp.A, point) <= minRangeLeeway)
                {
                    pairTemp.A = point;
                }
                if (Vector3.Distance(pairTemp.B, point) <= minRangeLeeway)
                {
                    pairTemp.B = point;
                }
            }
            vertexPairsTemp.Add(pairTemp);
            points.Add(pairTemp.A);
            points.Add(pairTemp.B);
        }

        // 2. Create all point pairs we will be observing, ignoring duplicate pairs and line pairs
        foreach (VertexPair pair in vertexPairsTemp)
        {
            foreach (Vector2 point in points)
            {
                if (point == pair.A) continue;
                VertexPair comparisonPair = new VertexPair(pair.A, point);
                if (comparisonPairs.Contains(comparisonPair)) continue;
                comparisonPairs.Add(comparisonPair);
            }

            foreach (Vector2 point in points)
            {
                if (point == pair.B) continue;
                VertexPair comparisonPair = new VertexPair(pair.B, point);
                if (comparisonPairs.Contains(comparisonPair)) continue;
                comparisonPairs.Add(comparisonPair);
            }
        }

        // 3. Calculate the sine from the signed angle
        for (int i = 0; i < comparisonPairs.Count; i++)
        {
            VertexPair pair = comparisonPairs[i];
            pair.angle = Mathf.Abs(Mathf.Sin(Vector3.SignedAngle(Vector3.up, pair.A - pair.B, Vector3.forward) * Mathf.Deg2Rad));
            comparisonPairs[i] = pair;
        }

        // 4. Sort the pair angle list
        comparisonPairs.Sort((x, y) =>
        {
            if (x.angle == y.angle) return 0;
            if (x.angle < y.angle) return -1;
            else return 1;
        });

        sourceRuneUID = GetHashCode();
    }
}

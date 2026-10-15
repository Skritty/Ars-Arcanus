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
    public struct VertexPair
    {
        public Vector2 A, B;
        public VertexPair(Vector2 A, Vector2 B)
        {
            this.A = A;
            this.B = B;
        }
    };
    public List<VertexPair> vertexPairs = new();
    public List<ShapeRenderer> extraShapes = new();
    public int UID;
    public float rotation;
    public void AddVertexPair(Vector2 A, Vector2 B)
    {
        vertexPairs.Add(new VertexPair(A, B));
        UpdateUID();
    }
    public Vector2 LocalizeAroundBoundsCenter()
    {
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);
        foreach (VertexPair pair in vertexPairs)
        {
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
            pair.A -= offset;
            pair.B -= offset;
            vertexPairs[i] = pair;
        }
        return offset;
    }
    public void UpdateUID()
    {
        // TODO: Figure out how to convert this into a number
    }

    /// <summary>
    /// 
    /// </summary>
    private void CalculatePointPair(Vector2 A, Vector2 B)
    {

    }
}

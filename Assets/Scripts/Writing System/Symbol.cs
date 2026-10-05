using System.Collections.Generic;
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
    private List<VertexPair> vertexPairs;
    public int UID;
    public float rotation;
    public void AddVertexPair(Vector2 A, Vector2 B)
    {
        vertexPairs.Add(new VertexPair(A, B));
        UpdateUID();
    }
    public void UpdateUID()
    {
        // TODO: Figure out how to convert this into a number
    }
}

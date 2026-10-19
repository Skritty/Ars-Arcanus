using Shapes;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
public class SymbolRenderer : ImmediateModeShapeDrawer
{
    public Color color;
    public float thickness;
    public Symbol symbol;

    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            Draw.Color = color;
            Draw.Matrix = Matrix4x4.TRS(transform.position + transform.up * transform.lossyScale.normalized.y * 0.0001f, transform.rotation * Quaternion.AngleAxis(90, Vector3.right), new Vector3(transform.lossyScale.x, transform.lossyScale.z, transform.lossyScale.y));

            foreach (ShapeRenderer shape in symbol.extraShapes)
            {
                shape.Color = color;
            }

            foreach (Symbol.VertexPair line in symbol.vertexPairs)
            {
                Draw.Line(line.A, line.B, thickness);
            }
        }
    }
}

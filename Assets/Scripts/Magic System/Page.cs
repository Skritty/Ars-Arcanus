using System.Collections.Generic;
using Shapes;
using UnityEngine;

/// <summary>
/// Stores runes and renders Symbols within its bounds.
/// It handles the moving of mana.
/// </summary>
[ExecuteAlways]
public class Page : ImmediateModeShapeDrawer
{
    public bool accessible = true;
    [SerializeReference]
    public List<Rune> allRunes;
    /// <summary>
    /// List of outermost runes. Nested runes are contained by these.
    /// </summary>
    public List<Rune> runes;

    public override void DrawShapes(Camera cam)
    {

        using (Draw.Command(cam))
        {

            // set up static parameters. these are used for all following Draw.Line calls
            Draw.LineGeometry = LineGeometry.Volumetric3D;
            Draw.ThicknessSpace = ThicknessSpace.Pixels;
            Draw.Thickness = 4; // 4px wide

            // set static parameter to draw in the local space of this object
            Draw.Matrix = transform.localToWorldMatrix;

            // draw lines
            Draw.Line(Vector3.zero, Vector3.right, Color.red);
            Draw.Line(Vector3.zero, Vector3.up, Color.green);
            Draw.Line(Vector3.zero, Vector3.forward, Color.blue);
        }

    }
    /*public override void DrawShapes(Camera cam)
    {

        using (Draw.Command(cam))
        {
            using (var p = new PolylinePath())
            {
                p.AddPoint(-1, -1);
                p.AddPoint(-1, 1);
                p.AddPoint(1, 1);
                p.AddPoint(1, -1);
                Draw.Polyline(p, closed: true, thickness: 0.1f, Color.red); // Drawing happens here
            } // Disposing of mesh data happens here
        }

        *//*using (Draw.Command(cam))
        {
            Draw.LineGeometry = LineGeometry.Flat2D;
            Draw.ThicknessSpace = ThicknessSpace.Meters;
            Draw.Thickness = 0.1f;
            Debug.Log("Test");
            using (var p = new PolylinePath())
            {
                p.AddPoint(-1, -1);
                p.AddPoint(-1, 1);
                p.AddPoint(1, 1);
                p.AddPoint(1, -1);
                Draw.Polyline(p, closed: true, thickness: 0.1f, Color.red); // Drawing happens here
            }
            foreach (Rune rune in allRunes)
            {
                using (var p = new PolylinePath())
                {
                    p.AddPoints(rune.associatedSymbol.GetPoints());
                    Draw.Polyline(p, closed: true, Color.black); // Drawing happens here
                } // Disposing of mesh data happens here
            }
        }*//*
    }*/
    public void AddRune(Rune newRune)
    {
        // Steps to creating and adding a new [drawn] rune:
        // 1. Symbol drawer tool sends updates to the Magic Manager until a valid UID is found
        // 2. When this happens, a copy of the matching rune is linked to the drawn symbol and is added to the Page it was drawn on
        // 3. The page tries adding the new rune to any root diffuse runes it already has
        // 4. Those diffuse runes check to see if any of their children contain it, etc.
        //    If one is valid, it adds it as the focus rune there and no where else.
        // 5. If it isn't a nested rune, it is added to the root runes list of the page.
        // 6. A null mana effect update is given to the new rune, which may produce an effect.

        foreach(Rune rune in runes)
        {
            if(rune is Rune_Container)
            {
                (rune as Rune_Container).TryAddRune(newRune);
            }
        }
    }
}

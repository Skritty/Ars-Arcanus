using System.Collections.Generic;
using Shapes;
using UnityEngine;
using UnityEngine.InputSystem;

public class WritingManager : ImmediateModeShapeDrawer
{
    public InputAction drawAction, finishAction;
    public Rune defaultVariableRune;
    [SerializeField]
    public Dictionary<int, Rune> runes = new();

    private Vector3 start, end;
    private Page page;
    private Symbol symbol;

    private new void OnEnable()
    {
        base.OnEnable();
        finishAction.performed += FinishSymbol;
        drawAction.Enable();
        finishAction.Enable();
    }
    private new void OnDisable()
    {
        base.OnDisable();
        finishAction.performed -= FinishSymbol;
        drawAction.Disable();
        finishAction.Disable();
    }

    private void Update()
    {
        AddPoint();
    }

    public override void DrawShapes(Camera cam)
    {
        if (page == null) return;
        using (Draw.Command(cam))
        {
            Draw.Matrix = Matrix4x4.TRS(
                page.transform.position + page.transform.up * 0.001f * page.transform.localScale.y,
                page.transform.rotation * Quaternion.AngleAxis(90, Vector3.right), 
                new Vector3(page.transform.lossyScale.x, page.transform.lossyScale.z, page.transform.lossyScale.y));
            Draw.Line(new Vector3(start.x, start.z, 0), new Vector3(end.x, end.z, 0));
            
            if (symbol != null)
            {
                foreach (Symbol.VertexPair line in symbol.vertexPairs)
                {
                    Draw.Line(line.A, line.B);
                }
            }
        }
    }

    private void AddPoint()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (drawAction.WasPressedThisFrame())
            {
                page = hit.transform.parent.gameObject.GetComponent<Page>();
                if (page == null) return;
                start = hit.point - page.transform.position;
                end = hit.point - page.transform.position;

                Debug.Log($"Page {page.gameObject} - Point {start}");
            }
            else if(drawAction.IsPressed())
            {
                if (page == null) return;
                end = hit.point - page.transform.position;
            }
            else if (drawAction.WasReleasedThisFrame())
            {
                if (page == null) return;
                end = hit.point - page.transform.position;
                Debug.Log($"Page {page.gameObject} - Point {end}");
                AddLineSegment();
            }
        }
    }

    private void AddLineSegment()
    {
        Debug.Log("Added");
        if(symbol == null) symbol = new Symbol();
        symbol.AddVertexPair(new Vector3(start.x, start.z, 0), new Vector3(end.x, end.z, 0));
    }

    private void FinishSymbol(InputAction.CallbackContext input)
    {
        if (symbol == null) return;

        Vector3 offset = symbol.LocalizeAroundBoundsCenter();
        symbol.UpdateUID();

        Rune rune = defaultVariableRune;
        if (runes.ContainsKey(symbol.UID)) rune = runes[symbol.UID];
        rune = Instantiate(rune);

        rune.transform.position = page.transform.position + new Vector3(offset.x, 0, offset.y);
        rune.associatedSymbol = symbol;
        page.AddRune(rune);

        symbol = null;
        start = Vector2.zero;
        end = Vector2.zero;
        page = null;
    }

    private void DeleteRune(InputAction.CallbackContext input)
    {

    }
}

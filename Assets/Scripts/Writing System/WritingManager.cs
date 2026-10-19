using System.Collections.Generic;
using System.Linq;
using Shapes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WritingManager : ImmediateModeShapeDrawer
{
    public InputAction drawAction, finishAction;
    public Rune defaultVariableRune;
    public List<Rune> runes = new();
    public float angleComparisonLeeway = 10f;
    public SoundEffect writeSFX, runeAddedSFX;

    private Vector3 start, end;
    private Page page;
    private Symbol symbol;
    private HashSet<int> addedSymbols = new();

    private new void OnEnable()
    {
        base.OnEnable();
        finishAction.performed += DeleteRune;
        finishAction.performed += FinishSymbol;
        drawAction.Enable();
        finishAction.Enable();
    }
    private new void OnDisable()
    {
        base.OnDisable();
        finishAction.performed -= FinishSymbol;
        finishAction.performed -= DeleteRune;
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
                Quaternion.AngleAxis(90, Vector3.right),
                Vector3.one);// new Vector3(page.transform.lossyScale.x, page.transform.lossyScale.z, page.transform.lossyScale.y));
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
                writeSFX?.Play(hit.point);
                //Debug.Log($"Page {page.gameObject} - Point {start}");
            }
            else if(drawAction.IsPressed())
            {
                if (page == null) return;
                Vector3 updatedEnd = hit.point - page.transform.position;
                end = updatedEnd;
            }
            else if (drawAction.WasReleasedThisFrame())
            {
                if (page == null) return;
                end = hit.point - page.transform.position;
                //Debug.Log($"Page {page.gameObject} - Point {end}");
                writeSFX?.Play(hit.point);
                AddLineSegment();
            }
        }
    }

    private void AddLineSegment()
    {
        if(symbol == null) symbol = new Symbol();
        symbol.AddVertexPair(new Vector3(start.x, start.z, 0), new Vector3(end.x, end.z, 0));
    }

    private void FinishSymbol(InputAction.CallbackContext input)
    {
        if (symbol == null || page == null) return;

        Vector3 offset = symbol.LocalizeAroundBoundsCenter();
        symbol.UpdateComparer();

        Rune runePrefab = GetRune(symbol, out bool isVariable);
        Debug.Log(runePrefab);
        // If this is a new symbol (a variable rune) keep track of it from here on out and keep a version of the rune to copy
        int UID = symbol.sourceRuneUID;
        if (isVariable && !addedSymbols.Contains(UID))
        {
            addedSymbols.Add(UID);
            runePrefab = Instantiate(runePrefab);
            runePrefab.associatedSymbol = symbol;
            runePrefab.associatedSymbol.UpdateComparer();
            runePrefab.gameObject.SetActive(false);
            runes.Add(runePrefab);
        }

        Rune rune = Instantiate(runePrefab,
            page.transform.position + page.transform.up * 0.001f * page.transform.localScale.y + new Vector3(offset.x, 0, offset.y),
            Quaternion.identity);
        rune.associatedSymbol = symbol;
        rune.associatedSymbol.sourceRuneUID = runePrefab.associatedSymbol.sourceRuneUID;
        rune.cannotBeDeleted = false;
        rune.updateTransformOnCreation = true;
        rune.gameObject.SetActive(true);
        page.AddRune(rune);
        runeAddedSFX?.Play(rune.transform.position);


        symbol.ScaleSymbol(new Vector3(Mathf.Sign(page.transform.lossyScale.x) * 25f, 25f, 25f));

        symbol = null;
        start = Vector2.zero;
        end = Vector2.zero;
        page = null;
    }

    private Rune GetRune(Symbol symbol, out bool variableRune)
    {
        foreach(Rune rune in runes.Where(x => x.associatedSymbol.comparisonPairs.Count == symbol.comparisonPairs.Count))
        {
            bool valid = true;
            for(int i = 0; i < symbol.comparisonPairs.Count; i++)
            {
                float deltaAngle = Mathf.Abs(rune.associatedSymbol.comparisonPairs[i].angle - symbol.comparisonPairs[i].angle);
                //if (symbol.comparisonPairs[i].angle < 90f) deltaAngle = Mathf.Abs(rune.associatedSymbol.comparisonPairs[i].angle - symbol.comparisonPairs[i].angle);
                //else deltaAngle = Mathf.Abs(rune.associatedSymbol.comparisonPairs[i].angle - symbol.comparisonPairs[i].angle) - 180f;

                //Debug.Log($"{rune}: {deltaAngle}");
                if (deltaAngle > angleComparisonLeeway)
                {
                    valid = false;
                    break;
                }
            }
            if (valid)
            {
                variableRune = false;
                return rune;
            }
        }
        variableRune = true;
        return defaultVariableRune;
    }

    private void DeleteRune(InputAction.CallbackContext input)
    {
        if (page != null) return;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Page page = hit.transform.parent.gameObject.GetComponent<Page>();
            if (page == null) return;
            Rune rune = page.GetRuneAtPosition(hit.point);
            if (rune == null || rune.cannotBeDeleted) return;
            page.RemoveRune(rune);
            Destroy(rune.gameObject);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using Shapes;
using System.ComponentModel;

[System.Serializable, ExecuteAlways]
public abstract class Rune : ImmediateModeShapeDrawer
{
    public enum TriggerType { None, Tick, ManaRecieved }
    public TriggerType triggerType;
    [SerializeField]
    protected Mana _storedMana;
    public virtual Mana StoredMana
    {
        get => _storedMana;
        set
        {
            _storedMana = value;
            _storedMana = _storedMana.Limit(maximumPower);
        }
    }

    public static Dictionary<int, Mana> _imbuedMana = new();
    public virtual Mana ImbuedMana
    {
        get
        {
            if (!_imbuedMana.ContainsKey(associatedSymbol.UID)) return new Mana(true);
            return _imbuedMana[associatedSymbol.UID];
        }
    }
    public Mana imbuedDebugReader;
    public float maximumPower = 8f; // Currently this just prevents the elemental alignment from going beyond this amount, but it could cause an effect once it exceeds it
    public Symbol associatedSymbol;
    public Page page;
    public Rune parent;

    public virtual void Initialize()
    {
        if (triggerType == TriggerType.Tick)
        {
            GameTick.Subscribe(Effect);
        }
        Spellbook.Instance.runeElementalDecay += ManaDecay;
        if (!_imbuedMana.ContainsKey(associatedSymbol.UID))
            _imbuedMana.Add(associatedSymbol.UID, StoredMana);
    }

    public void Destroy()
    {
        if (triggerType == TriggerType.Tick)
        {
            GameTick.Unsubscribe(Effect);
        }
        Spellbook.Instance.runeElementalDecay -= ManaDecay;
    }

    protected virtual void ManaDecay(float decayAmount)
    {
        if (_storedMana.elementalAlignment == Vector3.zero) return;

        if (_storedMana.deltaAlignment.x == 0)
        {
            _storedMana.elementalAlignment.x -= _storedMana.elementalAlignment.x * decayAmount;
        }
        if (_storedMana.deltaAlignment.y == 0)
        {
            _storedMana.elementalAlignment.y -= _storedMana.elementalAlignment.y * decayAmount;
        }
        if (_storedMana.deltaAlignment.z == 0)
        {
            _storedMana.elementalAlignment.z -= _storedMana.elementalAlignment.z * decayAmount;
        }

        _storedMana.voidAlignment = Vector3.zero;
        _storedMana.deltaAlignment = Vector3.zero;
    }

    public abstract void Effect();

    /// <summary>
    /// Imbues mana into the source rune of non-preset runes.
    /// This is usually just merging it, but not always.
    /// </summary>
    public abstract void Imbue(Mana mana);

    /// <summary>
    /// Extracts mana from the source rune.
    /// This mana can contain elemental alignments, rules, etc.
    /// Removes the stored mana from a non-preset rune.
    /// </summary>
    public abstract Mana Extract(float amountExtracted);

    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            DrawRune();
        }
    }

    public virtual void DrawRune()
    {
        Color symbolColor = StoredMana.GetManaColor();
        Draw.Color = Color.black;
        foreach(ShapeRenderer shape in associatedSymbol.extraShapes)
        {
            shape.Color = symbolColor;
        }
        foreach (Symbol.VertexPair line in associatedSymbol.vertexPairs)
        {
            // Note: this is probably horribly laggy
            // Glowy line underneath
            if(symbolColor != Color.black)
            {
                Draw.Matrix = Matrix4x4.TRS(transform.position + transform.up * transform.lossyScale.y * 0.001f, transform.rotation * Quaternion.AngleAxis(90, Vector3.right), new Vector3(transform.lossyScale.x, transform.lossyScale.z, transform.lossyScale.y));
                Draw.Line(line.A, line.B, Draw.Thickness * 1.2f, symbolColor);
            }
        }
        foreach (Symbol.VertexPair line in associatedSymbol.vertexPairs)
        {
            // Normal ink above
            Draw.Matrix = Matrix4x4.TRS(transform.position + transform.up * transform.lossyScale.y * 0.0011f, transform.rotation * Quaternion.AngleAxis(90, Vector3.right), new Vector3(transform.lossyScale.x, transform.lossyScale.z, transform.lossyScale.y));
            Draw.Line(line.A, line.B);
        }
    }

    public virtual void SuccessVisual() { }
    public virtual void FailureVisual() { }
}

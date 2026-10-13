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
    private Mana _storedMana;
    public Mana StoredMana
    {
        get => _storedMana;
        set
        {
            _storedMana = value;
            if (_storedMana.elementalAlignment.magnitude > maximumPower)
            {
                _storedMana.elementalAlignment = _storedMana.elementalAlignment.normalized * maximumPower;
            }
        }
    }

    public static Dictionary<int, Mana> _imbuedMana = new();
    public virtual Mana ImbuedMana
    {
        get
        {
            if (!_imbuedMana.ContainsKey(associatedSymbol.UID)) return new Mana();
            return _imbuedMana[associatedSymbol.UID];
        }
    }
    public Mana imbuedDebugReader;
    public float maximumPower = 10f; // Currently this just prevents the elemental alignment from going beyond this amount, but it could cause an effect once it exceeds it
    public Symbol associatedSymbol;

    private void Start()
    {
        if (!Application.isPlaying) return;
        if (triggerType == TriggerType.Tick)
        {
            Spellbook.Instance.tickGameState += Effect;
        }
        Spellbook.Instance.runeElementalDecay += ManaDecay;
        if (!_imbuedMana.ContainsKey(associatedSymbol.UID))
            _imbuedMana.Add(associatedSymbol.UID, StoredMana);
    }

    public void Destroy()
    {
        if (triggerType == TriggerType.Tick)
        {
            Spellbook.Instance.tickGameState -= Effect;
        }
        Spellbook.Instance.runeElementalDecay -= ManaDecay;
    }

    private void ManaDecay(float decayAmount)
    {
        _storedMana.elementalAlignment -= _storedMana.elementalAlignment.normalized * Mathf.Clamp(decayAmount, 0, _storedMana.elementalAlignment.magnitude);
        _storedMana.voidAlignment = Vector3.zero;
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
    public abstract Mana Extract();

    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            DrawRune();
        }
    }

    public void DrawRune()
    {
        Draw.Matrix = Matrix4x4.TRS(transform.position + transform.up * transform.lossyScale.y * 0.0001f, transform.rotation * Quaternion.AngleAxis(90, Vector3.right), new Vector3(transform.lossyScale.x, transform.lossyScale.z, transform.lossyScale.y));
        Color symbolColor = StoredMana.GetManaColor();
        foreach(ShapeRenderer shape in associatedSymbol.extraShapes)
        {
            shape.Color = symbolColor;
        }
        foreach (Symbol.VertexPair line in associatedSymbol.vertexPairs)
        {
            Draw.Line(line.A, line.B, symbolColor);
        }
    }

    public virtual void SuccessVisual() { }
    public virtual void FailureVisual() { }
}

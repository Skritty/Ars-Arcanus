using System.Collections.Generic;
using UnityEngine;
using Shapes;
using System.ComponentModel;
using System;

[System.Serializable, ExecuteAlways]
public abstract class Rune : ImmediateModeShapeDrawer
{
    [Flags]
    public enum TriggerType 
    { 
        None = 0, 
        Tick = 1 << 1, 
        ManaRecieved = 1 << 2, 
        Created = 1 << 3
    }
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
            if (!_imbuedMana.ContainsKey(associatedSymbol.sourceRuneUID)) return new Mana(true);
            return _imbuedMana[associatedSymbol.sourceRuneUID];
        }
    }
    public Mana imbuedDebugReader;
    public bool drawBounds;
    public float maximumPower = 8f; // Currently this just prevents the elemental alignment from going beyond this amount, but it could cause an effect once it exceeds it
    public Symbol associatedSymbol;
    public Page page;
    public Rune_Container parent;

    public virtual void Initialize()
    {
        if (triggerType.HasFlag(TriggerType.Tick))
        {
            GameTick.Subscribe(Effect);
        }
        Spellbook.Instance.runeElementalDecay += ManaDecay;
        if (!_imbuedMana.ContainsKey(associatedSymbol.sourceRuneUID))
            _imbuedMana.Add(associatedSymbol.sourceRuneUID, StoredMana);
        //associatedSymbol.UpdateBounds(transform.position, transform.lossyScale);
        if (triggerType.HasFlag(TriggerType.Created))
        {
            Effect();
        }
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying) return;
        if (triggerType.HasFlag(TriggerType.Tick))
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
    public virtual void Imbue(Mana mana) { }

    /// <summary>
    /// Extracts mana from the source rune.
    /// This mana can contain elemental alignments, rules, etc.
    /// Removes the stored mana from a non-preset rune.
    /// </summary>
    public virtual Mana Extract(float amountExtracted) => new Mana();

    public virtual bool PositionInBounds(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        return associatedSymbol.bounds.Contains(position);
    }

    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            DrawRune();
        }
    }

    public virtual void DrawRune()
    {
        if (page != null && !page.accessible) return;
        if (drawBounds)
        {
            Draw.Matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z));
            Draw.Color = new Color(1f, 0, 0, .2f);
            Draw.Cuboid(associatedSymbol.bounds.center, associatedSymbol.bounds.size);
        }
        Color symbolColor = StoredMana.GetManaColor();
        Draw.Color = Color.black;

        foreach(ShapeRenderer shape in associatedSymbol.extraShapes)
        {
            shape.Color = symbolColor;
        }

        Draw.Matrix = Matrix4x4.TRS(transform.position + transform.up * transform.lossyScale.y * 0.001f, transform.rotation * Quaternion.AngleAxis(90, Vector3.right), new Vector3(transform.lossyScale.x, transform.lossyScale.z, transform.lossyScale.y));
        foreach (Symbol.VertexPair line in associatedSymbol.vertexPairs)
        {
            // Note: this is probably horribly laggy
            // Glowy line underneath
            if(symbolColor != Color.black)
            {
                
                Draw.Line(line.A, line.B, Draw.Thickness * 1.2f, symbolColor);
            }
        }

        Draw.Matrix = Matrix4x4.TRS(transform.position + transform.up * transform.lossyScale.y * 0.0011f, transform.rotation * Quaternion.AngleAxis(90, Vector3.right), new Vector3(transform.lossyScale.x, transform.lossyScale.z, transform.lossyScale.y));
        foreach (Symbol.VertexPair line in associatedSymbol.vertexPairs)
        {
            // Normal ink above
            Draw.Line(line.A, line.B);
        }
    }

    private void OnValidate()
    {
        if (associatedSymbol != null && associatedSymbol.updateComparer) associatedSymbol.UpdateComparer();
    }

    public virtual void SuccessVisual() { }
    public virtual void FailureVisual() { }
}

using System.Collections.Generic;
using UnityEngine;
using Shapes;

[System.Serializable]
public abstract class Rune : MonoBehaviour
{
    public enum TriggerType { None, Tick, ManaRecieved }
    public TriggerType triggerType;
    public Rune sourceRune; // Used for imbuing
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
    public virtual Mana ImbuedMana => sourceRune.StoredMana;
    public float maximumPower = 10f; // Currently this just prevents the elemental alignment from going beyond this amount, but it could cause an effect once it exceeds it
    public Symbol associatedSymbol;

    public void Initialize()
    {
        if (triggerType == TriggerType.Tick)
        {
            Spellbook.Instance.tickGameState += Effect;
        }
        Spellbook.Instance.runeElementalDecay += _storedMana.Decay;
        //Spellbook.Instance.tickDrawUpdate += UpdateColor;
    }

    public void Destroy()
    {
        if (triggerType == TriggerType.Tick)
        {
            Spellbook.Instance.tickGameState -= Effect;
        }
        Spellbook.Instance.runeElementalDecay -= _storedMana.Decay;
        //Spellbook.Instance.tickDrawUpdate -= UpdateColor;
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

    public void DrawRune()
    {
        Draw.Matrix = Matrix4x4.TRS(transform.position - transform.up * 0.0001f, transform.rotation * Quaternion.AngleAxis(90, Vector3.right), transform.lossyScale * 0.004f);
        Color symbolColor = StoredMana.GetManaColor();
        foreach (Symbol.VertexPair line in associatedSymbol.vertexPairs)
        {
            Draw.Line(line.A, line.B, symbolColor);
        }
        
        //associatedSymbol.polyline.Color = StoredMana.GetManaColor();
    }

    public virtual void SuccessVisual() { }
    public virtual void FailureVisual() { }
}

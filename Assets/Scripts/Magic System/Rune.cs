using UnityEngine;

[System.Serializable]
public abstract class Rune
{
    public string name;
    public enum TriggerType { None, Tick, ManaRecieved }
    public TriggerType triggerType;
    public Rune sourceRune; // Used for imbuing
    private Mana _storedMana;
    public Mana StoredMana
    {
        get => _storedMana;
        set
        {
            _storedMana = value;
            if (_storedMana.Power > maximumPower)
            {
                _storedMana.elementalAlignment = _storedMana.elementalAlignment.normalized * maximumPower;
            }
            Draw();
        }
    }
    public virtual Mana ImbuedMana => sourceRune.StoredMana;
    protected float maximumPower; // Currently this just prevents the elemental alignment from going beyond this amount, but it could cause an effect once it exceeds it
    public Symbol associatedSymbol;

    public void Initialize()
    {
        if (triggerType == TriggerType.Tick)
        {
            MagicManager.Instance.tickGameState += Effect;
        }
        MagicManager.Instance.runeManaDecay += _storedMana.Decay;
    }

    public void Destroy()
    {
        MagicManager.Instance.runeManaDecay -= _storedMana.Decay;
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
    public virtual void Draw() { }
    public virtual void SuccessVisual() { }
    public virtual void FailureVisual() { }
}

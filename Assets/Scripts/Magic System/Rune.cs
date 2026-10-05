using UnityEngine;

[System.Serializable]
public abstract class Rune
{
    public string name;
    public Rune sourceRune; // Used for imbuing
    private Mana storedMana; // The mana currently being held, or in the case of the source rune, the imbued mana
    public Symbol associatedSymbol;

    public abstract void Effect(Mana mana);
    public abstract void Imbue(Mana mana);
    public abstract Mana Extract();
    public virtual void Draw(Mana mana) { }
    public virtual void SuccessVisual(Mana mana) { }
    public virtual void FailureVisual(Mana mana) { }
}

using UnityEngine;

public class Rune_ParameterSlot : Rune_Container
{
    public override Mana StoredMana 
    {
        get
        {
            if (focus == null) return new Mana();
            return focus.StoredMana;
        }
        set
        {
            if (focus == null) return;
            focus.StoredMana = value;
        }
    }

    public override Mana ImbuedMana
    {
        get
        {
            if (focus == null) return new Mana();
            return focus.ImbuedMana;
        }
    }
    public override void Effect()
    {
        // Do nothing
    }

    protected override void ManaDecay(float decayAmount)
    {
        // Don't
    }

    public override bool TryAddParameters()
    {
        // Cannot have parameters
        return false;
    }

    public override void DrawRune()
    {
        // Do not draw
    }

    public override Mana Extract(float amountExtracted)
    {
        if (focus == null) return new Mana();
        return focus.Extract(amountExtracted);
    }

    public override void Imbue(Mana mana)
    {
        if (focus == null) return;
        focus.Imbue(mana);
    }
}

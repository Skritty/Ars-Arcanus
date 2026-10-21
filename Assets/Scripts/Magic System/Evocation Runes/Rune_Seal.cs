using UnityEngine;

public class Rune_Seal : Rune
{
    private bool state;
    public override void Effect()
    {
        bool newState = !StoredMana.IsAligned(Alignment.Dark);
        if(newState != state) Spellbook.Instance.UpdatePageAccess(Spellbook.Instance.GetPageFromContainedRune(this), newState);
        state = newState;
    }

    public override Mana Extract(float amountExtracted)
    {
        return new Mana();
    }

    /// <summary>
    /// Overwrites the the mana on the rune instead of flowing
    /// </summary>
    public override void Imbue(Mana mana)
    {
        StoredMana = mana;
        Effect();
    }
}

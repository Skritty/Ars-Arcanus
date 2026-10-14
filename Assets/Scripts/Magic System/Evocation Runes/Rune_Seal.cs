using UnityEngine;

public class Rune_Seal : Rune
{
    public override void Effect()
    {
        Spellbook.Instance.UpdatePageAccess(Spellbook.Instance.GetPageFromContainedRune(this), !StoredMana.IsAligned(Alignments.Dark));
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

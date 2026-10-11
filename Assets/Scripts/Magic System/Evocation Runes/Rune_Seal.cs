using UnityEngine;

public class Rune_Seal : Rune
{
    public override void Effect()
    {
        if (StoredMana.IsAligned(Alignments.Dark))
        {
            Spellbook.Instance.UpdatePageAccess(Spellbook.Instance.GetPageFromContainedRune(this), false);
        }
        else
        {
            Spellbook.Instance.UpdatePageAccess(Spellbook.Instance.GetPageFromContainedRune(this), true);
        }
    }

    public override Mana Extract()
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

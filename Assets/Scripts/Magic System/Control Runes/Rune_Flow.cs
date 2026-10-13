using UnityEngine;

public class Rune_Flow : Rune
{
    // TODO: determine these
    public Rune from = null;
    public Rune to = null;

    public override void Effect()
    {
        if (to != null)
        {
            // Flow mana to the rune in front
            Mana toMana = StoredMana.FlowAmountTo(to.StoredMana);
            to.StoredMana += toMana;
            StoredMana -= toMana;
        }

        if (from != null)
        {
            // Flow mana from the rune behind
            Mana fromMana = from.StoredMana.FlowAmountTo(StoredMana);
            StoredMana += fromMana;
            from.StoredMana -= fromMana;
        }
    }

    public override Mana Extract()
    {
        return new Mana();
    }

    public override void Imbue(Mana mana)
    {
        // Do Nothing
    }
}

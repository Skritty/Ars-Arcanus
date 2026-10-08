using UnityEngine;

public class Rune_Flow : Rune
{
    public override void Effect()
    {
        // TODO: determine these
        Rune from = null;
        Rune to = null;

        // Flow mana to the rune in front
        Mana toMana = StoredMana.FlowAmountTo(to.StoredMana);
        to.StoredMana += toMana;
        StoredMana -= toMana;

        // Flow mana from the rune behind
        Mana fromMana = from.StoredMana.FlowAmountTo(StoredMana);
        StoredMana += fromMana;
        from.StoredMana -= fromMana;
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

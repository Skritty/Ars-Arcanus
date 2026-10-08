using UnityEngine;

public class Rune_Container_Extract : Rune_Container
{
    public override void Effect()
    {
        // Mana is split and flows evenly (25%) to each parameter
        Mana splitMana = StoredMana.Split(parameters.Count);
        foreach (Rune rune in parameters)
        {
            Mana movedSplit = splitMana.FlowAmountTo(rune.StoredMana);
            rune.StoredMana += movedSplit;
            StoredMana -= movedSplit;
        }

        // Mana is extracted from the focus rune and merged into the extract rune. Excess mana is destroyed
        Mana extractedMana = focus.Extract();
        StoredMana = Mana.Merge(maximumPower, StoredMana, extractedMana);
    }
}

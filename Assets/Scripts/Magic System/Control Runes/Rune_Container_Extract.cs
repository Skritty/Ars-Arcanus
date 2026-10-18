using UnityEngine;

public class Rune_Container_Extract : Rune_Container
{
    public float extractionRate;
    public override void Effect()
    {
        // Mana is split and flows evenly (25%) to each parameter
        Mana splitMana = StoredMana.Split(parameters.Count);
        foreach (Rune rune in parameters)
        {
            Mana movedSplit = splitMana.FlowAmountTo(rune.StoredMana);
            GameTick.Delay(() =>
            {
                rune.StoredMana += movedSplit;
                StoredMana -= movedSplit;
            });
        }

        // Mana is extracted from the focus rune and merged into the extract rune. Excess mana is destroyed
        if (focus != null)
        {
            Mana extractedMana = focus.Extract(extractionRate);
            GameTick.Delay(() =>
            {
                StoredMana = Mana.Merge(maximumPower, StoredMana, extractedMana);
            });
        }
    }

    public override bool TryAddParameters()
    {
        if (parameters.Count > 0) return false;
        UpdateParameterSlots(4);
        return true;
    }
}

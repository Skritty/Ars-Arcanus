using System.Linq;
using UnityEngine;

public class Rune_Container_Imbue : Rune_Container
{
    public float imbuementRate;
    public override void Effect()
    {
        // Mana flows from each parameter into 25% of the imbue's mana, which is then merged together
        Mana splitMana = StoredMana.Split(parameters.Count);
        Mana total = new Mana();
        foreach (Rune rune in parameters)
        {
            Mana fromParam = rune.StoredMana;// rune.StoredMana.FlowAmountTo(splitMana);
            total += fromParam;
            GameTick.Delay(() =>
            {
                rune.StoredMana -= fromParam;
            });
        }
        GameTick.Delay(() =>
        {
            StoredMana += total;
        });

        // Mana is imbued into the focus
        if (focus != null)
        {
            Mana amountImbued = StoredMana.Limit(imbuementRate);
            GameTick.Delay(() =>
            {
                focus.Imbue(amountImbued);
                StoredMana -= amountImbued;
            });
        }
    }

    public override bool TryAddParameters()
    {
        if (parameters.Count > 0) return false;
        UpdateParameterSlots(3);
        return true;
    }
}

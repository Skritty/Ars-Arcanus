using System.Linq;
using UnityEngine;

public class Rune_Container_Imbue : Rune_Container
{
    public override void Effect()
    {
        // Mana is imbued into the focus
        focus.Imbue(StoredMana);
        StoredMana = new Mana();

        // Mana flows from each parameter into 25% of the imbue's mana, which is then merged together
        Mana splitMana = StoredMana.Split(parameters.Count);
        foreach (Rune rune in parameters)
        {
            Mana fromParam = rune.StoredMana.FlowAmountTo(splitMana);
            StoredMana += fromParam;
            rune.StoredMana -= fromParam;
        }
    }
}

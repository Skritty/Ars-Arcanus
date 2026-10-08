using System.Linq;
using UnityEngine;

public class Rune_Container_Diffuse : Rune_Container
{
    public override void Effect()
    {
        // Parameter mana is first merged and sent to the focus (skipping the diffuse rune's stored mana)
        // With no parameters, the mana is sent directly to the focus rune
        if(parameters.Count > 0)
        {
            Mana mergedMana = Mana.Merge(maximumPower, parameters.Select(x => x.StoredMana).ToArray());
            focus.StoredMana += mergedMana;
            foreach (Rune rune in parameters)
            {
                rune.StoredMana = new Mana();
            }
        }
        else
        {
            focus.StoredMana += StoredMana;
            StoredMana = new Mana();
        }

            // Mana is then split and flows evenly to all parameters (% varies)
            Mana splitMana = StoredMana.Split(parameters.Count);
        foreach (Rune rune in parameters)
        {
            Mana toParam = splitMana.FlowAmountTo(rune.StoredMana);
            rune.StoredMana += toParam;
            StoredMana -= toParam;
        }
    }
}

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
            GameTick.Delay(() =>
            {
                focus.StoredMana += mergedMana;
                foreach (Rune rune in parameters)
                {
                    rune.StoredMana = new Mana(true);
                }
            });
        }
        else
        {
            GameTick.Delay(() =>
            {
                focus.StoredMana += StoredMana;
                StoredMana = new Mana(true);
            });
        }

        // Mana is then split and flows evenly to all parameters (% varies)
        Mana splitMana = StoredMana.Split(parameters.Count);
        Mana total = new Mana();
        foreach (Rune rune in parameters)
        {
            Mana toParam = rune.StoredMana;//splitMana.FlowAmountTo(rune.StoredMana);
            total += toParam;
            GameTick.Delay(() =>
            {
                rune.StoredMana += toParam;
            });
        }
        GameTick.Delay(() =>
        {
            StoredMana -= total;
        });
    }

    protected override void ManaDecay(float decayAmount)
    {
        if (focus == null) return;
        base.ManaDecay(decayAmount);
    }

    public override bool TryAddParameters()
    {
        if (parameters.Count >= 8) return false;
        UpdateParameterSlots(parameters.Count + 1);
        return true;
    }
}

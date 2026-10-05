using UnityEngine;

public class Rune_Container_Extract : Rune_Container
{
    public override void Effect(Mana mana)
    {
        mana = focus.Extract();
        foreach (Rune rune in parameters)
        {
            rune.Effect(mana);
        }
    }
}

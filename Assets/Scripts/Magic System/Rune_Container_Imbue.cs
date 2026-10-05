using UnityEngine;

public class Rune_Container_Imbue : Rune_Container
{
    public override void Effect(Mana mana)
    {
        foreach (Rune rune in parameters)
        {
            rune.Effect(mana);
        }
        focus.Imbue(mana);
    }
}

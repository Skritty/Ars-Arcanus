using UnityEngine;

public class Rune_Element : Rune
{
    public override void Effect()
    {
        
    }

    public override Mana Extract()
    {
        return ImbuedMana;
    }

    public override void Imbue(Mana mana)
    {
        // Do Nothing
    }
}

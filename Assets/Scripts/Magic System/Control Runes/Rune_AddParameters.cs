using UnityEngine;

public class Rune_AddParameters : Rune
{
    public override void Effect()
    {
        if (parent)
        {
            parent.TryAddParameters();
        }
        page.RemoveRune(this);
        Destroy(this);
    }

    public override Mana Extract(float amountExtracted)
    {
        return new Mana();
    }

    public override void Imbue(Mana mana)
    {
        // Do Nothing
    }
}

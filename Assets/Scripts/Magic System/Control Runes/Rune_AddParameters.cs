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
}

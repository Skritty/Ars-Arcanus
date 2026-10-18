using UnityEngine;

public class Rune_ContentsCompare : Rune_Container
{
    public Mana invalidOutputMana;
    public Mana validOutputMana;
    public Alignment requiredFocusAlignment;
    public bool anyAlignment;
    public Rune requiredFocusRune;
    public bool anyFocusRune;
    public override void Effect()
    {
        bool valid = true;
        if (anyFocusRune)
        {
            if (focus == null) valid = false;
            
        }
        else if(requiredFocusRune != focus) valid = false;
        if (!anyAlignment)
        {
            if (focus == null && !anyAlignment) valid = false;
            if (focus != null && !focus.StoredMana.IsAligned(requiredFocusAlignment)) valid = false;
        }
        GameTick.Delay(() =>
        {
            if (valid) StoredMana = validOutputMana;
            else StoredMana = invalidOutputMana;
        });
    }

    public override bool TryAddParameters()
    {
        if (parameters.Count >= 2) return false;
        UpdateParameterSlots(2);
        return true;
    }
}

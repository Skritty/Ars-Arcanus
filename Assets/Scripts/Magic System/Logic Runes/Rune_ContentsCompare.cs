using UnityEngine;

public class Rune_ContentsCompare : Rune_Container
{
    public Mana invalidOutputMana;
    public Mana validOutputMana;
    public Alignment requiredFocusAlignment;
    public bool anyAlignment;
    public Rune requiredFocusRune;
    public bool anyFocusRune;
    public bool reverseFlow;

    public override void Effect()
    {
        Rune target = this;
        Rune source = focus;
        if (reverseFlow)
        {
            target = focus;
            source = this;
        }
        bool valid = true;
        if (anyFocusRune)
        {
            if (source == null) valid = false;
            
        }
        else if(requiredFocusRune != source) valid = false;
        if (!anyAlignment)
        {
            if (source == null && !anyAlignment) valid = false;
            if (source != null && !source.StoredMana.IsAligned(requiredFocusAlignment)) valid = false;
        }
        GameTick.Delay(() =>
        {
            if (valid) target.StoredMana = validOutputMana;
            else target.StoredMana = invalidOutputMana;
        });
    }

    public override bool TryAddParameters()
    {
        if (parameters.Count >= 2) return false;
        UpdateParameterSlots(2);
        return true;
    }
}

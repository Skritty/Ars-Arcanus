using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// For editor placement only
/// </summary>
public class Rune_UnityTriggerContainer : Rune_Container
{
    public UnityEvent OnActivated;
    public Mana invalidOutputMana;
    public Mana validOutputMana;
    public bool copyFocusManaInstead;
    public Alignment requiredFocusAlignment;
    public bool anyAlignment;
    public Rune requiredFocusRune;
    public bool anyFocusRune;
    public bool setManaOnce;
    public bool triggerOnce = true;
    private bool triggered;
    public override void Effect()
    {
        bool valid = true;
        
        if (!anyFocusRune && requiredFocusRune == null)
        {
            if(focus != null) valid = false;
        }
        else if (anyFocusRune)
        {
            if (focus == null) valid = false;

        }
        else if (requiredFocusRune == null || focus == null || requiredFocusRune.associatedSymbol.sourceRuneUID != focus.associatedSymbol.sourceRuneUID) valid = false;
        if (!anyAlignment)
        {
            if (focus == null && !anyAlignment) valid = false;
            if (focus != null && !focus.StoredMana.IsAligned(requiredFocusAlignment)) valid = false;
        }

        if(!(setManaOnce && triggered))
        {
            GameTick.Delay(() =>
            {
                if (valid)
                {
                    if (focus != null && copyFocusManaInstead) StoredMana = focus.StoredMana;
                    else StoredMana = validOutputMana;
                }
                else StoredMana = invalidOutputMana;
            });
        }

        if (!(triggerOnce && triggered))
        {
            if (valid)
            {
                OnActivated.Invoke();
                triggered = true;
            }
        }
    }

    public override bool TryAddParameters()
    {
        return false;
    }
}

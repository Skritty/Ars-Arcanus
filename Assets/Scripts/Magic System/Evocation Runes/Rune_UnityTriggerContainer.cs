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
    public Alignment requiredFocusAlignment;
    public bool anyAlignment;
    public Rune requiredFocusRune;
    public bool anyFocusRune;
    public bool triggerOnce = true;
    private bool triggered;
    public override void Effect()
    {
        if (triggerOnce && triggered) return;

        bool valid = true;
        if (anyFocusRune)
        {
            if (focus == null) valid = false;

        }
        else if (requiredFocusRune != focus) valid = false;
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

        if (valid)
        {
            OnActivated.Invoke();
            triggered = true;
        }
    }

    public override bool TryAddParameters()
    {
        return false;
    }
}

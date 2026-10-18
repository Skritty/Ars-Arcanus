using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// For editor placement only
/// </summary>
public class Rune_UnityTrigger : Rune
{
    public UnityEvent OnActivated;
    public Alignment activationAlignment;
    public bool triggerOnce = true;
    private bool triggered;
    public override void Effect()
    {
        if (!StoredMana.IsAligned(activationAlignment)) return;
        if ((triggerOnce && triggered)) return;
        OnActivated.Invoke();
        triggered = true;
    }
}

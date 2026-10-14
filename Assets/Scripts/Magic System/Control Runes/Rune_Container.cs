using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A type of rune that acts as a container for other runes.
/// These are usually some combination of an enclosed geometric shape.
/// </summary>
public abstract class Rune_Container : Rune
{
    /// <summary>
    /// The focus is the rune at the center of the diffuse.
    /// Depending on the effect, it will either be read from or have its effect called.
    /// </summary>
    public Rune focus;

    /// <summary>
    /// The parameters are other diffuse runes bound to this one.
    /// If parameters are added, additional markings will be placed to accompany them.
    /// </summary>
    public List<Rune> parameters = new();

    public Vector3 parameterPlacementInitialDirection;
    public float parameterPlacementRadius;
    public Rune_ParameterSlot parameterSlot;
    public GameObject parameterDecorations;

    public override Mana ImbuedMana => focus.ImbuedMana;

    public override void Initialize()
    {
        base.Initialize();
        if (focus != null)
        {
            focus.parent = this;
            focus.transform.parent = transform;
            focus.transform.localScale = Vector3.one;
            focus.transform.localPosition = Vector3.zero;
        }
        UpdateParameterSlots(parameters.Count);
    }

    public override void Imbue(Mana mana)
    {
        focus.Imbue(mana);
    }

    public override Mana Extract(float amountExtracted)
    {
        return focus.Extract(amountExtracted);
    }

    public bool TryAddRune(Rune newRune)
    {
        if(ContainedInFocusBounds(newRune))
        {
            focus = newRune;
            return true;
        }

        foreach (Rune_Container rune in parameters)
        {
            if (rune.TryAddRune(newRune)) return true;
        }

        return false;
    }

    public abstract bool TryAddParameters();
    protected void UpdateParameterSlots(int newParameterCount)
    {
        // Note: this does not account for the removal of parameter slots
        for(int i = 0; i < newParameterCount; i++)
        {
            if (i >= parameters.Count)
            {
                Rune_ParameterSlot slot = Instantiate(parameterSlot, transform);
                parameters.Add(slot);
                page.AddRune(slot);
            }
            else if (parameters[i] is not Rune_ParameterSlot)
            {
                Rune_ParameterSlot slot = Instantiate(parameterSlot, transform);
                slot.focus = parameters[i];
                /*parameters[i].transform.parent = slot.transform;
                parameters[i].transform.localPosition = Vector3.zero;
                parameters[i].transform.localScale = Vector3.one;*/
                parameters[i] = slot;
                page.AddRune(slot);
            }

            Vector3 slotPosition = Quaternion.AngleAxis(360f / parameters.Count * i, Vector3.up) * parameterPlacementInitialDirection.normalized * parameterPlacementRadius;
            parameters[i].transform.localPosition = slotPosition;
        }
    }

    protected virtual bool ContainedInFocusBounds(Rune other)
    {
        // TODO: check and see if it contains the rune in its bounds
        return false;
    }

    public override void DrawRune()
    {
        if(parameterDecorations != null) parameterDecorations.SetActive(parameters.Count > 0);
        base.DrawRune();
    }
}

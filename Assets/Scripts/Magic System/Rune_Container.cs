using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A type of rune that acts as a container for other runes.
/// These are usually some combination of an enclosed geometric shape.
/// </summary>
public class Rune_Container : Rune
{
    /// <summary>
    /// The focus is the rune at the center of the diffuse.
    /// Depending on the effect, it will either be read from or have its effect called.
    /// </summary>
    public Rune focus;

    /// <summary>
    /// The parameters are other diffuse runes bound to this one.
    /// Mana will be sent to these before the focus rune.
    /// If parameters are added, additional markings will be placed to accompany them.
    /// </summary>
    public List<Rune_Container> parameters;

    public override void Effect(Mana mana)
    {
        foreach (Rune rune in parameters)
        {
            rune.Effect(mana);
        }
        focus.Effect(mana);
    }

    public override void Draw(Mana mana)
    {
        base.Draw(mana);
    }

    public override void Imbue(Mana mana)
    {
        focus.Imbue(mana);
    }

    public override Mana Extract()
    {
        Mana mana = focus.Extract();
        foreach (Rune rune in parameters)
        {
            rune.Effect(mana);
        }
        return mana;
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

    protected virtual bool ContainedInFocusBounds(Rune other)
    {
        // TODO: check and see if it contains the rune in its bounds
        return false;
    }
}

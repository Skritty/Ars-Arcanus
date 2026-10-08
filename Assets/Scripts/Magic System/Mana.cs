using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The magic system's information container class.
/// The Mana object is passed between runes and its information is modified by them.
/// New Mana is created by extracting or splitting.
/// Add additional information as needed.
/// </summary>
[System.Serializable]
public struct Mana
{
    /// <summary>
    /// +X = Fire, -X = Water
    /// +Y = Air, -Y = Earth
    /// +Z = Light, -Z = Dark
    /// </summary>
    public Vector3 elementalAlignment;

    /// <summary>
    /// X = Energy
    /// Y = Material
    /// Z = Divine
    /// </summary>
    public Vector3 voidAlignment;

    /// <summary>
    /// The amount of "energy" within mana, based on the chaos-order axis. Can be negative.
    /// </summary>
    public float Power
    {
        get
        {
            return (Quaternion.FromToRotation(Vector3.one, Vector3.up) * Vector3.Project(elementalAlignment, Vector3.one)).y;
            //return elementalAlignment.magnitude;
        }
    }
    //public List<(float percent, Rune rule)> rules; // Rules from preset runes can be imbued into non-preset runes to give them functionality

    public Mana(Vector3 elementalAlignment)//, List<Rune> rules)
    {
        this.elementalAlignment = elementalAlignment;
        this.voidAlignment = Vector3.zero;
        //this.rules = rules;
    }

    public void Decay(float decayAmount)
    {
        elementalAlignment -= elementalAlignment.normalized * decayAmount;
        voidAlignment = Vector3.zero;
    }

    public Mana Split(int count)
    {
        elementalAlignment /= count;
        return this;
    }

    /*public bool CanMerge(Mana recieving, out Mana spillover)
    {
        spillover = new Mana();
        // Mana with no power cannot be transferred!
        if (recieving.Power == 0) return false;

        // --The law of elemental confluence--
        // An element is considered stronger/weaker by its strongest alignment
        // This allows it to flow in a direction that would have an otherwise higher total power or higher axes
        if (Mathf.Sign(elementalAlignment.x) != Mathf.Sign(recieving.elementalAlignment.x) || Mathf.Abs(elementalAlignment.x) <= Mathf.Abs(recieving.elementalAlignment.x)) return true;
        if (Mathf.Sign(elementalAlignment.y) != Mathf.Sign(recieving.elementalAlignment.y) || Mathf.Abs(elementalAlignment.y) <= Mathf.Abs(recieving.elementalAlignment.y)) return true;
        if (Mathf.Sign(elementalAlignment.z) != Mathf.Sign(recieving.elementalAlignment.z) || Mathf.Abs(elementalAlignment.z) <= Mathf.Abs(recieving.elementalAlignment.z)) return true;
        return false;
    }*/

    /// <summary>
    /// Returns the amount of mana flowing out of current into the next.
    /// The returned mana should be added to next and subtracted from current.
    /// </summary>
    public Mana FlowAmountTo(Mana next)
    {
        return (this - next) / 2f;
    }

    public void CalculateVoidAlignment(Mana other)
    {
        // TODO
        /*if (Mathf.Sign(elementalAlignment.x) != Mathf.Sign(other.elementalAlignment.x))
        {
            voidAlignment += 
        }*/
    }

    public static Mana Merge(float maxPower, params Mana[] recieving)
    {
        Mana mergedMana = new Mana();
        mergedMana.elementalAlignment = Vector3.zero;
        foreach (Mana mana in recieving)
        {
            mergedMana.elementalAlignment += mana.elementalAlignment;
            /*foreach(Rune rule in rules)
            {
                if (!mergedMana.rules.Contains(rule)) mergedMana.rules.Add(rule);
            }*/
        }
        if(Mathf.Abs(mergedMana.Power) > maxPower)
        {
            mergedMana.elementalAlignment = mergedMana.elementalAlignment.normalized * maxPower;
        }
        return mergedMana;
    }

    public static Mana operator +(Mana lhs, Mana rhs)
    {
        lhs.elementalAlignment += rhs.elementalAlignment;
        return lhs;
    }

    public static Mana operator -(Mana lhs, Mana rhs)
    {
        lhs.elementalAlignment -= rhs.elementalAlignment;
        return lhs;
    }

    public static Mana operator *(Mana lhs, float rhs)
    {
        lhs.elementalAlignment *= rhs;
        return lhs;
    }

    public static Mana operator /(Mana lhs, float rhs)
    {
        lhs.elementalAlignment /= rhs;
        return lhs;
    }
}
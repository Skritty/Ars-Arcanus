using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum Alignment
{
    None = 1,
    Fire = 1 << 1,
    Water = 1 << 2,
    Air = 1 << 3,
    Earth = 1 << 4,
    Light = 1 << 5,
    Dark = 1 << 6,
    Energy = 1 <<7,
    Material = 1 << 8,
    Divine = 1 << 9,
    Chaos = 1 << 10,
    Order = 1 << 11,
    Nature = 1 << 12
}

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
    /// Used to know what axes should have mana decay applied
    /// </summary>
    public Vector3 deltaAlignment;

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

    public Mana(bool startWithDelta = true)
    {
        this.elementalAlignment = Vector3.zero;
        this.voidAlignment = Vector3.zero;
        this.deltaAlignment = Vector3.one * (startWithDelta ? 1 : 0);
    }

    public Mana(Vector3 elementalAlignment)
    {
        this.elementalAlignment = elementalAlignment;
        this.deltaAlignment = Vector3.zero;
        this.voidAlignment = Vector3.zero;
    }

    /// <summary>
    /// Find if the mana is aligned to all enabled flags
    /// </summary>
    public bool IsAligned(Alignment alignment)
    {
        if (alignment.HasFlag(Alignment.None))
            if (elementalAlignment.magnitude != 0) return false;
        if (alignment.HasFlag(Alignment.Fire))
            if (elementalAlignment.x <= 0) return false;
        if (alignment.HasFlag(Alignment.Water))
            if (elementalAlignment.x >= 0) return false;
        if (alignment.HasFlag(Alignment.Air))
            if (elementalAlignment.y <= 0) return false;
        if (alignment.HasFlag(Alignment.Earth))
            if (elementalAlignment.y >= 0) return false;
        if (alignment.HasFlag(Alignment.Light))
            if (elementalAlignment.z <= 0) return false;
        if (alignment.HasFlag(Alignment.Dark))
            if (elementalAlignment.z >= 0) return false;
        if (alignment.HasFlag(Alignment.Energy))
            if (elementalAlignment.x != 0) return false;
        if (alignment.HasFlag(Alignment.Material))
            if (elementalAlignment.y != 0) return false;
        if (alignment.HasFlag(Alignment.Divine))
            if (elementalAlignment.z != 0) return false;
        if (alignment.HasFlag(Alignment.Chaos))
            if (Power <= 0) return false;
        if (alignment.HasFlag(Alignment.Order))
            if (Power >= 0) return false;
        if (alignment.HasFlag(Alignment.Nature))
            if (elementalAlignment.x == 0 && elementalAlignment.y == 0) return false;
        return true;
    }

    public Color GetManaColor()
    {
        if (elementalAlignment.magnitude == 0) return Color.black;

        // Fire = Red (0)|(1,0,0), Earth = Green (120)|(0,-1,0), Water = Blue (240)|(-1,0,0)
        // Light = Yellow (60)|(0,0,1), Air = Cyan (180)|(0,1,0), Dark = Magenta (300)|(0,0,-1)
        // Power = Value
        // Power = Saturation
        float r = 0;
        float g = 0;
        float b = 0;

        // Add fire/water axis
        float exists = Vector3.Project(elementalAlignment, Vector3.right).x;
        if (exists > 0)
        {
            r += exists;
        }
        if (exists < 0)
        {
            b += -exists;
        }
        
        // Add earth/air axis
        exists = Vector3.Project(elementalAlignment, Vector3.up).y;
        if (exists > 0)
        {
            g += exists * 0.707f;
            b += exists * 0.707f;
        }
        if (exists < 0)
        {
            g += -exists;
         }

        // Add light/dark axis
        exists = Vector3.Project(elementalAlignment, Vector3.forward).z;
        if (exists > 0)
        {
            r += exists * 0.707f;
            g += exists * 0.707f;
        }
        if (exists < 0)
        {
            r += -exists * 0.707f;
            b += -exists * 0.707f;
        }
        
        /*float total = r + b + g;
        r /= total;
        g /= total;
        b /= total;*/

        float brightnessMultiplier = Mathf.Log(Mathf.Abs(elementalAlignment.magnitude) + 1, 2);
        return new Color(
            r / elementalAlignment.magnitude * brightnessMultiplier, 
            g / elementalAlignment.magnitude * brightnessMultiplier, 
            b / elementalAlignment.magnitude * brightnessMultiplier);
        //Color.RGBToHSV(new Color(r, g, b), out float H, out _, out _); // TODO: what the heck unity??
        //return Color.HSVToRGB(H, elementalAlignment.magnitude + 0.5f, elementalAlignment.magnitude + 0.5f);// Mathf.Log10(Mathf.Abs(Power * 5) + 1) + 0.2f, Mathf.Log10(Mathf.Abs(Power * 5) + 1) + 0.2f);
    }

    public Mana Split(int count)
    {
        elementalAlignment /= count;
        return this;
    }

    public Mana Limit(float maxMagnitude)
    {
        if (Mathf.Abs(elementalAlignment.x) > maxMagnitude) elementalAlignment.x = Mathf.Sign(elementalAlignment.x) * maxMagnitude;
        if (Mathf.Abs(elementalAlignment.y) > maxMagnitude) elementalAlignment.y = Mathf.Sign(elementalAlignment.y) * maxMagnitude;
        if (Mathf.Abs(elementalAlignment.z) > maxMagnitude) elementalAlignment.z = Mathf.Sign(elementalAlignment.z) * maxMagnitude;
        /*if (elementalAlignment.magnitude > maxMagnitude)
        {
            elementalAlignment = elementalAlignment.normalized * maxMagnitude;
        }*/
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
        Vector3 average = (elementalAlignment + next.elementalAlignment) / 2f;
        elementalAlignment -= average;
        return this;
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
        foreach (Mana mana in recieving)
        {
            mergedMana.elementalAlignment += mana.elementalAlignment;
        }
        mergedMana = mergedMana.Limit(maxPower);
        mergedMana.elementalAlignment.x = ((int)(mergedMana.elementalAlignment.x * 100)) / 100f;
        mergedMana.elementalAlignment.y = ((int)(mergedMana.elementalAlignment.y * 100)) / 100f;
        mergedMana.elementalAlignment.z = ((int)(mergedMana.elementalAlignment.z * 100)) / 100f;
        mergedMana.deltaAlignment = new Vector3(Mathf.Abs(mergedMana.elementalAlignment.x), Mathf.Abs(mergedMana.elementalAlignment.y), Mathf.Abs(mergedMana.elementalAlignment.z));
        return mergedMana;
    }

    public static Mana operator +(Mana lhs, Mana rhs)
    {
        lhs.elementalAlignment += rhs.elementalAlignment;// + rhs.elementalAlignment.normalized * 0.000001f;
        lhs.elementalAlignment.x = ((int)(lhs.elementalAlignment.x * 100)) / 100f;
        lhs.elementalAlignment.y = ((int)(lhs.elementalAlignment.y * 100)) / 100f;
        lhs.elementalAlignment.z = ((int)(lhs.elementalAlignment.z * 100)) / 100f;
        lhs.deltaAlignment += new Vector3(Mathf.Abs(rhs.elementalAlignment.x), Mathf.Abs(rhs.elementalAlignment.y), Mathf.Abs(rhs.elementalAlignment.z));
        return lhs;
    }

    public static Mana operator -(Mana lhs, Mana rhs)
    {
        lhs.elementalAlignment -= rhs.elementalAlignment;// + rhs.elementalAlignment.normalized * 0.000001f;
        lhs.elementalAlignment.x = ((int)(lhs.elementalAlignment.x * 100)) / 100f;
        lhs.elementalAlignment.y = ((int)(lhs.elementalAlignment.y * 100)) / 100f;
        lhs.elementalAlignment.z = ((int)(lhs.elementalAlignment.z * 100)) / 100f;
        lhs.deltaAlignment += new Vector3(Mathf.Abs(rhs.elementalAlignment.x), Mathf.Abs(rhs.elementalAlignment.y), Mathf.Abs(rhs.elementalAlignment.z));
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
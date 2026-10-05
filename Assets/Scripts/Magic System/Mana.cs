using UnityEngine;

/// <summary>
/// The magic system's information container class.
/// Mana is passed between runes and its information is modified by them.
/// Add additional information as needed.
/// </summary>
[System.Serializable]
public class Mana
{
    /// <summary>
    /// +X = Fire, -X = Water
    /// +Y = Air, -Y = Earth
    /// +Z = Chaos, -Z = Order
    /// </summary>
    public Vector3 elementalAlignment;
}
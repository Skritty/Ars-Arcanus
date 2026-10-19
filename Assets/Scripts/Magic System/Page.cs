using System.Collections.Generic;
using Shapes;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Stores runes and renders Symbols within its bounds.
/// It handles the moving of mana.
/// </summary>
[ExecuteAlways]
public class Page : MonoBehaviour
{
    public bool accessible = true;
    public bool block;
    public Transform runeParent;
    public List<Rune> runes = new();

    private void Start()
    {
        if (!Application.isPlaying) return;

        runes.Clear();
        foreach (Transform runeTransform in runeParent)
        {
            runes.Add(runeTransform.GetComponent<Rune>());
        }
        foreach(Rune rune in runes.ToArray())
        {
            rune.page = this;
            rune.Initialize();
        }
    }

    public bool ContainsRune(Rune rune)
    {
        foreach (Rune r in runes)
        {
            if (r == rune) return true;
        }
        return false;
    }

    public Rune GetRuneAtPosition(Vector3 pos)
    {
        foreach(Rune rune in runes)
        {
            if (rune.PositionInBounds(pos)) return rune;
        }
        return null;
    }

    public void AddRune(Rune newRune)
    {
        runes.Add(newRune);
        newRune.transform.parent = runeParent;
        if (newRune.updateTransformOnCreation)
        {
            newRune.transform.localScale = new Vector3(1f / Mathf.Abs(transform.lossyScale.x), 1f / Mathf.Abs(transform.lossyScale.y), 1f / Mathf.Abs(transform.lossyScale.z));
        }
        newRune.page = this;
        Rune_Container container = GetRuneAtPosition(newRune.transform.position) as Rune_Container;
        if(container) container.TryAddRune(newRune);
        newRune.Initialize();
    }

    public void RemoveRune(Rune rune)
    {
        if (rune.parent != null)
        {
            rune.parent.focus = null;
        }
        runes.Remove(rune);
    }

    public void Lock(bool locked)
    {
        block = locked;
        accessible = !locked;
    }
}

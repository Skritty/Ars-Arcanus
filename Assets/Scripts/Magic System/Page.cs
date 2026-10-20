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
        AddInitialRunes(runeParent);
        foreach(Rune rune in runes.ToArray())
        {
            rune.page = this;
            rune.Initialize();
        }
    }

    private void AddInitialRunes(Transform scan)
    {
        foreach (Transform runeTransform in scan)
        {
            if (runeTransform.GetComponent<Rune>() == null)
            {
                AddInitialRunes(runeTransform);
            }
            else
            {
                runes.Add(runeTransform.GetComponent<Rune>());
            }
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

    public List<Rune> GetRunesAtPosition(Vector3 pos)
    {
        List<Rune> possible = new();
        foreach(Rune rune in runes)
        {
            if (rune.PositionInBounds(pos)) possible.Add(rune);
        }
        if (possible.Count == 0) return possible;
        return possible;
    }

    public void AddRune(Rune newRune)
    {
        List<Rune> foundRunes = GetRunesAtPosition(newRune.transform.position);
        runes.Add(newRune);
        newRune.transform.parent = runeParent;
        newRune.transform.localPosition = new Vector3(newRune.transform.localPosition.x, 0, newRune.transform.localPosition.z);
        if (newRune.updateTransformOnCreation)
        {
            newRune.transform.localScale = new Vector3(1f / Mathf.Abs(transform.lossyScale.x), 1f / Mathf.Abs(transform.lossyScale.y), 1f / Mathf.Abs(transform.lossyScale.z));
        }
        newRune.page = this;
        foreach(Rune rune in foundRunes)
        {
            if(rune is Rune_Container)
            {
                if((rune as Rune_Container).TryAddRune(newRune)) break;
            }
        }
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

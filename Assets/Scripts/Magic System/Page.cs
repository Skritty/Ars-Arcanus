using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores runes and renders Symbols within its bounds.
/// It handles the moving of mana.
/// </summary>
public class Page : MonoBehaviour
{
    /// <summary>
    /// List of outermost runes. Nested runes are contained by these.
    /// </summary>
    public List<Rune> runes;

    public void AddRune(Rune newRune)
    {
        // Steps to creating and adding a new [drawn] rune:
        // 1. Symbol drawer tool sends updates to the Magic Manager until a valid UID is found
        // 2. When this happens, a copy of the matching rune is linked to the drawn symbol and is added to the Page it was drawn on
        // 3. The page tries adding the new rune to any root diffuse runes it already has
        // 4. Those diffuse runes check to see if any of their children contain it, etc.
        //    If one is valid, it adds it as the focus rune there and no where else.
        // 5. If it isn't a nested rune, it is added to the root runes list of the page.
        // 6. A null mana effect update is given to the new rune, which may produce an effect.

        foreach(Rune rune in runes)
        {
            if(rune is Rune_Container)
            {
                (rune as Rune_Container).TryAddRune(newRune);
            }
        }
    }
}

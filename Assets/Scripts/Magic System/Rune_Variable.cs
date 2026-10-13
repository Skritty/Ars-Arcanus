using UnityEngine;

public class Rune_Variable : Rune
{
    public override void Effect()
    {
        // No Effect
    }

    public override Mana Extract()
    {
        if (!_imbuedMana.ContainsKey(associatedSymbol.UID)) return new();
        Mana mana = ImbuedMana;
        _imbuedMana[associatedSymbol.UID] = new();
        imbuedDebugReader = _imbuedMana[associatedSymbol.UID];
        return mana;
    }

    public override void Imbue(Mana mana)
    {
        if (!_imbuedMana.ContainsKey(associatedSymbol.UID))
            _imbuedMana.Add(associatedSymbol.UID, mana);
        else _imbuedMana[associatedSymbol.UID] = Mana.Merge(float.MaxValue, _imbuedMana[associatedSymbol.UID], mana);
        imbuedDebugReader = _imbuedMana[associatedSymbol.UID];
    }
}

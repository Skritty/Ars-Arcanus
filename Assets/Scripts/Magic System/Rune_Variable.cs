using UnityEngine;

public class Rune_Variable : Rune
{
    public override void Effect()
    {
        // No Effect
    }

    public override Mana Extract(float amountExtracted)
    {
        if (!_imbuedMana.ContainsKey(associatedSymbol.UID)) return new();
        Mana mana = ImbuedMana.Limit(amountExtracted);
        _imbuedMana[associatedSymbol.UID] -= mana;
        imbuedDebugReader = _imbuedMana[associatedSymbol.UID];
        return mana;
    }

    public override void Imbue(Mana mana)
    {
        if (!_imbuedMana.ContainsKey(associatedSymbol.UID))
            _imbuedMana.Add(associatedSymbol.UID, mana);
        else _imbuedMana[associatedSymbol.UID] = Mana.Merge(float.MaxValue, _imbuedMana[associatedSymbol.UID], mana).Limit(maximumPower);
        imbuedDebugReader = _imbuedMana[associatedSymbol.UID];
    }
}

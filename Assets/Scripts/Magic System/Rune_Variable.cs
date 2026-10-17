using UnityEngine;

public class Rune_Variable : Rune
{
    public override void Effect()
    {
        // No Effect
    }

    public override Mana Extract(float amountExtracted)
    {
        if (!_imbuedMana.ContainsKey(associatedSymbol.sourceRuneUID)) return new();
        Mana mana = ImbuedMana.Limit(amountExtracted);
        _imbuedMana[associatedSymbol.sourceRuneUID] -= mana;
        imbuedDebugReader = _imbuedMana[associatedSymbol.sourceRuneUID];
        return mana;
    }

    public override void Imbue(Mana mana)
    {
        if (!_imbuedMana.ContainsKey(associatedSymbol.sourceRuneUID))
            _imbuedMana.Add(associatedSymbol.sourceRuneUID, mana);
        else _imbuedMana[associatedSymbol.sourceRuneUID] = Mana.Merge(float.MaxValue, _imbuedMana[associatedSymbol.sourceRuneUID], mana).Limit(maximumPower);
        imbuedDebugReader = _imbuedMana[associatedSymbol.sourceRuneUID];
    }
}

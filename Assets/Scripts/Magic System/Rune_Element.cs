using UnityEngine;

public class Rune_Element : Rune
{
    public override Mana StoredMana 
    { 
        get => _storedMana;
        set
        {
            // Readonly
        }
    }
    public override Mana ImbuedMana => _storedMana;

    public override void Initialize() { }

    public override void Effect()
    {
        // No Effect
    }

    public override Mana Extract(float amountExtracted)
    {
        return ImbuedMana.Limit(amountExtracted);
    }

    public override void Imbue(Mana mana)
    {
        // Do Nothing
    }
}

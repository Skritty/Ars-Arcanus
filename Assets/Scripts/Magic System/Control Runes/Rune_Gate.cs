using UnityEngine;

public class Rune_Gate : Rune_Container
{
    [field: SerializeField]
    public bool GateFlowStatus { get; set; }
    public override void Effect()
    {
        if (GateFlowStatus)
        {
            GameTick.Delay(() =>
            {
                StoredMana = focus.StoredMana;
            });
        }
    }

    public override bool TryAddParameters()
    {
        return false;
    }
}

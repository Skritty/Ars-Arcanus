using System;
using UnityEngine;

public class MagicManager
{
    public static MagicManager Instance;

    public Action tickGameState;
    public Action<float> runeManaDecay;
}

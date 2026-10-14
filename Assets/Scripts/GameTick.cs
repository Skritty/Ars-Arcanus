using System;
using System.Collections.Generic;
using UnityEngine;

public class GameTick : MonoBehaviour
{
    private static Dictionary<int, Action> tickGame = new();
    private static List<int> validOrders = new();
    private static Action delayedUpdate;

    public static void Subscribe(Action gameAction, int order = 0)
    {
        if (!validOrders.Contains(order))
        {
            validOrders.Add(order);
            validOrders.Sort();
            tickGame.Add(order, gameAction);
        }
        else
        {
            tickGame[order] += gameAction;
        }
    }

    public static void Unsubscribe(Action gameAction, int order = 0)
    {
        if (validOrders.Contains(order))
        {
            tickGame[order] -= gameAction;
            if (tickGame[order] == null)
            {
                tickGame.Remove(order);
            }
        }
    }

    public static void Delay(Action gameUpdate)
    {
        delayedUpdate += gameUpdate;
    }

    private void OnEnable()
    {
        Subscribe(DoDelayedUpdate, 100);
    }

    private void OnDisable()
    {
        Unsubscribe(DoDelayedUpdate, 100);
    }

    private void DoDelayedUpdate()
    {
        delayedUpdate?.Invoke();
        delayedUpdate = null;
    }

    private void FixedUpdate()
    {
        foreach(int order in validOrders)
        {
            tickGame[order]?.Invoke();
        }
    }

    private void OnDestroy()
    {
        tickGame.Clear();
        validOrders.Clear();
        delayedUpdate = null;
    }
}

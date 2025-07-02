using System.Collections.Generic;
using UnityEngine;

public class ProcessingUpdate : MonoBehaviour
{
    public static ProcessingUpdate Instance;

    private readonly List<ITick> Ticks = new();
    int TickCount;
    private readonly List<IFixedTick> FixedTicks = new();
    int FixedTickCount;
    private readonly List<ILateTick> LateTicks = new();
    int LateTickCount;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        for (int i = 0; i < TickCount; ++i)
            Ticks[i].Tick();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < FixedTickCount; ++i)
            FixedTicks[i].FixedTick();
    }

    private void LateUpdate()
    {
        for (int i = 0; i < LateTickCount; ++i)
            LateTicks[i].LateTick();
    }

    public void Subscribe(object obj)
    {
        if(obj is ITick tick)
        { 
            Ticks.Add(tick);
            TickCount = Ticks.Count;
        }
        if(obj is IFixedTick fixedTick)
        { 
            FixedTicks.Add(fixedTick);
            FixedTickCount = FixedTicks.Count;
        }
        if(obj is ILateTick lateTick)
        { 
            LateTicks.Add(lateTick);
            LateTickCount = LateTicks.Count;
        }
    }

    public void Unsubscribe(object obj)
    {
        if (obj is ITick tick)
        {
            Ticks.Remove(tick);
            TickCount = Ticks.Count;
        }
        if (obj is IFixedTick fixedTick)
        {
            FixedTicks.Remove(fixedTick);
            FixedTickCount = FixedTicks.Count;
        }
        if (obj is ILateTick lateTick)
        {
            LateTicks.Remove(lateTick);
            LateTickCount = LateTicks.Count;
        }
    }
}

public interface ITick{public void Tick();}
public interface IFixedTick{public void FixedTick();}
public interface ILateTick{public void LateTick();}
using System;
using System.Collections.Generic;
using Script.Characters;
using UnityEngine;

public interface IGameEvent { }

public class PlayerHPChanged : IGameEvent 
    {
        public int HP, MaxHP;
    }

public  class PlayerEXPAdded : IGameEvent
{
    public int amount;
}
public class HitPlayer: IGameEvent
{
    public EnemyData enemyData;
}

public class PlayerDeath : IGameEvent { }

public class NewGameStart : IGameEvent { }


public class RoomEnterEvent : IGameEvent
{
    public RoomData roomData;
}

public class RoomClearedEvent : IGameEvent
{
    public RoomEventProcessor sender;
}

public class RoomEnemyDeadEvent : IGameEvent
{
    public RoomEventProcessor sender;
    public EnemyBase enemy;
}

public static class GameEventBus
{
    private static readonly Dictionary<Type, List<Action<IGameEvent>>> _handlers = new();

    public static void Subscribe<T>(Action<T> handler) where T : IGameEvent
    {
        Type type = typeof(T);
        if (!_handlers.ContainsKey(type)) _handlers[type] = new List<Action<IGameEvent>>();

        _handlers[type].Add(e => handler((T)e));
    }

    public static void Publish<T>(T eventData) where T : IGameEvent
    {
        Type type = typeof(T);
        if (_handlers.TryGetValue(type, out var handlers))
        {
            foreach (var handler in handlers)
            {
                handler.Invoke(eventData);
            }
        }
    }

    public static void RemoveAllSubscribes()
    {
        _handlers.Clear();
    }
}

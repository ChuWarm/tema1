using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class GamePlayLogic 
{
    public PlayerController m_Player;
    
    int playerHP;
    int playerEXP;
    public GamePlayLogic(PlayerController player)
    {
        GameEventBus.Subscribe<PlayerHPChanged>(OnPlayerHPChanged);
        GameEventBus.Subscribe<RoomEnemyDeadEvent>(OnEnemyDeadEvent);
        GameEventBus.Subscribe<HitPlayer>(OnHitPlayer);
        // GameEventBus.Subscribe<RoomClearedEvent>()
    }

    void OnPlayerHPChanged(PlayerHPChanged e)
    {
        playerHP = e.HP;

        if (playerHP <= 0)
        {
            m_Player = null;
            GameEventBus.Publish<PlayerDeath>(new PlayerDeath());
        }
    }

  void OnEnemyDeadEvent(RoomEnemyDeadEvent e)
    {
        // enemy ���� ȿ�� ��� ?
        // �̰� ���⼭ ȣ���ϴ°� �³�?

        playerEXP += e.enemy.GetEnemyData.experienceGiven;
    }

    void OnHitPlayer(HitPlayer e)
    {
        int finalDamage = e.enemyData.attackPower - PlayerManager.Instance.resistance;

        GameEventBus.Publish<PlayerHPChanged>(new PlayerHPChanged
        {
            HP = playerHP - finalDamage,
        });
    }
}
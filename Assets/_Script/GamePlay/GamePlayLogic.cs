using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GamePlayLogic 
{
    public PlayerController m_Player;
    
    int playerHP;
    int playerEXP;
    List<EnemyBase> enemyBases;

    public GamePlayLogic(PlayerController player)
    {
        GameEventBus.Subscribe<PlayerHPChanged>(OnPlayerHPChanged);
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
}




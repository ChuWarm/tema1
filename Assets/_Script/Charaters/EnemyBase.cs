using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class EnemyFactory
{
    // Vector3 position 매개변수 추가
    public static EnemyBase SpawnEnemy(RoomEventHolder room, EnemyData enemyData, Vector3 position)
    {
        if (room.IsUnityNull()) return null;

        var basePrefab = Resources.Load<GameObject>("EnemyBase");

        // 추가된 코드
        var instance = MonoBehaviour.Instantiate(basePrefab, position, Quaternion.identity, room.transform);

        if (instance.TryGetComponent<EnemyBase>(out var enemyBase))
        {
            var enemy = enemyBase.Init(enemyData);
            room.RegisterEnemy(enemy);
            return enemy;
        }
        
        return null;
        //
        
        // MonoBehaviour.Instantiate(basePrefab, room.transform);
        //
        // if (!basePrefab.TryGetComponent<EnemyBase>(out var enemyBase))
        //     return null;
        //
        // return enemyBase.Init(enemyData);
    }
}

public class EnemyBase : MonoBehaviour
{
    [SerializeField] Transform m_visualHolder;
    [SerializeField] private EnemyData m_enemyData;
    public int health;
    float lastAttack;

    public EnemyBase Init(EnemyData data)
    {
        // 임시
        m_enemyData = data;
        //
        gameObject.name = data.enemyName;
        health = data.health;
        lastAttack = 0;

        /*
        var visual = Resources.Load<Transform>($"{data.visualResourceID}");

        visual.SetParent(visualHolder);
        visual.transform.localPosition = Vector3.zero;
        */

        return this;    
    }

    private void Update()
    {
        var player = GamePlayManager.Instance.gamePlayLogic.m_Player;

        if(Vector3.Distance(transform.position, player.transform.position) <= m_enemyData.attackRange)
        {
            if (Time.time >= lastAttack + m_enemyData.attackCooldown)
                Attack();
        }
        else
        {
            // 임시
            transform.position = 
                Vector3.MoveTowards(transform.position,
                    player.transform.position, 
                    m_enemyData.moveSpeed * Time.deltaTime);
            //
            
            // transform.Translate(player.transform.position);
        }
    }

    public void TakeDamage(int damage)
    {
        damage -= m_enemyData.resistance;
        
        health -= damage;

        if(health <= 0)
        {
            Die();
        }
    }

    void Attack()
    {
        lastAttack = Time.time;
    }

    void Die()
    {
        // 추가된 코드
        RoomEventHolder holder = GetComponent<RoomEventHolder>();
        holder?.OnEnemyDead(this);
        //
        
        GameEventBus.Publish<PlayerEXPAdded>(new PlayerEXPAdded
        {
            amount = m_enemyData.experienceGiven
        });
        
        // 추가된 코드
        Destroy(gameObject);
    }
}

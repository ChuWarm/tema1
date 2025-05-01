using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class EnemyFactory
{
    public static EnemyBase SpawnEnemy(RoomEventHolder room, EnemyData enemyData)
    {
        if (room.IsUnityNull()) return null;

        var basePrefab = Resources.Load<GameObject>("EnemyBase");

        MonoBehaviour.Instantiate(basePrefab, room.transform);

        if (!basePrefab.TryGetComponent<EnemyBase>(out var enemyBase))
            return null;

        return enemyBase.Init(enemyData);

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
            transform.Translate(player.transform.position);
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
        // 공격
        // 아마 오브젝트 발사

        lastAttack = Time.time;
    }

    void Die()
    {
        GameEventBus.Publish<PlayerEXPAdded>(new PlayerEXPAdded
        {
            amount = m_enemyData.experienceGiven
        });
    }
}





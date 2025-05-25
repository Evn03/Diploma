using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTowerBehaviour : MonoBehaviour
{
    private Tower tower;
    private float cooldownTimer = 0f;
    public GameObject bulletPrefab;

    void Start()
    {
        tower = GetComponent<Tower>();
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            GameObject target = FindTarget();
            if (target != null)
            {
                Shoot(target);
                cooldownTimer = tower.attackCooldown;
            }
        }
    }

    GameObject FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, tower.attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                return hit.gameObject;
        }
        return null;
    }

void Shoot(GameObject enemy)
{
    if (bulletPrefab != null)
    {
        GameObject bulletGO = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        bullet.Initialize(enemy.transform, 1f);
    }
}

    void OnDrawGizmosSelected()
    {
        if (tower != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, tower.attackRange);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    private float damage;
    private Transform target;

    public void Initialize(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            EnemyMover e = target.GetComponent<EnemyMover>();
            if (e != null)
            {
                e.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}

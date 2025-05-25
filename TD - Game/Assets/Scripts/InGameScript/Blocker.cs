using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour, IDamageable
{
    public float health = 3f;

    public bool IsDestroyed => health <= 0;

    public bool isExit = false;

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
        {
            if (isExit)
            {
                EndGameUI.Instance.ShowDefeat();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}

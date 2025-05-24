using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private List<EnemyMover> enemies = new List<EnemyMover>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Register(EnemyMover enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void Unregister(EnemyMover enemy)
    {
        enemies.Remove(enemy);
    }

    public void NotifyPathChanged()
    {
        foreach (var enemy in enemies)
        {
            enemy.RecalculatePath();
        }
    }
}
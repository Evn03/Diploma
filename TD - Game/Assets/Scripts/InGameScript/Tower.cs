using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public string towerName;
    public int cost;
    public TowerType type;

    public float attackRange = 3f;   
    public float attackCooldown = 1f; 

    public enum TowerType
    {
        Attack,
        Gold
    }
}



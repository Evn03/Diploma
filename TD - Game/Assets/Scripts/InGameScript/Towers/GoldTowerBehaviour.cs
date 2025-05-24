using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldTowerBehaviour : MonoBehaviour
{
    private Tower tower;
    public float interval = 3f;
    private float timer = 0f;

    void Start()
    {
        tower = GetComponent<Tower>();
        timer = interval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            GameManager.Instance.AddGold(5); // фиксированное золото
            timer = interval;
        }
    }
}


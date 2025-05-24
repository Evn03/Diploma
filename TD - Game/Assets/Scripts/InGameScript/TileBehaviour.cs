using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public int x;
    public int y;

    public void Init(int xCoord, int yCoord)
    {
        x = xCoord;
        y = yCoord;
    }

    private void OnMouseDown()
    {
        TowerPlacer.Instance.TryPlaceTower(x, y);
    }
}




using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public int x;
    public int y;

    private SpriteRenderer sr;
    private Color originalColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    public void Init(int xCoord, int yCoord)
    {
        x = xCoord;
        y = yCoord;
    }

    public void Highlight(Color color)
    {
        if (sr != null)
            sr.color = color;
    }

    public void ResetHighlight()
    {
        if (sr != null)
            sr.color = originalColor;
    }

    private void OnMouseDown()
    {
        TowerPlacer.Instance.TryPlaceTower(x, y);
    }
}




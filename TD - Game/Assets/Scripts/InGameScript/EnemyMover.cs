using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public float speed = 2f;
    public float health = 10f;
    public float attackDamage = 1f;
    public float attackInterval = 1f;

    private float attackTimer = 0f;
    private IDamageable currentTarget;
    private List<TileData> path;
    private int currentIndex = 0;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public System.Action OnDeath;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetMoving(true);
    }

    private void OnEnable()
    {
        EnemyManager.Instance?.Register(this);
    }

    private void OnDisable()
    {
        EnemyManager.Instance?.Unregister(this);
    }
    public void SetPath(List<TileData> path)
    {
        this.path = path;
        currentIndex = 0;
    }

    public void RecalculatePath()
    {
        int gridX = Mathf.RoundToInt(transform.position.x / MapParser.Instance.scale);
        int gridY = Mathf.RoundToInt(-transform.position.y / MapParser.Instance.scale);
        TileData currentTile = MapParser.Instance.GetTileAt(gridX, gridY);

        if (currentTile == null)
        {
            Debug.LogWarning($"EnemyMover.RecalculatePath: текущий тайл по позиции ({gridX}, {gridY}) не найден!");
            return;
        }
        Pathfinding pathfinder = new Pathfinding(MapParser.Instance.GetGrid(), MapParser.Instance.Width, MapParser.Instance.Height);
        List<TileData> newPath = pathfinder.FindPath(currentTile, MapParser.Instance.exitTile);
        SetPath(newPath);
    }

    private void Update()
    {
        if (path == null || currentIndex >= path.Count)
        {
            SetMoving(false);
            return;
        }

        TileData currentTile = path[currentIndex];
        Vector3 target = currentTile.worldPosition;

        if (currentTarget == null)
        {
            Collider2D hit = Physics2D.OverlapCircle(target, 0.1f);
            if (hit != null)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null && !damageable.IsDestroyed)
                {
                    currentTarget = damageable;
                    SetMoving(false);
                    attackTimer = attackInterval;
                    return;
                }
            }
        }

        if (currentTarget != null)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                currentTarget.TakeDamage(attackDamage);
                attackTimer = attackInterval;

                if (currentTarget.IsDestroyed)
                {
                    currentTarget = null;
                    SetMoving(true);
                }
            }
            return;
        }

        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = currentTile.y * 10 + 5;

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            currentIndex++;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    private void SetMoving(bool moving)
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", moving);
        }
    }
}

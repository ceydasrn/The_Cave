using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMove : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float patrolDistance = 4f;
    public float attackRange = 1.5f;
    public Transform playerTransform;
    private Rigidbody2D enemyRB;
    private Vector3 leftPatrolPos;
    private Vector3 rightPatrolPos;
    private bool movingRight = true;
    private bool isChasingPlayer = false;
    private bool shouldRetreat = false; // Geri çekilme kontrolü
    private bool isRetreating = false; // Geri çekilme durumu
    public float detectionRange = 5f;
    public float retreatDistance = 0.5f; // Geri çekilme mesafesi

    void Start()
    {
        enemyRB = GetComponent<Rigidbody2D>();
        enemyRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        leftPatrolPos = transform.position - new Vector3(patrolDistance / 2f, 0f, 0f);
        rightPatrolPos = transform.position + new Vector3(patrolDistance / 2f, 0f, 0f);
    }

    void Update()
    {
        if (!isChasingPlayer)
        {
            Patrolling();
        }
        else
        {
            ChasePlayer();
        }
        Attack();
    }

    void Patrolling()
    {
        if (movingRight && transform.position.x >= rightPatrolPos.x)
        {
            movingRight = false;
        }
        else if (!movingRight && transform.position.x <= leftPatrolPos.x)
        {
            movingRight = true;
        }

        float moveDirection = movingRight ? 1f : -1f;
        enemyRB.velocity = new Vector2(moveSpeed * moveDirection, enemyRB.velocity.y);
    }

    void ChasePlayer()
    {
        if (playerTransform == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > detectionRange)
        {
            isChasingPlayer = false;
        }
        else
        {
            float moveDirection = playerTransform.position.x > transform.position.x ? 1f : -1f;
            enemyRB.velocity = new Vector2(moveSpeed * moveDirection, enemyRB.velocity.y);

            // Geri çekilme kontrolü
            if (shouldRetreat)
            {
                shouldRetreat = false;
                isRetreating = true;
                Invoke("StopRetreat", 0.5f); // 0.5 saniye sonra geri çekilmeyi durdur
            }
        }
    }

    void Attack()
    {
        if (playerTransform == null)
            return;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isChasingPlayer = true;

            // Temas ettiğinde geri çekilme başlat
            shouldRetreat = true;
        }
    }

    void StopRetreat()
    {
        isRetreating = false;
    }

    void FixedUpdate()
    {
        // Geri çekilme durumunda
        if (isRetreating)
        {
            // Geri çekilme mesafesine göre hareket et
            float retreatDirection = transform.position.x < playerTransform.position.x ? -1f : 1f;
            enemyRB.velocity = new Vector2(moveSpeed * retreatDirection, enemyRB.velocity.y);
        }
    }
}
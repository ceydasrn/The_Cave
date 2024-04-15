using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBarVisibility : MonoBehaviour
{
    public GameObject healthBar; // Sağlık çubuğunu içeren oyun nesnesi
    public Transform player; // Oyuncunun konumunu içeren transform
    public float maxDistanceToShow = 5f; // Oyuncunun ve Düşmanın maksimum izin verilen mesafesi

    private void Start()
    {
        healthBar.SetActive(false); // Başlangıçta sağlık çubuğunu gizle
    }

    private void Update()
    {
        if (player != null)
        {
            // Düşman ve oyuncu arasındaki mesafeyi kontrol et
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Eğer mesafe izin verilen maksimum mesafeden küçük veya eşitse sağlık çubuğunu göster
            if (distanceToPlayer <= maxDistanceToShow)
            {
                healthBar.SetActive(true);
            }
            else
            {
                healthBar.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Eğer çarpışan nesne "Player" ise
        {
            healthBar.SetActive(true); // Sağlık çubuğunu göster
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Eğer "Player" nesnesi ile temas kesildiyse
        {
            healthBar.SetActive(false); // Sağlık çubuğunu gizle
        }
    }

    // Düşmanın canının sıfıra indiğinde tetiklenecek olan fonksiyon
    public void OnEnemyDeath()
    {
        healthBar.SetActive(false); // Sağlık çubuğunu gizle
    }
}

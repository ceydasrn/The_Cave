using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    public int damage;
    private bool colliderBusy = false;
    private bool befAttack;
    private bool speAttack;
    public bool isDead;
    private Animator animator;
    private float timer;
    private GameObject player;

    public Transform bullet; // Mermi prefabı
    public Transform muzzle; // Mermi başlangıç noktası
    public float bulletSpeed; // Mermi hızı
    public GameObject panelToOpen; // Panel değişkeni

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);

        // Düşman oyuncuya belirli bir mesafede mi?
        if(distance < 4)
        {
            timer += Time.deltaTime;

            // Belirli bir aralıkta mermi atma zamanı mı?
            if(timer >= 2 && currentHealth <= 50) // Eğer 2 saniyelik bir aralıkta mermi atılacaksa ve can %50'nin altındaysa
            {
                timer = 0; // Zamanlayıcıyı sıfırla
                ShootBullet(); // Mermi at
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !colliderBusy)
        {
            colliderBusy = true;
            collision.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
        else if (collision.CompareTag("Bullet"))
        {
            TakeDamage(Mathf.RoundToInt(collision.GetComponent<BulletManager>().bulletDamage));
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            colliderBusy = false;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Anger();
        }
    }

    public void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        // Düşmanın takip etmesini durdurmak için hareket betiğini devre dışı bırakın
        // Örneğin, bir NavMeshAgent kullanıyorsanız:
        // navMeshAgent.enabled = false;

        StartCoroutine(OpenPanelAfterDelay(3f));
    }

    IEnumerator OpenPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Paneli aç
        if (panelToOpen != null)
        {
            panelToOpen.SetActive(true);
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }

    IEnumerator Dying(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Hareket betiğini yeniden etkinleştirin
        // Örneğin, bir NavMeshAgent kullanıyorsanız:
        // navMeshAgent.enabled = true;

        // Oyun nesnesini yok edin
        Destroy(gameObject);
    }


    void Anger()
    {
        if (currentHealth <= 50 && !befAttack && !speAttack)
        {
            befAttack = true;
            // "BeforeAtt" animasyonunu başlat
            animator.SetTrigger("BeforeAtt");
            StartCoroutine(StartSpecialAttackAfterDelay(1f));
        }
        else if (currentHealth > 50 && speAttack)
        {
            // Can %50'nin üstündeyken SpeAtt'ı durdur
            speAttack = false;
            animator.ResetTrigger("SpeAtt");
        }
    }

    IEnumerator StartSpecialAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        speAttack = true;
        // "SkeletonSpeAttack" animasyonunu başlat
        animator.SetTrigger("SpeAtt");

        // Mermi at
        while (speAttack) // speAttack true olduğu sürece döngüye gir
        {
            yield return new WaitForSeconds(1f); // Her bir döngüde 1 saniye bekle
            ShootBullet(); // Mermi at
        }
    }


    void ShootBullet()
    {
        Instantiate(bullet, muzzle.position, Quaternion.identity);
    }

}

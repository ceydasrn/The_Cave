using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private PlayerMove playerMove;
    public bool isDead = false;

    private float timeSinceLastDamage = 0f;
    private float timeBetweenDamage = 1.5f; // 0.5 saniye aralıklarla hasar alacak
    private int damagePerInterval = 1; // Her hasar alınma aralığında alınacak hasar miktarı

    public Button healButton; // Canı artırmak için buton

    public GameObject dieEffect;
    public Transform rebornEffectSpawnPoint;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        // Başlangıç konumunu ve rotasyonunu kaydet
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        playerMove = GetComponent<PlayerMove>();

        // Butonun tıklanma olayına fonksiyonu ekle
        healButton.onClick.AddListener(Heal);
    }

    void Update()
    {
        timeSinceLastDamage += Time.deltaTime;

        if (timeSinceLastDamage >= timeBetweenDamage)
        {
            TakeDamage(damagePerInterval);
            timeSinceLastDamage = 0f;
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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Thorn"))
        {
            // Eğer çarpışan nesnenin etiketi "Thorn" ise, oyuncu ölür
            Die();
        }
    }

    void RespawnCrystals()
    {
        // GameManager nesnesini bul
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            // GameManager nesnesine bağlı GameManager script'ini kullanarak kristalleri yeniden oluştur
            gameManager.ResetCrystals();
        }
    }

    void Die()
    {
        Instantiate(dieEffect, transform.position, Quaternion.identity);
        isDead = true;
        playerMove.StopMovementForDuration(1f);
        // Coroutine kullanarak sıfırlama işlemini 2 saniye sonra gerçekleştir
        StartCoroutine(ResetAfterDelay());

        if (isDead)
        {
            var enemyHealth = FindObjectOfType<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.ResetHealth();
            }
        }
    }

    IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        
        // Oyuncu öldüğünde başlangıç konumuna ve rotasyonuna geri dön
        transform.position = startingPosition;
        transform.rotation = startingRotation;
        
        // Sabit pozisyondan efekti oluştur
        Instantiate(rebornEffectSpawnPoint, rebornEffectSpawnPoint.position, Quaternion.identity);

        playerMove.StopMovementForDuration(1f);

        // Canı yeniden doldur
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);

        // Kristalleri yeniden oluştur
        RespawnCrystals();

        // CrystalCounter'ı bul
        CrystalCounter crystalCounter = FindObjectOfType<CrystalCounter>();

        // Eğer bulunduysa, currentCrystals değerini sıfırla ve ekrandaki metni güncelle
        if (crystalCounter != null)
        {
            crystalCounter.currentCrystals = 0;
            crystalCounter.crystalText.text = ": " + crystalCounter.currentCrystals.ToString();
            crystalCounter.marketCrystalText.text = "Crystal: " + crystalCounter.currentCrystals.ToString();

            // Potion stoklarını sıfırla
            FindObjectOfType<ShopManager>().ResetPotionStocks();
        }
    }

    // Butona tıklanınca çağrılacak fonksiyon
    public void Heal()
    {
        // Canı artır
        currentHealth += 20;
        // Ancak maksimum canı geçmemesi için kontrol yap
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        // Can çubuğunu güncelle
        healthBar.SetHealth(currentHealth);
    }
}

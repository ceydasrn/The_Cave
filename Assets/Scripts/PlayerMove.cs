using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;
    Animator playerAnimator;

    public float moveSpeed = 5f; // Karakterin ileri hızı
    public float jumpSpeed = 1f, jumpFrequency = 1f, nextjumpTime;
    
    bool facingRight = true;
    public bool isGrounded = false;

    public Transform groundCheckPosition;
    public float groundCheckRadius;
    public LayerMask groundCheckLayer;

    // Yeni eklenen buton değişkeni
    public Button speedButton;

    // Orijinal hareket hızını saklayacak değişken
    private float originalMoveSpeed;

    // Butona son tıklama zamanını saklayacak değişken
    private float lastClickTime;

    // Butona tıklama aralığını belirleyecek değişken
    public float clickInterval = 5f;

    // Butona tıklanabileceğini belirten bayrak
    private bool canClick = true;

    // Yeni eklenen ölüm kontrolü
    private bool isDead = false;

    Transform muzzle;

    public Transform bullet;

    public float bulletSpeed;

    [SerializeField] private AudioSource collectionSoundEffect;

    void Start()
    {
        muzzle = transform.GetChild(4);

        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();

        // Butonun tıklama olayına metodu bağlama
        speedButton.onClick.AddListener(IncreaseMoveSpeed);

        // Orijinal hareket hızını kaydetme
        originalMoveSpeed = moveSpeed;
    }

    void Update()
    {
        // Eğer ölüm durumunda ise, hareketi kontrol etme
        if (!isDead)
        {
            HorizontalMove();

            OnGroundCheck();
            if (rb.velocity.x < 0 && facingRight)
            {
                FlipFace();
            }
            else if (rb.velocity.x > 0 && !facingRight)
            {
                FlipFace();
            }

            if (Input.GetAxis("Vertical") > 0 && isGrounded && (nextjumpTime < Time.timeSinceLevelLoad))
            {
                nextjumpTime = Time.timeSinceLevelLoad + jumpFrequency;
                Jump();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootBullet();
        }
    }

    void HorizontalMove()
    {
        if (!isDead) // Ölü değilse hareket et
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, rb.velocity.y);
        playerAnimator.SetFloat("playerWalkSpeed", Mathf.Abs(rb.velocity.x));
    }

    void FlipFace()
    {
        facingRight = !facingRight;
        Vector3 tempLocalScale = transform.localScale;
        tempLocalScale.x *= -1;
        transform.localScale = tempLocalScale;
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0f, jumpSpeed));
    }

    void OnGroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPosition.position, groundCheckRadius, groundCheckLayer);
        playerAnimator.SetBool("isGroundedAnim", isGrounded);
    }

    // Buton tıklandığında çağrılacak fonksiyon
    void IncreaseMoveSpeed()
    {
        // Butona tıklama izni varsa ve belirli bir süre geçtiyse devam et
        if (canClick && (Time.timeSinceLevelLoad - lastClickTime >= clickInterval))
        {
            // Buton tıklama iznini kapat
            canClick = false;

            // Hareket hızını arttırma
            moveSpeed = moveSpeed * 1.5f; // Örneğin, mevcut hızı iki katına çıkartıyoruz.
            jumpSpeed = jumpSpeed * 1.2f;

            // Son tıklama zamanını güncelle
            lastClickTime = Time.timeSinceLevelLoad;

            // Belirli bir süre sonra hareket hızını eski haline getirme
            StartCoroutine(ResetMoveSpeedAfterDelay(5f)); // 5 saniye sonra eski hıza dönecek
        }
    }

    // Belirli bir süre sonra hareket hızını eski haline getirecek yardımcı fonksiyon
    IEnumerator ResetMoveSpeedAfterDelay(float delay)
    {
        // Belirli bir süre bekle
        yield return new WaitForSeconds(delay);

        // Orijinal hareket hızına geri dönme
        moveSpeed = originalMoveSpeed;

        // Buton tıklama iznini tekrar aç
        canClick = true;
    }

    // PlayerHealth scriptinden çağrılacak fonksiyon
    public void StopMovementForDuration(float duration)
    {
        StartCoroutine(StopMovement(duration));
    }

    IEnumerator StopMovement(float duration)
    {
        // Belirli bir süre hareket etmeyi durdur
        moveSpeed = 0f;
        yield return new WaitForSeconds(duration);
        // Belirli bir süre sonra hareketi tekrar başlat
        moveSpeed = originalMoveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
   {
          if(other.CompareTag("Crystal"))
          {
               collectionSoundEffect.Play();
          }
   }

   void ShootBullet()
    {
        Transform tempBullet;
        tempBullet = Instantiate(bullet, muzzle.position, Quaternion.identity);

        if(!facingRight)
        {
            tempBullet.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
        }
        else
        {
            tempBullet.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        tempBullet.GetComponent<Rigidbody2D>().AddForce(muzzle.forward * bulletSpeed);
    }
}

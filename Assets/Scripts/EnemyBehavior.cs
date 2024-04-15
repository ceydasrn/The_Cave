using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    #region Public Variables
    public float attackDistance; //atak için minimum mesafe
    public float moveSpeed;
    public float timer; //ataklar arası cooldown için zamanlayıcı
    public Transform leftLimit;
    public Transform rightLimit;
    [HideInInspector] public Transform target;
    [HideInInspector] public bool inRange; //player görüş alanında mı kontrolü
    public GameObject hotZone;
    public GameObject triggerArea;
    public EnemyHealth enemyHealth;
    #endregion

    #region Private Variables
    private Animator anim;
    private float distance; //enemy ve player arasındaki mesafeyi sakla
    private bool attackMode;
    private bool cooling; //enemy ataktan sonra cooldownda mı kontrolü
    private float intTimer;
    #endregion
    
     void Awake()
    {
        SelectTarget();
        intTimer = timer; //zamanlayıcının başlangıç değerini sakla
        anim = GetComponent<Animator>();
    }

    void Update ()
    {
        if(!attackMode)
        {
            Move();
        }

        if(!InsideofLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("SkeletonAttack"))
        {
            SelectTarget();
        }

        if(inRange)
        {
            EnemyLogic();
        }

        // Enemy'nin her zaman player'a dönük olmasını sağla
        Flip();
    }


    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.position);

        if(distance > attackDistance)
        {
            StopAttack();
        }
        else if(attackDistance >= distance && cooling == false)
        {
            Attack();
        }

        if (cooling)
        {
            Cooldown();
            anim.SetBool("Attack", false);
        }
    }

    public void Move()
    {
        anim.SetBool("canWalk", true);

        // Eğer currentHealth %50 ve altındaysa ve SkeletonAttack animasyonu oynatılmıyorsa
        if (!enemyHealth.isDead && enemyHealth.currentHealth > 50 && !anim.GetCurrentAnimatorStateInfo(0).IsName("SkeletonAttack"))
        {
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }


    void Attack()
    {
        timer = intTimer; //player, saldırı görüş alanına girdiğinde zamanlayıcıyı sıfırla
        attackMode = true; //enemy'nin hâlâ saldırıp saldıramadığını görmek için

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
    }

    void Cooldown()
    {
        timer -= Time.deltaTime;

        if(timer <= 0 && cooling && attackMode)
        {
            cooling = false;
            timer = intTimer;
        }
    }

    void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
    }

    public void TriggerCooling()
    {
        cooling = true;
    }

    private bool InsideofLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }

    public void SelectTarget()
    {
        float distanceToLeft = Vector3.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector3.Distance(transform.position, rightLimit.position);

        if(distanceToLeft > distanceToRight)
        {
            target = leftLimit;
        }
        else
        {
            target = rightLimit;
        }

        Flip();
    }

    public void Flip()
    {
        Vector3 rotation = transform.eulerAngles;
        if(transform.position.x > target.position.x)
        {
            rotation.y = 0f;
        }
        else
        {
            rotation.y = 180f;
        }

        transform.eulerAngles = rotation;
    }
}

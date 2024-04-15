using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public float health;
    public int damage;
    bool colliderBusy = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.tag == "Player" && !colliderBusy)
        {
            colliderBusy = true;
            collision.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
        else if(collision.tag == "Bullet")
        {
            GetDamage(collision.GetComponent<BulletManager>().bulletDamage);
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            colliderBusy = false;
        }
    }

    public void GetDamage(float damage)
    {
        if ((health - damage) >= 0)
        {
            health -= damage;
        }
        else
        {
            health = 0;
        }
        AmIDead();

    }

    void AmIDead()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}

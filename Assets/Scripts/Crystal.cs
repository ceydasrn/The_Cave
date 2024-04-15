using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public int value;
    private Vector3 startPosition; // Kristalın başlangıç pozisyonunu saklamak için

    void Start()
    {
        // Kristalın başlangıç pozisyonunu kaydet
        startPosition = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            CrystalCounter.instance.IncreaseCrystals(value);
        }
    }

    // Kristalın başlangıç pozisyonuna geri dönmesini sağlar
    public void ResetPosition()
    {
        transform.position = startPosition;
    }
}

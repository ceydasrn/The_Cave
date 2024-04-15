using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager nesnesine kolayca erişmek için

    public GameObject crystalPrefab; // Kristallerin prefab'ı
    private Vector3[] initialCrystalPositions; // Kristallerin başlangıç pozisyonlarını saklamak için dizi

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // Kristallerin başlangıç pozisyonlarını kaydet
        GameObject[] crystals = GameObject.FindGameObjectsWithTag("Crystal");
        initialCrystalPositions = new Vector3[crystals.Length];
        for (int i = 0; i < crystals.Length; i++)
        {
            initialCrystalPositions[i] = crystals[i].transform.position;
        }
    }

    // Oyuncu öldüğünde veya sahne yeniden yüklendiğinde tüm kristalleri başlangıç pozisyonlarına göre yeniden oluşturur
    public void ResetCrystals()
    {
        // Kristalleri önce temizle
        DestroyAllCrystals();

        // Sonra başlangıç pozisyonlarına göre yeniden oluştur
        foreach (Vector3 position in initialCrystalPositions)
        {
            Instantiate(crystalPrefab, position, Quaternion.identity);
        }
    }

    // Sahnedeki tüm kristalleri yok eder
    private void DestroyAllCrystals()
    {
        GameObject[] crystals = GameObject.FindGameObjectsWithTag("Crystal");
        foreach (GameObject crystal in crystals)
        {
            Destroy(crystal);
        }
    }
}

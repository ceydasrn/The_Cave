using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CrystalCounter : MonoBehaviour
{
    public static CrystalCounter instance;

    public TMP_Text crystalText;
    public TMP_Text marketCrystalText; // Market ekranında gösterilecek kristal sayısı metin bileşeni

    public int currentCrystals = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        crystalText.text = ": " + currentCrystals.ToString();    
        marketCrystalText.text = "Crystals: " + currentCrystals.ToString(); // Market ekranındaki metni başlangıçta güncelle
    }

    public void IncreaseCrystals(int v)
    {
        currentCrystals += v;
        crystalText.text = ": " + currentCrystals.ToString();
        marketCrystalText.text = "Crystals: " + currentCrystals.ToString(); // Kristal sayısı değiştiğinde market ekranındaki metni güncelle
    }
}

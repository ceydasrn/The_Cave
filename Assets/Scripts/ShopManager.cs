using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public int[,] shopItems = new int[4, 4];
    public TMP_Text shopCrystalsTXT;
    public TMP_Text mainCrystalText;
    public TMP_Text[] potionTexts; // Potion textlerini tutacak dizi
    public GameObject[] potionPanels; // Potion panellerini tutacak dizi
    public TMP_Text[] potionQuantityTexts; // Potion miktarlarını gösterecek textler
    public Button healthPotBut; // Belirli buton değişkeni
    public Button speedPotBut;
    public GameObject shopPanel;
    public Text buttonText; // Butonun textini tutacak değişken
    private CrystalCounter crystalCounter; // CrystalCounter script'ine referans

    // Speed Pot butonuna basıldığı zaman geçen süreyi saklayacak değişken
    private float lastSpeedButtonPressTime;

    // Speed Pot butonunun cooldown süresi (saniye cinsinden)
    public float speedButtonCooldown = 5f;

    private bool isShopPanelActive = false;

    // Start is called before the first frame update
    void Start()
    {
        shopPanel.SetActive(false);
        // CrystalCounter script'inden referansı al
        crystalCounter = FindObjectOfType<CrystalCounter>();

        // Başlangıçta metni güncelle
        shopCrystalsTXT.text = "Crystals: " + crystalCounter.currentCrystals.ToString();

        //ID
        shopItems[1, 1] = 1;
        shopItems[1, 2] = 2;

        //Price
        shopItems[2, 1] = 5;
        shopItems[2, 2] = 5;

        //Quantity
        shopItems[3, 1] = 0;
        shopItems[3, 2] = 0;

        // Health Pot Button'a tıklanınca çağrılacak fonksiyonu butona atama
        healthPotBut.onClick.AddListener(ReduceHealthPotionCount);

        // Speed Pot Button'a tıklanınca çağrılacak fonksiyonu butona atama
        speedPotBut.onClick.AddListener(ReduceSpeedPotionCount);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
    {
        // Shop paneli aktifse pasif yap, pasifse aktif yap
        shopPanel.SetActive(!shopPanel.activeSelf);

        // Shop paneli aktifse oyunu duraklat
        if (shopPanel.activeSelf)
        {
            Time.timeScale = 0f;
            isShopPanelActive = true;
        }
        else // Shop paneli pasifse oyunu devam ettir
        {
            Time.timeScale = 1f;
            isShopPanelActive = false;
        }
    }
    }
    
    // Health Pot Button'a tıklanınca çağrılacak fonksiyon
    public void ReduceHealthPotionCount()
    {
        if (shopItems[3, 1] > 0)
        {
            shopItems[3, 1]--;
            potionQuantityTexts[0].text = "x " + shopItems[3, 1].ToString();
            potionTexts[0].text = ": " + shopItems[3, 1].ToString();

            if (shopItems[3, 1] == 0)
            {
                potionPanels[0].SetActive(false);
            }
        }
    }

    // Speed Pot Button'a tıklanınca çağrılacak fonksiyon
    public void ReduceSpeedPotionCount()
    {
        // Speed Pot butonunun tıklanma zamanını kontrol et
        if (Time.time - lastSpeedButtonPressTime >= speedButtonCooldown)
        {
            // Eğer cooldown süresi geçmişse işlem yap
            if (shopItems[3, 2] > 0)
            {
                shopItems[3, 2]--;
                potionQuantityTexts[1].text = "x " + shopItems[3, 2].ToString();
                potionTexts[1].text = ": " + shopItems[3, 2].ToString();

                if (shopItems[3, 2] == 0)
                {
                    potionPanels[1].SetActive(false);
                }
            }

            // Speed Pot butonuna basıldığı zamanı kaydet
            lastSpeedButtonPressTime = Time.time;

            // Butonun textini güncelle
            StartCoroutine(CountdownAndChangeText());
        }
    }

    // Butonun textini güncelleyecek coroutine
    IEnumerator CountdownAndChangeText()
    {
        buttonText.text = "5"; // Başlangıç değeri

        for (int i = 4; i > 0; i--)
        {
            yield return new WaitForSeconds(1f); // 1 saniye bekle
            buttonText.text = i.ToString(); // Geri sayımı güncelle
        }

        buttonText.text = "USE"; // Geri sayım tamamlandığında "USE" yaz
    }


    public void Buy()
    {
        GameObject ButtonRef = EventSystem.current.currentSelectedGameObject;

        // CrystalCounter script'inden kristal sayısını alarak kullan
        if (crystalCounter.currentCrystals >= shopItems[2, ButtonRef.GetComponent<ButtonInfo>().ItemID])
        {
            crystalCounter.currentCrystals -= shopItems[2, ButtonRef.GetComponent<ButtonInfo>().ItemID];
            shopItems[3, ButtonRef.GetComponent<ButtonInfo>().ItemID]++;
            shopCrystalsTXT.text = "Crystals: " + crystalCounter.currentCrystals.ToString();
            mainCrystalText.text = ": " + crystalCounter.currentCrystals.ToString();
            ButtonRef.GetComponent<ButtonInfo>().QuantityTxt.text = shopItems[3, ButtonRef.GetComponent<ButtonInfo>().ItemID].ToString();

            // Satın alınan potionun textini güncelle
            int potionID = ButtonRef.GetComponent<ButtonInfo>().ItemID;
            potionTexts[potionID - 1].text = ": " + shopItems[3, potionID].ToString();

            // Satın alınan potionun panelini etkinleştir
            EnablePotionPanel(potionID);

            // Satın alınan potionun miktarını gösteren metni güncelle
            potionQuantityTexts[potionID - 1].text = "x " + shopItems[3, potionID].ToString();
        }
    }

    void UpdatePotionTexts()
    {
        for (int i = 0; i < potionTexts.Length; i++)
        {
            potionTexts[i].text = ": " + shopItems[3, i + 1].ToString();
        }
    }

    void EnablePotionPanel(int potionID)
    {
        // Doğru panelin indeksini bulmak için potionID'yi kullan
        int panelIndex = potionID - 1;

        // Eğer belirtilen potion ID'sine sahip panel varsa, etkinleştir
        if (panelIndex >= 0 && panelIndex < potionPanels.Length)
        {
            potionPanels[panelIndex].SetActive(true);

            // Eğer açılan panelin üstünde başka bir panel varsa, açılan panelin üstüne yerleştir
            if (panelIndex > 0)
            {
                RectTransform rectTransform = potionPanels[panelIndex].GetComponent<RectTransform>();
                RectTransform upperRectTransform = potionPanels[panelIndex - 1].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(upperRectTransform.anchoredPosition.x, upperRectTransform.anchoredPosition.y + upperRectTransform.rect.height);
            }
        }
    }

    public void ResetPotionStocks()
    {
        for (int i = 0; i < shopItems.GetLength(1); i++)
        {
            shopItems[3, i] = 0;
        }

        // Potion metinlerini ve miktar metinlerini güncelle
        UpdatePotionTexts();
        UpdatePotionQuantityTexts();
    }

    void UpdatePotionQuantityTexts()
    {
        for (int i = 0; i < potionQuantityTexts.Length; i++)
        {
            int potionQuantity = shopItems[3, i + 1];
            potionQuantityTexts[i].text = "x " + potionQuantity.ToString();

            // Eğer potion miktarı 0 ise ilgili paneli kapat
            if (potionQuantity == 0)
            {
                potionPanels[i].SetActive(false);
            }
        }
    }
}

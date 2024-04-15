using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public TextMeshProUGUI DialogueText;
    public string[] Sentences;
    private int Index = 0;
    public float DialogueSpeed;
    public Animator DialogueAnimator;
    public GameObject panelToOpen; // Eklediğimiz panel değişkeni

    private bool isTyping = false; // Yazının yazılma işlemi devam ediyor mu kontrolü için

    void Start()
    {
        StartCoroutine(StartDialogueAfterDelay());
    }

    IEnumerator StartDialogueAfterDelay()
    {
        yield return new WaitForSeconds(1f); // 1 saniye bekleyin
        DialogueAnimator.SetTrigger("Enter");
        StartCoroutine(WriteSentence());
    }

    void Update()
{
    if (Input.GetKeyDown(KeyCode.Return))
    {
        if (!isTyping) // Yazı tam yüklenmeden Return tuşuna basılırsa
        {
            if (Index < Sentences.Length)
            {
                NextSentence();
            }
            else
            {
                DialogueText.text = "";
                DialogueAnimator.SetTrigger("Exit");
                
                // Eklenen paneli aç
                if (panelToOpen != null)
                {
                    panelToOpen.SetActive(true);
                }
            }
        }
        else // Yazı yüklenme sürecindeyken Return tuşuna basılırsa
        {
            // Yazıyı tam hale getir
            StopAllCoroutines();
            DialogueText.text = Sentences[Index];
            isTyping = false;
        }
    }
}


    void NextSentence()
    {
        Index++;
        if (Index < Sentences.Length)
        {
            DialogueText.text = "";
            StartCoroutine(WriteSentence());
        }
        else
        {
            // Tüm cümleler yazıldıktan sonra Return tuşuna basılırsa
            DialogueText.text = "";
            DialogueAnimator.SetTrigger("Exit");

            // Eklenen paneli aç
            if (panelToOpen != null)
            {
                panelToOpen.SetActive(true);
            }
        }
    }

    IEnumerator WriteSentence()
    {
        isTyping = true;
        if (Index < Sentences.Length)
        {
            foreach (char Character in Sentences[Index].ToCharArray())
            {
                DialogueText.text += Character;
                yield return new WaitForSeconds(DialogueSpeed);
            }
            isTyping = false;
        }
    }

}

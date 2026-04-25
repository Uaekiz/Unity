using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro kullanacaðýmýz için bu þart
using UnityEngine.SceneManagement;

public class CutscenePlayer : MonoBehaviour
{
    [Header("Görsel Referanslar")]
    public Image displayImage;
    public Image fadePanel;
    public TextMeshProUGUI altYaziText; // UI'daki metin kutusu

    [Header("Ayarlar")]
    public float fadeHizi = 1.0f;

    private int currentImageIndex = 0;
    private int currentTextIndex = 0; // O anki görselin kaçýncý metnindeyiz?
    private bool isTransitioning = false;

    void Start()
    {
        if (CutsceneSettings.oynatilacakGorseller != null)
        {
            displayImage.sprite = CutsceneSettings.oynatilacakGorseller[0];
            fadePanel.color = new Color(0, 0, 0, 0);
            MetniGuncelle();
        }
    }

    public void SonrakiAdim() // Butona baðlanan fonksiyon
    {
        if (isTransitioning) return;

        // Mevcut görselin metinleri bitti mi?
        if (currentTextIndex < CutsceneSettings.metinGruplari[currentImageIndex].Length - 1)
        {
            currentTextIndex++;
            MetniGuncelle();
        }
        else
        {
            // Metinler bitti, sonraki görsele geç
            StartCoroutine(GecisSekansi());
        }
    }

    void MetniGuncelle()
    {
        string key = CutsceneSettings.metinGruplari[currentImageIndex][currentTextIndex];
        altYaziText.text = LocalizationManager.GetText(key);
    }

    IEnumerator GecisSekansi()
    {
        isTransitioning = true;
        currentImageIndex++;

        if (currentImageIndex < CutsceneSettings.oynatilacakGorseller.Length)
        {
            // Kararma
            yield return StartCoroutine(Fade(1));

            displayImage.sprite = CutsceneSettings.oynatilacakGorseller[currentImageIndex];
            currentTextIndex = 0; // Yeni resim için metin sayacýný sýfýrla
            MetniGuncelle();

            yield return new WaitForSeconds(0.2f);

            // Açýlma
            yield return StartCoroutine(Fade(0));
            isTransitioning = false;
        }
        else
        {
            SahneyiBitir();
        }
    }

    IEnumerator Fade(float hedefAlpha)
    {
        float baslangicAlpha = fadePanel.color.a;
        float zaman = 0;
        while (zaman < 1)
        {
            zaman += Time.deltaTime * fadeHizi;
            float alpha = Mathf.Lerp(baslangicAlpha, hedefAlpha, zaman);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    void SahneyiBitir()
    {
        GameManager.IzlendiOlarakIsaretle(CutsceneSettings.mevcutAraSahneID);
        SceneManager.LoadScene(CutsceneSettings.sonrakiSahne);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutscenePlayer : MonoBehaviour
{
    [Header("Görsel Referanslar")]
    public Image displayImage;   // Ara sahne resminin göründüðü yer
    public Image fadePanel;      // Siyah karartma paneli (Alpha 0 olmalý)

    [Header("Ayarlar")]
    public float fadeHizi = 1.0f; // Kararma ve açýlma hýzý

    private int currentIndex = 0;
    private bool isTransitioning = false; // Týklama spamýný engellemek için

    void Start()
    {
        // 1. Kutuda resim var mý kontrol et
        if (CutsceneSettings.oynatilacakGorseller != null && CutsceneSettings.oynatilacakGorseller.Length > 0)
        {
            displayImage.sprite = CutsceneSettings.oynatilacakGorseller[0];

            // Baþlangýçta fade panelini þeffaf yapalým
            if (fadePanel != null)
            {
                fadePanel.color = new Color(0, 0, 0, 0);
            }
        }
        else
        {
            Debug.LogError("Görsel bulunamadý! CutsceneSettings doldurulmamýþ.");
        }
    }

    // Butonun 'OnClick' kýsmýna bunu baðla
    public void SonrakiGorsel()
    {
        // Eðer þu an bir geçiþ yapýlýyorsa týklamayý engelle
        if (isTransitioning) return;

        StartCoroutine(GecisSekansi());
    }

    IEnumerator GecisSekansi()
    {
        isTransitioning = true;

        // --- 1. ADIM: EKRANI KARART (Alpha 0 -> 1) ---
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeHizi;
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // --- 2. ADIM: RESMÝ DEÐÝÞTÝR ---
        currentIndex++;

        if (currentIndex < CutsceneSettings.oynatilacakGorseller.Length)
        {
            displayImage.sprite = CutsceneSettings.oynatilacakGorseller[currentIndex];

            // Ekran simsiyahken çok kýsa bekle (Daha sinematik durur)
            yield return new WaitForSeconds(0.2f);

            // --- 3. ADIM: EKRANI AÇ (Alpha 1 -> 0) ---
            while (alpha > 0)
            {
                alpha -= Time.deltaTime * fadeHizi;
                fadePanel.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            isTransitioning = false;
        }
        else
        {
            // Resimler bittiyse sahneyi bitir
            SahneyiBitir();
        }
    }

    void SahneyiBitir()
    {
        // Ýzlendi bilgisini iþaretle
        GameManager.IzlendiOlarakIsaretle(CutsceneSettings.mevcutAraSahneID);

        // Hedef sahneye git
        SceneManager.LoadScene(CutsceneSettings.sonrakiSahne);
    }
}
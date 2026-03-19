using UnityEngine;
using System.Collections;

public class AsansorKontrol : MonoBehaviour
{
    [Header("G�rsel Ayarlar")]
    public SpriteRenderer asansorSprite;
    public Sprite calisirGorsel;

    [Header("Animasyon")]
    public Animator asansorAnimator;
    public string acilmaTriggerIsmi = "acilma";

    [Header("Kontrol ID'leri")]
    public string[] kontrolIDleri = { "T_K_Sigorta_Slot", "T_K_Kondaktor_Slot", "T_K_Kablo_Slot" };

    private bool asansorCalisiyor = false;

    void Start()
    {
        // SAHNE YÜKLENDİĞİNDE KONTROL ET: Asansör daha önce tamir edilmiş mi?
        bool oncedenTamirEdilmisMi = true;
        foreach (string id in kontrolIDleri)
        {
            if (!GlobalData.DurumNedir(id))
            {
                oncedenTamirEdilmisMi = false;
                break;
            }
        }

        // Eğer tüm parçalar önceden takılmışsa (tamir edildiyse)
        // Eğer tüm parçalar önceden takılmışsa (tamir edildiyse)
        if (oncedenTamirEdilmisMi)
        {
            asansorCalisiyor = true; // Tekrar tamir sekansının çalışmasını engelle

            // 1. Animator'ı tamamen uyut. Böylece animasyonu baştan oynatmaya çalışmaz.
            if (asansorAnimator != null)
            {
                asansorAnimator.enabled = false; 
            }

            // 2. Direkt olarak çalışan (ışıklı/açık) görseli koy ve öylece kalsın
            if (asansorSprite != null && calisirGorsel != null)
            {
                asansorSprite.sprite = calisirGorsel;
            }
        }
    }

    public void Denetle()
    {
        if (asansorCalisiyor) return;

        bool tumParcalarTamam = true;
        foreach (string id in kontrolIDleri)
        {
            if (!GlobalData.DurumNedir(id))
            {
                tumParcalarTamam = false;
                break;
            }
        }

        if (tumParcalarTamam)
        {
            StartCoroutine(PuzzleTamamlandiSekansi());
        }
    }

    IEnumerator PuzzleTamamlandiSekansi()
    {
        asansorCalisiyor = true;

        if (EnvanterManager.Instance != null && EnvanterManager.Instance.asansorPaneli != null)
        {
            CanvasGroup cg = EnvanterManager.Instance.asansorCanvasGroup;

            // 1. A�AMA: ETK�LE��M� KES
            // Oyuncu ba�ard���n� anlas�n ama art�k t�klayamas�n
            if (cg != null)
            {
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }

            // --- 1.5 Saniye Bekle (Ba�ar� an�) ---
            yield return new WaitForSeconds(1.5f);

            // 2. A�AMADA: YAVA��A KAPANMA (FADE OUT)
            if (cg != null)
            {
                float fadeSure = 1.0f; // Kapanma h�z� (1 saniye)
                float baslangicAlpha = cg.alpha;

                for (float t = 0; t < fadeSure; t += Time.deltaTime)
                {
                    cg.alpha = Mathf.Lerp(baslangicAlpha, 0, t / fadeSure);
                    yield return null; // Her karede (frame) bekle
                }
                cg.alpha = 0;
            }

            // Paneli tamamen kapat
            EnvanterManager.Instance.asansorPaneli.SetActive(false);
            Debug.Log("Panel yava��a kapand�.");
        }

        // --- PANEL KAPANDIKTAN SONRA 1.5 SAN�YE DAHA BEKLE ---
        yield return new WaitForSeconds(1.5f);

        // 3. A�AMADA: ASANS�R� �ALI�TIR
        if (asansorSprite != null && calisirGorsel != null)
            asansorSprite.sprite = calisirGorsel;

        if (asansorAnimator != null)
            asansorAnimator.SetTrigger(acilmaTriggerIsmi);

        Debug.Log("Asans�r g�rkemli bir �ekilde a��ld�!");
    }
}
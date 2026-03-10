using UnityEngine;
using System.Collections;

public class AsansorKontrol : MonoBehaviour
{
    [Header("Görsel Ayarlar")]
    public SpriteRenderer asansorSprite;
    public Sprite calisirGorsel;

    [Header("Animasyon")]
    public Animator asansorAnimator;
    public string acilmaTriggerIsmi = "acilma";

    [Header("Kontrol ID'leri")]
    public string[] kontrolIDleri = { "T_K_Sigorta_Slot", "T_K_Kondaktor_Slot", "T_K_Kablo_Slot" };

    private bool asansorCalisiyor = false;

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

            // 1. AÞAMA: ETKÝLEÞÝMÝ KES
            // Oyuncu baþardýðýný anlasýn ama artýk týklayamasýn
            if (cg != null)
            {
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }

            // --- 1.5 Saniye Bekle (Baþarý aný) ---
            yield return new WaitForSeconds(1.5f);

            // 2. AÞAMADA: YAVAÞÇA KAPANMA (FADE OUT)
            if (cg != null)
            {
                float fadeSure = 1.0f; // Kapanma hýzý (1 saniye)
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
            Debug.Log("Panel yavaþça kapandý.");
        }

        // --- PANEL KAPANDIKTAN SONRA 1.5 SANÝYE DAHA BEKLE ---
        yield return new WaitForSeconds(1.5f);

        // 3. AÞAMADA: ASANSÖRÜ ÇALIÞTIR
        if (asansorSprite != null && calisirGorsel != null)
            asansorSprite.sprite = calisirGorsel;

        if (asansorAnimator != null)
            asansorAnimator.SetTrigger(acilmaTriggerIsmi);

        Debug.Log("Asansör görkemli bir þekilde açýldý!");
    }
}
using UnityEngine;
using System.Collections;

public class AsansorKontrol : MonoBehaviour
{
    [Header("Görsel Ayarlar")]
    public SpriteRenderer asansorSprite;
    public Sprite bozukGorsel;
    public Sprite calisirGorsel;

    [Header("Animasyon")]
    public Animator asansorAnimator;
    public string acilmaTriggerIsmi = "acilma";

    [Header("Panel Ayarlarý")]
    public GameObject asansorPaneli;
    public CanvasGroup panelCanvasGroup; // GetComponent yerine direkt referans
    public GameObject panelCikisButonu;

    [Header("Kontrol ID'leri")]
    public string[] kontrolIDleri = { "T_K_Sigorta_Slot", "T_K_Kondaktor_Slot", "T_K_Kablo_Slot" };

    private bool asansorCalisiyor = false;
    void Start()
    {
        // 1. Referanslar kopmussa (Missing ise) otomatik bulalým
        ReferanslariBul();

        // 2. Sahne açýldýðýnda son durumu denetle
        Denetle();
    }
    void ReferanslariBul()
    {
        // Eðer asansör paneli kayýpsa (Missing) veya null ise
        if (asansorPaneli == null)
        {
            // Önce sahnede her zaman AÇIK olan ana grubu bulalým (Hiyerarþideki adýný kontrol et!)
            GameObject anaGrup = GameObject.Find("BulmacaPanelleri");

            if (anaGrup != null)
            {
                // true parametresi sayesinde KAPALI olan alt objeleri de bulur!
                Transform[] tumCocuklar = anaGrup.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in tumCocuklar)
                {
                    if (t.name == "Panel_Asansor")
                    {
                        asansorPaneli = t.gameObject;
                        break;
                    }
                }
            }
            else
            {
                // Eðer ana grup yoksa, tüm sahnede kapalý objeleri de arayan en aðýr yöntemi kullan:
                asansorPaneli = GameObject.Find("Panel_Asansor"); // Son çare
            }
        }

        // Paneli bulduysak diðer parçalarý içine girip alalým
        if (asansorPaneli != null)
        {
            if (panelCanvasGroup == null)
                panelCanvasGroup = asansorPaneli.GetComponent<CanvasGroup>();

            // Find yerine GetComponentsInChildren(true) ile butonu da kapalý olsa bile bulalým
            if (panelCikisButonu == null)
            {
                foreach (Transform t in asansorPaneli.GetComponentsInChildren<Transform>(true))
                {
                    if (t.name == "Button") // Butonunun adý neyse o
                    {
                        panelCikisButonu = t.gameObject;
                        break;
                    }
                }
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
        Debug.Log("<color=cyan>Bulmaca bitti, sekans baþlýyor...</color>");

        // 1. Giriþleri Engelle
        if (panelCikisButonu != null) panelCikisButonu.SetActive(false);

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
            panelCanvasGroup.alpha = 0.5f; // Görsel olarak kapandýðýný doðrulamak için þeffaflaþtýrýyoruz
            Debug.Log("CanvasGroup etkileþimi kapatýldý.");
        }
        else
        {
            Debug.LogError("HATA: Panel Canvas Group referansý atanmamýþ!");
        }

        // 2. Bekleme Süresi
        yield return new WaitForSeconds(1.5f);

        // 3. Paneli Kapat
        if (asansorPaneli != null)
        {
            asansorPaneli.SetActive(false);
            Debug.Log("Panel SetActive(false) yapýldý.");
        }
        else
        {
            Debug.LogError("HATA: Asansör Paneli referansý atanmamýþ!");
        }

        // 4. Kýsa sessizlik
        yield return new WaitForSeconds(1.0f);

        // 5. Asansörü Çalýþtýr
        if (asansorSprite != null && calisirGorsel != null)
            asansorSprite.sprite = calisirGorsel;

        if (asansorAnimator != null)
        {
            asansorAnimator.SetTrigger(acilmaTriggerIsmi);
            Debug.Log("<color=green>Asansör Kapýsý Açýlýyor!</color>");
        }
    }
}
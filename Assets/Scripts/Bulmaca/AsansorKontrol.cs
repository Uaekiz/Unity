using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Sahne y�netimi i�in bu k�t�phane �art!
using UnityEngine.UI;

public class AsansorKontrol : MonoBehaviour
{
    [Header("Asansör Sesi")]
    public AudioClip asansorSesi;

    [Header("GGrsel Ayarlar")]
    public SpriteRenderer asansorSprite;
    public Sprite calisirGorsel;

    [Header("Animasyon")]
    public Animator asansorAnimator;
    public string acilmaTriggerIsmi = "acilma";

    [Header("Kontrol ID'leri")]
    public string[] kontrolIDleri = { "T_K_Sigorta_Slot", "T_K_Kondaktor_Slot", "T_K_Kablo_Slot" };

    [Header("Final Ayarlari")]
    public CanvasGroup finalPanelCG;       
    public GameObject etkilesimButonu;

    private bool asansorCalisiyor = false;

    void Start()
    {
        if (etkilesimButonu != null)
        {
            _interactButtonComponent = etkilesimButonu.GetComponent<Button>();
        }
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
        if (ArayuzSesleri.Instance != null && asansorSesi != null)
        {
            ArayuzSesleri.Instance.PanelSesiCal(asansorSesi);
        }

        // 3. A�AMADA: ASANS�R� �ALI�TIR
        if (asansorSprite != null && calisirGorsel != null)
            asansorSprite.sprite = calisirGorsel;

        if (asansorAnimator != null)
            asansorAnimator.SetTrigger(acilmaTriggerIsmi);

        Debug.Log("Asans�r g�rkemli bir �ekilde a��ld�!");
    }

         
    
    // YENİ: Butonun tıklama (onClick) olayına erişmek için
    private Button _interactButtonComponent;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Eğer giren oyuncuysa VE asansör tamir edildiyse
        if (collision.CompareTag("Player") && asansorCalisiyor)
        {
            // 1. BUTONU SAHİPLEN: Yetkiyi asansör alıyor
            PlayerMove.aktifEtkilesimObjesi = this.gameObject;

            if (_interactButtonComponent != null)
            {
                // 2. BAĞLANTILARI TEMİZLE: Varsa yakındaki kapının bağlantısını kopar
                _interactButtonComponent.onClick.RemoveAllListeners();

                // 3. KENDİ GÖREVİNİ EKLE: Butona basılınca TryToOpen çalışsın
                _interactButtonComponent.onClick.AddListener(TryToOpen);

                // 4. Butonu göster
                etkilesimButonu.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // SADECE BUTONUN SAHİBİ ASANSÖR İSE GİZLE:
            // (Eğer oyuncu asansörden çıkıp direkt kapıya girdiyse, butonu gizlememesi için)
            if (PlayerMove.aktifEtkilesimObjesi == this.gameObject)
            {
                if (_interactButtonComponent != null)
                {
                    // Butonun görevini boşalt ve gizle
                    _interactButtonComponent.onClick.RemoveAllListeners();
                    etkilesimButonu.SetActive(false);
                }

                // Sahibi kalmadı diye belirt
                PlayerMove.aktifEtkilesimObjesi = null;
            }
        }
    }

    public void TryToOpen()
    {
        if (asansorCalisiyor)
        {
            // Ekranda buton kalmasın
            if (etkilesimButonu != null) 
            {
                _interactButtonComponent.onClick.RemoveAllListeners(); // Güvenlik için temizle
                etkilesimButonu.SetActive(false);
            }
            
            StartCoroutine(OyunFinalSekansi());
        }
    }

    IEnumerator OyunFinalSekansi()
    {
        finalPanelCG.gameObject.SetActive(true);
        finalPanelCG.blocksRaycasts = true;

        float sayac = 0;
        while (sayac < 1f)
        {
            sayac += Time.deltaTime;
            finalPanelCG.alpha = sayac;
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        // Verileri sıfırla
        GameManager.oda1Temizlendi = false; 
        SaveManager.Kaydet(false);          
        GlobalData.oyunDurumlari.Clear();   
        GlobalData.sonCikisKapisi = "";

        SceneManager.LoadScene("AnaSayfa");
    }
}
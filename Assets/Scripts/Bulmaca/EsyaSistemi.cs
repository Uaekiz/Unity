using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EsyaSistemi : MonoBehaviour
{
    [Header("Oynanış Modu")]
    public bool koridordaMi = false;
    public Button elButonu;

    [Header("İçerik Ayarları")]
    public bool icindeEsyaVarMi = true;

    [Header("Ses Ayarları")]
    public AudioClip panelAcilmaSesi;
    public AudioClip esyaToplamaSesi;

    [System.Serializable]
    public struct EsyaBilgisi
    {
        public string esyaID;
        public int miktar;
        [Tooltip("Eğer bu eşya bir bulmacada kullanıldıysa oranın ID'sini yazın (Örn: T_K_Bant_Slot). Yoksa boş bırakın.")]
        public string kullanildigiYerID;
    }
    public List<EsyaBilgisi> esyaListesi;

    [Header("Resimler")]
    public Sprite kapaliResim;
    public Sprite acikBosResim;
    public Sprite acikDoluResim;

    [Header("Bulmaca Ayarları")]
    public bool buBirBulmacaMi = false;
    public string acilacakPanelIsmi;

    private SpriteRenderer spriteRenderer;
    private bool isAcik = false;
    private GameObject cachedPanel; // Paneli bir kez bulup hafızada tutuyorum (optimizeyşınss beybii huhuww)

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GorunumuGuncelle();

        if (koridordaMi && elButonu != null)
            elButonu.gameObject.SetActive(false);
    }

    // --- TETİKLEYİCİLER (Trigger) ---
    void OnTriggerEnter2D(Collider2D temas)
    {
        if (koridordaMi && temas.CompareTag("Player"))
        {
            PlayerMove.aktifEtkilesimObjesi = this.gameObject;
            if (elButonu != null)
            {
                elButonu.gameObject.SetActive(true);
                elButonu.onClick.RemoveAllListeners();
                elButonu.onClick.AddListener(EtkilesimeGir);
            }
        }
    }

    void OnTriggerExit2D(Collider2D temas)
    {
        if (koridordaMi && temas.CompareTag("Player") && PlayerMove.aktifEtkilesimObjesi == this.gameObject)
        {
            if (elButonu != null) elButonu.gameObject.SetActive(false);
            PlayerMove.aktifEtkilesimObjesi = null;
        }
    }

    public void EtkilesimeGir()
    {
        if (GameManager.Instance != null && !GameManager.oda1Temizlendi)
        {
            Debug.Log("Savaş bitmeden eşyalarla etkileşime giremezsin!");
            return; // Fonksiyonu burada iptal et, aşağıdaki eşya alma kodlarına inmesin!
        }
        // 1. BULMACA MANTIĞI 
        if (buBirBulmacaMi)
        {
            if (cachedPanel == null && EnvanterManager.Instance != null)
                cachedPanel = DerinlerdeAra(EnvanterManager.Instance.transform, acilacakPanelIsmi);

            if (cachedPanel != null) 
            {
                cachedPanel.SetActive(true); // Panel ekrana geliyor
                
                // YENİ: Panel ekrana geldiği an sesi çal!
                if (ArayuzSesleri.Instance != null && panelAcilmaSesi != null)
                {
                    ArayuzSesleri.Instance.PanelSesiCal(panelAcilmaSesi);
                }
            }
        }

        bool envanterDoluMu = HepsiniAldikMi();

        if (!isAcik)
        {
            isAcik = true;
        }
        else
        {
            if (icindeEsyaVarMi && !envanterDoluMu)
            {
                foreach (var esya in esyaListesi)
                {
                    GlobalData.DurumKaydet(esya.esyaID, true);
                    Debug.Log($"{esya.miktar} adet {esya.esyaID} alındı!");
                }

                if (ArayuzSesleri.Instance != null && esyaToplamaSesi != null)
                {
                    ArayuzSesleri.Instance.PanelSesiCal(esyaToplamaSesi);
                }

                if (EnvanterManager.Instance != null)
                    EnvanterManager.Instance.ArayuzuGuncelle();
            }
            else
            {
                isAcik = false;
            }
        }

        GorunumuGuncelle(); 
    }

    private GameObject DerinlerdeAra(Transform parent, string targetName)
    {
        if (string.IsNullOrEmpty(targetName)) return null;

        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == targetName) return child.gameObject;
        }
        return null;
    }

    bool HepsiniAldikMi()
    {
        if (esyaListesi.Count == 0) return true;
        foreach (var esya in esyaListesi)
        {
            bool cebimizdeMi = GlobalData.DurumNedir(esya.esyaID);
            
            bool kullanildiMi = false;
            if (!string.IsNullOrEmpty(esya.kullanildigiYerID))
            {
                kullanildiMi = GlobalData.DurumNedir(esya.kullanildigiYerID);
            }

            if (!cebimizdeMi && !kullanildiMi) 
                return false;
        }
        return true;
    }

    void GorunumuGuncelle()
    {
        if (spriteRenderer == null) return;

        if (!isAcik)
        {
            spriteRenderer.sprite = kapaliResim;
        }
        else
        {
            spriteRenderer.sprite = (icindeEsyaVarMi && !HepsiniAldikMi()) ? acikDoluResim : acikBosResim;
        }
    }
}
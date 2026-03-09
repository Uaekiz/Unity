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

    [System.Serializable]
    public struct EsyaBilgisi
    {
        public string esyaID;
        public int miktar;
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

    void OnMouseDown()
    {
        if (!koridordaMi) EtkilesimeGir();
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
        // 1. BULMACA MANTIĞI 
        if (buBirBulmacaMi)
        {
            if (cachedPanel == null && EnvanterManager.Instance != null)
                cachedPanel = DerinlerdeAra(EnvanterManager.Instance.transform, acilacakPanelIsmi);

            if (cachedPanel != null) cachedPanel.SetActive(true);
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
            if (!GlobalData.DurumNedir(esya.esyaID)) return false;
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
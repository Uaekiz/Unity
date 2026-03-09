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

    [Header("Resimler (Spriteler)")]
    public Sprite kapaliResim;
    public Sprite acikBosResim;
    public Sprite acikDoluResim;

    [Header("Bulmaca Ayarları")]
    public bool buBirBulmacaMi = false;
    [Tooltip("Hiyerarşideki panelin adını birebir yazın (Örn: Panel_Oda)")]
    public string acilacakPanelIsmi;

    private SpriteRenderer spriteRenderer;
    private bool isAcik = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isAcik = false;
        GorunumuGuncelle();

        if (koridordaMi && elButonu != null)
        {
            elButonu.gameObject.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        if (!koridordaMi) EtkilesimeGir();
    }

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
        if (koridordaMi && temas.CompareTag("Player"))
        {
            if (PlayerMove.aktifEtkilesimObjesi == this.gameObject)
            {
                if (elButonu != null)
                {
                    elButonu.gameObject.SetActive(false);
                    elButonu.onClick.RemoveAllListeners();
                }
                PlayerMove.aktifEtkilesimObjesi = null;
            }
        }
    }

    public void EtkilesimeGir()
    {
        // --- 1. BULMACA PANELİ MANTIĞI (İsme Göre Bulma) ---
        if (buBirBulmacaMi && !string.IsNullOrEmpty(acilacakPanelIsmi))
        {
            GameObject bulunanPanel = null;

            // 1. Önce EnvanterManager'ın altında (ne kadar derinde olursa olsun) ara
            if (EnvanterManager.Instance != null)
            {
                // Bu fonksiyon alt objelerin içinde de arama yapar
                bulunanPanel = DerinlerdeAra(EnvanterManager.Instance.transform, acilacakPanelIsmi);
            }

            if (bulunanPanel != null)
            {
                bulunanPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("HATA: '" + acilacakPanelIsmi + "' isimli panel hiçbir yerde bulunamadı!");
            }
        }

        // --- 2. KUTU AÇILMA / EŞYA ALMA MANTIĞI ---
        if (!isAcik)
        {
            isAcik = true;
            GorunumuGuncelle();
        }
        else
        {
            if (icindeEsyaVarMi && !HepsiniAldikMi())
            {
                foreach (var esya in esyaListesi)
                {
                    GlobalData.DurumKaydet(esya.esyaID, true);
                    Debug.Log(esya.miktar + " adet " + esya.esyaID + " alındı!");
                }

                if (EnvanterManager.Instance != null)
                {
                    EnvanterManager.Instance.ArayuzuGuncelle();
                }

                GorunumuGuncelle();
            }
            else
            {
                isAcik = false;
                GorunumuGuncelle();
            }
        }
    }

    // YENİ YARDIMCI FONKSİYON: Objeyi çocukların içinde ismen arar
    private GameObject DerinlerdeAra(Transform parent, string targetName)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == targetName)
            {
                return child.gameObject;
            }
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
            if (icindeEsyaVarMi && !HepsiniAldikMi())
            {
                spriteRenderer.sprite = acikDoluResim;
            }
            else
            {
                spriteRenderer.sprite = acikBosResim;
            }
        }
    }
}
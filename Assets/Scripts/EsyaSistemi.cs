using UnityEngine;
using UnityEngine.UI; // UI Butonlarını kullanmak için ekledik

public class EsyaSistemi : MonoBehaviour
{
    [Header("Oynanış Modu")]
    public bool koridordaMi = false; // Tiki kaldırırsan odadaki gibi TIKLAMAYLA çalışır. Tiklersen YAKLAŞINCA çalışır.
    public Button elButonu; // Sadece "koridordaMi" tikliyse buraya Canvas'taki El butonunu sürükle.

    [Header("İçerik Ayarları")]
    public bool icindeEsyaVarMi = true; 
    public string esyaID = "Oda1_Bant"; 

    [Header("Resimler (Spriteler)")]
    public Sprite kapaliResim;
    public Sprite acikBosResim;
    public Sprite acikDoluResim; 

    private SpriteRenderer spriteRenderer;
    private bool isAcik = false; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isAcik = false; 
        GorunumuGuncelle();

        // Eğer koridordaysak, oyun başında el butonunu gizle ki ekranda boşuna durmasın
        if (koridordaMi && elButonu != null)
        {
            elButonu.gameObject.SetActive(false);
        }
    }

    // --- 1. ODADA TIKLAMA SİSTEMİ ---
    void OnMouseDown()
    {
        // Sadece "koridordaMi" FALSE ise (Yani odadaysak) tıklama çalışsın
        if (!koridordaMi) 
        {
            EtkilesimeGir();
        }
    }

    // --- 2. KORİDORDA YAKLAŞMA SİSTEMİ ---
    void OnTriggerEnter2D(Collider2D temas)
    {
        if (koridordaMi && temas.CompareTag("Player"))
        {
            // Sahibi benim! Hafızaya adımı yazdırıyorum.
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
            // BUTONU SADECE SAHİBİ HALA BENSEM GİZLE!
            if (PlayerMove.aktifEtkilesimObjesi == this.gameObject)
            {
                if (elButonu != null)
                {
                    elButonu.gameObject.SetActive(false);
                    elButonu.onClick.RemoveAllListeners();
                }
                PlayerMove.aktifEtkilesimObjesi = null; // Sahibi kalmadı
            }
        }
    }

    // --- ETKİLEŞİM VE GÖRÜNTÜ (Aynı Kaldı) ---
    public void EtkilesimeGir()
    {
        if (!isAcik) 
        {
            isAcik = true;
            GorunumuGuncelle();
        }
        else 
        {
            bool esyaAlindiMi = icindeEsyaVarMi ? GlobalData.DurumNedir(esyaID) : true;

            if (icindeEsyaVarMi && !esyaAlindiMi) 
            {
                Debug.Log(esyaID + " Envantere Eklendi!");
                GlobalData.DurumKaydet(esyaID, true); 
                
                // Envanteri anında güncelle
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

    void GorunumuGuncelle()
    {
        if (!isAcik)
        {
            spriteRenderer.sprite = kapaliResim;
            Debug.Log(esyaID + " Görseli Gösteriliyor (Kapalı)");
        }
        else
        {
            bool esyaAlindiMi = icindeEsyaVarMi ? GlobalData.DurumNedir(esyaID) : true;
            if (icindeEsyaVarMi && !esyaAlindiMi)
            {
                spriteRenderer.sprite = acikDoluResim;
                Debug.Log(esyaID + " Görseli Gösteriliyor (Dolu)");
            }
            else
            {
                spriteRenderer.sprite = acikBosResim;
                Debug.Log(esyaID + " Görseli Gösteriliyor (Boş)");
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnvanterManager : MonoBehaviour
{
    // Singleton: Her yerden "EnvanterManager.Instance" diye ulaşmak için
    public static EnvanterManager Instance;

    [Header("Arayüz")]
    public GameObject envanterPaneli; // Savaşta kapatmak için tüm paneli tutarız
    public Image[] slotlar; // Ekrana dizdiğin Slot_1, Slot_2...

    // Hangi ID'ye hangi resim denk geliyor? Unity'den eşleştireceğiz.
    [System.Serializable]
    public struct EsyaResmi
    {
        public string esyaID; // Örn: "Oda1_Bant"
        public Sprite esyaGorseli; // Bant çizimin
    }
    
    [Header("Eşya Veritabanı")]
    public List<EsyaResmi> esyaKutuphanesi;

    private void Awake()
    {
        // ÖLÜMSÜZLÜK KODU: Bu Canvas sahneler arası silinmez!
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Kilit nokta burası
        }
        else
        {
            Destroy(gameObject); // Eğer odaya girip çıkarken 2. bir canvas oluşursa onu yok et
        }
    }

    private void Start()
    {
        ArayuzuGuncelle();
    }

    // Çekmeceden eşya alınca bu fonksiyonu çağıracağız
    public void ArayuzuGuncelle()
    {
        // 1. Önce tüm slotları temizle ve görünmez yap
        foreach (Image slot in slotlar)
        {
            slot.sprite = null;
            slot.color = new Color(1, 1, 1, 0); // Tamamen şeffaf
        }

        int siradakiBosSlot = 0;

        // 2. Kütüphanedeki eşyalara bak, alınmış olanları slotlara çiz
        foreach (EsyaResmi esya in esyaKutuphanesi)
        {
            if (GlobalData.DurumNedir(esya.esyaID) == true) // Eğer hafızada "Alındı" ise
            {
                if (siradakiBosSlot < slotlar.Length)
                {
                    slotlar[siradakiBosSlot].sprite = esya.esyaGorseli;
                    slotlar[siradakiBosSlot].color = new Color(1, 1, 1, 1); // Görünür yap
                    siradakiBosSlot++;
                }
            }
        }
    }

    // Savaş anında envanteri gizlemek/açmak için
    public void SavasModu(bool savastaMi)
    {
        if (envanterPaneli != null)
        {
            // Savaştaysa kapat, değilse aç
            envanterPaneli.SetActive(!savastaMi);
        }
    }
}
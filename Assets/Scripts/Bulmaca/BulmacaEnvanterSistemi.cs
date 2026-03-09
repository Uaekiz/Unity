using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BulmacaEnvanterSistemi : MonoBehaviour
{
    [System.Serializable]
    public struct BulmacaSlotu
    {
        public string esyaID;   // EnvanterManager'daki ile ayný olmalý
        public Image slotUI;    // Hiyerarþideki Image objesi
    }

    [Header("Bulmaca Eþya Eþleþmeleri")]
    public List<BulmacaSlotu> bulmacaSlotlari;

    void OnEnable()
    {
        BulmacaArayuzunuYenile();
    }

    public void BulmacaArayuzunuYenile()
    {
        if (bulmacaSlotlari == null || bulmacaSlotlari.Count == 0) return;

        foreach (var slotBilgisi in bulmacaSlotlari)
        {
            Guncelle(slotBilgisi);
        }
    }

    void Guncelle(BulmacaSlotu slotData)
    {
        if (slotData.slotUI == null) return;

        // 1. Oyuncu bu eþyaya sahip mi?
        bool sahipMi = GlobalData.DurumNedir(slotData.esyaID);

        // 2. Görseli ve Görünürlüðü Ayarla
        if (sahipMi)
        {
            slotData.slotUI.gameObject.SetActive(true);

            // Sprite'ý kütüphaneden çek
            Sprite gorsel = KutuphanedenResimBul(slotData.esyaID);
            if (gorsel != null) slotData.slotUI.sprite = gorsel;
        }
        else
        {
            // Eþya yoksa slotu gizle
            slotData.slotUI.gameObject.SetActive(false);
        }
    }

    Sprite KutuphanedenResimBul(string id)
    {
        if (EnvanterManager.Instance == null) return null;

        // Esya kütüphanesinde ara
        var bulunan = EnvanterManager.Instance.esyaKutuphanesi.Find(x => x.esyaID == id);
        return bulunan.esyaGorseli;
    }
}
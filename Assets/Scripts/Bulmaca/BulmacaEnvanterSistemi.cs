using UnityEngine;
using UnityEngine.UI;

public class BulmacaEnvanterSistemi : MonoBehaviour
{
    [Header("Bulmaca Ýçin Gereken Slotlar")]
    // Hiyerarþide oluþturduðun 4 Image objesini buraya sürükle
    public Image slotKontrolKalemi;
    public Image slotBant;
    public Image slotSigorta;
    public Image slotKondaktor;

    // Panel her açýldýðýnda (SetActive(true) olduðunda) otomatik yenilenir
    void OnEnable()
    {
        BulmacaArayuzunuYenile();
    }

    public void BulmacaArayuzunuYenile()
    {
        // EnvanterManager kütüphanesindeki ID'lerle birebir ayný olmalý!
        EsyayiGosterVeyaGizle(slotKontrolKalemi, "Kontrol Kalemi");
        EsyayiGosterVeyaGizle(slotBant, "Bant");
        EsyayiGosterVeyaGizle(slotSigorta, "Sigorta");
        EsyayiGosterVeyaGizle(slotKondaktor, "Kondaktor");
    }

    void EsyayiGosterVeyaGizle(Image slot, string id)
    {
        if (slot == null) return;

        // 1. GlobalData'dan oyuncu bu eþyayý almýþ mý kontrol et
        bool sahipMi = GlobalData.DurumNedir(id);

        if (sahipMi)
        {
            slot.gameObject.SetActive(true);
            slot.color = new Color(1, 1, 1, 1); // Görünür yap

            // 2. Görseli EnvanterManager'daki kütüphaneden otomatik al
            Sprite gorsel = KutuphanedenResimBul(id);
            if (gorsel != null) slot.sprite = gorsel;
        }
        else
        {
            // Eþya yoksa slotu kapat
            slot.gameObject.SetActive(false);
        }
    }

    Sprite KutuphanedenResimBul(string id)
    {
        if (EnvanterManager.Instance == null) return null;

        // EnvanterManager.cs içindeki esyaKutuphanesi listesinde tara
        foreach (var esya in EnvanterManager.Instance.esyaKutuphanesi)
        {
            if (esya.esyaID == id) return esya.esyaGorseli;
        }
        return null;
    }
}
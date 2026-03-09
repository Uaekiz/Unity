using UnityEngine;

public class AsansorKontrol : MonoBehaviour
{
    [Header("Görsel Ayarlar")]
    public SpriteRenderer asansorSprite;
    public Sprite bozukGorsel;
    public Sprite calisirGorsel; // Eðer animasyon yoksa kullanýlabilir

    [Header("Animasyon")]
    public Animator asansorAnimator;

    private bool asansorCalisiyor = false;

    void Start()
    {
        // Sahne açýldýðýnda durumunu kontrol et
        Denetle();
    }

    // Bu fonksiyonu "BulmacaSlotKontrol" içinden her parça takýldýðýnda çaðýracaðýz
    public void Denetle()
    {
        // 3 ana parçanýn da tamir edilip edilmediðini GlobalData'dan soruyoruz
        // SlotID'lerin neyse onlarý buraya birebir yazmalýsýn (Örn: "Sigorta_Slot")
        bool sigortaTamam = GlobalData.DurumNedir("T_K_Sigorta_Slot");
        bool kondaktorTamam = GlobalData.DurumNedir("T_K_Kondaktor_Slot");
        bool kabloTamam = GlobalData.DurumNedir("T_K_Kablo_Slot");

        if (sigortaTamam && kondaktorTamam && kabloTamam)
        {
            AsansoruCalistir();
        }
        else
        {
            if (asansorSprite != null) asansorSprite.sprite = bozukGorsel;
        }
    }

    void AsansoruCalistir()
    {
        if (asansorCalisiyor) return; // Zaten çalýþýyorsa tekrar tetikleme

        asansorCalisiyor = true;
        Debug.Log("Sistem Tamamlandý! Asansör çalýþýyor...");

        // Animasyonu baþlat
        if (asansorAnimator != null)
        {
            asansorAnimator.SetTrigger("acilma"); // Animator'daki tetikleyici ismi
        }

        // Eðer ses efekti eklemek istersen buraya ekleyebilirsin
    }
}
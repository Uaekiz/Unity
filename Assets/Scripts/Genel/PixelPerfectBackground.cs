using UnityEngine;

public class PixelPerfectBackground : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // 1. Kameranýn dünyadaki boyutlarýný hesapla
        float kameraYukseklik = Camera.main.orthographicSize * 2.0f;
        float kameraGenislik = kameraYukseklik * Screen.width / Screen.height;

        // 2. Sprite'ýn orijinal boyutlarýný al
        float spriteGenislik = sr.sprite.bounds.size.x;
        float spriteYukseklik = sr.sprite.bounds.size.y;

        // 3. Hem yatay hem dikey için ayrý ayrý gereken büyüme oranýný bul
        float scaleX = kameraGenislik / spriteGenislik;
        float scaleY = kameraYukseklik / spriteYukseklik;

        // 4. SÜNME OLMAMASI ÝÇÝN: Ýki orandan büyük olaný seçiyoruz (Uniform Scale)
        // Böylece pikseller kare kalýr, sadece ekraný tam dolduracak en büyük oranda eþit büyür.
        float enUygundurScale = Mathf.Max(scaleX, scaleY);

        // 5. Yeni ölçeði uygula (Z ekseni 1 kalmalý)
        transform.localScale = new Vector3(enUygundurScale, enUygundurScale, 1f);
    }
}
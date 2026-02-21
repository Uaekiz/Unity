using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Sahne değiştirmek için şart!

public class GameManager : MonoBehaviour
{
    // HAFIZA: Bu değişken sahne değişse bile RAM'de kalır (Static)
    public static bool oda1Temizlendi = false;

    public static GameManager Instance;

    [Header("Gerekli Referanslar")]
    public PlayerStats playerStats;
    public GameObject combatUI;
    public GameObject playerArms;
    public Image fadePanel;
    public Animator playerAnimator;

    [Header("Oda Durumu Objeleri")]
    public GameObject canliDusmanlarGrubu; // EnemyAI'ların olduğu baba obje
    public GameObject oluCesetlerGrubu;    // Sadece resimlerin olduğu baba obje
    public GameObject geriDonButonu;       // Koridora dönme butonu

    [Header("Düşman Takibi")]
    public int toplamDusmanSayisi;
    private int olenDusmanSayisi = 0;

    [Header("Sinematik Ayarları")]
    public float kararmaSuresi = 2.5f;
    public float acilmaSuresi = 2.0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // SAHNE AÇILDIĞINDA KONTROL ET
        if (oda1Temizlendi)
        {
            // Eğer daha önce temizlediysek:
            Debug.Log("Oda zaten temiz. İnceleme modu yükleniyor...");

            // 1. Canlıları yok et, Cesetleri aç
            if (canliDusmanlarGrubu != null) Destroy(canliDusmanlarGrubu); // Direkt siliyoruz
            if (oluCesetlerGrubu != null) oluCesetlerGrubu.SetActive(true);

            // 2. Savaş Arayüzünü Kapat, Geri Dön Butonunu Aç
            if (combatUI != null) combatUI.SetActive(false);
            if (playerArms != null) playerArms.SetActive(false);
            if (geriDonButonu != null) geriDonButonu.SetActive(true);

            // 3. Ekranı normal başlat (Fade-in yapmaya gerek yok veya hızlı yapabilirsin)
        }
        else
        {
            // Oda temiz değilse normal savaş başlasın
            EnvanterManager.Instance.SavasModu(true);
            if (oluCesetlerGrubu != null) oluCesetlerGrubu.SetActive(false); // Cesetler gizli
            if (geriDonButonu != null) geriDonButonu.SetActive(false);       // Buton gizli
        }
    }

    public void DusmanOldu()
    {
        olenDusmanSayisi++;

        if (olenDusmanSayisi >= toplamDusmanSayisi)
        {
            StartCoroutine(OlayYeriIncelemeModunaGec());
        }
    }

    IEnumerator OlayYeriIncelemeModunaGec()
    {
        Debug.Log("Çatışma bitti.");

        yield return new WaitForSeconds(1f);

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("reload");
        }

        if (playerStats != null) playerStats.Heal(1000);

        // --- EKRANI KARART ---
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime / kararmaSuresi;
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // --- EKRAN SİMSİYAH: SAHNE ARKASI DEĞİŞİKLİĞİ ---

        // 1. Canlı düşmanları kapat, ölü resimleri aç
        if (canliDusmanlarGrubu != null) canliDusmanlarGrubu.SetActive(false);
        if (oluCesetlerGrubu != null) oluCesetlerGrubu.SetActive(true);

        // 2. Kolları ve UI'ı kapat
        if (combatUI != null) combatUI.SetActive(false);
        if (playerArms != null) playerArms.SetActive(false);

        // 3. Geri Dön Butonunu Ortaya Çıkar

        // Hafızaya Kaydet
        oda1Temizlendi = true;

        yield return new WaitForSeconds(1.0f);

        // --- EKRANI AÇ ---
        while (alpha > 0)
        {
            alpha -= Time.deltaTime / acilmaSuresi;
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        Debug.Log("ARTIK İNCELEME MODUNDASIN!");
        if (geriDonButonu != null) geriDonButonu.SetActive(true);

        if (EnvanterManager.Instance != null)
        {
            EnvanterManager.Instance.SavasModu(false); // Savaş bitti, envanteri aç
        }

    }

    // --- BUTONA BAĞLAYACAĞIN FONKSİYON ---
    public void KoridoraDon()
    {
        Debug.Log("Koridora dönülüyor...");
        SceneManager.LoadScene("SampleScene");
        GlobalData.sonCikisKapisi = "Oda1";
    }
}
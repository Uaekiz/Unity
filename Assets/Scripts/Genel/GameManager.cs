using System.Collections;
using System.Collections.Generic; // Dictionary için bu şart!
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // --- GLOBAL DATA GÖREVLERİ (ARTIK BURADA) ---
    public static string sonCikisKapisi = "";
    public static Dictionary<string, bool> oyunDurumlari = new Dictionary<string, bool>();

    public static bool AraSahneIzlendiMi(string id)
    {
        string anahtar = "Izlendi_" + id;
        return oyunDurumlari.ContainsKey(anahtar) ? oyunDurumlari[anahtar] : false;
    }

    public static void IzlendiOlarakIsaretle(string id)
    {
        string anahtar = "Izlendi_" + id;
        if (oyunDurumlari.ContainsKey(anahtar)) oyunDurumlari[anahtar] = true;
        else oyunDurumlari.Add(anahtar, true);
    }
    // --------------------------------------------

    public static bool oda1Temizlendi = false;
    public static GameManager Instance;

    [Header("Gerekli Referanslar")]
    public PlayerStats playerStats;
    public GameObject combatUI;
    public GameObject playerArms;
    public Image fadePanel;
    public Animator playerAnimator;

    [Header("Oda Durumu Objeleri")]
    public GameObject canliDusmanlarGrubu;
    public GameObject oluCesetlerGrubu;
    public GameObject geriDonButonu;

    [Header("Düşman Takibi")]
    public int toplamDusmanSayisi;
    private int olenDusmanSayisi = 0;

    [Header("Sinematik Ayarları")]
    public float kararmaSuresi = 2.5f;
    public float acilmaSuresi = 2.0f;

    [Header("GameOver Ayarları")]
    public GameObject gameOverPanel;
    public string anaMenuSahneAdi = "MainMenu";

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (oda1Temizlendi)
        {
            Debug.Log("Oda zaten temiz. İnceleme modu yükleniyor...");
            if (canliDusmanlarGrubu != null) Destroy(canliDusmanlarGrubu);
            if (oluCesetlerGrubu != null) oluCesetlerGrubu.SetActive(true);
            if (combatUI != null) combatUI.SetActive(false);
            if (playerArms != null) playerArms.SetActive(false);
            if (geriDonButonu != null) geriDonButonu.SetActive(true);
        }
        else
        {
            if (EnvanterManager.Instance != null) EnvanterManager.Instance.SavasModu(true);
            if (oluCesetlerGrubu != null) oluCesetlerGrubu.SetActive(false);
            if (geriDonButonu != null) geriDonButonu.SetActive(false);
        }
    }

    public void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void TekrarDene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AnaSayfayaDon()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("AnaSayfa");
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
        yield return new WaitForSeconds(1f);
        if (playerAnimator != null) playerAnimator.SetTrigger("reload");
        if (playerStats != null) playerStats.Heal(1000);

        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime / kararmaSuresi;
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        if (canliDusmanlarGrubu != null) canliDusmanlarGrubu.SetActive(false);
        if (oluCesetlerGrubu != null) oluCesetlerGrubu.SetActive(true);
        if (combatUI != null) combatUI.SetActive(false);
        if (playerArms != null) playerArms.SetActive(false);

        oda1Temizlendi = true;
        yield return new WaitForSeconds(1.0f);

        while (alpha > 0)
        {
            alpha -= Time.deltaTime / acilmaSuresi;
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        if (geriDonButonu != null) geriDonButonu.SetActive(true);
        SaveManager.Kaydet(true);

        if (EnvanterManager.Instance != null) EnvanterManager.Instance.SavasModu(false);
    }

    public void KoridoraDon()
    {
        sonCikisKapisi = "Oda1"; // Artık direkt buna erişebiliriz
        SceneManager.LoadScene("SampleScene");
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Giriş Ara Sahnesi Görselleri")]
    public Sprite[] introGorselleri;

    void Start()
    {
        if (EnvanterManager.Instance != null)
        {
            Destroy(EnvanterManager.Instance.gameObject);
        }
    }

    public void PlayGame()
    {
        if (!GameManager.AraSahneIzlendiMi("Giris"))
        {
            CutsceneSettings.oynatilacakGorseller = introGorselleri;

            // Her resim için anahtar grupları (Örn: 1. resim için 2 cümle, 2. için 1 cümle)
            CutsceneSettings.metinGruplari = new string[][]
            {
            new string[] { "INTRO_1_1", "INTRO_1_2" }, // 1. Resim metinleri
            new string[] { "INTRO_2_1", "INTRO_2_2" },  // 2. Resim metinleri
            new string[] { "INTRO_3_1", "INTRO_3_2" },
            new string[] { "INTRO_4_1", "INTRO_4_2" }// ... resim sayın kadar devam etmeli
            };

            CutsceneSettings.sonrakiSahne = "SampleScene";
            CutsceneSettings.mevcutAraSahneID = "Giris";
            SceneManager.LoadScene("AraSahne");
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan çıkıldı.");
        Application.Quit();
    }
}
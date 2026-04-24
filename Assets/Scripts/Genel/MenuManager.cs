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
        // DÜZELTME: GlobalData yerine GameManager yazıyoruz
        if (!GameManager.AraSahneIzlendiMi("Giris"))
        {
            CutsceneSettings.oynatilacakGorseller = introGorselleri;
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
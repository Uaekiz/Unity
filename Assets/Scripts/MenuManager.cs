using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için bu kütüphane þart!

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
       
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan çýkýldý.");
        Application.Quit(); // Bu sadece gerçek oyunda (exe/apk) çalýþýr
    }
}
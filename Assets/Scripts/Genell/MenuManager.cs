using UnityEngine;
using UnityEngine.SceneManagement; // Sahne y�netimi i�in bu k�t�phane �art!

public class MenuManager : MonoBehaviour
{

    void Start()
    {
        if (EnvanterManager.Instance != null)
        {
            Destroy(EnvanterManager.Instance.gameObject);
        }
    }
    public void PlayGame()
    {

        GlobalData.sonCikisKapisi = "";
        
        if (GlobalData.oyunDurumlari != null)
        {
            GlobalData.oyunDurumlari.Clear();
        }

        GameManager.oda1Temizlendi = false;
       
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan ��k�ld�.");
        Application.Quit(); // Bu sadece ger�ek oyunda (exe/apk) �al���r
    }

    
}
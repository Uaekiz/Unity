using UnityEngine;
using UnityEngine.SceneManagement;

public class KoridorManager : MonoBehaviour
{
    public GameObject oyuncu; // Senin karakterin
    
    public Transform noktaOda1Onu;

    void Start()
    {
        string gelenKapi = GlobalData.sonCikisKapisi;

        if (gelenKapi == "Oda1")
        {
            oyuncu.transform.position = noktaOda1Onu.position;
            // İstersen burada oyuncunun yönünü de kapıya dönük yapabilirsin
        }
    }

    
}
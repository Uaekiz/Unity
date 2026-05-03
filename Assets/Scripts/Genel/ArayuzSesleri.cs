using UnityEngine;

public class ArayuzSesleri : MonoBehaviour
{
    // Her yerden ulaşabilmemiz için sihirli kelime (Singleton)
    public static ArayuzSesleri Instance; 

    public AudioSource arayuzSesKaynagi;

    void Awake()
    {
        // Kod başlarken kendini tanıtsın
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PanelSesiCal(AudioClip sesDosyasi)
    {
        if (arayuzSesKaynagi != null && sesDosyasi != null)
        {
            arayuzSesKaynagi.PlayOneShot(sesDosyasi);
        }
    }
}
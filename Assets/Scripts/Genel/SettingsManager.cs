using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    // Singleton yapısı: Her yerden kolayca ulaşabilmemizi sağlar
    public static SettingsManager Instance;

    [Header("UI Referansları")]
    public GameObject ayarlarPaneli;

    void Awake()
    {
        // Eğer sahnede zaten bir Ayarlar yöneticisi varsa, yenisini yok et (Kopya oluşumunu engeller)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ölüm ekranında veya odalarda silinmesini engeller
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Bu fonksiyonu butonlara bağlayacağız
    public void AyarlariAc()
    {
        if (ayarlarPaneli != null)
        {
            ayarlarPaneli.SetActive(true);
        }
    }

    // Çarpı (X) veya Geri butonuna bağlayacağız
    public void AyarlariKapat()
    {
        if (ayarlarPaneli != null)
        {
            ayarlarPaneli.SetActive(false);
        }
    }
}
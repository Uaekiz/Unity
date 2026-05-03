using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    // Singleton yapısı: Her yerden kolayca ulaşabilmemizi sağlar
    public static SettingsManager Instance;

    [Header("UI Referansları")]
    public GameObject ayarlarPaneli;

    [Header("Ses Ayarlari")]
    public float anlikMuzikSesi = 1f; // Müzik eklendiğinde bu değeri okuyacağız

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

    // Mevcut genel ses ayarın (Ana Ses için)
    public void SesSeviyesiniAyarla(float sesDegeri)
    {
        AudioListener.volume = sesDegeri; 
    }

    // YENİ: Sadece müzik slider'ı için hazırlık
    public void MuzikSeviyesiniAyarla(float muzikDegeri)
    {
        anlikMuzikSesi = muzikDegeri;
        
        // İleride müziği eklediğinde buraya şu tarz bir kod gelecek:
        // if(arkaPlanMuzigi != null) arkaPlanMuzigi.volume = anlikMuzikSesi;
    }
}
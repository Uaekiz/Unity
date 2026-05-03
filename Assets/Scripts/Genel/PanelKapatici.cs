using UnityEngine;

public class PanelKapatici : MonoBehaviour
{
    [Header("Ayarlar")]
    public GameObject kapanacakPanel; // Kapatılacak olan arayüz
    public AudioClip kapanmaSesi;     // Çalınacak ses

    // Bu metodu X butonuna bağlayacağız
    public void Kapat()
    {
        // 1. Önce sesi ölümsüz yöneticimize (Singleton) fırlat
        if (ArayuzSesleri.Instance != null && kapanmaSesi != null)
        {
            ArayuzSesleri.Instance.PanelSesiCal(kapanmaSesi);
        }

        // 2. Ardından paneli gizle
        if (kapanacakPanel != null)
        {
            kapanacakPanel.SetActive(false);
        }
    }
}
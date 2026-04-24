using UnityEngine;

public class AyarlarTetikleyici : MonoBehaviour
{
    // Bu fonksiyonu Ölüm Ekranındaki (veya oyundaki diğer) ayarlar butonuna bağlayacağız
    public void AyarlariAc()
    {
        // Eğer SettingsManager diğer sahneden başarıyla bizimle geldiyse paneli aç
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.AyarlariAc();
        }
        else
        {
            Debug.LogWarning("SettingsManager bulunamadı! Ana menüden başlanmamış olabilir.   ");
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Gerekli Referanslar")]
    public PlayerStats playerStats;      
    public GameObject combatUI;          
    public GameObject playerArms;        
    public Image fadePanel;
    
    // YENİ: Karakterin animasyonunu kontrol etmek için
    public Animator playerAnimator; 

    [Header("Düşman Takibi")]
    public int toplamDusmanSayisi;       
    private int olenDusmanSayisi = 0;

    [Header("Sinematik Ayarları")]
    public float kararmaSuresi = 2.5f; // Animasyonun süresine yakın olmalı
    public float acilmaSuresi = 2.0f;  

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void DusmanOldu()
    {
        olenDusmanSayisi++;
        
        if (olenDusmanSayisi >= toplamDusmanSayisi)
        {
            StartCoroutine(OlayYeriIncelemeModunaGec());
        }
    }

    IEnumerator OlayYeriIncelemeModunaGec()
    {
        Debug.Log("Çatışma bitti.");

        // 1. Önce çok kısa bir "Es" verelim (Son düşman yere düşsün)
        yield return new WaitForSeconds(1f); 

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("reload"); 
        }

        // 3. Canı Fulle (Arka planda)
        if (playerStats != null) playerStats.Heal(1000);

        // 4. EKRANI YAVAŞÇA KARART (Fade Out)
        // Animasyon oynarken aynı anda burası çalışacak (Paralel işlem)
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime / kararmaSuresi; 
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // --- EKRAN ŞU AN SİMSİYAH ve ANİMASYON BİTTİ ---
        
        // 5. Fazlalıkları Kapat
        if (combatUI != null) combatUI.SetActive(false); 
        if (playerArms != null) playerArms.SetActive(false); 

        yield return new WaitForSeconds(1.0f); // Karanlıkta bekleme

        // 6. EKRANI AÇ (Fade In)
        while (alpha > 0)
        {
            alpha -= Time.deltaTime / acilmaSuresi;
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        Debug.Log("ARTIK İNCELEME MODUNDASIN!");
    }
}
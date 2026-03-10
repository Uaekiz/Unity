using UnityEngine;
using UnityEngine.UI; // UI bileşenlerine erişmek için şart

public class PlayerStats : MonoBehaviour
{
    [Header("Genel Ayarlar")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI Ayarları")]
    public Image damageOverlay; // Hazırladığımız Image'ı buraya sürükleyeceğiz

    [Header("Durum")]
    public bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateDamageVisual(); // Başlangıçta ekranı temizle
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Oyuncu Vuruldu! Kalan Can: " + currentHealth);

        UpdateDamageVisual(); // Her hasar aldığında görseli güncelle

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateDamageVisual(); // Can dolunca da ekranı temizle
    }

    // --- EKRAN KARARTMA FONKSİYONU ---
    void UpdateDamageVisual()
    {
        if (damageOverlay != null)
        {
            // Can yüzdesini hesapla (0 ile 1 arası)
            // 100 canda -> 1.0 | 0 canda -> 0.0
            float healthPercentage = currentHealth / maxHealth;

            // Opaklık (Alpha) canın tersi olmalı. 
            // Can 1 iken Alpha 0 (görünmez), Can 0 iken Alpha 1 (tam görünür)
            float alpha = 1 - healthPercentage;

            // Image'ın rengini koru ama Alpha değerini güncelle
            Color tempColor = damageOverlay.color;
            tempColor.a = alpha;
            damageOverlay.color = tempColor;
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("OYUNCU ÖLDÜ! GAME OVER.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
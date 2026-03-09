using UnityEngine;
using UnityEngine.UI; // İleride Can barı eklersen lazım olur

public class PlayerStats : MonoBehaviour
{
    [Header("Genel Ayarlar")]
    public float maxHealth = 100f;   // Maksimum Can
    public float currentHealth;      // Şu anki Can

    [Header("Durum")]
    public bool isDead = false;

    void Start()
    {
        // Oyun başlarken canı fulle
        currentHealth = maxHealth;
    }

    // --- HASAR ALMA FONKSİYONU ---
    // Bu fonksiyonu Düşmanlar çağıracak
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Oyuncu Vuruldu! Kalan Can: " + currentHealth);

        // İleride buraya ekranı kızartma veya can barı düşürme kodu ekleyeceğiz.

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // --- İYİLEŞME FONKSİYONU ---
    // Belki ileride can kiti alırsa
    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
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
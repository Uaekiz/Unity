using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Can Ayarları")]
    public float maxHealth = 100f;
    private float currentHealth;
    public bool isDead = false;

    [Header("Referanslar")]
    public Animator animator;
    public Collider2D headCollider; // Kafadaki Circle Collider
    public Transform firePoint;     // Merminin çıkacağı yer (Opsiyonel)

    [Header("Saldırı Ayarları")]
    public float burstRate = 0.2f;    // Seri atış hızı (Mermiler arası süre)
    public float cooldownTime = 1.5f; // 6 mermiden sonraki bekleme süresi

    void Start()
    {
        currentHealth = maxHealth;
        
        // Oyun başlar başlamaz senaryoyu başlat
        StartCoroutine(CombatRoutine());
    }

    // --- SENARYO DÖNGÜSÜ ---
    IEnumerator CombatRoutine()
    {
        // 1. ADIM: SİLAHI ÇEK (Sadece 1 kere)
        yield return new WaitForSeconds(0.1f); // Çok kısa bekle ki Animator hazırlansın
        animator.SetTrigger("Saldiri");

        // Silah çekme animasyonu bitene kadar bekle (Tahmini 1-2 saniye)
        // Burayı animasyonunun tam süresine göre ayarla!
        yield return new WaitForSeconds(1.35f); 

        // 2. ADIM: SAVAŞ DÖNGÜSÜ
        while (!isDead)
        {
            // 6 Kere Ateş Et (Burst Fire)
            for (int i = 0; i < 6; i++)
            {
                if (isDead) break; // Ölürse döngüyü kır

                FireShot();
                
                // İki mermi arasındaki o kısa bekleme
                yield return new WaitForSeconds(burstRate);
            }

            // 6 Mermiyi sıktı, şimdi 1.5 saniye bekle
            if (!isDead)
            {
                yield return new WaitForSeconds(cooldownTime);
            }
        }
    }

    void FireShot()
    {
        // Ateş animasyonunu tetikle
        animator.SetTrigger("Ates");

        // BURAYA MERMİ VEYA RAYCAST KODUNU EKLEYEBİLİRSİN
        // Örn: Instantiate(mermi, firePoint.position, ...);
        // Veya ses çalma kodu.
    }

    // --- HASAR SİSTEMİ ---
    public void HasarAl(float damage, Collider2D vurulanCollider)
    {
        if (isDead) return; // Zaten ölüyse tekrar vurma

        // Kafa mı Gövde mi?
        if (vurulanCollider == headCollider)
        {
            // HEADSHOT - TEK ATAR
            Debug.Log("HEADSHOT!");
            Die(true); // true = Headshot
        }
        else
        {
            // BODY SHOT - Can Azaltır
            currentHealth -= damage;
            Debug.Log("Gövde hasarı. Kalan can: " + currentHealth);

            if (currentHealth <= 0)
            {
                Die(false); // false = Normal Ölüm
            }
            else
            {
                // Ölmediyse Hasar animasyonu oynat
                animator.SetTrigger("Hasar");
            }
        }
    }

    void Die(bool isHeadshot)
    {
        isDead = true;
        StopAllCoroutines(); // Ateş etme döngüsünü anında kes

        if (isHeadshot)
        {
            animator.SetTrigger("Headshot");
        }
        else
        {
            animator.SetTrigger("Olum");
        }

        // Cesede takılıp kalmayalım diye colliderları kapat
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        // Scripti kapat (Daha fazla işlem yapmasın)
        this.enabled = false;
    }
}
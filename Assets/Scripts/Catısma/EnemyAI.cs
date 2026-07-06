using System.Collections;
using System.Collections.Generic; // Listeleri kullanmak için gerekli
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // --- YENİ EKLENEN YAPI ---
    [System.Serializable]
    public struct SaldiriDeseni
    {
        public string aciklama;      // Kendine not (Örn: "3 sık 1 vur")
        public int toplamMermi;      // Bu turda kaç mermi sıkacak?
        public int isabetSayisi;     // Bunların kaçı %100 isabet edecek?
    }
    // -------------------------

    [Header("Düşman Tipi")]
    public bool siperKullanirMi = false;

    [Header("Can Ayarları")]
    public float maxHealth = 100f;
    private float currentHealth;
    public bool isDead = false;

    [Header("Örüntülü Saldırı Ayarları (YENİ)")]
    public List<SaldiriDeseni> saldiriDuzenleri; // Inspector'dan dolduracağın liste
    private int suankiDuzenIndex = 0; // Hangi sıradayız?

    [Header("Zamanlama Ayarları")]
    public float atisHizi = 0.15f;
    public float beklemeSuresi = 2.0f;
    public float baslamaGecikmesi = 1f;

    [Header("Siper Ayarları")]
    public float siperdenCikisSuresi = 0.5f; 
    public float sipereGirisSuresi = 0.5f;   

    [Header("Nişan ve Hasar")]
    public float oyuncuyaHasar = 10f;

    [Header("Referanslar")]
    public Animator animator;
    public Collider2D headCollider;
    public Transform firePoint;     
    public Transform player; 
    private PlayerStats playerStats;

    [Header("Düşman Sesleri")]
    public AudioSource dusmanSesKaynagi; // Düşmanın üzerindeki AudioSource
    public AudioClip tekElAtesSesi;

    // Gizlilik kontrolü
    private bool isAcikta = false; 

    void Start()
    {
        currentHealth = maxHealth;
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // --- YENİ EKLENEN KISIM ---
        // Oyun başlarken senin can scriptini bulup hafızaya alıyor
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
        }
        // --------------------------

        StartCoroutine(CombatRoutine());
    }

    void Update()
    {
        if (!isDead && player != null) FacePlayer();
    }

    void FacePlayer()
    {
        float absScaleX = Mathf.Abs(transform.localScale.x);
        float currentScaleY = transform.localScale.y;
        float currentScaleZ = transform.localScale.z;

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(absScaleX, currentScaleY, currentScaleZ);
        else
            transform.localScale = new Vector3(-absScaleX, currentScaleY, currentScaleZ);
    }

    IEnumerator CombatRoutine()
    {
        yield return new WaitForSeconds(baslamaGecikmesi);

        // Karakteri ilk kez saldırı pozisyonuna sok
        animator.SetTrigger("Saldiri");
        if (!siperKullanirMi) isAcikta = true;

        yield return new WaitForSeconds(1f);

        while (!isDead)
        {
            SaldiriDeseni mevcutDuzen = saldiriDuzenleri[suankiDuzenIndex];
            List<bool> mermiSonuclari = new List<bool>();
            for (int i = 0; i < mevcutDuzen.toplamMermi; i++)
                mermiSonuclari.Add(i < mevcutDuzen.isabetSayisi);
            Karistir(mermiSonuclari);

            // --- SİPERDEN ÇIKMA ---
            if (siperKullanirMi)
            {
                animator.SetTrigger("Cikis");
                yield return new WaitForSeconds(siperdenCikisSuresi);
                isAcikta = true;
            }

            // --- ATEŞ ETME ---
            for (int i = 0; i < mevcutDuzen.toplamMermi; i++)
            {
                if (isDead) break;
                FireShot(mermiSonuclari[i]);
                // Her atıştan sonra animator otomatik olarak Ates_Bekleme'ye düşecek
                yield return new WaitForSeconds(atisHizi);
            }

            StartCoroutine(SesiYumusakcaKes(0.06f));

            // --- SAVAŞ SONRASI BEKLEME ---
            if (siperKullanirMi && !isDead)
            {
                isAcikta = false;
                animator.SetTrigger("Giris");
                yield return new WaitForSeconds(sipereGirisSuresi);
            }
            else
            {
                // Siper kullanmıyorsa, ateş bittiğinde karakter Ates_Bekleme'de 
                // heykel gibi silahı doğrultmuş şekilde bekleyecek.
            }

            suankiDuzenIndex = (suankiDuzenIndex + 1) % saldiriDuzenleri.Count;

            if (!isDead) yield return new WaitForSeconds(beklemeSuresi);

            // Bekleme süresi bittiğinde döngü başa döner ve tekrar ateş başlar.
        }
    }

    IEnumerator SesiYumusakcaKes(float fadeSuresi = 0.05f)
    {
        if (dusmanSesKaynagi != null && dusmanSesKaynagi.isPlaying)
        {
            float baslangicSes = dusmanSesKaynagi.volume;
            
            for (float t = 0; t < fadeSuresi; t += Time.deltaTime)
            {
                dusmanSesKaynagi.volume = Mathf.Lerp(baslangicSes, 0f, t / fadeSuresi);
                yield return null;
            }

            dusmanSesKaynagi.Stop();
            dusmanSesKaynagi.volume = baslangicSes; // Sonraki atışlar için sesi eski haline getir
        }
    }

    void FireShot(bool isabetEtsinMi)
    {
        // Ates_Bekleme'den çıkıp ateş animasyonuna girmesi için trigger'ı ateşliyoruz
        animator.SetTrigger("Ates");

        if (dusmanSesKaynagi != null && tekElAtesSesi != null)
        {
            dusmanSesKaynagi.volume = 1f; // Sesi fulle (önceki fade-out'tan kalma olmasın)
            dusmanSesKaynagi.pitch = Random.Range(0.96f, 1.04f); // Hafif ton çeşitliliği
            dusmanSesKaynagi.clip = tekElAtesSesi;
            dusmanSesKaynagi.Play(); 
        }

        if (firePoint != null && player != null)
        {
            Vector2 directionToPlayer = (player.position - firePoint.position).normalized;
            Vector2 finalDirection;

            if (isabetEtsinMi)
            {
                finalDirection = directionToPlayer;
                if (playerStats != null) playerStats.TakeDamage(oyuncuyaHasar);
            }
            else
            {
                float zorunluSapma = Random.Range(0, 2) == 0 ? 15f : -15f;
                finalDirection = Quaternion.Euler(0, 0, zorunluSapma) * directionToPlayer;
            }
            Debug.DrawRay(firePoint.position, finalDirection * 50f, isabetEtsinMi ? Color.red : Color.yellow, 0.1f);
        }
    }

    // Listeyi rastgele karıştırma fonksiyonu (Shuffle)
    void Karistir<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void HasarAl(float damage, Collider2D vurulanCollider)
    {
        if (isDead) return;

        if (vurulanCollider == headCollider)
        {
            Die(true); 
        }
        else
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die(false);
            }
            else
            {
                if (isAcikta)
                {
                    animator.ResetTrigger("Hasar"); 
                    animator.SetTrigger("Hasar");
                }
            }
        }
    }

    void Die(bool isHeadshot)
    {
        isDead = true;
        StopAllCoroutines();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DusmanOldu();
        }
        
        if (isHeadshot) animator.SetTrigger("Headshot");
        else animator.SetTrigger("Olum");
        
        foreach (Collider2D col in GetComponents<Collider2D>()) col.enabled = false;
        this.enabled = false;
    }
}
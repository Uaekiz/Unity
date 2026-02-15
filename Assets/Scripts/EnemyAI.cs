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
        
        animator.SetTrigger("Saldiri");
        if (!siperKullanirMi) isAcikta = true; 

        yield return new WaitForSeconds(1.5f); 

        // SAVAŞ DÖNGÜSÜ
        while (!isDead)
        {
            // 1. LİSTEDEN ŞU ANKİ ÖRÜNTÜYÜ AL
            if (saldiriDuzenleri.Count == 0)
            {
                Debug.LogError("Lütfen Inspector'dan Saldırı Düzenleri listesini doldur!");
                yield break;
            }

            SaldiriDeseni mevcutDuzen = saldiriDuzenleri[suankiDuzenIndex];

            // 2. VURUŞ LİSTESİNİ HAZIRLA (Matematiksel Hesap)
            // Örn: 3 mermi, 1 isabet ise -> [True, False, False] (Karışık sıralı)
            List<bool> mermiSonuclari = new List<bool>();
            
            for (int i = 0; i < mevcutDuzen.toplamMermi; i++)
            {
                if (i < mevcutDuzen.isabetSayisi)
                    mermiSonuclari.Add(true); // Vuracak
                else
                    mermiSonuclari.Add(false); // Iskalaması lazım
            }
            
            // Listeyi karıştır ki hep ilk mermiler vurmasın (Doğallık için)
            Karistir(mermiSonuclari);


            // --- SİPERDEN ÇIKMA ---
            if (siperKullanirMi)
            {
                animator.SetTrigger("Cikis");
                yield return new WaitForSeconds(0.2f); 
                isAcikta = true; 
                yield return new WaitForSeconds(siperdenCikisSuresi - 0.2f); 
            }

            // --- ATEŞ ETME (Belirlenen Düzene Göre) ---
            for (int i = 0; i < mevcutDuzen.toplamMermi; i++)
            {
                if (isDead) break;
                
                // Merminin akıbetini (Vuracak mı, Iskalayacak mı) gönderiyoruz
                FireShot(mermiSonuclari[i]);
                
                yield return new WaitForSeconds(atisHizi);
            }

            // --- SİPERE GİRME ---
            if (siperKullanirMi && !isDead)
            {
                isAcikta = false; 
                animator.SetTrigger("Giris"); 
                yield return new WaitForSeconds(sipereGirisSuresi); 
            }

            // --- SIRADAKİ ÖRÜNTÜYE GEÇ ---
            // Listenin sonuna geldiysek başa dön (Modülo işlemi)
            suankiDuzenIndex = (suankiDuzenIndex + 1) % saldiriDuzenleri.Count;

            if (!isDead) yield return new WaitForSeconds(beklemeSuresi);
        }
    }

    void FireShot(bool isabetEtsinMi)
    {
        animator.SetTrigger("Ates");

        // Görsel efekt için yön hesabı (Sadece kırmızı çizgi çıksın diye)
        if (firePoint != null && player != null)
        {
            Vector2 directionToPlayer = (player.position - firePoint.position).normalized;
            Vector2 finalDirection;

            if (isabetEtsinMi)
            {
                // --- İSABET DURUMU ---
                finalDirection = directionToPlayer;
                Debug.DrawRay(firePoint.position, finalDirection * 50f, Color.red, 0.1f);

                // FİZİK YOK, DİREKT HASAR VAR!
                // Eğer oyuncunun can scriptini bulduysak, direkt canını yak.
                if (playerStats != null)
                {
                    playerStats.TakeDamage(oyuncuyaHasar);
                }
            }
            else
            {
                // --- ISKA DURUMU ---
                // Mermiyi bilerek yamuk atıyoruz, oyuncuya değmiyor.
                float zorunluSapma = Random.Range(0, 2) == 0 ? 15f : -15f;
                finalDirection = Quaternion.Euler(0, 0, zorunluSapma) * directionToPlayer;
                
                // Sarı çizgi (Iska)
                Debug.DrawRay(firePoint.position, finalDirection * 50f, Color.yellow, 0.1f);
                
                // Burada hasar verme kodu YOK. Sadece görsel çizgi var.
            }
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
        
        if (isHeadshot) animator.SetTrigger("Headshot");
        else animator.SetTrigger("Olum");
        
        foreach (Collider2D col in GetComponents<Collider2D>()) col.enabled = false;
        this.enabled = false;
    }
}
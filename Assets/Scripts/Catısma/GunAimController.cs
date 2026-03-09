using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GunAimController : MonoBehaviour
{
    [Header("Kontroller")]
    public Joystick joystick;
    private Animator animator;

    // Durum Kontrolü
    private bool isCurrentlyAiming = false;

    [Header("Hassasiyet Ayarları")]
    public float horizontalSensitivity = 6.0f;
    public float verticalSensitivity = 1.0f;
    private Vector3 initialPosition;

    [Header("Ateş ve Lazer Ayarları")]
    public Transform firePoint;       // Namlunun ucu (Inspector'dan ata)
    public LineRenderer lineRenderer; // Lazer çizgisi (Inspector'dan ata)
    public LayerMask hitLayers;       // Vurulabilir katmanlar
    public float range = 50f;         // Menzil
    public float fireRate = 0.25f;    // Atış hızı
    private float nextFireTime = 0f;

    void Start()
    {
        // Script ve Animator artık AYNI objede (holding) olduğu için:
        animator = GetComponent<Animator>();

        initialPosition = transform.localPosition;
        if (lineRenderer != null) lineRenderer.enabled = false;
    }

    void Update()
    {

        if (isReloading) return;
        // Joystick verilerini al (Pozisyon hesaplaması için)
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        // --- İŞTE İSTEDİĞİN DEĞİŞİKLİK BURADA ---
        // Eskiden magnitude > threshold diyorduk.
        // Şimdi diyoruz ki: Joystick'e parmak değiyor mu?
        // Değiyorsa (isTouched = true), hareket etmese bile nişan alıyordur.
        bool wantsToAim = joystick.isTouched;

        // 1. Nişan Almaya Başlama Anı
        if (wantsToAim && !isCurrentlyAiming)
        {
            isCurrentlyAiming = true;
            if (animator != null) animator.SetTrigger("StartAim");

            AimPosition(horizontal, vertical);
        }
        // 2. Nişan Almayı Bırakma Anı (Parmak çekildi)
        else if (!wantsToAim && isCurrentlyAiming)
        {
            isCurrentlyAiming = false;
            if (animator != null) animator.SetTrigger("StopAim");

            ResetPosition();
        }
        // 3. Nişan Almaya Devam Ediliyorsa
        else if (isCurrentlyAiming)
        {
            AimPosition(horizontal, vertical);
        }
        // 4. Normal Duruş
        else
        {
            ResetPosition();
        }
    }

    // --- SENİN ORİJİNAL HAREKET KODLARIN (DOKUNULMADI) ---
    void AimPosition(float x, float y)
    {
        float offsetX = x * horizontalSensitivity;
        float offsetY = y * verticalSensitivity;
        Vector3 aimOffset = new Vector3(offsetX, offsetY, 0);
        Vector3 targetPosition = initialPosition + aimOffset;

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 10f);
        transform.localRotation = Quaternion.identity;
    }

    void ResetPosition()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * 5f);
        transform.localRotation = Quaternion.identity;
    }

    // --- ATEŞ ETME SİSTEMİ (LAZERLİ) ---

    public AmmoManager ammoManager;
    public void ShootGun()
    {
        // EMNİYET: Elin joystickte değilse (nişan almıyorsan) ATEŞ ETME.
        // Joystick ortada olsa bile elin üstündeyse 'isCurrentlyAiming' true olacağı için burası çalışır.
        if (!isCurrentlyAiming)
        {
            return;
        }

        if (ammoManager != null && !ammoManager.CanShoot())
        {
            Debug.Log("Mermi Bitti!");
            return; // Mermi yoksa ateş etme, fonksiyonu burada bitir.
        }

        if (Time.time >= nextFireTime)
        {
            StartCoroutine(FireProcess());
            nextFireTime = Time.time + fireRate;

            if (ammoManager != null)
            {
                ammoManager.DecreaseAmmo();
            }
        }
    }

    // GunAimController.cs içindeki FireProcess IEnumerator'ı

    IEnumerator FireProcess()
    {
        // Ateş animasyonu
        if (animator != null) animator.SetTrigger("shoot");

        
        // Bu komut, firePoint'in tam olduğu koordinattaki collider'ı bulur.
        Collider2D hitCollider = Physics2D.OverlapPoint(firePoint.position, hitLayers);

        // Görsel Efekt (Line Renderer yerine anlık bir "Flash" veya nokta koyabilirsin)
        // Eğer LineRenderer kullanacaksan bile sadece namlu ucunda küçük bir çizgi olmalı.
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, firePoint.position);
            // Lazer ileri gitmez, sadece namlunun ucunda ufak bir parlama yapar
            lineRenderer.SetPosition(1, firePoint.position + (Vector3.forward * 0.1f)); 
        }

        if (hitCollider != null)
        {
            // BİR ŞEYİ VURDUK!
            Debug.Log("Vurulan: " + hitCollider.name);

            // Düşman scriptini al
            EnemyAI dusman = hitCollider.GetComponent<EnemyAI>();
            
            if (dusman != null)
            {
                // Vurulan collider'ı gönderiyoruz (Kafa mı gövde mi anlasın diye)
                dusman.HasarAl(21f, hitCollider); 
            }
        }
        else
        {
            Debug.Log("Hiçbir Şey Vurulmadı!");
        }

        yield return new WaitForSeconds(0.05f);
        if (lineRenderer != null) lineRenderer.enabled = false;
    }
    private bool isReloading = false;

   

    public void StartReload()
    {
        if (!isReloading)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        isCurrentlyAiming = false; // Nişan almayı bırak

        if (animator != null) animator.SetTrigger("reload");

        // Animasyonun süresi kadar bekle (Örn: 1.5 saniye)
        yield return new WaitForSeconds(2.16f);

        ammoManager.ResetAmmo();
        isReloading = false; // Joystick kilidini aç
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GunAimController : MonoBehaviour
{
    [Header("Kontroller")]
    public Joystick joystick;
    private Animator animator;

    [Header("Nişan Alma Hassasiyeti")]
    public float aimThreshold = 0.1f; // Biraz tolerans tanıdık
    public float horizontalSensitivity = 6.0f;
    public float verticalSensitivity = 1.0f;
    private Vector3 initialPosition;
    
    // Durum kontrolü (Senin eski değişkenin)
    private bool isCurrentlyAiming = false;

    [Header("Lazer ve Ateş Ayarları")]
    public Transform firePoint;       // Namlunun ucu (Inspector'dan ata)
    public LineRenderer lineRenderer; // Lazer çizgisi (Inspector'dan ata)
    public LayerMask hitLayers;       // Neleri vurabilir?
    public float range = 50f;         // Menzil
    public float fireRate = 0.25f;    // Atış hızı
    private float nextFireTime = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        initialPosition = transform.localPosition;

        // Lazeri başlangıçta kapat
        if (lineRenderer != null) lineRenderer.enabled = false;
    }

    void Update()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        float inputMagnitude = new Vector2(horizontal, vertical).magnitude;

        // Eşik değerini geçti mi?
        bool wantsToAim = inputMagnitude > aimThreshold;

        // --- SENİN ESKİ ANİMASYON MANTIĞIN ---

        // 1. Nişan Almaya Başlama Anı
        if (wantsToAim && !isCurrentlyAiming)
        {
            isCurrentlyAiming = true;
            
            // Senin Trigger'ın: StartAim
            if(animator != null) animator.SetTrigger("StartAim"); 

            AimPosition(horizontal, vertical);
        }
        // 2. Nişan Almayı Bırakma Anı
        else if (!wantsToAim && isCurrentlyAiming)
        {
            isCurrentlyAiming = false;

            // Senin Trigger'ın: StopAim
            if(animator != null) animator.SetTrigger("StopAim");

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

    // --- SENİN ESKİ HAREKET FONKSİYONLARIN (AYNEN KORUNDU) ---
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

    // --- YENİ EKLENEN ATEŞ ETME FONKSİYONU ---
    // UI Butonuna bunu bağlayacaksın
    public void ShootGun()
    {
        // 1. GÜVENLİK KİLİDİ: Eğer karakter şu an nişan almıyorsa (isCurrentlyAiming false ise)
        // Ateş etme tuşuna basılsa bile burası çalışmayı durdurur.
        if (!isCurrentlyAiming) 
        {
            return; 
        }

        // 2. Zamanlayıcı Kontrolü
        if (Time.time >= nextFireTime)
        {
            StartCoroutine(FireProcess());
            nextFireTime = Time.time + fireRate;
        }
    }

    IEnumerator FireProcess()
    {
        // Senin Trigger'ın: shoot
        if (animator != null) animator.SetTrigger("shoot");

        // --- LAZER / RAYCAST İŞLEMİ ---
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, firePoint.position);

            // Işın gönder
            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, range, hitLayers);

            if (hit.collider != null)
            {
                // Bir şeye çarptı
                lineRenderer.SetPosition(1, hit.point);
                Debug.Log("Vurulan: " + hit.collider.name);

                // Target scripti varsa hasar ver
                Target hedef = hit.collider.GetComponent<Target>();
                if (hedef != null)
                {
                    hedef.HasarAl(10f);
                }
            }
            else
            {
                // Boşa gitti
                lineRenderer.SetPosition(1, firePoint.position + firePoint.right * range);
            }

            // Lazerin ekranda kalma süresi (Görsel efekt)
            yield return new WaitForSeconds(0.05f);
            lineRenderer.enabled = false;
        }
    }
}
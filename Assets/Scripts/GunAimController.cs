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
        animator = GetComponent<Animator>();
        initialPosition = transform.localPosition;

        if (lineRenderer != null) lineRenderer.enabled = false;
    }

    void Update()
    {
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
            if(animator != null) animator.SetTrigger("StartAim");
            
            AimPosition(horizontal, vertical);
        }
        // 2. Nişan Almayı Bırakma Anı (Parmak çekildi)
        else if (!wantsToAim && isCurrentlyAiming)
        {
            isCurrentlyAiming = false;
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
    public void ShootGun()
    {
        // EMNİYET: Elin joystickte değilse (nişan almıyorsan) ATEŞ ETME.
        // Joystick ortada olsa bile elin üstündeyse 'isCurrentlyAiming' true olacağı için burası çalışır.
        if (!isCurrentlyAiming) 
        {
            return; 
        }

        if (Time.time >= nextFireTime)
        {
            StartCoroutine(FireProcess());
            nextFireTime = Time.time + fireRate;
        }
    }

    IEnumerator FireProcess()
    {
        // Ateş Animasyonu
        if (animator != null) animator.SetTrigger("shoot");

        // Lazer ve Vuruş İşlemi
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, firePoint.position);

            // Unity Ayarlarından "Physics 2D > Queries Start In Colliders" kapalı olsun!
            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, range, hitLayers);

            if (hit.collider != null)
            {
                // Vurduk
                lineRenderer.SetPosition(1, hit.point);
                Debug.Log("Vurulan: " + hit.collider.name);

                // Hedef scripti varsa hasar ver
                Target hedef = hit.collider.GetComponent<Target>();
                if (hedef != null)
                {
                    hedef.HasarAl(10f);
                }
            }
            else
            {
                // Boşa sıktık
                lineRenderer.SetPosition(1, firePoint.position + firePoint.right * range);
            }

            yield return new WaitForSeconds(0.05f);
            lineRenderer.enabled = false;
        }
    }
}
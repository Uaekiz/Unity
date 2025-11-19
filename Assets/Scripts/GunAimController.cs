using UnityEngine;
using UnityEngine.UI;

public class GunAimController : MonoBehaviour
{
    // Inspector'dan atayacaðýmýz bileþenler:
    public Joystick joystick;
    public SpriteRenderer gunSpriteRenderer;
    public Sprite holdGunSprite;
    public Sprite aimGunSprite;

    private Animator animator;

    public float aimThreshold = 0.05f;

    // YENÝ YATAY/DÝKEY HASSASÝYET DEÐÝÞKENLERÝ
    // Yatay (Sað/Sol) hareketin ne kadar kayacaðýný belirler
    public float horizontalSensitivity = 1.0f; 
    // Dikey (Yukarý/Aþaðý) hareketin ne kadar kayacaðýný belirler
    public float verticalSensitivity = 1.0f; 

    // Kolun baþlangýç pozisyonunu tutmak için
    private Vector3 initialPosition; 

    void Start()
    {
        animator = GetComponent<Animator>();
        // Script'in eklendiði objenin (Gun_System_Pivot) baþlangýç yerel pozisyonunu kaydeder.
        initialPosition = transform.localPosition; 

        // Baþlangýçta normal tutuþ sprite'ý ile baþla
        if (gunSpriteRenderer != null && holdGunSprite != null)
        {
            gunSpriteRenderer.sprite = holdGunSprite;
        }
    }

    void Update()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        float inputMagnitude = new Vector2(horizontal, vertical).magnitude;

        // Joystick hareket ettiðinde (niþan alma moduna geçiþ)
        if (inputMagnitude > aimThreshold)
        {
            animator.SetBool("IsAiming", true);

            // Sprite deðiþtirme
            if (gunSpriteRenderer != null && aimGunSprite != null)
            {
                gunSpriteRenderer.sprite = aimGunSprite;
            }
            // Pozisyon Kaydýrma
            AimPosition(horizontal, vertical);
        }
        // Joystick býrakýldýðýnda (normal tutuþ moduna dönüþ)
        else
        {
            animator.SetBool("IsAiming", false);

            // Sprite deðiþtirme
            if (gunSpriteRenderer != null && holdGunSprite != null)
            {
                gunSpriteRenderer.sprite = holdGunSprite;
            }
            // Baþlangýç pozisyonuna geri dönme
            ResetPosition();
        }
    }

    void AimPosition(float x, float y)
    {
        // YATAY KAYDIRMA HESAPLAMASI
        // x girdisi ile horizontalSensitivity çarpýlýr.
        float offsetX = x * horizontalSensitivity;
        
        // DÝKEY KAYDIRMA HESAPLAMASI
        // y girdisi ile verticalSensitivity çarpýlýr.
        float offsetY = y * verticalSensitivity;

        // Yeni Kaydýrma Ofseti (X ve Y için farklý hassasiyet kullanýlarak oluþturulur)
        Vector3 aimOffset = new Vector3(offsetX, offsetY, 0);

        // Hedef pozisyon: Baþlangýç pozisyonu + Kaydýrma Ofseti
        Vector3 targetPosition = initialPosition + aimOffset;

        // Yumuþak hareket için Lerp kullan
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 10f);

        // Tabancanýn açýsýný sabit tut (dönmesini engeller)
        transform.localRotation = Quaternion.identity; 
    }

    void ResetPosition()
    {
        // Baþlangýç pozisyonuna geri dön
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * 5f);

        // Açýyý sýfýrda tut
        transform.localRotation = Quaternion.identity;
    }
}
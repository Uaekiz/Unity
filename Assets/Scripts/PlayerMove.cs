using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 4f;
    private int moveDir = 0; // -1 = sol, 0 = dur, 1 = sağ
    bool left = false;
    private Rigidbody2D rb;
    private PlayerAnimationController _animationController;
    private float _minXLimit;
    private float _maxXLimit;

    // YENİ EKLEME: Karakterin yarı genişliğini hesaplamak için
    private float _charHalfWidth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        _animationController = GetComponentInChildren<PlayerAnimationController>();

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            // Collider'ın X boyutunun yarısını alıyoruz.
            _charHalfWidth = collider.size.x / 2.0f;
        }
        else
        {
            // BoxCollider2D yoksa, bir uyarı ver ve varsayılan bir değer kullan
            Debug.LogError("PlayerMove script'inin olduğu objede BoxCollider2D bulunamadı! Varsayılan genişlik kullanılıyor.");
            _charHalfWidth = 0.5f; // Varsayılan değer
        }

    }

    // Kamera kontrolcüsünden çağrılacak yeni metot.
    // Bu, PlayerMove'a sahenin sınırlarını bildirir.
    public void SetHorizontalLimits(float minX, float maxX)
    {
        _minXLimit = minX;
        _maxXLimit = maxX;
    }

    public void SetMovementDirection(int direction)
    {
        moveDir = direction;

        // Ekranda dokunma varsa kontrol et
        if (direction != 0)
        {
            left = direction < 0; // Eğer -1 ise sola bak
        }

        _animationController.UpdateMovementAnimation(moveDir,left);

    }

    void FixedUpdate()
    {
        // Fizik hareketi Rigidbody üzerinden yapıyoruz
        rb.linearVelocity = new Vector2(moveDir * speed, rb.linearVelocity.y);

        // YENİ SINIR KONTROLÜ: Karakterin sınırda durmasını sağla.

        // Sol sınır kontrolü: Karakter sola giderken ve KENARI sınıra ulaştıysa hızı kes.
        if (transform.position.x - _charHalfWidth <= _minXLimit && moveDir < 0)
        {
            // Hızı sıfırla
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            // Karakterin pozisyonunu tam sınıra ayarla, böylece içine geçmez
            transform.position = new Vector3(_minXLimit + _charHalfWidth, transform.position.y, transform.position.z);
        }

        // Sağ sınır kontrolü: Karakter sağa giderken ve KENARI sınıra ulaştıysa hızı kes.
        if (transform.position.x + _charHalfWidth >= _maxXLimit && moveDir > 0)
        {
            // Hızı sıfırla
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            // Karakterin pozisyonunu tam sınıra ayarla, böylece içine geçmez
            transform.position = new Vector3(_maxXLimit - _charHalfWidth, transform.position.y, transform.position.z);
        }

    }
}

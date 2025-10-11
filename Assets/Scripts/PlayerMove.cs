using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 4f;
    public Animator animator;
    private int moveDir = 0; // -1 = sol, 0 = dur, 1 = sağ
    bool left = false;
    private Rigidbody2D rb;
    private PlayerAnimationController _animationController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        _animationController = GetComponentInChildren<PlayerAnimationController>();

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
    }
}

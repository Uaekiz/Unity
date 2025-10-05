using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;
    public Animator animator;
    private int moveDir = 0; // -1 = sol, 0 = dur, 1 = sağ
    bool left = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveDir = 0; 

        // Ekranda dokunma varsa kontrol et
        foreach (Touch touch in Input.touches)
        {
            if (touch.position.y < Screen.height / 6)
            {
                if (touch.position.x > Screen.width / 2) // Ekranın sağ tarafına dokunursa
                {
                    moveDir = 1; 
                    left = false;
                }
                else if (touch.position.x < Screen.width / 2) // Ekran�n sol tarafına dokunursa
                {
                    moveDir = -1; 
                    left = true;
                }
            }
        }
        
        animator.SetInteger("moveDir", moveDir);
        animator.SetBool("left", left);

    }

    void FixedUpdate()
    {
        // Fizik hareketi Rigidbody üzerinden yapıyoruz
        rb.linearVelocity = new Vector2(moveDir * speed, rb.linearVelocity.y);
    }
}

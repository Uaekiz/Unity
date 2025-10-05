using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;          // Takip edilecek oyuncu
    public SpriteRenderer background; // Background sprite'� (soldan sa�a s�n�r i�in)

    private float minX;
    private float maxX;
    private float camHalfWidth;

    void Start()
    {
        if (player == null || background == null)
        {
            Debug.LogError("Player veya Background atanmad�!");
            return;
        }

        // Kamera yar� geni�li�i (d�nyadaki birim olarak)
        camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        // Background'un sol ve sa� s�n�rlar�
        float bgLeft = background.bounds.min.x;
        float bgRight = background.bounds.max.x;

        // Kameran�n gidebilece�i minimum ve maksimum X pozisyonlar�
        minX = bgLeft + camHalfWidth;
        maxX = bgRight - camHalfWidth;

        // Ba�lang��ta kameray� player'�n yan�na getir
        Vector3 startPos = transform.position;
        startPos.x = Mathf.Clamp(player.position.x, minX, maxX);
        transform.position = startPos;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Player'�n x pozisyonunu kameraya uyarla
        Vector3 newPos = transform.position;
        newPos.x = Mathf.Clamp(player.position.x, minX, maxX);
        transform.position = newPos;
    }
}

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;          // Takip edilecek oyuncu
    public SpriteRenderer background; // Background sprite'ý (soldan saða sýnýr için)

    private float minX;
    private float maxX;
    private float camHalfWidth;

    void Start()
    {
        if (player == null || background == null)
        {
            Debug.LogError("Player veya Background atanmadý!");
            return;
        }

        // Kamera yarý geniþliði (dünyadaki birim olarak)
        camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        // Background'un sol ve sað sýnýrlarý
        float bgLeft = background.bounds.min.x;
        float bgRight = background.bounds.max.x;

        // Kameranýn gidebileceði minimum ve maksimum X pozisyonlarý
        minX = bgLeft + camHalfWidth;
        maxX = bgRight - camHalfWidth;

        // Baþlangýçta kamerayý player'ýn yanýna getir
        Vector3 startPos = transform.position;
        startPos.x = Mathf.Clamp(player.position.x, minX, maxX);
        transform.position = startPos;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Player'ýn x pozisyonunu kameraya uyarla
        Vector3 newPos = transform.position;
        newPos.x = Mathf.Clamp(player.position.x, minX, maxX);
        transform.position = newPos;
    }
}

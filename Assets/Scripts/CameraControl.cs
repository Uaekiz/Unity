using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // ... (Deðiþkenler ayný kalýr)
    public Transform player;
    public GameObject backgroundParent;

    private float minX;
    private float maxX;
    private float camHalfWidth;

    private PlayerMove _playerMoveScript;

    void Start()
    {
        if (player == null || backgroundParent == null)
        {
            Debug.LogError("Player veya Background Parent atanmadý!");
            return;
        }

        // 1. PlayerMove script'ine referans al
        _playerMoveScript = player.GetComponent<PlayerMove>();

        // 2. Kamera yarý geniþliðini hesapla
        camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        // 3. PROFESYONEL ÝYÝLEÞTÝRME: Sýnýrlarý hesapla (minX ve maxX'in deðerlerini belirleyen blok)
        Bounds combinedBounds = new Bounds();
        bool first = true;
        SpriteRenderer[] childrenRenderers = backgroundParent.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer renderer in childrenRenderers)
        {
            if (first)
            {
                combinedBounds = renderer.bounds;
                first = false;
            }
            else
            {
                combinedBounds.Encapsulate(renderer.bounds);
            }
        }

        if (first)
        {
            Debug.LogError("Background Parent objesi altýnda SpriteRenderer bulunamadý!");
            return;
        }

        float bgLeft = combinedBounds.min.x;
        float bgRight = combinedBounds.max.x;

        // minX ve maxX deðerleri burada atanýr.
        minX = bgLeft + camHalfWidth;
        maxX = bgRight - camHalfWidth;

        // 4. HATA DÜZELTÝLDÝ: Hesaplanan sýnýrlarý PlayerMove'a gönder
        if (_playerMoveScript != null)
        {
            _playerMoveScript.SetHorizontalLimits(bgLeft, bgRight);
        }

        // 5. Baþlangýçta kamerayý player'ýn yanýna getir
        Vector3 startPos = transform.position;
        startPos.x = Mathf.Clamp(player.position.x, minX, maxX);
        transform.position = startPos;
    }

    void LateUpdate()
    {
        // ... (LateUpdate metodu doðru kalýr)
        if (player == null) return;

        // Kamerayý sýnýrlandýrýlmýþ X pozisyonuna taþý
        Vector3 camNewPos = transform.position;
        camNewPos.x = Mathf.Clamp(player.position.x, minX, maxX);
        transform.position = camNewPos;
    }
}
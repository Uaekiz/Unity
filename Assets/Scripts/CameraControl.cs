using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // ... (De�i�kenler ayn� kal�r)
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
            Debug.LogError("Player veya Background Parent atanmad�!");
            return;
        }

        // 1. PlayerMove script'ine referans al
        _playerMoveScript = player.GetComponent<PlayerMove>();

        // 2. Kamera yar� geni�li�ini hesapla
        camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        // 3. PROFESYONEL �Y�LE�T�RME: S�n�rlar� hesapla (minX ve maxX'in de�erlerini belirleyen blok)
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
            Debug.LogError("Background Parent objesi alt�nda SpriteRenderer bulunamad�!");
            return;
        }

        float bgLeft = combinedBounds.min.x;
        float bgRight = combinedBounds.max.x;

        // minX ve maxX de�erleri burada atan�r.
        minX = bgLeft + camHalfWidth;
        maxX = bgRight - camHalfWidth;

        // 4. HATA D�ZELT�LD�: Hesaplanan s�n�rlar� PlayerMove'a g�nder
        if (_playerMoveScript != null)
        {
            _playerMoveScript.SetHorizontalLimits(bgLeft, bgRight);
        }

        // 5. Ba�lang��ta kameray� player'�n yan�na getir
        Vector3 startPos = transform.position;
        startPos.x = Mathf.Clamp(player.position.x, minX, maxX);
        transform.position = startPos;
    }

    void LateUpdate()
    {
        // ... (LateUpdate metodu do�ru kal�r)
        if (player == null) return;

        // Kameray� s�n�rland�r�lm�� X pozisyonuna ta��
        Vector3 camNewPos = transform.position;
        camNewPos.x = Mathf.Clamp(player.position.x, minX, maxX);
        transform.position = camNewPos;
    }
}
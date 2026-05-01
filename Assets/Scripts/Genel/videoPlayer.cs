using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class IntroVideoManager : MonoBehaviour
{
    [Header("UI Referanslarý")]
    public GameObject combatUI;
    public GameObject envanterUI;
    public Image fadePanel;

    [Header("Ayarlar")]
    public float fadeHizi = 1.5f;
    public string videoID = "KoridorIntrosu"; // Bu videoya özel bir ID

    private VideoPlayer videoPlayer;
    private bool isTransitioning = false;

    void Awake()
    {
        // --- KRÝTÝK KONTROL: Video daha önce izlendi mi? ---
        if (GameManager.AraSahneIzlendiMi(videoID))
        {
            // Eðer izlendiyse video objesini anýnda yok et ve UI'larý aç
            HizliBaslat();
            return;
        }

        videoPlayer = GetComponent<VideoPlayer>();

        // Video oynayacaksa baþlangýç ayarlarý
        if (combatUI != null) combatUI.SetActive(false);
        if (envanterUI != null) envanterUI.SetActive(false);
        if (fadePanel != null)
        {
            fadePanel.color = new Color(0, 0, 0, 1);
            fadePanel.raycastTarget = true;
        }

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void Start()
    {
        // Eðer video objesi hala hayattaysa (izlenmemiþse) coroutine'i baþlat
        if (videoPlayer != null)
        {
            StartCoroutine(VideoBaslayincaSiyahiKaldir());
        }
    }

    // Video izlendiyse her þeyi normal haline getiren fonksiyon
    void HizliBaslat()
    {
        if (combatUI != null) combatUI.SetActive(true);
        if (envanterUI != null) envanterUI.SetActive(true);
        if (fadePanel != null)
        {
            fadePanel.color = new Color(0, 0, 0, 0);
            fadePanel.raycastTarget = false;
        }
        Destroy(gameObject); // Video oynatýcýyý sahneden sil
    }

    IEnumerator VideoBaslayincaSiyahiKaldir()
    {
        while (!videoPlayer.isPlaying)
        {
            yield return null;
        }
        if (fadePanel != null) fadePanel.color = new Color(0, 0, 0, 0);
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (!isTransitioning) StartCoroutine(GecisSekansi());
    }

    IEnumerator GecisSekansi()
    {
        isTransitioning = true;

        // 1. ADIM: EKRANI KARART
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeHizi;
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        videoPlayer.Stop();

        // 2. ADIM: Ýzlendi olarak iþaretle (Artýk koridora dönünce oynamayacak)
        GameManager.IzlendiOlarakIsaretle(videoID);

        if (combatUI != null) combatUI.SetActive(true);
        if (envanterUI != null) envanterUI.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        // 3. ADIM: EKRANI AÇ
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeHizi;
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadePanel.raycastTarget = false;
        Destroy(gameObject);
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame && videoPlayer != null && videoPlayer.isPlaying)
        {
            if (!isTransitioning) StartCoroutine(GecisSekansi());
        }
    }
}
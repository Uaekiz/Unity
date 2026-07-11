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
    public RawImage videoEkrani; // YENÝ: Canvas'ta oluþturduðumuz Raw Image'ý buraya baðlayacaðýz

    [Header("Ayarlar")]
    public float fadeHizi = 1.5f;
    public string videoID = "KoridorIntrosu";

    private VideoPlayer videoPlayer;
    private bool isTransitioning = false;

    void Awake()
    {
        // --- KRÝTÝK KONTROL: Video daha önce izlendi mi? ---
        if (GameManager.AraSahneIzlendiMi(videoID))
        {
            HizliBaslat();
            return;
        }

        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false; // YENÝ: Flicker engellemek için kodla baþlatacaðýz

        // Video oynayacaksa baþlangýç ayarlarý
        if (combatUI != null) combatUI.SetActive(false);
        if (envanterUI != null) envanterUI.SetActive(false);

        if (fadePanel != null)
        {
            fadePanel.color = new Color(0, 0, 0, 1);
            fadePanel.raycastTarget = true;
        }

        // YENÝ: Baþlangýçta video ekraný kapalý kalsýn, video hazýr olunca açacaðýz
        if (videoEkrani != null) videoEkrani.gameObject.SetActive(false);

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void Start()
    {
        if (videoPlayer != null && !GameManager.AraSahneIzlendiMi(videoID))
        {
            StartCoroutine(VideoGarantiliBaslat());
        }
    }

    // Video izlendiyse her þeyi normal haline getiren fonksiyon
    void HizliBaslat()
    {
        if (combatUI != null) combatUI.SetActive(true);
        if (envanterUI != null) envanterUI.SetActive(true);
        if (videoEkrani != null) videoEkrani.gameObject.SetActive(false); // YENÝ
        if (fadePanel != null)
        {
            fadePanel.color = new Color(0, 0, 0, 0);
            fadePanel.raycastTarget = false;
        }
        Destroy(gameObject);
    }

    // ESKÝ COROUTINE YERÝNE DAHA GÜVENLÝ VE PROFESYONEL BAÞLANGIÇ
    IEnumerator VideoGarantiliBaslat()
    {
        videoPlayer.Prepare(); // Videoyu önbelleðe al

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        // Video artýk hazýr! Ekraný aç ve oynat
        if (videoEkrani != null) videoEkrani.gameObject.SetActive(true);
        videoPlayer.Play();

        // En az 2 kare (frame) çizilene kadar siyah perdeyi kaldýrma (Flicker kesin çözüm)
        while (videoPlayer.frame < 2)
        {
            yield return null;
        }

        // Video görünmeye baþladý, artýk siyah perdeyi anýnda kaldýrabiliriz
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
        if (videoEkrani != null) videoEkrani.gameObject.SetActive(false); // YENÝ: Video ekranýný gizle

        // 2. ADIM: Ýzlendi olarak iþaretle
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
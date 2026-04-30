using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class IntroVideoManager : MonoBehaviour
{
    [Header("UI Referanslarý")]
    public GameObject combatUI;      // Can barý ve diðer savaþ arayüzleri
    public GameObject envanterUI;    // Envanterin ana objesi

    [Header("Geçiþ Ayarlarý")]
    public Image fadePanel;
    public float fadeHizi = 1.0f;

    private VideoPlayer videoPlayer;
    private bool isSkipping = false;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += VideoyuBitir;

        // 1. ADIM: Video baþladýðýnda UI'larý gizle
        

        if (fadePanel != null) fadePanel.color = new Color(0, 0, 0, 0);
    }

    void VideoyuBitir(VideoPlayer vp)
    {
        if (!isSkipping) StartCoroutine(GecisSekansi());
        if (combatUI != null) combatUI.SetActive(true);
        if (envanterUI != null) envanterUI.SetActive(true);
    }

    IEnumerator GecisSekansi()
    {
        isSkipping = true;

        // Ekraný karart
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeHizi;
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        videoPlayer.Stop();
        gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        // 2. ADIM: Ekran simsiyahken UI'larý tekrar aktif et
        // (Ekran açýldýðýnda hazýr bekliyor olacaklar)
        if (combatUI != null) combatUI.SetActive(true);
        if (envanterUI != null) envanterUI.SetActive(true);

        // Ekraný yavaþça aç
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeHizi;
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        Destroy(gameObject, 1f);
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame && videoPlayer.isPlaying)
        {
            if (!isSkipping) StartCoroutine(GecisSekansi());
        }
    }
}
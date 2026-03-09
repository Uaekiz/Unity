using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class BulmacaSurukle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Vector2 baslangicPozisyonu;

    [Header("Eþya Kimliði")]
    public string esyaID; // EnvanterManager ve SlotID ile birebir ayný olmalý

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Baþlangýç koordinatlarýný kaydet (anchoredPosition kullanýmý UI için daha stabildir)
        baslangicPozisyonu = rectTransform.anchoredPosition;

        // Sürüklenen eþyayý hiyerarþide en alta al ki diðer slotlarýn ÜSTÜNDE görünsün
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f; // Sürükleme hissi için biraz daha þeffaflýk
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Eþyayý baþlangýç yerine (slotuna) geri gönder
        rectTransform.anchoredPosition = baslangicPozisyonu;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }
}
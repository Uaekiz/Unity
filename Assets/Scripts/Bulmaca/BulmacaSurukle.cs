using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class BulmacaSurukle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 baslangicPozisyonu;
    private CanvasGroup canvasGroup;
    public string esyaID; // Burasý "KontrolKalemi" olmalý (boþluksuz!)

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        baslangicPozisyonu = transform.position; // Dönüþ yerini hatýrla
        canvasGroup.blocksRaycasts = false;      // Altýndaki slotun bizi görmesini saðla
        canvasGroup.alpha = 0.7f;                // Sürüklerken biraz þeffaf yap
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; // Fareyi takip et
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = baslangicPozisyonu; // Eþyayý her zaman yerine geri gönder
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Camera anaKamera;

    void Start()
    {
        anaKamera = Camera.main;
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        // New Input System ile tıklama veya dokunma kontrolü
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 tiklamaPozisyonu = Pointer.current.position.ReadValue();
            RaycastHit2D hit = Physics2D.Raycast(anaKamera.ScreenToWorldPoint(tiklamaPozisyonu), Vector2.zero);

            if (hit.collider != null)
            {
                // Eğer tıkladığımız objede EsyaSistemi varsa onu çalıştır
                EsyaSistemi esya = hit.collider.GetComponent<EsyaSistemi>();
                if (esya != null && !esya.koridordaMi)
                {
                    esya.EtkilesimeGir();
                }
            }
        }
    }
}
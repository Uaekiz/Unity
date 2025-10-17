using UnityEngine;
using UnityEngine.SceneManagement; // Sahne y�netimi i�in gerekli

public class SceneDoor : DoorController
{
    [Header("Scene Loading")]
    public string targetSceneName; // Inspector'dan hedef sahne ad�n� girin.

    // DoorController'daki TryToOpen() metodu isLocked kontrol�nden ge�tikten sonra
    // OnSuccessfulInteraction() metodunu �a��r�r. Biz onu ge�ersiz k�laca��z (override).
    protected override void OnSuccessfulInteraction()
    {
        base.OnSuccessfulInteraction(); // Temel s�n�f�n mant���n� �al��t�r (�imdilik bo�)

        //// Butonu gizle
        //if (interactButton != null)
        //{
        //    interactedButton.SetActive(false);
        //}

        // Sahneyi y�kle
        SceneManager.LoadScene(targetSceneName);
    }
}
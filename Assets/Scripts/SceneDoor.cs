using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için gerekli

public class SceneDoor : DoorController
{
    [Header("Scene Loading")]
    public string targetSceneName; // Inspector'dan hedef sahne adýný girin.

    // DoorController'daki TryToOpen() metodu isLocked kontrolünden geçtikten sonra
    // OnSuccessfulInteraction() metodunu çaðýrýr. Biz onu geçersiz kýlacaðýz (override).
    protected override void OnSuccessfulInteraction()
    {
        base.OnSuccessfulInteraction(); // Temel sýnýfýn mantýðýný çalýþtýr (þimdilik boþ)

        //// Butonu gizle
        //if (interactButton != null)
        //{
        //    interactedButton.SetActive(false);
        //}

        // Sahneyi yükle
        SceneManager.LoadScene(targetSceneName);
    }
}
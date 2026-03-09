using UnityEngine;
using UnityEngine.SceneManagement; // Sahne y�netimi i�in gerekli

public class SceneDoor : DoorController
{
    [Header("Scene Loading")]
    public string targetSceneName; // Inspector'dan hedef sahne ad�n� girin.

    protected override void OnSuccessfulInteraction()
    {
        base.OnSuccessfulInteraction(); 
        SceneManager.LoadScene(targetSceneName);
    }
}
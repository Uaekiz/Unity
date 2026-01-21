using UnityEngine;
using TMPro;

public class AmmoManager : MonoBehaviour
{
    public int currentAmmo = 15;
    public TextMeshProUGUI ammoText;

    // YENÝ EKLEDÝÐÝMÝZ KISIM: Reload butonu referansý
    public GameObject reloadButton;

    void Start()
    {
        UpdateUI();
    }

    public bool CanShoot()
    {
        return currentAmmo > 0;
    }

    public void DecreaseAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateUI();
        }
    }

    // YENÝ EKLEDÝÐÝMÝZ KISIM: Mermiyi fulleyen fonksiyon
    public void ResetAmmo()
    {
        currentAmmo = 15;
        UpdateUI();
    }

    void UpdateUI()
    {
        ammoText.text = currentAmmo.ToString();

        if (currentAmmo <= 0)
        {
            ammoText.color = Color.red;
            // Mermi 0 ise butonu göster
            if (reloadButton != null) reloadButton.SetActive(true);
        }
        else
        {
            ammoText.color = Color.white;
            // Mermi varsa butonu gizle
            if (reloadButton != null) reloadButton.SetActive(false);
        }
    }
}
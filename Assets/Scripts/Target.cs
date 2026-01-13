using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;

    // Bu fonksiyonu silah çağıracak
    public void HasarAl(float miktar)
    {
        health -= miktar;
        
        // Vurulduğunu anlamak için rengini kırmızı yapalım
        GetComponent<SpriteRenderer>().color = Color.red;
        
        // 0.1 saniye sonra rengi geri düzelt (Yanıp sönme efekti)
        Invoke("RengiDuzelt", 0.1f);

        if (health <= 0)
        {
            Oldu();
        }
    }

    void RengiDuzelt()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void Oldu()
    {
        // Hedef yok olsun
        Destroy(gameObject);
        Debug.Log("Hedef parçalandı!");
    }
}
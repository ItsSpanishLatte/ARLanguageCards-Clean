using UnityEngine;

public class QuizObject : MonoBehaviour
{
    [Header("Ayarlar")]
    public string nesneAdi; 
    
    // Tıklanma olayını yakalar
    void OnMouseDown()
    {
        // Yöneticiye bu nesnenin adını gönder
        if(GroundPlaneQuizManager.Instance != null)
        {
            // Buradaki değişken adını da güncelledik
            GroundPlaneQuizManager.Instance.CevapVer(nesneAdi, this.gameObject);
        }
    }
}
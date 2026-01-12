using UnityEngine;

public class QuizObject : MonoBehaviour
{
    [Header("Ayarlar")]
    public string nesneAdi; 
    
    void OnMouseDown()
    {
        if(GroundPlaneQuizManager.Instance != null)
        {
            GroundPlaneQuizManager.Instance.CevapVer(nesneAdi, this.gameObject);
        }
    }
}
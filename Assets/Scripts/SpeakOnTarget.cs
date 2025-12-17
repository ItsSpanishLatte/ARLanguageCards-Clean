using UnityEngine;

public class SpeakOnTarget : MonoBehaviour
{
    public string kelime; // Örn: Apple
    public string cumle;  // Örn: This is an apple

    public void SpeakNow()
    {
        if (TTSManager.Instance != null)
        {
            // Robotun hafızasına iki veriyi de gönderiyoruz
            TTSManager.Instance.KartTanimlaVeOku(kelime, cumle);
        }
    }
}
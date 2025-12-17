using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TTSManager : MonoBehaviour
{
    public static TTSManager Instance;
    public AudioSource audioSource;

    // HAFIZA (İkiye ayırdık)
    private string hafizaKelime = "";
    private string hafizaCumle = "";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 1. KART BULUNUNCA BU ÇAĞRILACAK (Otomatik Okuma)
    public void KartTanimlaVeOku(string kelime, string cumle)
    {
        hafizaKelime = kelime;
        hafizaCumle = cumle;

        // Kart ilk göründüğünde ikisini peş peşe okusun (Apple... This is an apple)
        Speak(kelime + ". " + cumle);
    }

    // 2. KELİME BUTONU İÇİN
    public void SadeceKelimeyiOku()
    {
        if (!string.IsNullOrEmpty(hafizaKelime)) Speak(hafizaKelime);
    }

    // 3. CÜMLE BUTONU İÇİN
    public void SadeceCumleyiOku()
    {
        if (!string.IsNullOrEmpty(hafizaCumle)) Speak(hafizaCumle);
    }

    // --- SES İNDİRME MOTORU (Aynı kalıyor) ---
    private void Speak(string text)
    {
        StartCoroutine(SesIndirVeCal(text));
    }

    IEnumerator SesIndirVeCal(string text)
    {
        string url = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&q=" + text + "&tl=en";
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TTSManager : MonoBehaviour
{
    public static TTSManager Instance;
    public AudioSource audioSource;

    [Header("Dil Ayarı (en=İng, de=Alm, tr=Tr)")]
    public string dilKodu = "en"; // Burayı Inspector'dan "de" yaparsan Almanca okur!

    // HAFIZA
    private string hafizaKelime = "";
    private string hafizaCumle = "";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void KartTanimlaVeOku(string kelime, string cumle)
    {
        hafizaKelime = kelime;
        hafizaCumle = cumle;
    }

    public void SadeceKelimeyiOku()
    {
        if (!string.IsNullOrEmpty(hafizaKelime)) Speak(hafizaKelime);
    }

    public void SadeceCumleyiOku()
    {
        if (!string.IsNullOrEmpty(hafizaCumle)) Speak(hafizaCumle);
    }

    public void Speak(string text)
    {
        StartCoroutine(SesIndirVeCal(text));
    }

    IEnumerator SesIndirVeCal(string text)
    {
        // URL'nin sonundaki "tl=" kısmına dilKodu değişkenini ekledik
        string url = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&q=" + text + "&tl=" + dilKodu;
        
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Ses İndirme Hatası (İnterneti Kontrol Et): " + www.error);
            }
        }
    }
}
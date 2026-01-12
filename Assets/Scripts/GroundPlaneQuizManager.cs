using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class GroundPlaneQuizManager : MonoBehaviour
{
    public static GroundPlaneQuizManager Instance;

    // --- KATEGORİ YAPISI ---
    [System.Serializable] 
    public class Kategori
    {
        public string kategoriAdi; 
        public List<GameObject> prefabs; 
    }

    [Header("Kategori Ayarları")]
    public List<Kategori> kategoriler; 

    [Header("UI Elemanları")]
    public TextMeshProUGUI soruText;
    public TextMeshProUGUI sonucText;
    public GameObject baslaButonu;

    [Header("Ayarlar")]
    public Transform spawnPoint; // Ground Plane Stage altındaki boş obje
    [Range(0.1f, 2.0f)]
    public float objeBoyutu = 0.5f; // Inspector'dan boyutu ayarla

    private GameObject dogruCevapObjesi;
    private string dogruCevapAdi;
    private List<GameObject> sahnedeOlusturulanlar = new List<GameObject>();

    void Awake() { Instance = this; }

    public void OyunAlaniYerlesti()
    {
        // Kareye tıklayınca burası çalışır
        if(baslaButonu != null) baslaButonu.SetActive(true);
        if(sonucText != null) sonucText.text = "Alan Yerleşti! Başla'ya bas.";
    }

    public void SoruyuBaslat() // Butona tıklayınca burası çalışır
    {
        if(baslaButonu != null) baslaButonu.SetActive(false);
        EskileriTemizle();

        // ADIM 1: Kategori Kontrolü
        if (kategoriler == null || kategoriler.Count == 0) {
            Debug.LogError("HATA: Kategori listesi boş!");
            return;
        }

        // ADIM 2: Spawn Point Kontrolü (Çok Önemli)
        if (spawnPoint == null) {
            Debug.LogError("HATA: Spawn Point atanmamış! Inspector'dan atama yap.");
            return;
        }

        // ADIM 3: Rastgele kategori seç
        int rastgeleKategoriIndex = Random.Range(0, kategoriler.Count);
        Kategori secilenKategori = kategoriler[rastgeleKategoriIndex];

        // ADIM 4: Rastgele 3 obje seç
        List<GameObject> secilenler = new List<GameObject>();
        List<GameObject> havuz = new List<GameObject>(secilenKategori.prefabs);

        for (int i = 0; i < 3; i++)
        {
            if (havuz.Count == 0) break;
            int r = Random.Range(0, havuz.Count);
            secilenler.Add(havuz[r]);
            havuz.RemoveAt(r); 
        }

        // ADIM 5: Doğru cevabı belirle
        if (secilenler.Count > 0)
        {
            int dogruIndex = Random.Range(0, secilenler.Count);
            dogruCevapObjesi = secilenler[dogruIndex];
            
            // QuizObject scriptinden ismi al
            var quizObj = dogruCevapObjesi.GetComponent<QuizObject>();
            if (quizObj != null) 
                dogruCevapAdi = quizObj.nesneAdi;
            else 
                dogruCevapAdi = dogruCevapObjesi.name; // Script yoksa obje adını al

            soruText.text = "BUL BAKALIM: " + dogruCevapAdi.ToUpper();
            sonucText.text = "";
            
            SoruSesiniDinle();
        }

        // ADIM 6: Objeleri Diz (Spawn Point İçine)
        // Pozisyonları biraz açtık ki birbirine girmesin (X ekseninde)
        Vector3[] pozisyonlar = new Vector3[] {
            new Vector3(-0.5f, 0, 0), 
            new Vector3(0, 0, 0),     
            new Vector3(0.5f, 0, 0)   
        };

        for (int i = 0; i < secilenler.Count; i++)
        {
            // Önce objeyi yarat
            GameObject yeniObje = Instantiate(secilenler[i]);
            
            // Sonra SpawnPoint'in çocuğu yap (Yere yapışsın diye)
            yeniObje.transform.SetParent(spawnPoint, false);
            
            // Pozisyonu ve boyutu ayarla
            yeniObje.transform.localPosition = pozisyonlar[i];
            yeniObje.transform.localScale = Vector3.one * objeBoyutu; 
            
            // Kameraya baktır (Y ekseninde dönsün sadece)
            Vector3 targetPosition = Camera.main.transform.position;
            targetPosition.y = yeniObje.transform.position.y; // Yere paralel baksın
            yeniObje.transform.LookAt(targetPosition);
            
            // İsmi aktar (QuizObject varsa)
            var yeniQuizObj = yeniObje.GetComponent<QuizObject>();
            var orjinalQuizObj = secilenler[i].GetComponent<QuizObject>();
            
            if (yeniQuizObj != null && orjinalQuizObj != null)
            {
                yeniQuizObj.nesneAdi = orjinalQuizObj.nesneAdi;
            }
            
            sahnedeOlusturulanlar.Add(yeniObje);
        }
    }

    public void SoruSesiniDinle()
    {
        // Eğer TTSManager varsa konuştur
        // (Kodunda TTSManager yoksa burası hata vermez, kontrol ekledim)
        GameObject ttsManager = GameObject.Find("TTSManager"); // Örnek kontrol
        // Burayı senin TTS sistemine göre açabilirsin
    }

    public void CevapVer(string tiklananAd, GameObject obje)
    {
        if (tiklananAd == dogruCevapAdi)
        {
            sonucText.text = "AFERİN! DOĞRU.";
            sonucText.color = Color.green;
            Invoke("SoruyuBaslat", 2.0f); 
        }
        else
        {
            sonucText.text = "YANLIŞ, TEKRAR DENE.";
            sonucText.color = Color.red;
            Destroy(obje); // Yanlış olanı sahneden sil
        }
    }

    void EskileriTemizle()
    {
        foreach (var item in sahnedeOlusturulanlar)
        {
            if (item != null) Destroy(item);
        }
        sahnedeOlusturulanlar.Clear();
    }
}
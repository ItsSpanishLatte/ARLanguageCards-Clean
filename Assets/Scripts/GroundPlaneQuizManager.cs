using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class GroundPlaneQuizManager : MonoBehaviour
{
    public static GroundPlaneQuizManager Instance;

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
    public Transform spawnPoint; 
    [Range(0.01f, 1.0f)]
    public float objeBoyutu = 0.5f; 

    private GameObject dogruCevapObjesi;
    private string dogruCevapAdi;
    private List<GameObject> sahnedeOlusturulanlar = new List<GameObject>();

    void Awake() { Instance = this; }

    public void OyunAlaniYerlesti()
    {
        if(baslaButonu != null) baslaButonu.SetActive(true);
        if(sonucText != null) sonucText.text = "Alan Yerleşti! Başla'ya bas.";
    }

    public void SoruyuBaslat()
    {
        if(baslaButonu != null) baslaButonu.SetActive(false);
        EskileriTemizle();

        if (kategoriler == null || kategoriler.Count == 0) return;
        if (spawnPoint == null) { Debug.LogError("Spawn Point yok!"); return; }

        int rastgeleKategoriIndex = Random.Range(0, kategoriler.Count);
        Kategori secilenKategori = kategoriler[rastgeleKategoriIndex];

        List<GameObject> secilenler = new List<GameObject>();
        List<GameObject> havuz = new List<GameObject>(secilenKategori.prefabs);

        for (int i = 0; i < 3; i++)
        {
            if (havuz.Count == 0) break;
            int r = Random.Range(0, havuz.Count);
            secilenler.Add(havuz[r]);
            havuz.RemoveAt(r); 
        }

        if (secilenler.Count > 0)
        {
            int dogruIndex = Random.Range(0, secilenler.Count);
            dogruCevapObjesi = secilenler[dogruIndex];
            
            var quizObj = dogruCevapObjesi.GetComponent<QuizObject>();
            dogruCevapAdi = (quizObj != null) ? quizObj.nesneAdi : dogruCevapObjesi.name;

            soruText.text = "BUL BAKALIM: " + dogruCevapAdi.ToUpper();
            sonucText.text = "";
            
            SoruSesiniDinle();
        }

        Vector3[] pozisyonlar = new Vector3[] {
            new Vector3(-0.5f, 0, 0), 
            new Vector3(0, 0, 0),     
            new Vector3(0.5f, 0, 0)   
        };

        for (int i = 0; i < secilenler.Count; i++)
        {
            GameObject yeniObje = Instantiate(secilenler[i]);
            yeniObje.transform.SetParent(spawnPoint, false);
            yeniObje.transform.localPosition = pozisyonlar[i];
            yeniObje.transform.localScale = Vector3.one * objeBoyutu; 
            
            Vector3 targetPosition = Camera.main.transform.position;
            targetPosition.y = yeniObje.transform.position.y; 
            yeniObje.transform.LookAt(targetPosition);
            
            var yeniQuizObj = yeniObje.GetComponent<QuizObject>();
            var orjinalQuizObj = secilenler[i].GetComponent<QuizObject>();
            if (yeniQuizObj != null && orjinalQuizObj != null) yeniQuizObj.nesneAdi = orjinalQuizObj.nesneAdi;
            
            sahnedeOlusturulanlar.Add(yeniObje);
        }
    }

    public void SoruSesiniDinle()
    {
        if (string.IsNullOrEmpty(dogruCevapAdi)) return;

        if (TTSManager.Instance != null)
        {
            TTSManager.Instance.Speak(dogruCevapAdi); 
        }
        else
        {
            Debug.LogWarning("TTSManager sahnede bulunamadı! Hierarchy'e ekledin mi?");
        }
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
            Destroy(obje); 
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
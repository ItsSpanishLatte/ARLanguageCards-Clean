using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections; 

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
    public float objeBoyutu = 0.15f;
    public float aralik = 0.20f;

    [Header("Otomatik Boyut Ayarı")]
    public float hedefBoyut = 0.15f;

    [Header("Mesafe Ayarı")]
    public float derinlik = 0.5f;

    [Header("İnce Ayar")]
    public float zeminOfseti = 0.0f;

    private bool alanYerlesti = false;
    private bool oyunBasladi = false;
    private bool soruAktif = false;
    
    private bool gecisYapiliyor = false; 

    private string dogruCevapAdi;
    private List<GameObject> sahnedekiObjeler = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!oyunBasladi || !soruAktif || gecisYapiliyor) return; 

        bool tiklandi = false;
        Vector2 ekranPos = Vector2.zero;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            tiklandi = true;
            ekranPos = Input.mousePosition;
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            tiklandi = true;
            ekranPos = Input.GetTouch(0).position;
        }
#endif

        if (!tiklandi || Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(ekranPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            QuizObject quizObj = hit.collider.GetComponent<QuizObject>();
            if (quizObj == null)
                quizObj = hit.collider.GetComponentInParent<QuizObject>();

            if (quizObj != null)
                CevapVer(quizObj.nesneAdi, hit.collider.gameObject);
        }
    }

    public void OyunAlaniYerlesti()
    {
        if (alanYerlesti) return;
        alanYerlesti = true;
        if (baslaButonu) baslaButonu.SetActive(true);
        if (sonucText) sonucText.text = "Alan yerleşti! Başla'ya bas.";
    }

    public void SoruyuBaslat()
    {
        if (soruAktif) return;

        oyunBasladi = true;
        soruAktif = true;
        gecisYapiliyor = false; // Kilidi aç

        if (baslaButonu) baslaButonu.SetActive(false);
        if (sonucText) sonucText.text = "";

        EskileriTemizle(); 

        Kategori kategori = kategoriler[Random.Range(0, kategoriler.Count)];
        List<GameObject> havuz = new List<GameObject>(kategori.prefabs);
        List<GameObject> secilenler = new List<GameObject>();

        for (int i = 0; i < 3 && havuz.Count > 0; i++)
        {
            int r = Random.Range(0, havuz.Count);
            secilenler.Add(havuz[r]);
            havuz.RemoveAt(r);
        }

        int dogruIndex = Random.Range(0, secilenler.Count);
        QuizObject dogruPrefabScript = secilenler[dogruIndex].GetComponent<QuizObject>();

        dogruCevapAdi = dogruPrefabScript != null && !string.IsNullOrEmpty(dogruPrefabScript.nesneAdi)
            ? dogruPrefabScript.nesneAdi.Trim()
            : secilenler[dogruIndex].name.Replace("(Clone)", "").Trim();

        if (soruText) soruText.text = "BUL BAKALIM: " + dogruCevapAdi.ToUpper();

        if (TTSManager.Instance != null) TTSManager.Instance.Speak(dogruCevapAdi);

        for (int i = 0; i < secilenler.Count; i++)
        {
            GameObject orjinalPrefab = secilenler[i];
            GameObject yeniObje = Instantiate(orjinalPrefab, spawnPoint);
            OtomatikBoyutAyarla(yeniObje);

            float x = (i - (secilenler.Count - 1) / 2f) * aralik;
            yeniObje.transform.localPosition = new Vector3(x, 0.0f, derinlik);

            Vector3 camPos = Camera.main.transform.position;
            camPos.y = yeniObje.transform.position.y; 
            yeniObje.transform.LookAt(camPos);

            YereOtur(yeniObje);

            QuizObject orjinalQO = orjinalPrefab.GetComponent<QuizObject>();
            QuizObject yeniQO = yeniObje.GetComponent<QuizObject>();
            if (yeniQO == null) yeniQO = yeniObje.AddComponent<QuizObject>();

            if (orjinalQO != null) yeniQO.nesneAdi = orjinalQO.nesneAdi;
            else yeniQO.nesneAdi = orjinalPrefab.name.Replace("(Clone)", "").Trim();

            sahnedekiObjeler.Add(yeniObje);
        }
    }

    void OtomatikBoyutAyarla(GameObject obj)
    {
        // (Senin mevcut kodun aynı kalsın)
        obj.transform.localScale = Vector3.one;
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;
        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers) bounds.Encapsulate(r.bounds);
        float enBuyukKenar = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        if (enBuyukKenar <= 0f) return;
        float scale = hedefBoyut / enBuyukKenar;
        obj.transform.localScale = Vector3.one * scale;
    }

    void YereOtur(GameObject obj)
    {
        // (Senin mevcut kodun aynı kalsın)
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;
        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers) bounds.Encapsulate(r.bounds);
        float objeninEnAltNoktasiY = bounds.min.y;
        float zeminY = spawnPoint != null ? spawnPoint.position.y : 0f;
        float fark = zeminY - objeninEnAltNoktasiY;
        obj.transform.position += new Vector3(0, fark + zeminOfseti, 0);
    }

    public void CevapVer(string tiklananAd, GameObject obje)
    {
        if (!soruAktif || gecisYapiliyor) return;

        if (tiklananAd.Trim().ToLower() == dogruCevapAdi.Trim().ToLower())
        {
            if (sonucText)
            {
                sonucText.text = "AFERİN! DOĞRU.";
                sonucText.color = Color.green;
            }

            soruAktif = false;
            gecisYapiliyor = true; 

            StartCoroutine(SoruGecisRutini());
        }
        else
        {
            if (sonucText)
            {
                sonucText.text = "YANLIŞ, TEKRAR DENE.";
                sonucText.color = Color.red;
            }

            sahnedekiObjeler.Remove(obje);
            Destroy(obje);
        }
    }

    IEnumerator SoruGecisRutini()
    {
        yield return new WaitForSeconds(1.5f);

        EskileriTemizle();

        yield return null; 

        SoruyuBaslat();
    }

    void EskileriTemizle()
    {
        QuizObject[] kalanlar = FindObjectsOfType<QuizObject>();
        foreach (QuizObject q in kalanlar)
            if (q != null) Destroy(q.gameObject);

        sahnedekiObjeler.Clear();
    }
}
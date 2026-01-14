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
            QuizObject quizObj = hit.collider.GetComponentInParent<QuizObject>();

            if (quizObj != null)
            {
                CevapVer(quizObj.nesneAdi, quizObj.gameObject);
            }
        }
    }

    public void OyunAlaniYerlesti()
    {
        if (alanYerlesti) return;
        alanYerlesti = true;

        spawnPoint.SetParent(transform, false);
        spawnPoint.localPosition = Vector3.zero;
        spawnPoint.localRotation = Quaternion.identity;

        if (baslaButonu) baslaButonu.SetActive(true);
        if (sonucText) sonucText.text = "Alan yerleşti! Başla'ya bas.";
    }

    public void SoruyuBaslat()
    {
        if (soruAktif || gecisYapiliyor)
        return;

        oyunBasladi = true;
        soruAktif = true;
        gecisYapiliyor = false;

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
        QuizObject dogruQO = secilenler[dogruIndex].GetComponent<QuizObject>();

        dogruCevapAdi = dogruQO != null && !string.IsNullOrEmpty(dogruQO.nesneAdi)
            ? dogruQO.nesneAdi.Trim()
            : secilenler[dogruIndex].name.Replace("(Clone)", "").Trim();

        if (soruText) soruText.text = "BUL BAKALIM: " + dogruCevapAdi.ToUpper();
        if (TTSManager.Instance != null) TTSManager.Instance.Speak(dogruCevapAdi);

        for (int i = 0; i < secilenler.Count; i++)
        {
            GameObject yeniObje = Instantiate(secilenler[i]);

            OtomatikBoyutAyarla(yeniObje);

            float x = (i - (secilenler.Count - 1) / 2f) * aralik;
            Vector3 sag = spawnPoint.right * x;
            Vector3 ileri = spawnPoint.forward * derinlik;
            yeniObje.transform.position = spawnPoint.position + sag + ileri;

            Vector3 camPos = Camera.main.transform.position;
            camPos.y = yeniObje.transform.position.y;
            yeniObje.transform.LookAt(camPos);

            Physics.SyncTransforms();
            YereOtur(yeniObje);

            QuizObject yeniQO = yeniObje.GetComponent<QuizObject>();
            if (yeniQO == null) yeniQO = yeniObje.AddComponent<QuizObject>();

            QuizObject orjinalQO = secilenler[i].GetComponent<QuizObject>();
            yeniQO.nesneAdi = orjinalQO != null
                ? orjinalQO.nesneAdi
                : secilenler[i].name.Replace("(Clone)", "").Trim();

            sahnedekiObjeler.Add(yeniObje);
        }
    }

    void OtomatikBoyutAyarla(GameObject obj)
    {
        obj.transform.localScale = Vector3.one;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers) bounds.Encapsulate(r.bounds);

        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        if (maxSize <= 0f) return;

        float scale = hedefBoyut / maxSize;
        obj.transform.localScale = Vector3.one * scale;
    }

    void YereOtur(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers) bounds.Encapsulate(r.bounds);

        float enAltY = bounds.min.y;
        float zeminY = spawnPoint != null ? spawnPoint.position.y : 0f;
        float fark = zeminY - enAltY;

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

        gecisYapiliyor = false;
        soruAktif = false;

        yield return null;

        SoruyuBaslat();
    }

    void EskileriTemizle()
    {
        QuizObject[] tumQuizObjeleri = FindObjectsOfType<QuizObject>();
        foreach (QuizObject q in tumQuizObjeleri)
            Destroy(q.gameObject);

        sahnedekiObjeler.Clear();
    }
}

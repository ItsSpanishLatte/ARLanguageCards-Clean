using UnityEngine;
using System.Collections;

public class ARObjectController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float donusHizi = 0.3f; 
    public float buyutmeHizi = 0.001f; 
    public float minBoyut = 0.1f;
    public float maxBoyut = 3f;

    [Header("Telaffuz Ayarları")]
    public int gecmeNotu = 60;
    public string objeAdi = "apple";

    [Header("Tap Zıplama Ayarları")]
    public float ziplamaGucu = 0.05f;

    private Animator anim;

    private bool surukleniyorMu = false;
    private float dragThreshold = 0.5f; 

    private Vector3 baslangicPozisyonu;
    private bool efektOynuyorMu = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (!string.IsNullOrEmpty(objeAdi))
        {
            objeAdi = objeAdi.ToLower().Trim();
        }
        baslangicPozisyonu = transform.localPosition;
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                surukleniyorMu = false; 
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (touch.deltaPosition.magnitude > dragThreshold)
                {
                    surukleniyorMu = true; 

                    float rotY = -touch.deltaPosition.x * donusHizi;

                    transform.Rotate(Vector3.up, rotY, Space.World);
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (!surukleniyorMu)
                {
                    TapIslemi(touch.position);
                }
            }
        }

        else if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 t0Prev = t0.position - t0.deltaPosition;
            Vector2 t1Prev = t1.position - t1.deltaPosition;

            float prevDist = Vector2.Distance(t0Prev, t1Prev);
            float currDist = Vector2.Distance(t0.position, t1.position);

            float fark = currDist - prevDist;

            Vector3 yeniBoyut = transform.localScale + Vector3.one * fark * buyutmeHizi;

            yeniBoyut.x = Mathf.Clamp(yeniBoyut.x, minBoyut, maxBoyut);
            yeniBoyut.y = Mathf.Clamp(yeniBoyut.y, minBoyut, maxBoyut);
            yeniBoyut.z = Mathf.Clamp(yeniBoyut.z, minBoyut, maxBoyut);

            transform.localScale = yeniBoyut;
        }
    }

    void TapIslemi(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                if (!efektOynuyorMu)
                {
                    StartCoroutine(ZiplamaEfekti());

                    if (TTSManager.Instance != null)
                        TTSManager.Instance.Speak(objeAdi);
                }
            }
        }
    }

    public void SonucuDegerlendir(int puan)
    {
        if (puan >= gecmeNotu)
            Basarili();
        else
            Basarisiz();
    }

    void Basarili()
    {
        if (anim != null)
            anim.SetTrigger("Don");

        if (TTSManager.Instance != null)
            TTSManager.Instance.Speak("Excellent! " + objeAdi);
    }

    void Basarisiz()
    {
        if (TTSManager.Instance != null)
            TTSManager.Instance.Speak("Try again.");
    }

    IEnumerator ZiplamaEfekti()
    {
        efektOynuyorMu = true;

        float sure = 0.15f;
        float gecen = 0f;

        Vector3 anlikPozisyon = transform.localPosition;
        Vector3 hedef = anlikPozisyon + Vector3.up * ziplamaGucu;

        while (gecen < sure)
        {
            transform.localPosition = Vector3.Lerp(anlikPozisyon, hedef, gecen / sure);
            gecen += Time.deltaTime;
            yield return null;
        }

        gecen = 0f;
        while (gecen < sure)
        {
            transform.localPosition = Vector3.Lerp(hedef, anlikPozisyon, gecen / sure);
            gecen += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = anlikPozisyon; 
        efektOynuyorMu = false;
    }

    public string GetObjeAdi()
    {
        return objeAdi;
    }
}
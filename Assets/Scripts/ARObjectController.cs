using UnityEngine;
using System.Collections;

public class ARObjectController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float donusHizi = 0.2f;
    public float buyutmeHizi = 0.02f;
    public float minBoyut = 0.1f;
    public float maxBoyut = 0.8f;

    [Header("Telaffuz Ayarları")]
    public int gecmeNotu = 60;
    public string objeAdi = "apple";

    [Header("Tap Zıplama Ayarları")]
    public float ziplamaGucu = 0.05f;

    private Animator anim;

    private bool surukleniyorMu = false;
    private float dragThreshold = 10f;
    private Vector3 baslangicPozisyonu;
    private bool efektOynuyorMu = false;

    private float baslangicMesafe;
    private Vector3 pinchBaslangicScale;

    private Vector3 mouseClickOrigin;
    private Vector3 sonMousePozisyonu;
    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        transform.localScale = originalScale;
        efektOynuyorMu = false;
    }

    void Start()
    {
        anim = GetComponent<Animator>();

        if (!string.IsNullOrEmpty(objeAdi))
            objeAdi = objeAdi.ToLower().Trim();

        baslangicPozisyonu = transform.localPosition;
    }

    void Update()
    {
        // TOUCH KONTROLLERİ
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
                    float rotY = -touch.deltaPosition.x * 0.15f;
                    transform.Rotate(0f, rotY, 0f, Space.World);
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (!surukleniyorMu)
                    TapIslemi(touch.position);
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
            {
                baslangicMesafe = Vector2.Distance(t0.position, t1.position);
                pinchBaslangicScale = transform.localScale;
            }
            else
            {
                float simdikiMesafe = Vector2.Distance(t0.position, t1.position);
                if (Mathf.Approximately(baslangicMesafe, 0)) return;

                float oran = simdikiMesafe / baslangicMesafe;
                float hedefScale = Mathf.Clamp(
                    pinchBaslangicScale.x * oran,
                    minBoyut,
                    maxBoyut
                );

                transform.localScale = Vector3.one * hedefScale;
            }
        }

        // MOUSE KONTROLLERİ
        else if (Input.touchCount == 0)
        {
            // Mouse Down
            if (Input.GetMouseButtonDown(0))
            {
                sonMousePozisyonu = Input.mousePosition;
                surukleniyorMu = false;
            }
            // Mouse Drag → FREE ROTATE (X + Y)
            else if (Input.GetMouseButton(0))
            {
                Vector3 delta = Input.mousePosition - sonMousePozisyonu;

                if (delta.magnitude > 2f)
                {
                    surukleniyorMu = true;

                    float rotY = -delta.x * donusHizi;
                    float rotX = delta.y * donusHizi;

                    transform.Rotate(rotX, rotY, 0f, Space.World);

                }

                sonMousePozisyonu = Input.mousePosition;
            }
            // Mouse Up → TAP
            else if (Input.GetMouseButtonUp(0))
            {
                if (!surukleniyorMu)
                    TapIslemi(Input.mousePosition);
            }

            // Scroll → SCALE
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                float currentScale = transform.localScale.x;
                float yeniScale = Mathf.Clamp(
                currentScale + scroll * buyutmeHizi,
                minBoyut,
                maxBoyut
            );

            transform.localScale = Vector3.one * yeniScale;
            }
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

    IEnumerator ZiplamaEfekti()
    {
        efektOynuyorMu = true;

        float sure = 0.15f;
        float gecen = 0f;

        Vector3 start = baslangicPozisyonu;
        Vector3 hedef = start + Vector3.up * ziplamaGucu;

        while (gecen < sure)
        {
            transform.localPosition = Vector3.Lerp(start, hedef, gecen / sure);
            gecen += Time.deltaTime;
            yield return null;
        }

        gecen = 0f;
        while (gecen < sure)
        {
            transform.localPosition = Vector3.Lerp(hedef, start, gecen / sure);
            gecen += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = start;
        efektOynuyorMu = false;
    }

    public void SonucuDegerlendir(int puan)
    {
        if (puan >= gecmeNotu)
            Basarili();
        else
            Basarisiz();
    }

    public string GetObjeAdi()
    {
        return objeAdi;
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
}

using UnityEngine;
using System.Collections; 

public class ARObjectController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float donusHizi = 100.0f;
    public float buyutmeHizi = 2.0f;
    public float minBoyut = 0.1f;    
    public float maxBoyut = 5.0f;    

    [Header("Tap (Tıklama) Ayarları")]
    public float ziplamaGucu = 0.5f;

    private Vector3 sonMousePozisyonu;
    private bool surukleniyorMu = false;
    
    private Vector3 baslangicPozisyonu;
    private bool efektOynuyorMu = false;

    void Start()
    {
        baslangicPozisyonu = transform.localPosition;
    }

    void Update()
    {
        //FREE ROTATE 
        if (Input.GetMouseButtonDown(0))
        {
            sonMousePozisyonu = Input.mousePosition;
            surukleniyorMu = false;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - sonMousePozisyonu;

            if (delta.magnitude > 2f) 
            {
                surukleniyorMu = true;
                
                float rotY = -delta.x * donusHizi * Time.deltaTime;
                float rotX = delta.y * donusHizi * Time.deltaTime;

                transform.Rotate(rotX, rotY, 0, Space.World);
            }
            
            sonMousePozisyonu = Input.mousePosition;
        }

        //SCALE
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Vector3 yeniBoyut = transform.localScale + Vector3.one * scroll * buyutmeHizi;

            yeniBoyut.x = Mathf.Clamp(yeniBoyut.x, minBoyut, maxBoyut);
            yeniBoyut.y = Mathf.Clamp(yeniBoyut.y, minBoyut, maxBoyut);
            yeniBoyut.z = Mathf.Clamp(yeniBoyut.z, minBoyut, maxBoyut);

            transform.localScale = yeniBoyut;
        }

        //TAP
        if (Input.GetMouseButtonUp(0) && !surukleniyorMu)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform || hit.transform.IsChildOf(transform))
                {
                    if (!efektOynuyorMu) StartCoroutine(ZiplamaEfekti());
                                        TetikleSes();
                }
            }
        }
    }

    IEnumerator ZiplamaEfekti()
    {
        efektOynuyorMu = true;
        
        float sure = 0.2f; // Çıkış süresi
        float gecenSure = 0;
        
        Vector3 hedefYukseklik = baslangicPozisyonu + Vector3.up * ziplamaGucu;

        // Yukarı Çık
        while (gecenSure < sure)
        {
            transform.localPosition = Vector3.Lerp(baslangicPozisyonu, hedefYukseklik, (gecenSure / sure));
            gecenSure += Time.deltaTime;
            yield return null;
        }

        // Aşağı İn
        gecenSure = 0;
        while (gecenSure < sure)
        {
            transform.localPosition = Vector3.Lerp(hedefYukseklik, baslangicPozisyonu, (gecenSure / sure));
            gecenSure += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = baslangicPozisyonu;
        efektOynuyorMu = false;
    }

    void TetikleSes()
    {
        if (TTSManager.Instance != null)
        {
            var speakScript = GetComponentInParent<SpeakOnTarget>();
            if (speakScript != null)
            {
                TTSManager.Instance.Speak(speakScript.kelime);
            }
        }
    }
}
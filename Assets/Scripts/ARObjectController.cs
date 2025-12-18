using UnityEngine;

public class ARObjectController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float donusHizi = 100.0f;  // Hızları yüksek tuttum ki net görünsün
    public float buyutmeHizi = 2.0f;
    public float minBoyut = 0.1f;    
    public float maxBoyut = 5.0f;    

    private Vector3 sonMousePozisyonu;
    private bool surukleniyorMu = false;

    void Update()
    {
        // --- 1. FREE ROTATE (HER YÖNE DÖNDÜRME) ---
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
                
                // Mouse SAĞA/SOLA giderse -> Obje Y ekseninde (Kendi etrafında) döner
                float rotY = -delta.x * donusHizi * Time.deltaTime;
                
                // Mouse YUKARI/AŞAĞI giderse -> Obje X ekseninde (Öne/Arkaya) döner
                float rotX = delta.y * donusHizi * Time.deltaTime;

                // Space.World kullanıyoruz ki elma baş aşağı dursa bile kontroller karışmasın
                transform.Rotate(rotX, rotY, 0, Space.World);
            }
            
            sonMousePozisyonu = Input.mousePosition;
        }

        // --- 2. SCALE (BÜYÜTME) ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Vector3 yeniBoyut = transform.localScale + Vector3.one * scroll * buyutmeHizi;

            yeniBoyut.x = Mathf.Clamp(yeniBoyut.x, minBoyut, maxBoyut);
            yeniBoyut.y = Mathf.Clamp(yeniBoyut.y, minBoyut, maxBoyut);
            yeniBoyut.z = Mathf.Clamp(yeniBoyut.z, minBoyut, maxBoyut);

            transform.localScale = yeniBoyut;
        }

        // --- 3. TAP (TIKLAMA) ---
        if (Input.GetMouseButtonUp(0) && !surukleniyorMu)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    Debug.Log("Meyveye Tıklandı!"); 
                }
            }
        }
    }
}
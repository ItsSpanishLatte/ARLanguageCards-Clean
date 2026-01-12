using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ScoreboardManager : MonoBehaviour
{
    [Header("UI Baðlantýlarý")]
    public GameObject satirPrefab;

    [Header("Listeler")]
    public Transform kelimeContent;
    public Transform cumleContent;

    public TextMeshProUGUI durumYazisi;

    FirebaseFirestore db;
    FirebaseAuth auth;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
        VerileriGetir();
    }

    void VerileriGetir()
    {
        if (auth.CurrentUser == null) return;

        string userId = auth.CurrentUser.UserId;

        // --- DÝL BELÝRLEME ---
        // Sahne ismi "De" ile bitiyorsa Almanca verilerini, bitmiyorsa Ýngilizceyi getirir.
        string aktifDil = SceneManager.GetActiveScene().name.EndsWith("De") ? "Almanca" : "Ingilizce";

        if (durumYazisi != null) durumYazisi.text = (aktifDil == "Almanca") ? "Laden..." : "Loading...";

        // Firestore Sorgusu: Sadece ilgili dili getir (WhereEqualTo filtresi eklendi)
        db.Collection("Users").Document(userId).Collection("Gecmis")
            .WhereEqualTo("dil", aktifDil)
            .OrderByDescending("tarih")
            .GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Firebase Hatasý: " + task.Exception);
                    return;
                }

                // Listeleri temizle
                foreach (Transform child in kelimeContent) Destroy(child.gameObject);
                foreach (Transform child in cumleContent) Destroy(child.gameObject);

                QuerySnapshot snapshot = task.Result;

                if (snapshot.Count == 0)
                {
                    if (durumYazisi != null) durumYazisi.text = (aktifDil == "Almanca") ? "Keine Daten." : "No data yet.";
                }
                else
                {
                    if (durumYazisi != null) durumYazisi.text = (aktifDil == "Almanca") ? "Erfolge" : "Achievements";
                }

                foreach (DocumentSnapshot belge in snapshot.Documents)
                {
                    Dictionary<string, object> veri = belge.ToDictionary();
                    if (veri.ContainsKey("hedef") && veri.ContainsKey("puan"))
                    {
                        string hedef = veri["hedef"].ToString();
                        string puan = veri["puan"].ToString();
                        string tur = veri.ContainsKey("tur") ? veri["tur"].ToString() : "Kelime";

                        // Türüne göre uygun listeye ekle
                        Transform hedefKutu = (tur == "Cumle") ? cumleContent : kelimeContent;
                        GameObject yeniSatir = Instantiate(satirPrefab, hedefKutu);
                        yeniSatir.GetComponent<TextMeshProUGUI>().text = $"{hedef} : {puan}";
                    }
                }
            });
    }

    public void AnaMenuyeDon()
    {
        // Almanca profilinden Almanca ana menüye, diðerinden normal menüye döner.
        string sahneAdi = SceneManager.GetActiveScene().name.EndsWith("De") ? "MainMenuDe" : "MainMenu";
        SceneManager.LoadScene(sahneAdi);
    }
}
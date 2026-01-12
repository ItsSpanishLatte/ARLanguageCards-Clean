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

        string aktifDil = SceneManager.GetActiveScene().name.EndsWith("De") ? "Almanca" : "Ingilizce";

        if (durumYazisi != null) durumYazisi.text = (aktifDil == "Almanca") ? "Laden..." : "Loading...";

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

                        Transform hedefKutu = (tur == "Cumle") ? cumleContent : kelimeContent;
                        GameObject yeniSatir = Instantiate(satirPrefab, hedefKutu);
                        yeniSatir.GetComponent<TextMeshProUGUI>().text = $"{hedef} : {puan}";
                    }
                }
            });
    }

    public void AnaMenuyeDon()
    {
        string sahneAdi = SceneManager.GetActiveScene().name.EndsWith("De") ? "MainMenuDe" : "MainMenu";
        SceneManager.LoadScene(sahneAdi);
    }
}
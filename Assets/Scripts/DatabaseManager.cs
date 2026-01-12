using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Analytics;
using System.Collections.Generic;
using Firebase.Auth;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;
    FirebaseFirestore db;
    FirebaseAuth auth;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
    }

    public void SkoruKaydet(string kelime, int puan, string tur, string dil)
    {
        if (auth.CurrentUser == null) return; // Giriþ yapmamýþsa kaydetme

        string userId = auth.CurrentUser.UserId;

        Dictionary<string, object> veri = new Dictionary<string, object>
        {
            { "hedef", kelime },
            { "puan", puan },
            { "tur", tur }, // "Kelime" veya "Cümle"
            { "dil", dil }, // "Ingilizce" veya "Almanca" etiketi
            { "tarih", FieldValue.ServerTimestamp }
        };

        db.Collection("Users").Document(userId).Collection("Gecmis").AddAsync(veri);
        Debug.Log($"Skor ({dil}) Veritabanýna Gönderildi!");
    }

    public void LogTut(string olayAdi, string deger)
    {
        FirebaseAnalytics.LogEvent(olayAdi, new Parameter("deger", deger));
        Debug.Log($"Analitik yollandý: {olayAdi} -> {deger}");
    }
}
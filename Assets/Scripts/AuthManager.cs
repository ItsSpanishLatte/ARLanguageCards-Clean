using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance; 

    [Header("G�R�� EKRANI (Ana Sahne)")]
    public InputField girisEmailInput;
    public InputField girisPasswordInput;
    public TextMeshProUGUI bildirimText;

    [Header("KAYIT PANEL� (Pop-up)")]
    public GameObject kayitPaneli;
    public InputField kayitEmailInput;
    public InputField kayitPasswordInput;
    public TextMeshProUGUI kayitBildirimText;

    private FirebaseAuth auth;

    private bool isGermanSelected = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        if (kayitPaneli != null)
            kayitPaneli.SetActive(false);

        isGermanSelected = false;
    }

    public void CikisYap()
    {
        if (auth != null)
        {
            auth.SignOut(); 
            Debug.Log("Oturum kapat�ld�.");
        }

        SceneManager.LoadScene("LoginScene");
    }

    public void AlmancaSec()
    {
        isGermanSelected = true;
        if (bildirimText != null)
        {
            bildirimText.text = "Modus: Deutsch ausgewählt";
            bildirimText.color = Color.white;
        }
    }

    public void KayitPaneliniAc()
    {
        if (kayitPaneli != null) kayitPaneli.SetActive(true);
        if (kayitBildirimText != null) kayitBildirimText.text = "";
        if (bildirimText != null) bildirimText.text = "";
    }

    public void KayitPaneliniKapat() => kayitPaneli.SetActive(false);

    public void KayitOl()
    {
        string email = kayitEmailInput.text;
        string password = kayitPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            if (kayitBildirimText != null) kayitBildirimText.text = "Alanlar� doldurun.";
            return;
        }

        StartCoroutine(KayitIslemi(email, password));
    }

    private IEnumerator KayitIslemi(string email, string password)
    {
        if (kayitBildirimText != null) kayitBildirimText.text = "Kaydediliyor...";
        var islem = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => islem.IsCompleted);

        if (islem.Exception != null)
        {
            if (kayitBildirimText != null) kayitBildirimText.text = HataMesajiniSadelestir(islem.Exception);
        }
        else
        {
            if (bildirimText != null) bildirimText.text = "Kay�t Ba�ar�l�!";
            KayitPaneliniKapat();
            if (girisEmailInput != null) girisEmailInput.text = email;
        }
    }

    public void GirisYap()
    {
        string email = girisEmailInput.text;
        string password = girisPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            if (bildirimText != null) bildirimText.text = "Bilgiler eksik.";
            return;
        }

        StartCoroutine(GirisIslemi(email, password));
    }

    private IEnumerator GirisIslemi(string email, string password)
    {
        if (bildirimText != null) bildirimText.text = "Ba�lan�yor...";
        var islem = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => islem.IsCompleted);

        if (islem.Exception != null)
        {
            if (bildirimText != null) bildirimText.text = HataMesajiniSadelestir(islem.Exception);
        }
        else
        {
            if (bildirimText != null) bildirimText.text = "Ho� geldiniz!";
            yield return new WaitForSeconds(1.0f);

            if (isGermanSelected)
                SceneManager.LoadScene("MainMenuDe");
            else
                SceneManager.LoadScene("MainMenu");
        }
    }

    private string HataMesajiniSadelestir(System.AggregateException exception)
    {
        FirebaseException firebaseEx = exception.GetBaseException() as FirebaseException;
        if (firebaseEx == null) return "Hata olu�tu.";

        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

        switch (errorCode)
        {
            case AuthError.InvalidEmail: return "E-posta ge�ersiz.";
            case AuthError.WrongPassword: return "�ifre hatal�.";
            case AuthError.UserNotFound: return "Hesap bulunamad�.";
            case AuthError.EmailAlreadyInUse: return "E-posta kay�tl�.";
            case AuthError.WeakPassword: return "�ifre zay�f.";
            case AuthError.NetworkRequestFailed: return "�nternet yok.";
            default: return "Tekrar deneyin.";
        }
    }
}
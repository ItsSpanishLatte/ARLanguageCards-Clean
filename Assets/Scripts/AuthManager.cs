using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    [Header("GÝRÝÞ EKRANI (Ana Sahne)")]
    public InputField girisEmailInput;
    public InputField girisPasswordInput;
    public TextMeshProUGUI bildirimText; // Giriþ ekranýndaki hata yazýsý

    [Header("KAYIT PANELÝ (Pop-up)")]
    public GameObject kayitPaneli;
    public InputField kayitEmailInput;
    public InputField kayitPasswordInput;
    public TextMeshProUGUI kayitBildirimText; // YENÝ: Kayýt panelindeki hata yazýsý

    private FirebaseAuth auth;

    private void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        if (kayitPaneli != null)
            kayitPaneli.SetActive(false);
    }

    public void KayitPaneliniAc()
    {
        kayitPaneli.SetActive(true);
        if (kayitBildirimText != null) kayitBildirimText.text = "";
        bildirimText.text = "";
    }

    public void KayitPaneliniKapat() => kayitPaneli.SetActive(false);

    public void KayitOl()
    {
        string email = kayitEmailInput.text;
        string password = kayitPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            if (kayitBildirimText != null) kayitBildirimText.text = "Alanlarý doldurun.";
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
            bildirimText.text = "Kayýt Baþarýlý!";
            KayitPaneliniKapat();
            girisEmailInput.text = email;
        }
    }

    public void GirisYap()
    {
        string email = girisEmailInput.text;
        string password = girisPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            bildirimText.text = "Bilgiler eksik.";
            return;
        }

        StartCoroutine(GirisIslemi(email, password));
    }

    private IEnumerator GirisIslemi(string email, string password)
    {
        bildirimText.text = "Baðlanýyor...";
        var islem = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => islem.IsCompleted);

        if (islem.Exception != null)
        {
            bildirimText.text = HataMesajiniSadelestir(islem.Exception);
        }
        else
        {
            bildirimText.text = "Hoþ geldiniz!";
            yield return new WaitForSeconds(1.0f);
            SceneManager.LoadScene("MainMenu");
        }
    }

    // Teknik hatalarý kýsa kullanýcý dostu cümlelere çevirir
    private string HataMesajiniSadelestir(System.AggregateException exception)
    {
        FirebaseException firebaseEx = exception.GetBaseException() as FirebaseException;
        if (firebaseEx == null) return "Hata oluþtu.";

        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

        switch (errorCode)
        {
            case AuthError.InvalidEmail: return "E-posta geçersiz.";
            case AuthError.WrongPassword: return "Þifre hatalý.";
            case AuthError.UserNotFound: return "Hesap bulunamadý.";
            case AuthError.EmailAlreadyInUse: return "E-posta kayýtlý.";
            case AuthError.WeakPassword: return "Þifre zayýf.";
            case AuthError.NetworkRequestFailed: return "Ýnternet yok.";
            default: return "Tekrar deneyin.";
        }
    }
}
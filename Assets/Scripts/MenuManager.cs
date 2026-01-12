using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth; // Çýkýþ iþlemi için gerekli

public class MenuManager : MonoBehaviour
{
    // Yardýmcý fonksiyon: Þu an Almanca bir menüde miyiz?
    // Sahne isminin sonu "De" ile bitiyorsa true döner.
    private bool IsGermanMenu()
    {
        return SceneManager.GetActiveScene().name.EndsWith("De");
    }

    public void FruitsSahnesiniAc()
    {
        // Almanca menüdeyse FruchteScene, deðilse FruitsScene açýlýr
        string sceneName = IsGermanMenu() ? "FruchteScene" : "FruitsScene";
        SceneManager.LoadScene(sceneName);
    }

    public void AnimalsSahnesiniAc()
    {
        // Almanca menüdeyse TiereScene, deðilse AnimalsScene açýlýr
        string sceneName = IsGermanMenu() ? "TiereScene" : "AnimalsScene";
        SceneManager.LoadScene(sceneName);
    }

    public void VehiclesSahnesiniAc()
    {
        // Almanca menüdeyse FahrzeugeScene, deðilse VehiclesScene açýlýr
        string sceneName = IsGermanMenu() ? "FahrzeugeScene" : "VehiclesScene";
        SceneManager.LoadScene(sceneName);
    }

    public void AcProfil()
    {
        // Almanca menüdeyse ScoreboardSceneDe, deðilse ScoreboardScene açýlýr
        string sceneName = IsGermanMenu() ? "ScoreboardSceneDe" : "ScoreboardScene";
        SceneManager.LoadScene(sceneName);
    }

    public void AcQuiz()
    {
        // Quiz sahnesi için ayný mantýk
        string sceneName = IsGermanMenu() ? "QuizSceneDe" : "QuizScene";
        SceneManager.LoadScene(sceneName);
    }

    // --- YENÝ: ÇIKIÞ YAPMA FONKSÝYONU ---
    public void CikisYap()
    {
        // Firebase oturumunu sonlandýr (Uygulama tekrar açýldýðýnda kullanýcý login ekranýna düþer)
        if (FirebaseAuth.DefaultInstance != null)
        {
            FirebaseAuth.DefaultInstance.SignOut();
            Debug.Log("Oturum kapatýldý.");
        }

        // Kullanýcýyý en baþa, Login ekranýna gönder
        SceneManager.LoadScene("LoginScene");
    }
}
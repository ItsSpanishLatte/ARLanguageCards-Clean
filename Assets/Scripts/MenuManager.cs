using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Yardýmcý fonksiyon: Þu an Almanca menüde miyiz?
    private bool IsGermanMenu()
    {
        return SceneManager.GetActiveScene().name.EndsWith("De");
    }

    public void FruitsSahnesiniAc()
    {
        // Almanca menüdeyse FruchteSceneDe, deðilse FruitsScene açýlýr
        string sceneName = IsGermanMenu() ? "FruchteScene" : "FruitsScene";
        SceneManager.LoadScene(sceneName);
    }

    public void AnimalsSahnesiniAc()
    {
        // Almanca menüdeyse TiereSceneDe, deðilse AnimalsScene açýlýr
        string sceneName = IsGermanMenu() ? "TiereScene" : "AnimalsScene";
        SceneManager.LoadScene(sceneName);
    }

    public void VehiclesSahnesiniAc()
    {
        // Almanca menüdeyse FahrzeugeSceneDe (veya senin isimlendirmenle VehiclesScene_DE), deðilse VehiclesScene açýlýr
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
        // Quiz sahnesi için de ayný mantýk (varsa QuizSceneDe yoksa varsayýlan)
        string sceneName = IsGermanMenu() ? "QuizSceneDe" : "QuizScene";
        SceneManager.LoadScene(sceneName);
    }
}
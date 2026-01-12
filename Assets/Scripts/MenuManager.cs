using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth; 

public class MenuManager : MonoBehaviour
{
    private bool IsGermanMenu()
    {
        return SceneManager.GetActiveScene().name.EndsWith("De");
    }

    public void FruitsSahnesiniAc()
    {
        string sceneName = IsGermanMenu() ? "FruchteScene" : "FruitsScene";
        SceneManager.LoadScene(sceneName);
    }

    public void AnimalsSahnesiniAc()
    {
        string sceneName = IsGermanMenu() ? "TiereScene" : "AnimalsScene";
        SceneManager.LoadScene(sceneName);
    }

    public void VehiclesSahnesiniAc()
    {
        string sceneName = IsGermanMenu() ? "FahrzeugeScene" : "VehiclesScene";
        SceneManager.LoadScene(sceneName);
    }

    public void AcProfil()
    {
        string sceneName = IsGermanMenu() ? "ScoreboardSceneDe" : "ScoreboardScene";
        SceneManager.LoadScene(sceneName);
    }

    public void AcQuiz()
    {
        string sceneName = IsGermanMenu() ? "QuizSceneDe" : "QuizScene";
        SceneManager.LoadScene(sceneName);
    }

    public void CikisYap()
    {
        if (FirebaseAuth.DefaultInstance != null)
        {
            FirebaseAuth.DefaultInstance.SignOut();
            Debug.Log("Oturum kapatýldý.");
        }

        SceneManager.LoadScene("LoginScene");
    }
}
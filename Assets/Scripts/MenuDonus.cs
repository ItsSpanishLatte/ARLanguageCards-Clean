using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuDonus : MonoBehaviour
{
    public void MenuyeDon()
    {
        if (SpeechManager.Instance != null)
        {
            Destroy(SpeechManager.Instance.gameObject);
        }

        string currentScene = SceneManager.GetActiveScene().name;
        bool isGerman = currentScene.EndsWith("De") ||
                        currentScene.Contains("Frucht") ||
                        currentScene.Contains("Tier") ||
                        currentScene.Contains("Fahrzeug");

        string hedefMenü = isGerman ? "MainMenuDe" : "MainMenu";

        SceneManager.LoadScene(hedefMenü);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuDonus : MonoBehaviour
{
    public void MenuyeDon()
    {
        // SpeechManager'ý temizle
        if (SpeechManager.Instance != null) Destroy(SpeechManager.Instance.gameObject);

        // Þu anki sahne "De" ile bitiyorsa Almanca ana menüye, bitmiyorsa normal menüye dön
        string hedefSahne = SceneManager.GetActiveScene().name.EndsWith("De") ? "MainMenuDe" : "MainMenu";

        SceneManager.LoadScene(hedefSahne);
    }
}
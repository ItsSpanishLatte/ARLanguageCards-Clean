using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void FruitsSahnesiniAc()
    {
        SceneManager.LoadScene("FruitsScene");
    }

    public void AnimalsSahnesiniAc()
    {
        SceneManager.LoadScene("AnimalsScene");
    }
}
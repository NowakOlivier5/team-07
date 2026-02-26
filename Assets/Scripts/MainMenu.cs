using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayButton() {
        SceneManager.LoadScene("SampleScene");
        Debug.Log("Loading scene");
    }
    public void QuitButton() {
        Application.Quit();
        Debug.Log("Quit button test");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Loads the SampleScene when play is clicked
    public void PlayButton() {
        SceneManager.LoadScene("SampleScene");
        Debug.Log("Loading scene");
    }

    // Quits the applications when the game project is exported
    public void QuitButton() {
        Application.Quit();
        Debug.Log("Quit button test");
    }
}

using UnityEngine;

// A small gamemanager script was made to allow a smooth transition from main menu to the gameplay scene in singleplayer
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isSinglePlayer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

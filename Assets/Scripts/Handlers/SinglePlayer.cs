using UnityEngine;

// An extension of the gamemanager script but this exists within the main level scene
public class SinglePlayer : MonoBehaviour
{
    public GameObject singlePlayer;

    private void Start()
    {
        singlePlayer.SetActive(GameManager.Instance.isSinglePlayer);
    }
}

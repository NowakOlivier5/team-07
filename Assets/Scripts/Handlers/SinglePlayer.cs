using UnityEngine;

public class SinglePlayer : MonoBehaviour
{
    public GameObject singlePlayer;

    private void Start()
    {
        singlePlayer.SetActive(GameManager.Instance.isSinglePlayer);
    }
}

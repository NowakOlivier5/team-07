using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MMSteamMulti
{
    public class PlayerLobbyHandler : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnReadyStatusChanged))]
        public bool isReady = false;
        public Button readyButton;
        public TextMeshProUGUI nameText;

        void Start()
        {
            readyButton.interactable = isLocalPlayer;
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            readyButton.interactable = true;
            isReady = false;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            LobbyUIManager.Instance.RegisterPlayer(this);
        }

        [Command]
        void CmdSetReady()
        {
            isReady = !isReady;
            OnReadyStatusChanged(!isReady, isReady);
        }

        public void OnReadyButtonClicked()
        {
            CmdSetReady();
        }

        void SetSelectedButtonColor(Color color)
        {
            ColorBlock cb = readyButton.colors;
            cb.normalColor = color;
            cb.selectedColor = color;
            cb.disabledColor = color;
            readyButton.colors = cb;
        }

        void OnReadyStatusChanged(bool oldValue, bool newValue)
        {
            if (NetworkServer.active)
            {
                LobbyUIManager.Instance.CheckAllPlayersReady();
            }

            if (isReady)
            {
                SetSelectedButtonColor(Color.green);
            }
            else
            {
                SetSelectedButtonColor(Color.white);
            }
        }
    }
}
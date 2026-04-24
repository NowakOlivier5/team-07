using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Collections;
using Steamworks;

// This script handles the networking for the multiplayer lobby
// References: https://www.youtube.com/watch?v=WBOvM2pjt9E
namespace MMSteamMulti
{
    public class SteamLobby : MonoBehaviour
    {
        public static SteamLobby Instance;
        public GameObject hostButton = null;
        public ulong lobbyID;
        public NetworkManager networkManager;
        public PanelSwapper panelSwapper;
        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        protected Callback<LobbyEnter_t> lobbyEntered;
        protected Callback<LobbyChatUpdate_t> lobbyChatUpdate;

        private const string HostAddressKey = "HostAddress";

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            networkManager = GetComponent<NetworkManager>();
            if (!SteamManager.Initialized)
            {
                Debug.LogError("Steam is not initalized. Make sure to run this game in the steam environment");
                return;
            }
            panelSwapper.gameObject.SetActive(true);
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        }

        public void HostLobby()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
        }

        void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError("Failed to create lobby: " + callback.m_eResult);
                return;
            }

            Debug.Log("Lobby successfully created. Lobby ID: " + callback.m_ulSteamIDLobby);
            networkManager.StartHost();

            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
            lobbyID = callback.m_ulSteamIDLobby;
        }

        void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            Debug.Log("Join request received for lobby: " + callback.m_steamIDLobby);

            if (NetworkClient.isConnected || NetworkClient.active)
            {
                Debug.Log("NetworkClient is active or connected. Disconnecting before joining new lobby");
                NetworkManager.singleton.StopClient();
                NetworkClient.Shutdown();
            }
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        void OnLobbyEntered(LobbyEnter_t callback)
        {
            if (NetworkServer.active)
            {
                Debug.Log("Already in a lobby as a host. Ignorning join request");
                return;
            }
            lobbyID = callback.m_ulSteamIDLobby;
            string _hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
            networkManager.networkAddress = _hostAddress;
            Debug.Log("Entered lobby: " + callback.m_ulSteamIDLobby);
            networkManager.StartClient();
            panelSwapper.SwapPanel("LobbyPanel");
        }

        void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
        {
            if (callback.m_ulSteamIDLobby != lobbyID) return;

            EChatMemberStateChange stateChange = (EChatMemberStateChange)callback.m_rgfChatMemberStateChange;
            Debug.Log($"LobbyChatUpdate: {stateChange}");

            bool shouldUpdate = stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeEntered) ||
                                stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeLeft) ||
                                stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeDisconnected) ||
                                stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeKicked) ||
                                stateChange.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeBanned);

            if (shouldUpdate)
            {
                StartCoroutine(DelayedNameUpdate(0.5f));
                LobbyUIManager.Instance?.CheckAllPlayersReady();
            }
        }

        private IEnumerator DelayedNameUpdate(float delay)
        {
            if (LobbyUIManager.Instance == null)
            {
                Debug.LogWarning("Lobby UI Manager.Instance is null, skipping name update");
                yield break;
            }
            yield return new WaitForSeconds(delay);
            LobbyUIManager.Instance?.UpdatePlayerLobbyUI();
        }

        public void LeaveLobby()
        {
            CSteamID currentOwner = SteamMatchmaking.GetLobbyOwner(new CSteamID(lobbyID));
            CSteamID me = SteamUser.GetSteamID();
            var lobby = new CSteamID(lobbyID);
            List<CSteamID> members = new List<CSteamID>();

            int count = SteamMatchmaking.GetNumLobbyMembers(lobby);

            for (int i = 0; i < count; i++)
            {
                members.Add(SteamMatchmaking.GetLobbyMemberByIndex(lobby, i));
            }

            if (lobbyID != 0)
            {
                SteamMatchmaking.LeaveLobby(new CSteamID(lobbyID));
                lobbyID = 0;
            }

            if (NetworkServer.active && currentOwner == me)
            {
                NetworkManager.singleton.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopClient();
            }

            panelSwapper.gameObject.SetActive(true);
            this.gameObject.SetActive(true);
            panelSwapper.SwapPanel("MainMenuPanel");
        }
    }
}


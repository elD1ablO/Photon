using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    [SerializeField] GameObject mainScreen;
    [SerializeField] GameObject lobbyScreen;

    [Header("MainScreen")]
    [SerializeField] Button createRoomButton;
    [SerializeField] Button joinRoomButton;

    [Header("Lobby Screen")]
    [SerializeField] TextMeshProUGUI playerListText;
    [SerializeField] Button startGameButton;

    void Start()
    {
        //disable buttons at start as we're not connected to server;
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    void SetScreen(GameObject screen)
    {
        //deactivate all screens

        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        //enable requested screen
        screen.SetActive(true);
    }

    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }
    public void OnJoinRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

    //called when player name input field updated
    public void  OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);

        //updates lobby for everyone when there's a new player in lobby 
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    //called when player leaves room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    //updates player lists and hosts buttons
    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerListText.text = "";
        // display all players currently in lobby
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "/n";
        }

        //only the host can start the game
        if(PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;
    }

    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    public void OnStartGameButton()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }
}

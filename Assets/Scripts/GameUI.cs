using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUI : MonoBehaviour
{
    public PlayerUIContainer[] playerContainers;
    public TextMeshProUGUI winText;

    float updateTimer;

    public static GameUI instance;
    
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitializePlayerUI();
    }

    void Update()
    {
        UpdatePlayerUI();
    }
    void InitializePlayerUI()
    {
        //loop through all containers
        for(int i = 0; i < playerContainers.Length; i++)
        {
            PlayerUIContainer container = playerContainers[i];

            if (i < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[i].NickName;
                container.hatTimeSlider.maxValue = GameManager.instance.timeToWin;
            }
            else 
                container.obj.SetActive(false);
        }
    }
    void UpdatePlayerUI()
    {
        for (int i = 0; i < GameManager.instance.players.Length; i++)
        {
            if(GameManager.instance.players[i] != null)
            {
                playerContainers[i].hatTimeSlider.value = GameManager.instance.players[i].currentHatTime;
            }
        }
    }

    public void SetWinText(string winnerName)
    {
        winText.gameObject.SetActive(true);
        winText.text = winnerName + " wins!";
    }
}


[System.Serializable]
public class PlayerUIContainer
{
    public GameObject obj;
    public TextMeshProUGUI nameText;
    public Slider hatTimeSlider;
}


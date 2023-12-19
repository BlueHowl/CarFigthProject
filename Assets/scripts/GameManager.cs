using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{
    public byte MAXPLAYER = 2;

    public const byte STARTGAMEEVENTCODE = 1;

    public GameObject lobbyCanvas;
    public GameObject waitingText;

    public GameObject raceResult;
    public Transform resultHolder;

    public List<Transform> raceSpawnPoints = new List<Transform>(1) { null };

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckStart() {
        if(PhotonNetwork.PlayerList.Length < MAXPLAYER) {
            waitingText.SetActive(true);
            //StartGameEvent(); //(test)
        } else if(PhotonNetwork.PlayerList.Length == MAXPLAYER){
            StartGameEvent();
        }
    }

    private void StartGameEvent()
    {
        //object[] content = new object[] {}; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(STARTGAMEEVENTCODE, new object[0], raiseEventOptions, SendOptions.SendReliable);
    }
}

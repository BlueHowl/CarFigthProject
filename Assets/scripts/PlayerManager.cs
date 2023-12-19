using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour, IOnEventCallback
{
    PhotonView view;
    GameManager gm;
    public GameObject Canvas;
    public GameObject CanvasTop;
    public GameObject CanvasStartLights;
    public GameObject CanvasRaceResult;
    public GameObject cam;

    public GameObject car;

    public GameObject[] touchObjects;

    public GameObject[] startLights;

    public Movements movScript;
    public LapTracker lapScript;

    public string nickName = "default";
    public int spriteIndex = 0;//private?
    public Sprite[] carSprites;
    public SpriteRenderer sr;
    public Image im_track;

    public Text[] Places;

    public AudioListener aL;

    // Start is called before the first frame update
    void Start()
    {
        view = gameObject.GetComponent<PhotonView>();
        gm = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameManager>();

        if(view.IsMine) {
            //_SetView();

            cam.SetActive(true);
            Canvas.SetActive(true);

            movScript.enabled = true;
            //lapScript.enabled = true;

            nickName = PlayerPrefs.GetString("nickname", "default");
            spriteIndex = PlayerPrefs.GetInt("sIndex", 0);
            /*
            int sound = PlayerPrefs.GetInt("Sound", 0);

            if(sound == 1) {
                AudioListener.volume = 0;
            }
            */
            _ShowSprite(spriteIndex);
        } else {
            aL.enabled = false;

            this.gameObject.tag = "otherPlayers";
            this.gameObject.GetComponentInChildren<BoxCollider2D>().enabled = false;

            foreach(GameObject touchObj in touchObjects) {
                touchObj.SetActive(false);
            }

            _GetOthersSprites();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void _FinishGame() {
        gm.raceResult.SetActive(true);
        CanvasTop.SetActive(false);

        object[] instantationData = new object[1];
        instantationData[0] = nickName;
        PhotonNetwork.Instantiate("ResultItem", new Vector3(0, 0, 0), Quaternion.identity, 0, instantationData);

        //CanvasRaceResult.SetActive(true);
        //view.RPC("UpdateScore", RpcTarget.All, nickName);
    }
    /*
    [PunRPC]
    void UpdateScore(string pNickName) {
        Places[placeCounter].text = pNickName;
        placeCounter++;
    }
    */
    void StartGame() {
        if(view.IsMine) {
            CanvasTop.SetActive(true);
            movScript.freeze = true;
            movScript.rb2D.velocity = Vector2.zero;
            movScript.transform.localRotation = Quaternion.Euler(0, 0, 0);
            gm.waitingText.SetActive(false);
            lapScript.enabled = true;
            lapScript.Reset();

            int id = PhotonNetwork.LocalPlayer.ActorNumber;
            view.RPC("MoveCar", RpcTarget.All, view.ViewID, id);

            StartCoroutine(StartRace(3f));
        }

    }

    private IEnumerator StartRace(float waitTime)
    {
        startLights[0].SetActive(true);
        yield return new WaitForSeconds(waitTime);
        startLights[1].SetActive(true);
        yield return new WaitForSeconds(1f);
        startLights[2].SetActive(true);
        yield return new WaitForSeconds(1f);
        startLights[3].SetActive(true);
        yield return new WaitForSeconds(1f);
        startLights[4].SetActive(true);
        movScript.freeze = false;
        yield return new WaitForSeconds(0.5f);
        CanvasStartLights.SetActive(false);
    }
    /*
    public void _SetView() {
        view.RPC("SetView", RpcTarget.All, view.ViewID);
    }

    [PunRPC]
    void SetView(int player) {
        PhotonView _view = PhotonView.Find(player);
        _view.GetComponent<PlayerManager>().view = _view;
    }
    */
    [PunRPC]
    void MoveCar(int player, int actor) {
        Transform temp = gm.raceSpawnPoints[(actor == -1) ? 0 : actor % gm.raceSpawnPoints.Count]; //round robin
        GameObject p = PhotonView.Find(player).gameObject;
        p.transform.position = temp.position;

        Transform car = p.GetComponent<PlayerManager>().car.transform;

        car.transform.localPosition = new Vector3(0, 0, 0);
        car.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void _InitDrift() {
        view.RPC("InitDrift", RpcTarget.All, view.ViewID);
    }

    [PunRPC]
    void InitDrift(int player) {
        Movements playerMov = PhotonView.Find(player).GetComponentInChildren<Movements>();

        foreach(TrailRenderer trail in playerMov.trails) {
            trail.emitting = true;
        }
        playerMov.skidSound.Play();
    }


    public void _EndDrift() {
        view.RPC("EndDrift", RpcTarget.All, view.ViewID);
    }

    [PunRPC]
    void EndDrift(int player) {
        Movements playerMov = PhotonView.Find(player).GetComponentInChildren<Movements>();
        /*
        if(playerMov.leftTrail != null && playerMov.rightTrail != null) {
            playerMov.leftTrail.transform.parent = null;//trailTrashBin;
            playerMov.rightTrail.transform.parent = null;//trailTrashBin;
        }
        */

        foreach(TrailRenderer trail in playerMov.trails) {
            trail.emitting = false;
        }

        playerMov.skidSound.Stop();
    }
    /*
    public void _ShowSprite(int tex) {
        im_track.sprite = carSprites[tex];
        view.RPC("ShowSprite", RpcTarget.All, view.ViewID, tex);
    }

    [PunRPC]
    void ShowSprite(int player, int tex) {
        PlayerManager _pm = PhotonView.Find(player).GetComponent<PlayerManager>();
        //_pm.spriteIndex = tex;
        _pm.sr.sprite = carSprites[tex];
    }*/

    public void _ShowSprite(int tex) {
        im_track.sprite = carSprites[tex];
        view.RPC("ShowSprite", RpcTarget.All, tex);
    }

    [PunRPC]
    void ShowSprite(int tex) {
        spriteIndex = tex;
        sr.sprite = carSprites[tex];
    }

    public void _GetOthersSprites() {
        view.RPC("GetOthersSprites", RpcTarget.Others);
    }

    [PunRPC]
    void GetOthersSprites() {
        //PhotonView.Find(player).GetComponent<PlayerManager>().sr.sprite = carSprites[tex];
        _SendSprite(view.ViewID, spriteIndex);
        //sr.sprite = carSprites[spriteIndex];
    }

    public void _SendSprite(int player, int tex) {
        view.RPC("SendSprite", RpcTarget.Others, player, tex);
    }

    [PunRPC]
    void SendSprite(int player, int tex) {
        //if(view.ViewID == reedemer) {
        PhotonView.Find(player).GetComponent<PlayerManager>().sr.sprite = carSprites[tex];
        //}
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == GameManager.STARTGAMEEVENTCODE)
        {
            StartGame();
        }
    }

}

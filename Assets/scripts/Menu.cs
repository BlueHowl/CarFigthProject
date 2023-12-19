using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using Photon.PUN;

public class Menu : MonoBehaviour
{
    public InputField nicknameInput;

    public Image muteImg;

    public Sprite[] soundImgs;

    public GameObject carSprite; 
    public GameObject mainScreen;
    public GameObject loadingScreen;
    public GameObject customScreen;
    public GameObject levelScreen;

    public Transform background;
    public Transform menuRoad;

    public Sprite[] carSprites;

    public SpriteRenderer sr;

    private int spriteIndex = 0;

    private bool startChanging = false;
    private bool endChanging = false;

    private float roadScaleFactor = 0f;
    private float roadPositionFactor = 0f;

    public float changingSpeed = 6f;

    private int currentScreen = 0;

    private int Sound = 0;

    private AudioSource _as;
    public AudioClip airWrench;

    // Start is called before the first frame update
    void Start()
    {   
        _as = GetComponent<AudioSource>();

        string nickname = PlayerPrefs.GetString("nickname", "default");
        Sound = PlayerPrefs.GetInt("Sound", 0);

        if(Sound == 1) {
            AudioListener.volume = 0;
        }

        if(Sound == 1) {
            muteImg.sprite = soundImgs[1];
        }

        if(!nickname.Equals("default")) {
            nicknameInput.text = nickname;
        }

        spriteIndex = PlayerPrefs.GetInt("sIndex", 0);
        sr.sprite = carSprites[spriteIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape)) {
            if(currentScreen == 1 || currentScreen == 2) {
                CHANGESCREEN(0);
            }
        }

        if(startChanging) {
            if(sr.transform.position.y < 7f) {
                sr.transform.position +=  new Vector3(0, changingSpeed * Time.deltaTime, 0);
            } else {
                sr.sprite = carSprites[spriteIndex];
                sr.transform.position =  new Vector3(0, -6f, 0);
                startChanging = false;
                endChanging = true;
            }
        } else if(endChanging) {
            if(sr.transform.position.y < 0f) {
                sr.transform.position +=  new Vector3(0, changingSpeed * Time.deltaTime, 0);
            } else {
                endChanging = false;
            }
        }

        if(currentScreen == 2) {
            if(roadScaleFactor < 1f) {
                roadScaleFactor += 5f * Time.deltaTime;
            }
        } else {
            if(roadScaleFactor > 0f) {
                roadScaleFactor -= 5f * Time.deltaTime;
            }
        }

        float val = Mathf.Lerp(1f, 2.5f, roadScaleFactor);
        menuRoad.localScale = new Vector3(val, val, val);


        if(currentScreen == 1) {
            if(roadPositionFactor < 1f) {
                roadPositionFactor += 2.5f * Time.deltaTime;
            }
        } else {
            if(roadPositionFactor > 0f) {
                roadPositionFactor -= 2.5f * Time.deltaTime;
            }
        }

        val = Mathf.Lerp(-5.5f, -15.5f, roadPositionFactor);
        background.position = new Vector3(background.position.x, val, background.position.z);

    }

    public void CHANGESCREEN(int id) {
        carSprite.SetActive(true);

        mainScreen.SetActive(false);
        customScreen.SetActive(false);
        levelScreen.SetActive(false);

        switch(id) {
            case 0:
                currentScreen = 0;
                mainScreen.SetActive(true);
                break;

            case 1:
                currentScreen = 1;
                customScreen.SetActive(true);
                break;

            case 2:
                currentScreen = 2;
                carSprite.SetActive(false);
                levelScreen.SetActive(true);
                break;
        }

        _as.PlayOneShot(airWrench);
    }

    public void LOADSCENE(string name) {
        mainScreen.SetActive(false);
        loadingScreen.SetActive(true);
        SceneManager.LoadSceneAsync(name); // load with photon
        //PhotonNetwork.LoadSceneAsync(name);//  LoadLevel(0);
    }

    
    public void CHANGENICKNAME(string nickname) {
        PlayerPrefs.SetString("nickname", nickname);
    }

    public void INCREMENTSINDEX() {
        if(!startChanging && !endChanging) {
            spriteIndex = (spriteIndex + 1) % carSprites.Length;
            PlayerPrefs.SetInt("sIndex", spriteIndex);
            //sr.sprite = carSprites[spriteIndex];
            startChanging = true;
        }
    }

    public void DECREMENTSINDEX() {
        if(!startChanging && !endChanging) {
            if(spriteIndex - 1 < 0) {
                spriteIndex = carSprites.Length-1;
            } else {
                spriteIndex = (spriteIndex - 1) % carSprites.Length;
            }
            PlayerPrefs.SetInt("sIndex", spriteIndex);
            //sr.sprite = carSprites[spriteIndex];
            startChanging = true;
        }
    }

    public void SOUNDBTN() {
        Sound = (Sound + 1) % 2;
        if(Sound == 0) {
            muteImg.sprite = soundImgs[0];
        } else {
            muteImg.sprite = soundImgs[1];
        }

        AudioListener.volume = 1 - AudioListener.volume;
        PlayerPrefs.SetInt("Sound", Sound);

        _as.PlayOneShot(airWrench);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;

public class Movements : MonoBehaviour
{
    //variables
    PhotonView photonView;
    public Rigidbody2D rb2D;
    public GameObject touchObject;
    public GameObject touch1Object;
    public Text debugText;

    public Transform car_sprite;

    public TrailRenderer[] trails;

    public AudioSource skidSound;
    public AudioSource motorSound;

    public float max_motor_pitch = 1f;

    private Vector2 TouchVector;
    private Vector2 TouchVector1;

    public PlayerManager PM;

    public float speed = 0f;
    private float maxSpeed = 9f;

    private float deg = 0f;
    private float distance = 0f;
    private float degSpeedFactor = 1f;

    private bool touchSide = false;

    private bool stoppedAccelerating = true;
    private bool stopped = true;

    public bool freeze = false;

    private bool isDrifting = false;

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);

        Physics2D.gravity = Vector2.zero;
        rb2D.velocity = Vector2.zero; // ???todo rpc sync
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount > 0)
        {
            // Get the touch info
            Touch t = Input.GetTouch(0);
            //debugText.text = "touch1 " + t;
            if (Input.touchCount > 1)
            {
                // Get the touch info
                Touch t1 = Input.GetTouch(1);

                // Determine l'ordre de passage des points de pression
                if(t.phase == TouchPhase.Began || t1.phase == TouchPhase.Began) {
                    stopped = false;
                    stoppedAccelerating = false;

                    if(t.position.x > t1.position.x) {
                        touchSide = true;
                    } else {
                        touchSide = false;
                    }
                }


                if(t.phase == TouchPhase.Ended || t1.phase == TouchPhase.Ended) {
                    //stopped = true;
                    stoppedAccelerating = true;
                    //drift = false;
                }

                
                if(touchSide) {
                    TouchVector = t.position;
                    TouchVector1 = t1.position;
                } else {
                    TouchVector = t1.position;
                    TouchVector1 = t.position;
                }
            } 
        }

    }

    void FixedUpdate()
    {
        //ternary condition for rev boost while drifting
        motorSound.pitch = Mathf.Lerp(1f, max_motor_pitch, (rb2D.velocity.magnitude / maxSpeed) + (isDrifting ? 1f : 0f));

        if(stoppedAccelerating) {
            //make the car slightly return straight if stopped while turning
            if(rb2D.velocity.magnitude > 0.5f) {
                rb2D.velocity -= 0.05f * rb2D.velocity;
                if(Mathf.Abs(deg) > 0) {
                    if(deg < 0) {
                        deg += 90f * Time.deltaTime;
                    } else {
                        deg -= 90f * Time.deltaTime;
                    }
                    this.transform.Rotate(0, 0, (-deg-90 / 180) * Time.fixedDeltaTime);
                    car_sprite.localRotation = Quaternion.Euler(0, 0, -deg / degSpeedFactor);
                }
            } else {
                stoppedAccelerating = false;
                stopped = true;
            }
            return;
        }

        if(stopped) {
            rb2D.velocity = Vector2.zero;
            if(isDrifting) {
                //drift = false;
                isDrifting = false;
                PM._EndDrift();
                //dropTrail();
            }
            return;
        }

        HandleTwoFingerMovement();
    }

    //two finger function
    void HandleTwoFingerMovement(){
        //touch position visualizer
        touchObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(TouchVector.x, TouchVector.y, 10));
        touch1Object.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(TouchVector1.x, TouchVector1.y, 10));

        //calculs
        float dx = TouchVector.x * Screen.height/Screen.width - TouchVector1.x * Screen.height/Screen.width;
        float dy = TouchVector.y - TouchVector1.y;

        //if(LimitSpeedFactor(TouchVector) && LimitSpeedFactor(TouchVector1)) { //limiter la distance
        distance = (Mathf.Clamp(Hypotenuse(dx, dy), 0, Screen.height) / Screen.height) * 100;
            // Debug.Log(distance);
        //}

        float rads = Mathf.Atan2(-dy,dx);
        rads %= 2*Mathf.PI;
        deg = Mathf.Clamp(rads * Mathf.Rad2Deg, -90, 90);

        //Debug.Log(deg);

        if(!freeze) {
            //rotate
            this.transform.Rotate(0, 0, (-deg-90 / 180) * Time.fixedDeltaTime);

            //Debug.Log(distance);
            //if(distance >= 60) {
                //rb2D.AddForce(transform.up * (distance / 500));// * Time.deltaTime;
            degSpeedFactor = ((135 - Mathf.Abs(deg)) / 180) * 5;
            speed = (distance * degSpeedFactor * Time.fixedDeltaTime / 7f) * maxSpeed;
            rb2D.velocity = transform.up * Mathf.Clamp(speed, 0, maxSpeed);
            //} 
            /*else {
                rb2D.velocity = Vector2.zero;
            }*/

            //rotate sprite (drift)
            //if(Mathf.Abs(deg) <= 90f) {
            car_sprite.localRotation = Quaternion.Euler(0, 0, -deg / degSpeedFactor);
            //}
        }

        //Debug.Log(Mathf.Abs(deg) / (maxSpeed+1 - speed));

        //drift gestion
        if(Mathf.Abs(deg) / (maxSpeed+1 - speed) >= 8f) { //speed >= 4f && Mathf.Abs(deg) >= 40f) {
            //drift = true;
            //Handheld.Vibrate(); //ajouter vibration 
            if(!isDrifting) {
                isDrifting = true;

                PM._InitDrift();
            }

        } else {
            isDrifting = false;
            PM._EndDrift();
        }

        debugText.text = "Deg : " + deg + "\ndegfact : " + degSpeedFactor + "\nSpeed : " + speed + "\nFps : " + (int)(Time.frameCount / Time.time);
    }

    float Hypotenuse(float a, float b)
    {
        return Mathf.Sqrt(a * a + b * b);
    }

/*
    bool LimitSpeedFactor(Vector2 pos) {
        float x = pos.x * Screen.height/Screen.width; //pos x étendu à la hauteur de l'écran
        float y = pos.y;

        float dx = pos.x - (Screen.height / 2);
        float dy = pos.y - (Screen.height / 2);
        float dist = Hypotenuse(dx, dy);

        if(dist < (Screen.height / 2)) { //distance du centre < rayon
            return true;
        }

        return false;
    }
*/

}
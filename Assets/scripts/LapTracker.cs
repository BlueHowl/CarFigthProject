using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapTracker : MonoBehaviour
{
    private int triggerCounter = 0;
    private int lapCounter = 0;
    //public static int lastTriggerNumber = 22;
    public int MAXLAP = 3;
    public Text lapCount;
    public Slider lapSlider;

    public Transform[] CheckPoints;

    public GameObject wrongWay;

    private int distBetweenChecks = 0;
    private int lastDistBetweenChecks = 0;
    private float checkPointAdvancement = 0f;

    private Movements mov;

    public PlayerManager PM;


    void Start() {
        mov = GetComponent<Movements>();
        CheckPoints = GameObject.FindGameObjectsWithTag("CheckPoints")[0].GetComponent<CheckPoints>().CheckPointsList;

        lapCount.text = lapCounter + " / " + MAXLAP;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "trigger")
        {
            int tn = other.gameObject.GetComponent<Trigger>().triggerNumber;
            Debug.Log(triggerCounter + " : num :" + tn);
            if(triggerCounter == tn) {
                //wrongWay.SetActive(false);

                if(triggerCounter == CheckPoints.Length-1) {
                    triggerCounter = 0;
                    checkPointAdvancement = 0;
                    lapCounter++;
                    lapCount.text = lapCounter + " / " + MAXLAP;

                    if(lapCounter >= MAXLAP) {
                        mov.freeze = true;
                        PM._FinishGame();
                    }

                } else {
                    triggerCounter++;
                    lastDistBetweenChecks = distBetweenChecks;
                }

                int index = (triggerCounter-1) % CheckPoints.Length;
                if(index != -1) {
                    distBetweenChecks = CalculateDistanceBetween(CheckPoints[index], CheckPoints[triggerCounter]);                  
                }
                
                lapSlider.value = triggerCounter;
            }
            /*else if(triggerCounter < tn) {
                wrongWay.SetActive(true);
            }*/

            /*else if(triggerCounter == tn-1 && triggerCounter > 0) {
                triggerCounter--;
            }*/
        }
    }

    void Update() {
        int index = (triggerCounter-1) % CheckPoints.Length;
        if(index != -1) {
            int dist = CalculateDistanceBetween(this.transform, CheckPoints[index]);
            float temp = triggerCounter - 1 + (float)dist / (float)distBetweenChecks;

            int indexLast = (index-1) % CheckPoints.Length;
            if(indexLast != -1) {
                int distLast = CalculateDistanceBetween(this.transform, CheckPoints[indexLast]);

                if(distLast > lastDistBetweenChecks && distLast > dist) {
                    if(temp > checkPointAdvancement) {
                        checkPointAdvancement = temp;
                        lapSlider.value = temp;
                    }   
                }
                /*else if(distLast < lastDistBetweenChecks || distLast < dist) {
                    Debug.Log("à l'envers");
                }*/
                
            }  

        }
    }

    int CalculateDistanceBetween(Transform a, Transform b) {
        float dx = a.position.x - b.position.x;
        float dy = a.position.y - b.position.y;
        return (int)Mathf.Sqrt(dx * dx + dy * dy);
    }

    public void Reset() {
        lapCounter = 0;
        triggerCounter = 0;
        distBetweenChecks = 0;
        lastDistBetweenChecks = 0;
        checkPointAdvancement = 0f;
    }
}

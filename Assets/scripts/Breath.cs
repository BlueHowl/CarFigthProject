using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breath : MonoBehaviour
{
    float breathVal = 0f;
    bool phase = false;

    public float minScale = 0.3f;
    public float maxScale = 1f;

    public float speed = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(breathVal < 1f && !phase) {
            breathVal += speed * Time.deltaTime;
        } else {
            phase = true;
        }

        if(breathVal > 0f && phase) {
            breathVal -= speed * Time.deltaTime;
        } else {
            phase = false;
        }

        float val = Mathf.Lerp(minScale, maxScale, breathVal);
        this.transform.localScale = new Vector2(val, val);
    }
}

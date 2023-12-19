using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    private Transform[] allChildren;
    // Start is called before the first frame update
    void Start()
    {
        allChildren = GetComponentsInChildren<Transform>();

        StartCoroutine(BlinkRoutine(1f, 0.5f));
    }

    private IEnumerator BlinkRoutine(float showTime, float hideTime)
    {
        SwitchState(true);
        yield return new WaitForSeconds(showTime);
        SwitchState(false);
        yield return new WaitForSeconds(hideTime);
    }

    private void SwitchState(bool state) {
        foreach (Transform child in allChildren)
        {
            child.gameObject.SetActive(state);
        }
    }
}

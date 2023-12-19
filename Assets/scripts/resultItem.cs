using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class resultItem : MonoBehaviour
{
    public Text position;
    public Text nickname;

    void Start() {
        PhotonView pv = GetComponent<PhotonView>();
        this.transform.parent = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameManager>().resultHolder;
 
        object[] data = pv.InstantiationData;
        nickname.text = (string) data[0];

        int pos = this.transform.parent.childCount;
        position.text = pos.ToString();

        if(pos == 2) {
            position.color = new Color32(154, 153, 152, 255);
        } else if(pos == 3) {
            position.color = new Color32(137, 99, 60, 255);
        } else if(pos > 3) {
            position.color = Color.white;
        }
    }
}

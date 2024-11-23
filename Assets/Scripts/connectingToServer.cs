using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class connectingToServer : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Menu");
    }
    // Update is called once per frame
}
;
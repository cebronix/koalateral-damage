using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MyPhoton : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
      PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
      SceneManager.LoadScene("Menu");
    }
}

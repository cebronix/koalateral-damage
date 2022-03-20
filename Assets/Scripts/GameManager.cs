using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using Cinemachine;

public class GameManager : MonoBehaviourPunCallbacks
{
  [Header("Stats")]
  public bool gameEnded = false;

  public float invincibleDuration;


  [Header("Players")]
  public string playerPrefabLocation;
  public Transform[] spawnPoints;
  public PlayerController[] players;
//   public int playerWithHat;
  public int playersInGame;

  // instace
  public static GameManager instance;

  void Awake() {
    // instance
    instance = this;
  }

  void Start() {
    players = new PlayerController[PhotonNetwork.PlayerList.Length];
    photonView.RPC("ImInGame", RpcTarget.AllBuffered);
  }

  [PunRPC]
  void ImInGame() {
    playersInGame++;
    if(playersInGame == PhotonNetwork.PlayerList.Length) {
      SpawnPlayer();
    }
  }

  [PunRPC]
  public void LastPlayer(int playerId) {
    if(playersInGame == 1) {
      photonView.RPC("WinGame", RpcTarget.AllBuffered, playerId);
    }
  }

  void SpawnPlayer() {
    // Instantiate the player across the network
    GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
    GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().Follow = playerObj.transform;
    
    // Get the player script
    PlayerController playerScript = playerObj.GetComponent<PlayerController>();

    // Temporary so all players don't look the same
    // playerScript.photonView.RPC("Recolor", RpcTarget.AllBuffered);

    // Initialize the player
    playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
  }

  public PlayerController GetPlayer(int playerId) {
    return players.First(x => x.id == playerId);
  }

  public PlayerController GetPlayer(GameObject playerObj) {
    return players.First(x => x.gameObject == playerObj);
  }

  [PunRPC]
  void WinGame(int playerId) {
    gameEnded = true;
    PlayerController player = GetPlayer(playerId);
    GameUI.instance.SetWinText(player.photonPlayer.NickName);
    Invoke("GoBackToMenu", 5.0f);
  }

  void GoBackToMenu() {
    PhotonNetwork.LeaveRoom();
    NetworkManager.instance.ChangeScene("Menu");
  }
}

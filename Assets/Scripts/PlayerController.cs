using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
  [HideInInspector]
  public int id;

  [Header("Info")]

  [Header("Components")]
  public Player photonPlayer;

  [PunRPC]
  public void Initialize(Player player) {
    photonPlayer = player;
    id = player.ActorNumber;
    GameManager.instance.players[id -1] = this;
  }

  // Temporary so all players don't look the same
  [PunRPC]
  public void Recolor() {
    SpriteRenderer renderer = GetComponent<SpriteRenderer>();
    renderer.color = UnityEngine.Random.ColorHSV();
  }

  [PunRPC]
  public void Die() {
    Destroy(gameObject);

    GameManager.instance.playersInGame--;

    if(PhotonNetwork.IsMasterClient) {
      GameManager.instance.LastPlayer(id);
    }
  }

//   void Update() {
//     if(PhotonNetwork.IsMasterClient) {
//     //   if(curHatTime >= GameManager.instance.timeToWin && !GameManager.instance.gameEnded) {
//     //     GameManager.instance.gameEnded = true;
//     //     GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, id);
//     //   }
//     }

//   void TryJump() {
//     Ray ray = new Ray(transform.position, Vector3.down);

//     if(Physics.Raycast(ray, 0.7f)) {
//       rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
//     }
//   }

//   public void SetHat(bool hasHat) {
//     hatObject.SetActive(hasHat);
//   }

//   void OnCollisionEnter(Collision collision) {
//     if(!photonView.IsMine)
//       return;

//     // did we hit another player?
//     if(collision.gameObject.CompareTag("Player")) {
//       // do they have the hat?
//       if(GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithHat) {
//         // can we get the hat?
//         if(GameManager.instance.CanGetHat()) {
//           // give us the hat
//           GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
//         }
//       }
//     }
//   }

//   public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
//     if(stream.IsWriting) {
//       stream.SendNext(curHatTime);
//     } else if(stream.IsReading) {
//       curHatTime = (float)stream.ReceiveNext();
//     }
//   }
}

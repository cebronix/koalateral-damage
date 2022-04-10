using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
  [HideInInspector]
  public int id;
  public Rigidbody2D rb;
  protected PlayerMovement playerMovement;
  protected SpriteRenderer playerSprite;

  [Header("Info")]

  [Header("Components")]
  public Player photonPlayer;

  void Start() {
    playerMovement = GetComponent<PlayerMovement>();
    playerSprite = GetComponent<SpriteRenderer>();
    // playerHealth = GetComponent<Health>();
  }

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

  void OnTriggerEnter2D(Collider2D other) {
    // Hop over low walls if standing, hide if crawling
    if(other.gameObject.tag == "lowWall") {
      rb = playerMovement.rb;
      Vector3 posA = rb.transform.position;
      Vector3 posB = other.gameObject.transform.position;
      Vector3 dir = (posB - posA).normalized;
      BoxCollider2D lowWallBlock = other.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>();

      if((dir.y < 0) && (playerMovement.isCrawling == false)) {
        Vector3 hop = new Vector3 (posA.x, posA.y -0.75f, posA.z);
        rb.transform.position = hop;
      } else if((dir.y < 0) && (playerMovement.isCrawling == true)) {
        GetComponent<PhotonView>().RPC("JumpLayer", RpcTarget.AllBuffered, "lowWalls", 12);
        lowWallBlock.enabled = true;
      } else if((dir.y > 0) && (playerMovement.isCrawling == false)) {
        Vector3 hop = new Vector3 (posA.x, posA.y +0.75f, posA.z);
        rb.transform.position = hop;
        lowWallBlock.enabled = false;
      } else if((dir.y > 0) && (playerMovement.isCrawling == true)) {
        GetComponent<PhotonView>().RPC("JumpLayer", RpcTarget.AllBuffered, "Player", 6);
        lowWallBlock.enabled = true;
      }
    }
  }

  void OnCollisionEnter2D(Collision2D other) {
    if((other.gameObject.tag == "tunnel") && (playerMovement.isCrawling == true)) {
      Collider2D tunnel = other.collider;
      tunnel.isTrigger = true;
      GetComponent<PhotonView>().RPC("JumpLayer", RpcTarget.AllBuffered, "Background", 11);
    }
  }
  
  // Reset after trigger
  void OnTriggerExit2D(Collider2D other) {
    if(other.gameObject.tag == "lowWall") {
      BoxCollider2D lowWallBlock = other.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>();
      GetComponent<PhotonView>().RPC("JumpLayer", RpcTarget.AllBuffered, "Player", 6);
      lowWallBlock.enabled = false;
    } else if(other.gameObject.tag == "tunnel") {
      BoxCollider2D tunnel = other.transform.gameObject.GetComponent<BoxCollider2D>();
      tunnel.isTrigger = false;
      GetComponent<PhotonView>().RPC("JumpLayer", RpcTarget.AllBuffered, "Player", 6);
    }
  }

  [PunRPC]
  public void JumpLayer(string sortLayer, int layer) {
    playerSprite.sortingLayerName = sortLayer;
    gameObject.layer = layer;
  }

  [PunRPC]
  public void Die() {
    Destroy(gameObject);

    GameManager.instance.playersInGame--;

    if(PhotonNetwork.IsMasterClient) {
      GameManager.instance.LastPlayer(id);
    }
  }

  public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    if(stream.IsWriting) {
      stream.SendNext(photonPlayer);
    } else {
      photonPlayer = (Photon.Realtime.Player)stream.ReceiveNext();
    }
  }
}

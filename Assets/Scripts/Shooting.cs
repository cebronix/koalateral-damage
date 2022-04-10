using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPun
{
  public Transform FirePoint;
  public GameObject dartPrefab;
  public GameObject trapPrefab;
  public Vector3 trap_position;
  public BoxCollider2D bc;
  private bool reloaded = true;
  protected PlayerMovement playerMovement;
  public PlayerController playerController;

  void Start() {
    playerMovement = GetComponentInParent<PlayerMovement>();
  }
    
  void Update() {
    if(!photonView.IsMine) return;
    if(Input.GetButtonDown("Fire1") && reloaded && playerMovement.isCrawling == false) {
      GetComponent<PhotonView>().RPC("Shoot", RpcTarget.AllBuffered);
    }
    if(Input.GetButtonDown("layTrap")) {
      GetComponent<PhotonView>().RPC("Trap", RpcTarget.AllBuffered);
    }
  }

  [PunRPC]
  void Shoot() {
    WeaponStats weaponStats = GetComponentInParent<WeaponStats>();
    if(weaponStats.ammo > 0 && photonView.IsMine) {
      reloaded = false;
      GameObject dart = PhotonNetwork.Instantiate(dartPrefab.name, FirePoint.position, FirePoint.rotation);
      dart.gameObject.name = "dart";
      Rigidbody2D rb = dart.GetComponent<Rigidbody2D>();
      rb.AddForce(FirePoint.up * weaponStats.dartForce, ForceMode2D.Impulse);
      weaponStats.photonView.RPC("expendAmmo", RpcTarget.AllBuffered);
      StartCoroutine(Reload(weaponStats.fireRate));
    }
  }

  [PunRPC]
  void Trap() {
    if(playerMovement.playerSprite.flipX == false && photonView.IsMine) {
      trap_position = transform.position + new Vector3(0 +1,0,0);
    } else if(playerMovement.playerSprite.flipX == true && photonView.IsMine) {
      trap_position = transform.position + new Vector3(0 -1,0,0);
    }
    if(photonView.IsMine) {
      playerController = GetComponent<PlayerController>();
      GameObject trap = PhotonNetwork.Instantiate(trapPrefab.name, trap_position, Quaternion.identity);
      trap.GetComponent<trap>().setTrapOwner(PhotonNetwork.LocalPlayer.ActorNumber);
      trap.gameObject.name = "trap";
    }
  }

  public IEnumerator Reload(float fireRate) {
     yield return new WaitForSeconds(fireRate);
     reloaded = true;
  }
}

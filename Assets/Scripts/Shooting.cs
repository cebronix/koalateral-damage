using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPun
{
  public Transform FirePoint;
  public GameObject dartPrefab;
  private bool reloaded = true;
  protected PlayerMovement playerMovement;

  void Start() {
    playerMovement = GetComponentInParent<PlayerMovement>();
  }
    
  void Update() {
    if(!photonView.IsMine) return;
    if(Input.GetButtonDown("Fire1") && reloaded && playerMovement.isCrawling == false) {
      GetComponent<PhotonView>().RPC("Shoot", RpcTarget.AllBuffered);
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

  public IEnumerator Reload(float fireRate) {
     yield return new WaitForSeconds(fireRate);
     reloaded = true;
  }
}

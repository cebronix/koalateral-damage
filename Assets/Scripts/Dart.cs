using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Dart : MonoBehaviourPun
{
  public GameObject hitEffect;
  public Rigidbody2D rb;
  public bool liveAmmo = true;
  public float maxAngle = 176;
  protected WeaponStats weaponStats;
  protected BoxCollider2D firedDart;
  protected Health health;

  void Awake() {
    firedDart = GetComponent<BoxCollider2D>();
  }
  
  void OnCollisionEnter2D(Collision2D other) {
    Vector2 normal = other.contacts[0].normal;
    Vector2 vel = rb.velocity;
    health = other.collider.GetComponent<Health>();
    if(gameObject != null) {
      if((other.gameObject.name == "Player" || other.gameObject.name == "Player(Clone)") && liveAmmo == true) {
        health.takeDamage(1);
        Destroy(gameObject);
        return;
      } else {
        if (Vector2.Angle(vel, -normal) >= maxAngle) {
          GetComponent<PhotonView>().RPC("stopDart", RpcTarget.AllBuffered, 0.02f);
        } else {
          rb.velocity = Vector2.Reflect(vel, normal);
          GetComponent<PhotonView>().RPC("stopDart", RpcTarget.AllBuffered, 0.5f);
        }
      }
    }
  }

   void OnTriggerEnter2D(Collider2D other) {
      weaponStats = other.GetComponent<WeaponStats>();
      if((other.gameObject.name == "Player" || other.gameObject.name == "Player(Clone)") && weaponStats.ammo < weaponStats.maxAmmo && liveAmmo == false) {
        weaponStats.addAmmo(1);
        GetComponent<PhotonView>().RPC("DestroyDart", RpcTarget.AllBuffered);
      }
   }

  [PunRPC]
  protected IEnumerator stopDart(float waitTime) {
    rb.velocity = rb.velocity / 6;
    yield return new WaitForSeconds(waitTime);
    rb.velocity = Vector2.zero;
    rb.angularVelocity = 0f;
    firedDart.isTrigger = true;
    liveAmmo = false;
  }

  [PunRPC]
    void DestroyDart() {
        Destroy(gameObject);
    }

  // [PunRPC]
  // public void playerHit() {
  //   if(photonView.IsMine) return;
  //     // GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
  //     // Destroy(effect, 5f);
  //   health.takeDamage(1);
  // }

}
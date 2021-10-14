using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmmoPack : MonoBehaviour
{
    protected WeaponStats weaponStats;
    void Start() {
        StartCoroutine(LateStart(1f));
    }

    IEnumerator LateStart(float waitTime)
     {
        yield return new WaitForSeconds(waitTime);
        GameObject player = GameObject.Find("Player(Clone)");
        weaponStats = player.GetComponent<WeaponStats>();
     }

    [PunRPC]
    void OnTriggerEnter2D(Collider2D other) {
     if((other.gameObject.name == "Player" || other.gameObject.name == "Player(Clone)") && weaponStats.ammo < weaponStats.maxAmmo) {
        GetComponent<PhotonView>().RPC("pickupAmmoPack", RpcTarget.AllBuffered);
      }
    }

    [PunRPC]
    public void pickupAmmoPack() {
        weaponStats.addAmmo(4);
        Destroy(gameObject);
    }
}
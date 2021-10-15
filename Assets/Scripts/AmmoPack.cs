using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmmoPack : MonoBehaviour
{
    protected WeaponStats weaponStats;
    private int damage = 4;

    [PunRPC]
    void OnTriggerEnter2D(Collider2D other) {
        if(gameObject != null) {
            weaponStats = other.GetComponent<WeaponStats>();
            if((other.gameObject.name == "Player" || other.gameObject.name == "Player(Clone)") && weaponStats.ammo <= weaponStats.maxAmmo - damage) {
                weaponStats.addAmmo(damage);
                GetComponent<PhotonView>().RPC("DestroyPack", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void DestroyPack() {
        Destroy(gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthPack : MonoBehaviour
{
    protected Health health;
    private int heal = 4;

    [PunRPC]
    void OnTriggerEnter2D(Collider2D other) {
        if(gameObject != null) {
            health = other.GetComponent<Health>();
            if((other.gameObject.name == "Player" || other.gameObject.name == "Player(Clone)") && health.health < health.maxHealth) {
                health.healDamage(heal);
                GetComponent<PhotonView>().RPC("DestroyPack", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void DestroyPack() {
        Destroy(gameObject);
    }
}

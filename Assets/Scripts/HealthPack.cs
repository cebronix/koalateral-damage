using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthPack : MonoBehaviour
{
    protected Health health;
    void Start() {
        StartCoroutine(LateStart(1f));
    }

    IEnumerator LateStart(float waitTime)
     {
        yield return new WaitForSeconds(waitTime);
        GameObject player = GameObject.Find("Player(Clone)");
        health = player.GetComponent<Health>();
     }

    [PunRPC]
    void OnTriggerEnter2D(Collider2D other) {
        if(gameObject != null) {
            if((other.gameObject.name == "Player" || other.gameObject.name == "Player(Clone)") && health.health < health.maxHealth) {
                health.healDamage(1);
                Destroy(gameObject);
            }
        }
    }
}

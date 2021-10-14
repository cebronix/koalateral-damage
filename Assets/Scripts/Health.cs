using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Health : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;

    public float health = 0;
    public float maxHealth = 5;
    public PlayerController playerController;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting) {
            stream.SendNext(health);
        } else {
            health = (float)stream.ReceiveNext();
        }

    }

    void Start() {
        health = maxHealth;
        if(!photonView.IsMine) {
            Destroy(ui);
        }
    }

    public void takeDamage(int damage) {
        health -= damage;
        healthbarImage.fillAmount = health/maxHealth;
    }

    public void healDamage(int heal) {
        health += heal;
        healthbarImage.fillAmount = health/maxHealth;
    }

    void Update() {
        if(!photonView.IsMine) return;
        if(health <= 0) {
            playerController.GetComponent<PhotonView>().RPC("Die", RpcTarget.AllBuffered);
        }
    }
}

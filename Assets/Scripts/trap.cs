using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class trap : MonoBehaviourPunCallbacks, IPunObservable {
    protected BoxCollider2D laidTrap;
    protected Health health;
    public int trapOwner;
    protected SpriteRenderer[] children;
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting) {
        stream.SendNext(trapOwner);
        } else {
        trapOwner = (int)stream.ReceiveNext();
        }
    }

    void Awake() {
        laidTrap = GetComponent<BoxCollider2D>();
        laidTrap.isTrigger = false;
        StartCoroutine(TrapEnable(laidTrap));
    }

    [PunRPC]
    public void setTrapOwner(int trappedBy) {
        trapOwner = trappedBy;
    }

    [PunRPC]
    void OnTriggerEnter2D(Collider2D other) {
        int localPlayer = PhotonNetwork.LocalPlayer.ActorNumber;
        if(other.gameObject.name == "Player(Clone)" && trapOwner != localPlayer) {
            health = other.GetComponent<Health>();
            health.takeDamage(4);
            GetComponent<PhotonView>().RPC("DestroyTrap", RpcTarget.AllBuffered);
        }
    }

    // Trap takes time to set
    [PunRPC]
    public IEnumerator TrapEnable(BoxCollider2D laidTrap) {
        children = laidTrap.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer child in children) {
            if(child.name == "minimap icon" && !photonView.IsMine) {
                StartCoroutine(FadeOut(0.5f, child));
            }

            if(child.name == "spring") {
                StartCoroutine(FadeOut(0.5f, child));
            }
        }
        yield return new WaitForSeconds(3);
        laidTrap.isTrigger = true;
    }

    [PunRPC]
    public IEnumerator FadeOut(float fadeSpeed, SpriteRenderer child) {
        while (child.color.a > 0) {
            Color objectColor = child.color;
            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            child.color = objectColor;
            yield return null;
        }
    }

    [PunRPC]
    void DestroyTrap() {
        Destroy(gameObject);
    }
}

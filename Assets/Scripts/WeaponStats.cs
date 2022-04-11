using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class WeaponStats : MonoBehaviourPunCallbacks, IPunObservable
{
  public int maxAmmo = 20;
  public int ammo = 0;
  public int trapInv = 0;
  public float fireRate = 0;
  public float dartForce = 10f;
  public TextMeshProUGUI ammoCount;

  public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    if(stream.IsWriting) {
      stream.SendNext(ammo);
    } else {
      ammo = (int)stream.ReceiveNext();
    }
  }

  void Start() {
    ammo = maxAmmo;
    trapInv = 1;
    fireRate = 0.5f;
  }

  [PunRPC]
  public void expendAmmo() {
    if(!photonView.IsMine) return;
    ammo--;
    ammoCount.SetText("x {0}", ammo);
  }

  [PunRPC]
  public void addAmmo(int darts) {
    if(!photonView.IsMine) return;
    ammo += darts;
    ammoCount.SetText("x {0}", ammo);
  }

  [PunRPC]
  public void expendTrapInv() {
    if(!photonView.IsMine) return;
    trapInv--;
  }

  [PunRPC]
  public void addTrapInv() {
    if(!photonView.IsMine) return;
    ammo ++;
  }

}

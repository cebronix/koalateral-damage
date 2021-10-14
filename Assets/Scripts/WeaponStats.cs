using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponStats : MonoBehaviourPunCallbacks, IPunObservable
{
  public int maxAmmo = 20;
  public int ammo = 0;
  public float fireRate = 0;
  public float dartForce = 10f;

  public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
      if(stream.IsWriting) {
          stream.SendNext(ammo);
      } else {
          ammo = (int)stream.ReceiveNext();
      }
  }

  void Start() {
    ammo = maxAmmo;
    fireRate = 0.5f;
  }

  [PunRPC]
  public void expendAmmo() {
    if(!photonView.IsMine) return;
    ammo--;
  }

  [PunRPC]
  public void addAmmo(int darts) {
    if(!photonView.IsMine) return;
    ammo += darts;
  }

}

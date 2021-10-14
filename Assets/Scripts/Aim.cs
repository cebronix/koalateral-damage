using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Aim : MonoBehaviourPun
{
  private Transform aimTransform;
  public Camera cam;
  //PhotonView photonView;

  void Start() {
    //photonView = GetComponent<PhotonView>();
  }
  
  private void Awake() {
    aimTransform = transform.Find("Weapon");
    cam = Camera.main;
  }

  private void Update() {
    if(!photonView.IsMine) return;
    Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    Vector3 lookDir = (mousePos - transform.position).normalized;
    float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
    aimTransform.eulerAngles = new Vector3(0, 0, angle);
  }
}

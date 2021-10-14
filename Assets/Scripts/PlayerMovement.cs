using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
  public float moveSpeed = 5f;
  public Rigidbody2D rb;
  Vector2 movement;
  PhotonView photonView;
  private Vector3 jukeDir;
  private float jukeSpeed;
  private State state;
  private enum State {
    Normal,
    Juking,
    Crawling,
  }

  void Start() {
    photonView = GetComponent<PhotonView>();
    state = State.Normal;
  }

  void Update() {
    if(photonView.IsMine) {
      switch (state) {
      case State.Normal:
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        Juke();
        Crawl();
        break;
      case State.Juking:
        HandleJuking();
        break;
      case State.Crawling:
        HandleCrawling();
        break;
      }
    }
  }

  void FixedUpdate() {
    if(!photonView.IsMine) return;
    rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
  }

  void Juke() {
    if(Input.GetMouseButtonDown(1)) {
      state = State.Juking;
      Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      jukeDir = (worldPosition - transform.position).normalized;
      jukeSpeed = 50f;
    }
  }

  void Crawl() {
    if(Input.GetKeyDown("space")) {
      state = State.Crawling;
    }
  }

  private void HandleJuking() {
    transform.position += jukeDir * jukeSpeed * Time.deltaTime;
    jukeSpeed -= jukeSpeed * 5f * Time.deltaTime;
    if(jukeSpeed < 5f) {
      state = State.Normal;
    }
  }

  private void HandleCrawling() {
    //
  }
    
}

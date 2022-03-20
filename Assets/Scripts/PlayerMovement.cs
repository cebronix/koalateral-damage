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
  public bool isCrawling = false;
  public GameObject weapon;
  protected SpriteRenderer playerSprite;
  protected SpriteRenderer gunDrawn;
  protected float originalRadius;
  public Animator animator;
  private enum State {
    Normal,
    Juking,
  }

  void Start() {
    photonView = GetComponent<PhotonView>();
    playerSprite = GetComponent<SpriteRenderer>();
    gunDrawn = weapon.GetComponent<SpriteRenderer>();
    originalRadius = GetComponent<CircleCollider2D>().radius;
    animator = GetComponent<Animator>();
    state = State.Normal;
  }

  void Update() {
    if(photonView.IsMine) {
      switch (state) {
      case State.Normal:
        movement.x = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("speed", Mathf.Abs(movement.x));
        if (movement.x > 0) {
          playerSprite.flipX = false;
        } else if (movement.x < 0) {
          playerSprite.flipX = true;
        }
        movement.y = Input.GetAxisRaw("Vertical");
        Juke();
        Crawl();
        break;
      case State.Juking:
        HandleJuking();
        break;
      }
    }
  }

  void FixedUpdate() {
    if(!photonView.IsMine) return;
    if(isCrawling == false) {
      rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    } else {
      rb.MovePosition(rb.position + movement * moveSpeed/3 * Time.fixedDeltaTime);
    }
  }

  void Juke() {
    if(Input.GetMouseButtonDown(1)) {
      state = State.Juking;
      Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      jukeDir = (worldPosition - transform.position).normalized;
      jukeSpeed = 50f;
    }
  }

  private void HandleJuking() {
    transform.position += jukeDir * jukeSpeed * Time.deltaTime;
    jukeSpeed -= jukeSpeed * 5f * Time.deltaTime;
    if(jukeSpeed < 5f) {
      state = State.Normal;
    }
  }

  void Crawl() {
    if(Input.GetKeyDown("space")) {
      if(!photonView.IsMine) return;
      isCrawling = !isCrawling;
      photonView.RPC("HolsterWeapon", RpcTarget.AllBuffered);
      if(isCrawling == true) {
        transform.localScale = new Vector3(1f, 0.5f, 1f);
        GetComponent<CircleCollider2D>().radius = originalRadius * 0.5f;
      } else {
        transform.localScale = new Vector3(1f, 1f, 1f);
        GetComponent<CircleCollider2D>().radius = originalRadius;
      }
    }
  }

  [PunRPC]
  void HolsterWeapon() {
    gunDrawn.enabled = !gunDrawn.enabled;
  }
}

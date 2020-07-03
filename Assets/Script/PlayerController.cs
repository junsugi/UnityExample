using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    AudioSource audioSource;

    Rigidbody2D rigid;
    SpriteRenderer render;
    Animator animator;
    CapsuleCollider2D capsuleCollier;

    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;


    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollier = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();


    }

    void PlaySound(String action) {
        switch (action) {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }

    //단발적인 키 입력은 Update에서 하는 것
    void Update() 
    {
        //점프
        if (Input.GetButtonDown("Jump") && !animator.GetBool("IsJumping")) {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("IsJumping", true);
            PlaySound("JUMP");
        }

        //방향키를 떼면 급격하게 속도 줄여주는 로직 (Stop Speed)
        if (Input.GetButtonUp("Horizontal")) {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //방향 전환
        if (Input.GetButton("Horizontal")) {
            render.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //Animation 전환
        if(Mathf.Abs(rigid.velocity.x) < 0.4) {
            animator.SetBool("IsWalking", false);
        } 
        else {
            animator.SetBool("IsWalking", true);
        }
    }

    public void VelocityZero() {
        rigid.velocity = Vector2.zero;
    }

    //물리 기반 작업할 때
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h * 2, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed) {   //Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        } 
        else if (rigid.velocity.x < maxSpeed * (-1)) {   //Left Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        if(rigid.velocity.y < 0) {
            //Landing Platform (위치, 빔 방향, 색)
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            //빔을 쏘고 빔을 맞은 오브젝트에 대한 정보 (여기서는 layer가 Platform인 오브젝트만 받겠다.)
            RaycastHit2D rayhit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayhit.collider != null) {
                //플레이어의 절반 크기
                if (rayhit.distance < 0.5f) {
                    animator.SetBool("IsJumping", false);
                }
            }
        }
    }

    //물리 충돌
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enermy") {
            //Attack
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y) {
                OnAttack(collision.transform);
                PlaySound("ATTACK");
            }
            else {
                OnDamaged(collision.transform.position);
                PlaySound("DAMAGED");
            }
           
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Item") {
            //Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze) {
                gameManager.stagePoint += 50;
            } else if (isSilver) {
                gameManager.stagePoint += 100;
            } else if (isGold) {
                gameManager.stagePoint += 200;
            }

            PlaySound("ITEM");
            //Deactive Item
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish") {
            PlaySound("FINISH");
            // Next Stage
            gameManager.NextStage();
        }
    }

    void OnAttack(Transform enermy) 
    {
        gameManager.stagePoint += 100;

        //Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        //Enermy Die
        EnermyMove enermyMove = enermy.GetComponent<EnermyMove>();
        enermyMove.OnDamaged();
        PlaySound("DIE");
    }

    void OnDamaged(Vector2 targetPos) 
    {
        //Health Down
        gameManager.HealthDown();

        //PlayerDamaged = 11 (한 대 맞으면 Layer 변경)
        this.gameObject.layer = 11;

        //피격 시 색 변경
        render.color = new Color(1, 1, 1, 0.4f);

        //맞으면 팅겨 나감
        int dirc = this.transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 9, ForceMode2D.Impulse);

        //Animation
        animator.SetTrigger("damaged");

        Invoke("OffDamaged", 2);
    }

    void OffDamaged() 
    {
        //Player = 10;
        this.gameObject.layer = 10;
        render.color = new Color(1, 1, 1, 1);
    }

    public void OnDie() {
        //Sprite Alpha
        render.color = new Color(1, 1, 1, 0.4f);
        //Sprite FlipY
        render.flipY = true;
        //Collider Disable
        capsuleCollier.enabled = false;
        //Die effect
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        PlaySound("DIE");
    }
}


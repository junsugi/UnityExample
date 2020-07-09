using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public float maxSpeed;
    Rigidbody2D rigid;
    Animator animator;
    SpriteRenderer render;
    CapsuleCollider2D capsuleCollier;

    public GameManagerTest gameManager;

    public float jumpPower;

    
    void Awake()
    {
        this.rigid = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.render = GetComponent<SpriteRenderer>();
        this.capsuleCollier = GetComponent<CapsuleCollider2D>();
    }

    void Update() 
    {
        //방향 전환
        if (Input.GetButton("Horizontal")) {
            render.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //Animation 전환
        if (Mathf.Abs(rigid.velocity.x) < 0.4) {
            animator.SetBool("isWalking", false);
        }
        else {
            animator.SetBool("isWalking", true);
        }

        //Landing Platform (위치, 빔 방향, 색)
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
        //빔을 쏘고 빔을 맞은 오브젝트에 대한 정보 (여기서는 layer가 Platform인 오브젝트만 받겠다.)
        RaycastHit2D rayhit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayhit.collider != null) {
            //플레이어의 절반 크기
            if (rayhit.distance < 0.5f) {
                animator.SetBool("isJumping", false);
            }
        }

        //방향키를 떼면 급격하게 속도 줄여주는 로직 (Stop Speed)
       if (Input.GetButtonUp("Horizontal")) {
           rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }
    }

    void FixedUpdate()
    {
        //좌우 입력을 확인하기 위한 객체를 받는다.
        float horizontal = Input.GetAxisRaw("Horizontal");
        //입력된 값에 따라 RigidBody가 적용된 스프라이트(=Player)에 힘을 가한다.
        rigid.AddForce(Vector2.right * horizontal, ForceMode2D.Impulse);

        if(rigid.velocity.x > maxSpeed) {   //오른쪽으로 이동할 때 속도 제한
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        } else if (rigid.velocity.x < maxSpeed * (-1)) {   //왼쪽으로 이동할 때 속도 제한
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        //Jump
        if (Input.GetButtonDown("Jump") && !animator.GetBool("isJumping")) {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
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
            }
            else if (isSilver) {
                gameManager.stagePoint += 100;
            }
            else if (isGold) {
                gameManager.stagePoint += 200;
            }
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish") {
            gameManager.NextStage();
        }
    }

    //물리 충돌
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Enermy") {
            //Attack
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y) {
                OnAttack(collision.transform);
            }  else {
                OnDamaged(collision.transform.position);
            }       
        }
    }
    void OnAttack(Transform enermy) {
        //Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        //Enermy Die
        TestEnermyScript enermyMove = enermy.GetComponent<TestEnermyScript>();
        enermyMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos) {
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
        animator.SetTrigger("Damaged");

        Invoke("OffDamaged", 2);
    }

    void OffDamaged() {
        //Player = 10;
        this.gameObject.layer = 10;
        render.color = new Color(1, 1, 1, 1);
    }

    public void VelocityZero() {
        rigid.velocity = Vector2.zero;
    }
    public void OnDie() {
        Time.timeScale = 0;
        //Sprite Alpha
        render.color = new Color(1, 1, 1, 0.4f);
        //Sprite FlipY
        render.flipY = true;
        //Collider Disable
        capsuleCollier.enabled = false;
        //Die effect
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }
}

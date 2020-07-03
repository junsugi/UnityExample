using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnermyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator animator;
    SpriteRenderer render;
    CapsuleCollider2D capsuleCollier;

    public int nextMove;

    void Awake()
    {
        this.rigid = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.render = GetComponent<SpriteRenderer>();
        this.capsuleCollier = GetComponent<CapsuleCollider2D>();

        Invoke("Think", 5);
    }

    void FixedUpdate()
    {
        //Move
        this.rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //자기 한수 앞
        Vector2 frontVector = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        //Platform Check
        Debug.DrawRay(frontVector, Vector3.down, new Color(0, 1, 0));
        //빔을 쏘고 빔을 맞은 오브젝트에 대한 정보 (여기서는 layer가 Platform인 오브젝트만 받겠다.)
        RaycastHit2D rayhit = Physics2D.Raycast(frontVector, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayhit.collider == null) {
            Turn();
        }
    }

    //재귀함수
    void Think() 
    {
        nextMove = Random.Range(-1, 2);
        //애니메이션
        animator.SetInteger("WalkSpeed", nextMove);
        //방향
        if(nextMove != 0) {
            render.flipX = nextMove == 1;
        }

        //재귀함수 호출
        float nextThinkTime = Random.Range(2f, 6f);
        Invoke("Think", nextThinkTime);
    }

    void Turn() {
        //방향 바꾸기
        nextMove *= -1;
        //캐릭터 뒤집기
        render.flipX = nextMove == 1;
        //생각 취소
        CancelInvoke();
        //다시 생각
        Invoke("Think", 5);
    }

    public void OnDamaged() {
        //Sprite Alpha
        render.color = new Color(1, 1, 1, 0.4f);
        //Sprite FlipY
        render.flipY = true;
        //Collider Disable
        capsuleCollier.enabled = false; ;
        //Die effect
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Destroy enermy
        Invoke("DeActive", 5);
    }

    void DeActive() {
        this.gameObject.SetActive(false);
    }
}

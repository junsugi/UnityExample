using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public float maxSpeed;
    Rigidbody2D rigid;

    
    void Awake()
    {
        this.rigid = GetComponent<Rigidbody2D>();
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
    }
}

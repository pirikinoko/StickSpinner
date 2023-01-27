using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveObject : MonoBehaviour
{
    //��{
    float floreSpeed = 1.3f, circleSpeed = 1.0f;
    Rigidbody2D rbody2D;
    //Circle�p
    Vector2 StartPos;
    public float RightLimit = 1.0f, LeftLimit = 1.0f;
    float ReverseCoolDown = 1.0f;
    bool reverse = true;
    void Start()
    {
        rbody2D = GetComponent<Rigidbody2D>();
        StartPos = this.gameObject.transform.position;

        if (this.gameObject.CompareTag("Reverse"))    //���]�^�O�Ői�s�������]
        {
            circleSpeed *= -1;
        }
    }

    void Update()
    {
        if (this.gameObject.name.Contains("Circle"))
        {
            MoveCircle();
        }
        if (this.gameObject.name.Contains("Flore"))
        {
            MoveFlore();
        }      
    }
    //�܂��͏��A�ǁA����Circle�ɓ��������甽�]
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Surface"))
        {
            floreSpeed *= -1;
            circleSpeed *= -1;
            reverse = false;
        }

    }

    void MoveFlore()
    {
        if (ButtonInGame.Paused == 1)
        {
            rbody2D.velocity = new Vector2(0f, 0f);
        }
        else
        {
            rbody2D.velocity = new Vector2(floreSpeed, 0f);
        }
    }

    void MoveCircle()
    {
        if (reverse == false)
        {
            ReverseCoolDown -= Time.deltaTime;
            if (ReverseCoolDown < 0)
            {
                reverse = true;
                ReverseCoolDown = 1.0f;
            }
        }
        //�����ʒu���x����1�������甽�]
        if (this.gameObject.transform.position.x > StartPos.x + RightLimit && reverse || this.gameObject.transform.position.x < StartPos.x - LeftLimit && reverse)
        {
            circleSpeed *= -1;
            reverse = false;
        }

    }

}

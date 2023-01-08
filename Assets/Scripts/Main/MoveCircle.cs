using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCircle : MonoBehaviour
{
    float speed = 1f;
    Vector2 StartPos;
    public Rigidbody2D rbody2D;
    float ReverseCoolDown = 1.0f;
    public float RightLimit = 1.0f, LeftLimit = 1.0f;
    bool reverse = true;
    // Start is called before the first frame update
    void Start()
    {
        rbody2D = GetComponent<Rigidbody2D>();
        StartPos = this.gameObject.transform.position;
        if (this.gameObject.CompareTag("Reverse"))
        {
            speed *= -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //É|Å[ÉYíÜÇÕí‚é~
        if (ButtonInGame.Paused == 1)
        {
            rbody2D.velocity = new Vector2(0f, 0f);
        }
        else
        {
            rbody2D.velocity = new Vector2(speed, 0f);
        }

        if(reverse == false)
        {
            ReverseCoolDown -= Time.deltaTime;
            if(ReverseCoolDown < 0)
            {
                reverse = true;
                ReverseCoolDown = 1.0f;
            }
        }
        //èâä˙à íuÇÊÇËxé≤Ç…1ìÆÇ¢ÇΩÇÁîΩì]
        if (this.gameObject.transform.position.x > StartPos.x + RightLimit && reverse || this.gameObject.transform.position.x < StartPos.x - LeftLimit && reverse)
        {
            speed *= -1;
            reverse = false;
        }
     
    }
    //Ç‹ÇΩÇÕè∞ÅAï«ÅAëºÇÃCircleÇ…ìñÇΩÇ¡ÇΩÇÁîΩì]
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall") || col.gameObject.CompareTag("Surface"))
        {
            speed *= -1;
            reverse = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFlore : MonoBehaviour
{
    private float speed = 1.3f;
    public Rigidbody2D rbody2D;
    // Start is called before the first frame update
    void Start()
    {
        rbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(ButtonInGame.Paused == 1)
        {
            rbody2D.velocity = new Vector2(0f, 0f);
        }
        else
        {
            rbody2D.velocity = new Vector2(speed, 0f);
        }
        
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            speed *= -1;
        }
        else if (other.gameObject.CompareTag("Surface"))
        {
            speed *= -1;
        }
    }
}

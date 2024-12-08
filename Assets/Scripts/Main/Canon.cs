using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{

    Vector2 canonPos, firePos, fireForce;
    Rigidbody2D ballRb;
    public float cycle = 1.0f;
    float time;
    public float xMin, xMax;
    public float yMin, yMax;
    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if (time > 0) 
        {
            return;
        }
        canonPos = this.transform.position;
        firePos = canonPos;
        firePos.x -= 0.5f;
        GameObject obj = Instantiate(Resources.Load("Ball") as GameObject, firePos, Quaternion.identity);
        ballRb = obj.GetComponent<Rigidbody2D>();

        float xForce = Random.Range(xMin, xMax);
        float yForce = Random.Range(-yMin, yMax);
        fireForce = new Vector3(-xForce, yForce);  // 力を設定
        ballRb.linearVelocity = fireForce;

        Destroy(obj, 5f);
        time = cycle;
    }
}
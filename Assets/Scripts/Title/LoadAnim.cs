using UnityEngine;

public class LoadAnim : MonoBehaviour
{
    float speed, increaseRate = 800;
    int maxSpeed, minSpeed;
    bool increase;
    Transform _transform;
    Vector3 rotZ;
    // Start is called before the first frame update
    void Start()
    {
        increase = false;
        maxSpeed = 1500;
        minSpeed = 300;
        speed = maxSpeed;
        _transform = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (increase)
        {
            speed += increaseRate * Time.deltaTime;
        }
        else
        {
            speed -= increaseRate * Time.deltaTime;
        }
        if (speed > maxSpeed)
        {
            increase = false;
        }
        else if (speed < minSpeed)
        {
            increase = true;
        }

        rotZ.z += (int)(speed * Time.deltaTime);
        _transform.eulerAngles = rotZ;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowEffect : MonoBehaviour
{
    //基本
    bool start;
    string direction = "Right";
    public float speed = 1.3f, delay = 0;
    Rigidbody2D rbody2D;
    RigidbodyConstraints2D defaultConstraints;
    public enum Direction
    {
        Right,
        Left,
    }
    // Inspector上で選択できるようにする変数
    public Direction selectedDirection;
    private string[] optionStrings = { "Right", "Left" };

    //移動制限
    Vector2 StartPos;
    public float RightLimit = 1.0f, LeftLimit = 1.0f;
    float tmpSpeed = 0;

    //透明度変更
    float fadeSpeed = 1.4f;
    private Material material;
    private Color originalColor;

    IEnumerator startDelay(float delaySec)
    {
        yield return new WaitForSeconds(delaySec);
        start = true;
    }
    void Start()
    {
        start = false;
        if (optionStrings[(int)selectedDirection] == "Left")
        {
            speed *= -1;
        }

        rbody2D = GetComponent<Rigidbody2D>();
        if (rbody2D != null)
        {
            defaultConstraints = rbody2D.constraints;
        }

        StartPos = this.gameObject.transform.position;
        if (this.gameObject.CompareTag("Reverse"))    //反転タグで進行方向反転
        {
            speed *= -1;
        }
        //色取得
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
            originalColor = material.color;
        }
    }

    void Update()
    {
        if (!start)
        {
            StartCoroutine(startDelay(delay));
            return;
        }
        RigidbodyConstraints2D constraints = rbody2D.constraints;
            if (ButtonInGame.Paused == 1)
            {
                constraints = RigidbodyConstraints2D.FreezePosition;
            }
            else
            {
                constraints = defaultConstraints;
            }
            rbody2D.constraints = constraints;

        WaveObj();
    }

    void WaveObj()
    {
        rbody2D.velocity = new Vector2(speed, 0f);
        if (this.gameObject.transform.position.x > StartPos.x + RightLimit || this.gameObject.transform.position.x < StartPos.x - LeftLimit)
        {
            StartCoroutine(DelaySpawn());
        }
        float fadeOutTime = 0.6f;
        if (this.gameObject.transform.position.x > StartPos.x + RightLimit - fadeOutTime || this.gameObject.transform.position.x < StartPos.x - LeftLimit + fadeOutTime)
        {
            float timeToReachLimit;
            if (speed > 0)
            {
                timeToReachLimit = ((StartPos.x + RightLimit) - this.gameObject.transform.position.x) / speed;
            }
            else
            {
                timeToReachLimit = (this.gameObject.transform.position.x - (StartPos.x - LeftLimit)) / Mathf.Abs(speed);
            }
            float newAlpha = Mathf.Clamp01(material.color.a - (1 / timeToReachLimit) * Time.deltaTime);
            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            material.color = newColor;
        }
    }
    IEnumerator DelaySpawn()
    {
        this.gameObject.transform.position = StartPos;
        material.color = originalColor;
        yield return new WaitForSeconds(1);
    }
}

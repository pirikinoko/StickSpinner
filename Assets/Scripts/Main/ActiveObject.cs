using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ActiveObject : MonoBehaviour  //動く床などのオブジェクト制御用
{
    //基本
    bool isAnimationStarted;
    string direction = "Right";
    public float speed = 1.3f, delay = 0;
    Rigidbody2D rbody2D;
    GameSetting gameSetting;
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
    public float RightLimit = 1.0f, LeftLimit = 1.0f, UpLimit = 1.0f, DownLimit = 1.0f;
    float ReverseCoolDown = 1.0f;
    bool reverse = true;
    float tmpSpeed = 0;
    //サイズ制限
    float startSize, sizeLimit;
    public float sizeMulti;
    //オブジェクト削除
    public float deleteTimer = 1.0f;
    //透明度変更
    float fadeSpeed = 1.4f;
    private Material material;
    private Color originalColor;

    void Start()
    {
        gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
        rbody2D = GetComponent<Rigidbody2D>();
        Renderer renderer = GetComponent<Renderer>();
        isAnimationStarted = false;
        StartPos = this.gameObject.transform.position;
        startSize = transform.localScale.y;
        sizeLimit = startSize * sizeMulti;

        if (optionStrings[(int)selectedDirection] == "Left")
        {
            speed *= -1;
        }


        if (rbody2D != null)
        {
            defaultConstraints = rbody2D.constraints;
        }


        if (this.gameObject.CompareTag("Reverse"))    //反転タグで進行方向反転
        {
            speed *= -1;
        }

        if (renderer != null)
        {
            material = renderer.material;
            originalColor = material.color;
        }
    }

    IEnumerator startDelay(float delaySec)
    {
        yield return new WaitForSeconds(delaySec);
        isAnimationStarted = true;
    }

    void Update()
    {
        //オンラインの場合は全プレイヤーのセットアップ完了を待つ
        if (GameStart.gameMode1 == "Online")
        {
            if (!GameSetting.setupEnded) { return; }
        }

        //遅延の設定がある場合は遅延
        if (!isAnimationStarted)
        {
            StartCoroutine(startDelay(delay));
            return;
        }


        if (this.gameObject.name.Contains("Effect"))
        {
            deleteTimer -= Time.deltaTime;
            if (deleteTimer < 0)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            if (this.gameObject.name.Contains("Anim")) { return; }
            RigidbodyConstraints2D constraints = rbody2D.constraints;

            if (gameSetting.isPaused)
            {
                constraints = RigidbodyConstraints2D.FreezePosition;
            }
            else
            {
                constraints = defaultConstraints;
            }
            rbody2D.constraints = constraints;
            if (this.gameObject.name.Contains("MoveCircle"))
            {
                LimitMove();
            }
            if (this.gameObject.name.Contains("MoveFloor"))
            {
                LimitMove();
            }
            if (this.gameObject.name.Contains("Y"))
            {
                LimitMoveY();
            }
            if (this.gameObject.name.Contains("MoveBlock"))
            {
                LimitMove();
            }
            if (this.gameObject.name.Contains("ExtendFloor"))
            {
                ExtendY();
            }
            if (this.gameObject.name.Contains("Rotate"))
            {
                RotateObj();
            }
            if (this.gameObject.name.Contains("Wave"))
            {
                WaveObj();
            }
        }



    }


    void ColReverse()
    {
        if (gameSetting.isPaused)
        {
            rbody2D.velocity = new Vector2(0f, 0f);
        }
        else
        {
            rbody2D.velocity = new Vector2(speed, 0f);
        }
    }

    void ExtendY()
    {
        Vector3 localScale = transform.localScale;
        localScale.y += speed * Time.deltaTime;
        transform.localScale = localScale;

        Vector2 objPos = this.transform.position;
        objPos.y += (speed / 2) * Time.deltaTime;
        this.transform.position = objPos;

        if (localScale.y > sizeLimit || localScale.y <= 0)
        {
            speed *= -1;
        }
    }
    void LimitMove()
    {
        rbody2D.velocity = new Vector2(speed, 0f);
        if (reverse == false)
        {
            ReverseCoolDown -= Time.deltaTime;
            if (ReverseCoolDown < 0)
            {
                reverse = true;
                ReverseCoolDown = 1.0f;
            }
        }
        //初期位置よりx軸に1動いたら反転
        if (this.gameObject.transform.position.x > StartPos.x + RightLimit && reverse || this.gameObject.transform.position.x < StartPos.x - LeftLimit && reverse)
        {
            speed *= -1;
            reverse = false;
        }
    }

    void LimitMoveY()
    {
        rbody2D.velocity = new Vector2(0, speed);
        if (reverse == false)
        {
            ReverseCoolDown -= Time.deltaTime;
            if (ReverseCoolDown < 0)
            {
                reverse = true;
                ReverseCoolDown = 1.0f;
            }
        }
        //初期位置よりx軸に1動いたら反転
        if (this.gameObject.transform.position.y > StartPos.y + UpLimit && reverse || this.gameObject.transform.position.y < StartPos.y - DownLimit && reverse)
        {
            speed *= -1;
            reverse = false;
        }

    }

    void RotateObj()
    {
        Transform myTransform = this.transform;
        Vector3 worldAngle = myTransform.eulerAngles;
        worldAngle.z += speed * Time.deltaTime;
        myTransform.eulerAngles = worldAngle; // 回転角度を設定
    }

    void WaveObj()
    {
        rbody2D.velocity = new Vector2(speed, 0f);
        if (this.gameObject.transform.position.x > StartPos.x + RightLimit  || this.gameObject.transform.position.x < StartPos.x - LeftLimit )
        {
            StartCoroutine(DelaySpawn());
        }
        float fadeOutTime = 0.2f;
        if (this.gameObject.transform.position.x > StartPos.x + RightLimit - fadeOutTime || this.gameObject.transform.position.x < StartPos.x - LeftLimit + fadeOutTime)
        {
            float timeToReachLimit;
            if (speed > 0)
            {
                timeToReachLimit = ((StartPos.x + RightLimit) - this.gameObject.transform.position.x) / speed;
            }
            else 
            {
                timeToReachLimit = (this.gameObject.transform.position.x - (StartPos.x - LeftLimit) ) / Mathf.Abs(speed);
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
        

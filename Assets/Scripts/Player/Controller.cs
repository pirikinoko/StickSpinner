using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Controller : MonoBehaviour
{
    [SerializeField]
    public int id;                                      // プレイヤー番号(1～4)
    [SerializeField]
    KeyCode KeyLeft, KeyRight;                          // 左右キー(キーボード使用時)
    float rotSpeed = 160f;                              // 棒の回転速度
    [SerializeField]
    float CoolTime_ = 0.2f;                             // ジャンプのクールタイム初期化用
    float coolTime = 0.2f;                              // ジャンプのクールタイム
    [SerializeField]
    public GameObject deadTimer;
    [SerializeField]
    Text SensText;
    [SerializeField]
    Sprite[] aryFace = new Sprite[6];
    [SerializeField]
    public GameObject arrow;
    int face { get; set; }                               　　 // 顔設定用乱数を入れておく1～100
    float stickRot = 0f;                                　　　// 棒の角度
    float jumpforce = 8.3f;                            　　　 // Y軸ジャンプ力
    bool  onFloor, onSurface, onPlayer, onStick, onPinball;   // 接触している時は true
    bool  inputCrossX;                               　　　 　// 十字ボタンの入力があるときはtrue
    float delay = 0.15f;
    bool delayFlag = false;
    bool isRespowing = false;
    private GameObject nameTag;        //ネームタグ
    SpriteRenderer stickSprite;       // 棒スプライト
    Rigidbody2D stickRb;              // 棒のRigidbody  
    SpriteRenderer parentSprite;      // プレイヤーの顔
    private Vector3 playerPos, pausedPos, latestPos; 　　　　　　　　 　//プレイヤー,棒の位置
    private Vector2 Playerspeed, speedWhenPaused, deadPlayerPos;　　　　//プレイヤー速度,ポーズ直前のプレイヤー速度
    private float   saveCount = 0; 　　　　　　　　　　　　　　　　　   // ポーズ処理に使用
    private Body    body;
    GameObject bodyObj;






    readonly int[] aryFaceRatio = { 25, 50, 70, 88, 94, 100};   // 顔の変化用


    void Start()
    {     
        nameTag = GameObject.Find("P" + id.ToString() + "Text");
        // 親スプライト・スティックスプライトを得る
        parentSprite =  transform.parent.gameObject.GetComponent<SpriteRenderer>();
        stickSprite  = GetComponent<SpriteRenderer>();
        bodyObj = transform.parent.gameObject;

        isRespowing    = false;
        stickRot     = 0f;
        coolTime     = 0.0f;
        onSurface    = false;
        onPlayer     = false;
        onStick      = false;
        stickRb = GetComponent<Rigidbody2D>();

        if (GameStart.Stage == 1)
        {
            arrow.SetActive(true);
        }
        else
        {
            arrow.SetActive(false);
        }

        // 顔をランダムで設定する
        face = UnityEngine.Random.Range(1, 100);
        for(int i = 0; i < aryFaceRatio.Length; i++)
        {
            if(face >= aryFaceRatio[i] && face < aryFaceRatio[i + 1])
            {
                parentSprite.sprite = aryFace[i];
            }
		}
        // ゲームプレイ時の処理(タイトル画面では行わない)
        if (SceneManager.GetActiveScene().name == "Stage")
        {     
            if (GameStart.Stage == 4)
            {
                GameObject.Find("P" + id.ToString() + "Face").GetComponent<SpriteRenderer>().sprite = parentSprite.sprite;

            }     
        }

    }

    // 入力は Update で行う
    void Update()
    {
        body = bodyObj.GetComponent<Body>();// 親から Body を取得する
        onFloor   = onSurface | onPlayer | onStick | body.onSurface | body.onPlayer | body.onStick; // 何かに接触している時は true
        onPinball = onPinball | body.onPinball; 
        Acceleration();
        if (!(isRespowing))
        {
            if (GameSetting.Playable && ButtonInGame.Paused != 1 || GameStart.inDemoPlay) //プレイヤー数選択画面でも操作可能
            {
                Jump();
            }
            Move();
        }
        GuideArrow();
        ChangeSensitivity();
        ExitDelay();
    }

    // 移動は FixedUpdateで行う※Inputの入力が入りにくくなる
    void FixedUpdate()
    {
        // プレイヤー速度取得
        Playerspeed = ((transform.parent.gameObject.transform.position - latestPos) / Time.deltaTime);
        if (ButtonInGame.Paused == 1 && saveCount == 0)
        {
            pausedPos   = transform.parent.gameObject.transform.position;
            speedWhenPaused = Playerspeed;
            saveCount = 1;
        }
        if (ButtonInGame.Paused == 1)
        {
            stickRb.velocity = new Vector2(0, 0);
            transform.parent.gameObject.transform.position = pausedPos;
            stickRb.MoveRotation(stickRot);
        }
        if (ButtonInGame.Paused == 0 && saveCount == 1)
        {
            stickRb.velocity = new Vector2(speedWhenPaused.x * 2.1f, speedWhenPaused.y * 2.1f);
            saveCount = 0;
        }
        latestPos = transform.parent.gameObject.transform.position;

        stickRb = GetComponent<Rigidbody2D>();  
        stickRb.MoveRotation(stickRot);            // 角度反映 これはポーズ時も行う
    }


    // ジャンプ
    void Jump()
    {
        // クールタイム
        if (coolTime > 0)
        {
            coolTime -= Time.deltaTime;
            return;   //処理中断
        }

        // キー(あらかじめ左右のどちらかが押されていて、もう一方のキーが押された瞬間を調べる)
        bool key1 = Input.GetKeyDown(KeyRight) && Input.GetKey(    KeyLeft);
        bool key2 = Input.GetKey(    KeyRight) && Input.GetKeyDown(KeyLeft);

        // キー(左右キーどちらも押した瞬間かを調べる)
        bool key3 = Input.GetKeyDown(KeyRight) && Input.GetKeyDown(KeyLeft);
        
        if (onFloor && (key1 || key2 || key3 || ControllerInput.jump[id - 1]))
        {
            // ジャンプの方向を求める
            float rotZ = transform.eulerAngles.z;

            if (rotZ <   0) { rotZ += 360; }// 0 度未満なら正の値にする
            if (rotZ > 180) { rotZ -= 180; }//上に向いているほうの棒の角度のみ取得

            float jumpDirection;                        // 棒の回転値に合わせて飛ぶ方向を求める
            if (rotZ < 180) { jumpDirection = 6; }
            else { jumpDirection = 18; }
            jumpDirection = (jumpDirection - rotZ / 15) * 1.15f;

            //棒が真横を向いているときはジャンプできない
            if (!(rotZ > 179) && !(rotZ < 1))
            {
                stickRb.velocity = new Vector2(jumpDirection, jumpforce);
                onFloor = false; onPinball = false; onPlayer = false; onStick = false; onSurface = false; body.onSurface = false; body.onPlayer = false; body.onStick = false;
                //効果音鳴らす
                SoundEffect.PowanTrigger = 1;
            }

            // クールタイム(この時間は入力を受け付けない)
            coolTime = CoolTime_;
        
        }
    }
    //ガイド矢印
    void GuideArrow() 
    {
        float rotZ = transform.eulerAngles.z;
        if (rotZ < 0) { rotZ += 360; }// 0 度未満なら正の値にする
        if (rotZ > 180) { rotZ -= 180; }//上に向いているほうの棒の角度のみ取得
        
        arrow.transform.position = this.transform.position;
        arrow.transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    // 移動
    void Move()
    {
        rotSpeed = 120 + Settings.rotStage[id - 1] * 4;  //感度反映
        if (GameSetting.Playable && ButtonInGame.Paused != 1 || GameStart.inDemoPlay) //プレイヤー数選択画面でも操作可能
        {
            if (Input.GetKey(KeyRight) || ControllerInput.Lstick[id - 1] > 0) { stickRot -= rotSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyLeft) || ControllerInput.Lstick[id - 1] < 0) { stickRot += rotSpeed * Time.deltaTime; }
        }
    }


    // 感度調整
    void ChangeSensitivity()
    {
        if (GameStart.inDemoPlay) //プレイヤー数選択画面でのみ操作可能
        {
            if (ControllerInput.crossX[id - 1] == 0) { inputCrossX = false; }
            //十字ボタン(横)を一回倒すごとに感度を一段階変更
            if (ControllerInput.crossX[id - 1] >= 0.1f && inputCrossX == false) { Settings.rotStage[id - 1] += 1; inputCrossX = true; SoundEffect.BunTrigger = 1; }
            if (ControllerInput.crossX[id - 1] <= -0.1f && inputCrossX == false) { Settings.rotStage[id - 1] -= 1; inputCrossX = true; SoundEffect.BunTrigger = 1; }
            SensText.text = Settings.rotStage[id - 1].ToString();
        }
    }

    // 死亡
    public void StartDead()
    {
        arrow.SetActive(false);
        isRespowing = true;
        stickSprite.enabled = false;
        parentSprite.enabled = false;
        StartCoroutine(Respown());
    }

    // リスポーン
    IEnumerator Respown()
    {
        //他のプレイヤーの邪魔にならないよう当たり判定OFF
        this.GetComponent<BoxCollider2D>().enabled = false;
        bodyObj.GetComponent<BoxCollider2D>().enabled = false;
        //位置固定
        Rigidbody2D bodyRb2D = bodyObj.GetComponent<Rigidbody2D>();
        bodyRb2D.constraints = RigidbodyConstraints2D.FreezeAll;

        nameTag.SetActive(false);
        deadTimer.SetActive(true);
        deadTimer.transform.position = gameObject.transform.position + new Vector3(0, 0.5f, 0);
        Text deadText = deadTimer.GetComponent<Text>();
        deadText.text = "3";
        SoundEffect.BunTrigger = 1;
		yield return new WaitForSeconds(1.0f);					// 待ち時間
        deadText.text = "2";
        SoundEffect.BunTrigger = 1;
        yield return new WaitForSeconds(1.0f);					// 待ち時間
        deadText.text = "1";
        SoundEffect.BunTrigger = 1;
        yield return new WaitForSeconds(1.0f);					// 待ち時間
        deadText.text = "";


        //当たり判定O
        this.GetComponent<BoxCollider2D>().enabled = true;
        bodyObj.GetComponent<BoxCollider2D>().enabled = true;
        //位置固定解除
        bodyRb2D.constraints = RigidbodyConstraints2D.None; bodyRb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        //チェックポイントにリスポーン
        bodyObj.transform.position = GameSetting.respownPos[id - 1];

        if (GameStart.Stage == 1)
        {
            arrow.SetActive(true);
        }
        nameTag.SetActive(true);
        stickSprite.enabled = true;
        parentSprite.enabled = true;
        isRespowing = false;
    }



    // コリジョン
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Surface")) { SoundEffect.BonTrigger = 1;}
        if (onPinball && other.gameObject.CompareTag("Surface")) { stickRb.velocity = -Playerspeed * 2; } //ピンボールゾーンでの床との接触時反発
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Surface")) { onSurface = true; delay = 0.1f; delayFlag = false; }
        if (other.gameObject.CompareTag("Player"))  { onPlayer = true; }
        if (other.gameObject.CompareTag("Stick"))   { onStick = true; }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pinball")) { onPinball = true; }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Surface")) { delay = 0.1f;  delayFlag = true; }
        if (other.gameObject.CompareTag("Player")) { onPlayer = false; }
        if (other.gameObject.CompareTag("Stick")) { onStick = false; }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pinball")) { onPinball = false; }
    }
    void ExitDelay()    //床との設置判定に余裕を
    {
        if (delayFlag)
        {
            delay -= Time.deltaTime;
            if(delay < 0)
            {
                onSurface = false;
                delay = 0.1f;
                delayFlag = false;
            }
        }
    }

    // Stage3のピンボールゾーンのオブジェクトに触れると加速
    public void Acceleration()
    {
        if (onPinball && GameSetting.Playable && ButtonInGame.Paused != 1)
        {
            var material = GetComponent<Rigidbody2D>().sharedMaterial;
            material.friction = 0f;
        }
        else if (onPinball == false)
        {
            var material = GetComponent<Rigidbody2D>().sharedMaterial;
            material.friction = 2f;
        }
    }

   
}


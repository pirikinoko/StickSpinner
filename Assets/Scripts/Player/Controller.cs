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
    [SerializeField]
    float RotSpeed = 160f;                              // 棒の回転速度
    [SerializeField]
    float CoolTime_ = 0.2f;                             // ジャンプのクールタイム

    float RotStage = 10;                                // 棒の回転速度0-20段階
    float StickRot = 0f;                                // 棒の角度
    float jumpforce = 8.3f;                             // ジャンプ力
    bool  onFloor;                                      // ジャンプ可能な状態
    bool  onSurface, onPlayer, onStick, onPinball;      // 接触している時は true
    float CoolTime = 0.2f;                              // ジャンプのクールタイム
    bool  inputCrossX;                                  // 十字ボタンの入力があるときはtrue
    float delay = 0.15f;
    bool delayFlag = false;
    // プレイヤーごとの回転速度
    ///public static float[] rotSpeed = {160,160,160,160 }; // これは感度調整用なのか?
    public static float[] rotStage = { 10, 10, 10, 10 };

    //[SerializeField]
    //float RotSpeed_ = 160f, RotStage_ = 10;

    [SerializeField]
    Text SensText;

    [SerializeField]
    Sprite[] aryFace = new Sprite[6];                   


     int Face {get; set;}                                // 顔設定用乱数を入れておく1～100

    string padHorizontalName, padJumpName, padChangeSensName, XpadJumpName; // ゲームパット名
    Rigidbody2D Stickrbody2D;             // 棒のRigidbody
                                          // 親の顔
    SpriteRenderer parentSprite;//, spriteRenderer2;

    bool isRespown = false;


    private Vector3 PlayerPos, StickPos, PausedPlayerPos, PausedStickPos, latestPos; //プレイヤー,棒の位置
    private Vector2 Playerspeed, PausedPlayerspeed, deadPlayerPos;//プレイヤー速度,ポーズ直前のプレイヤー速度
    private float   SavePausedPos = 0; // ポーズ処理に使用
    private Body    body;
    /*　/ゲームオブジェクトなど  */

    /* ボタン関連 */
    public static string Controllers { get; set; }

    int connected; //接続されているコントローラーの数
    string[] keyName;
    public static bool usingController = true;
    GameObject bodyObj;     
    GameObject deadTimer;
    SpriteRenderer stickSprite;                 // スティックスプライト
    GameObject nameTag;                         // 名前のゲームオブジェクト

    readonly int[] aryFaceRatio = { 25, 50, 70, 88, 94, 100};   // 顔の変化用


    void Start()
    {
        if (Controllers == "")
        {
            usingController = false;
        }
        // 名前
        nameTag = GameObject.Find("P" + id.ToString() + "Text");

        // 親スプライト・スティックスプライトを得る
        parentSprite =  transform.parent.gameObject.GetComponent<SpriteRenderer>();
        stickSprite  = GetComponent<SpriteRenderer>();
        bodyObj = transform.parent.gameObject;
        // カウントダウンタイマー
        deadTimer = GameObject.Find("P" + id.ToString() + "CountDown");

        // これなんだろう
        RotStage = rotStage[id - 1];
        //RotSpeed = rotSpeed[id - 1];

        isRespown    = false;
        StickRot     = 0f;
        CoolTime     = 0.0f;
        onSurface    = false;
        onPlayer     = false;
        onStick      = false;
        Stickrbody2D = GetComponent<Rigidbody2D>();
        body         = transform.parent.gameObject.GetComponent<Body>();// 親から Body を取得する
        

        // 顔をランダムで設定する
        Face = Random.Range(1, 100);
        for(int i = 0; i < aryFaceRatio.Length; i++)
        {
            if(Face <= aryFaceRatio[i])
            {
                parentSprite.sprite = aryFace[i];
            }
		}

        // ステージならば spriteRenderer.sprite をコピーする。これで処理がすっきりする
/*      if (GameStart.Stage == 4 && GameSetting.Playable)
        {
            spriteRenderer2.sprite = spriteRenderer.sprite;
        }
*/
    }

    // 入力は Update で行う
    void Update()
    {
        onFloor   = onSurface | onPlayer | onStick | body.onSurface | body.onPlayer | body.onStick;// 何かに接触している時は true
        onPinball = onPinball | body.onPinball; ;
        Acceleration();
        if (!isRespown)
        {
            if (GameSetting.Playable && ButtonInGame.Paused != 1 || GameStart.inDemoPlay) //プレイヤー数選択画面でも操作可能
            {
                Jump();
            }
            Move();
        }
    
        ChangeSensitivity();
        getControllerType();
        InputControllerButton();
        ExitDelay();
    }

    // 移動は FixedUpdate で行う※Inputの入力が入りにくくなる
    void FixedUpdate()
    {
        // プレイヤー速度取得
        StickPos    = transform.position;
        Playerspeed = ((transform.parent.gameObject.transform.position - latestPos) / Time.deltaTime);
        if (ButtonInGame.Paused == 1 && SavePausedPos == 0)
        {
            PausedStickPos    = transform.position;
            PausedPlayerPos   = transform.parent.gameObject.transform.position;
            PausedPlayerspeed = Playerspeed;
            SavePausedPos = 1;
        }
        if (ButtonInGame.Paused == 1)
        {
            Stickrbody2D.velocity = new Vector2(0, 0);
            transform.parent.gameObject.transform.position = PausedPlayerPos;
            transform.position = PausedStickPos;
            Stickrbody2D.MoveRotation(StickRot);
        }
        if (ButtonInGame.Paused == 0 && SavePausedPos == 1)
        {
            Stickrbody2D.velocity = new Vector2(PausedPlayerspeed.x * 2.1f, PausedPlayerspeed.y * 2.1f);
            SavePausedPos = 0;
        }
        latestPos = transform.parent.gameObject.transform.position;

        Stickrbody2D.MoveRotation(StickRot);            // 角度反映 これはポーズ時も行う
    }


    // ジャンプ
    void Jump()
    {
        // クールタイム
        if (CoolTime > 0)
        {
            CoolTime -= Time.deltaTime;
            return;
        }

        // キー(あらかじめ左右のどちらかが押されていて、もう一方のキーが押された瞬間を調べる)
        bool key1 = Input.GetKeyDown(KeyRight) && Input.GetKey(    KeyLeft);
        bool key2 = Input.GetKey(    KeyRight) && Input.GetKeyDown(KeyLeft);

        // キー(左右キーどちらも押した瞬間かを調べる)
        bool key3 = Input.GetKeyDown(KeyRight) && Input.GetKeyDown(KeyLeft);

        //上の三つだとちゃんと床などに設置してからキーを押さないとジャンプできない(棒が少し地面でバウンドしてる時にバウンドのたびにタイミングよく押さないとジャンプできない)ので反応が悪くなるためこれを使うしかない
        bool key4 = Input.GetKey(KeyRight) && Input.GetKey(KeyLeft);
        
        if (onFloor && (key1 || key2 || key3 || key4 || GetJumpButtonDown()))
        {
            // ジャンプの方向を求める

            float rotZ = transform.eulerAngles.z;
            if (rotZ <   0) { rotZ += 360; }// 0 度未満なら正の値にする
            if (rotZ > 180) { rotZ -= 180; }//上に向いているほうの棒の角度のみ取得

            float jumpDirection;                        // 棒の回転値に合わせて飛ぶ方向を求める
            if (rotZ < 180) { jumpDirection = 6; }
            else { jumpDirection = 18; }

            // ジャンプさせる
            jumpDirection = (jumpDirection - rotZ / 15) * 1.15f;
            //棒が真横を向いているときはジャンプできない
            if (!(rotZ > 179) && !(rotZ < 1))
            {
                Stickrbody2D.velocity = new Vector2(jumpDirection, jumpforce);
                //効果音鳴らす
                SoundEffect.PowanTrigger = 1;
            }

            // クールタイム(この時間は入力を受け付けない)
            CoolTime = CoolTime_;
        }
    }

    // 移動
    void Move()
    {
        if (GameSetting.Playable && ButtonInGame.Paused != 1 || GameStart.inDemoPlay) //プレイヤー数選択画面でも操作可能
        {
            float horizotalValue = Input.GetAxis("Horizontal");
            if (Input.GetKey(KeyRight) || horizotalValue >=  0.1f) { StickRot -= RotSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyLeft)  || horizotalValue <= -0.1f) { StickRot += RotSpeed * Time.deltaTime; }
        }
    }

    // 死亡
    public void StartDead()
    {
        isRespown = false;
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
		yield return new WaitForSeconds(1.0f);					// 待ち時間
        deadText.text = "2";
		yield return new WaitForSeconds(1.0f);					// 待ち時間
        deadText.text = "1";
		yield return new WaitForSeconds(1.0f);					// 待ち時間
        deadText.text = "";


        //当たり判定O
        this.GetComponent<BoxCollider2D>().enabled = true;
        bodyObj.GetComponent<BoxCollider2D>().enabled = true;
        //位置固定解除
        bodyRb2D.constraints = RigidbodyConstraints2D.None; bodyRb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        //チェックポイントにリスポーン
        bodyObj.transform.position = CheckPoint.respownPos[id - 1];

        stickSprite.enabled = true;
        parentSprite.enabled = true;
        isRespown = true;
    }


    // 感度調整 @@一旦保留
    void ChangeSensitivity()
    {
        if (GameStart.inDemoPlay) //プレイヤー数選択画面でのみ操作可能
        {
            float horizotalValue = Input.GetAxis("Horizontal");

            if (horizotalValue == 0) { inputCrossX = false; }
            //十字ボタン(横)を一回倒すごとに感度ステージを一段階変更
            if (horizotalValue >=  0.1f && inputCrossX == false) { RotStage += 1; inputCrossX = true; }
            if (horizotalValue <= -0.1f && inputCrossX == false) { RotStage -= 1; inputCrossX = true; }

            RotSpeed = 120 + RotStage * 4;  // 120 + 4 * 10(RotStage初期値) = 160をベースに感度ステージごとに4変更
            //@@SensText.text = rotStage[id - 1].ToString();
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                if (id == i + 1)
                {
                    RotStage += TitleButtonClick.sensChange[i];
                    TitleButtonClick.sensChange[i] = 0;
                    rotStage[i] = RotStage;
                    //@@rotSpeed[i] = RotSpeed; // ここで調整しているのかな?
                }
            }
            //上限下限の設定
            if (RotStage > 20)
            {
                RotStage = 20;
            }
            if (RotStage < 1)
            {
                RotStage = 1;
            }
        }
    }

    // コリジョン
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Surface")) { SoundEffect.BonTrigger = 1;}
        if (onPinball && other.gameObject.CompareTag("Surface")) { Stickrbody2D.velocity = -Playerspeed * 2; } //ピンボールゾーンでの床との接触時反発
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
    void ExitDelay()
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
            Stickrbody2D.gravityScale = 1;
        }
        else if (onPinball == false)
        {
            var material = GetComponent<Rigidbody2D>().sharedMaterial;
            material.friction = 2f;
            Stickrbody2D.gravityScale = 1;
        }
    }

    //コントローラー1-4の種類を判別
    private void getControllerType()
    {
        string[] joystickNames = Input.GetJoystickNames();
        connected = joystickNames.Length;       //コントローラーの接続台数を反映
        for (int i = 0; i < joystickNames.Length; i++)
        {
            Controllers = CheckControllerName(joystickNames[i]);
        }
    }

    //コントローラーの名前によって種類を判別
    string CheckControllerName(string ControllerName)
    {
        if (ControllerName.ToLower().Contains("xbox"))
        {
            return "XBOX";
        }
        else if (ControllerName.ToLower().Contains("playstation"))
        {
            return "PS";
        }
        else if (ControllerName.ToLower().Contains("f310"))
        {
            return "Logicool";
        }
        else
        {
            return "OTHER";
        }
    }

    //ボタン入力受付
    void InputControllerButton()
    {

    }

    /*void CheckControllerState()
    {
        bool anyButtonInput = false;
        bool keyORMouseInput = false;
        bool Bbutton = false;
        for (int i = 0; i < connected; i++)
        {
            Bbutton = playerKey.jump;
            anyButtonInput = playerKey.start || playerKey.next || playerKey.back || playerKey.plus || playerKey.minus || playerKey.X != 0 || playerKey.Y != 0 || playerKey.horizontal != 0;
        }
        keyORMouseInput = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
        if (anyButtonInput)
        {
            usingController = true;
        }
        if (keyORMouseInput)
        {
            usingController = false;
        }
    }*/

    // 押した瞬間
    public bool GetNextButtonDown()
    {
        return Input.GetButtonDown("Next_" + id.ToString());
	}
    public bool GetBackButtonDown()
    {
        return Input.GetButtonDown("XBack_" + id.ToString());
	}
    public bool GetStartButtonDown()
    {
        return Input.GetButtonDown("XStart_" + id.ToString());
	}
    public bool GetJumpButtonDown()
    {
        return Input.GetButtonDown("Jump_" + id.ToString());
	}

    public bool GetNextButtonHold()
    {
        return Input.GetButton("Next_" + id.ToString());
	}
    public bool GetBackButtonHold()
    {
        return Input.GetButton("XBack_" + id.ToString());
	}
    public bool GetStartButtonHold()
    {
        return Input.GetButton("XStart_" + id.ToString());
	}
    public bool GetJumpButtonHold()
    {
        return Input.GetButton("Jump_" + id.ToString());
	}

    public bool GetNextButtonUp()
    {
        return Input.GetButtonUp("Next_" + id.ToString());
	}
    public bool GetBackButtonUp()
    {
        return Input.GetButtonUp("XBack_" + id.ToString());
	}
    public bool GetStartButtonUp()
    {
        return Input.GetButtonUp("XStart_" + id.ToString());
	}
    public bool GetJumpButtonUp()
    {
        return Input.GetButtonUp("Jump_" + id.ToString());
	}
}


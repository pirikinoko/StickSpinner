using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Controller : MonoBehaviour
{
    [SerializeField]
    int id;                                             // プレイヤー番号(1～4)

    /* 処理関連*/
    [SerializeField]
    KeyCode KeyLeft, KeyRight;                          // 左右キー(キーボード使用時)
    [SerializeField]
    float RotSpeed = 160f;                              // 棒の回転速度
    [SerializeField]
    float CoolTime_ = 0.2f;                             // ジャンプのクールタイム
    float RotStage = 10;                                //棒の回転速度0-20段階
    float StickRot = 0f;                                //棒の角度
    float jumpforce = 8.3f;                             //ジャンプ力
    bool onFloor;                                       // ジャンプ可能な状態
    bool onSurface, onPlayer, onStick, onPinball;       // 接触している時は true
    float CoolTime = 0.2f;                              // ジャンプのクールタイム
    bool inputCrossX;                                   //十字ボタンの入力があるときはtrue
    public static float P1RotSpeed = 160f, P2RotSpeed = 160f, P3RotSpeed = 160f, P4RotSpeed = 160f, P1RotStage = 10, P2RotStage = 10, P3RotStage = 10, P4RotStage = 10; // プレイヤーごとの回転速度
    public int Face;                                    //顔設定用乱数を入れておく1～100
    /* /処理関連*/

    /*　ゲームオブジェクトなど  */
    [SerializeField]
    Text SensText;
    string padHorizontalName, padJumpName, padChangeSensName, XpadJumpName; // ゲームパット名
    Rigidbody2D Stickrbody2D;             // 棒のRigidbody
                                          //顔ランダム用                                      
    public SpriteRenderer spriteRenderer, spriteRenderer2;
    public Sprite Face1, Face2, Face3, Face4, Face5, Face6;
    private Vector3 PlayerPos, StickPos, PausedPlayerPos, PausedStickPos, latestPos; //プレイヤー,棒の位置
    private Vector2 Playerspeed, PausedPlayerspeed;//プレイヤー速度,ポーズ直前のプレイヤー速度
    private float SavePausedPos = 0; // ポーズ処理に使用
    Body body;
    /*　/ゲームオブジェクトなど  */

    /* ボタン関連 */
    public static string[] Controllers { get; set; } = new string[4];
    int connected; //接続されているコントローラーの数
    string[] keyName;
    public static bool usingController = true;
    // より構造的にするならばクラスを作って一括管理する
    public class _Key
    {
        public string Controller;
        public bool start, startHold;       // 同じような項目は横並びに並べる
        public bool next, nextHold;
        public bool back, backHold;
        public bool plus, plusHold;
        public bool minus, minusHold;
        public bool jump, jumpHold;         // ジャンプも Hold　があったほうがいい
        public float horizontal;
        public float X;                     // crossXReception だと長いのでこれくらいでいいと思う
        public float Y;
    }

    public _Key[] playerKey { get; set; } = new _Key[4];  //


    /* /ボタン関連 */

    void Start()
    {
        if (Controllers[0] == "")
        {
            usingController = false;
        }

        if (this.gameObject.name == "Stick1")
        {
            RotStage = P1RotStage;
            RotSpeed = P1RotSpeed;
        }
        else if (this.gameObject.name == "Stick2")
        {
            RotStage = P2RotStage;
            RotSpeed = P2RotSpeed;
        }
        else if (this.gameObject.name == "Stick3")
        {
            RotStage = P3RotStage;
            RotSpeed = P3RotSpeed;
        }
        else
        {
            RotStage = P4RotStage;
            RotSpeed = P4RotSpeed;
        }
        StickRot = 0f;
        CoolTime = 0.0f;
        onSurface = false;
        onPlayer = false;
        onStick = false;
        Stickrbody2D = GetComponent<Rigidbody2D>();
        body = transform.parent.gameObject.GetComponent<Body>();// 親から Body を取得する
                                                                // 顔ランダム設定
        Face = Random.Range(1, 100);
        if (Face <= 25)                         // 25 以下なら Face1
        {
            spriteRenderer.sprite = Face1;
        }
        else if (Face <= 50)                   // 50 以下なら Face2  ※ else if とすることで Face >= 26 は不要になる
        {                                       // else if は (Face <= 25)　でなければという意味
            spriteRenderer.sprite = Face2;
        }
        else if (Face <= 70)
        {
            spriteRenderer.sprite = Face3;
        }
        else if (Face <= 88)
        {
            spriteRenderer.sprite = Face4;
        }
        else if (Face <= 94)
        {
            spriteRenderer.sprite = Face5;
        }
        else // if ( Face <= 100)               // 最後は判定不要になる
        {
            spriteRenderer.sprite = Face6;
        }

        // ステージならば spriteRenderer.sprite をコピーする。これで処理がすっきりする
        if (GameStart.Stage == 4 && GameSetting.Playable)
        {
            spriteRenderer2.sprite = spriteRenderer.sprite;
        }

    }

    // 入力は Update で行う
    void Update()
    {
        onFloor = onSurface | onPlayer | onStick
            | body.onSurface | body.onPlayer | body.onStick;// 何かに接触している時は true
        onPinball = onPinball | body.onPinball; ;
        Acceleration();
        if (GameSetting.Playable && ButtonInGame.Paused != 1 || GameStart.InSelectPN) //プレイヤー数選択画面でも操作可能
        {
            Jump();
        }
        Move();
        ChangeSensitivity();
        getControllerType();
        InputControllerButton();
    }

    // 移動は FixedUpdate で行う※Inputの入力が入りにくくなる
    void FixedUpdate()
    {

        //プレイヤー速度取得
        StickPos = this.transform.position;
        Playerspeed = ((transform.parent.gameObject.transform.position - latestPos) / Time.deltaTime);
        if (ButtonInGame.Paused == 1 && SavePausedPos == 0)
        {
            PausedStickPos = this.transform.position;
            PausedPlayerPos = transform.parent.gameObject.transform.position;
            PausedPlayerspeed = Playerspeed;
            SavePausedPos = 1;
        }
        if (ButtonInGame.Paused == 1)
        {
            Stickrbody2D.velocity = new Vector2(0, 0);
            transform.parent.gameObject.transform.position = PausedPlayerPos;
            this.transform.position = PausedStickPos;
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
    //角度をベクトルに変換
    public static Vector2 AngleToVector2(float angle)
    {
        var radian = angle * (Mathf.PI / 180);
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
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
        bool key1 = Input.GetKeyDown(KeyRight) && Input.GetKey(KeyLeft);
        bool key2 = Input.GetKey(KeyRight) && Input.GetKeyDown(KeyLeft);
        // キー(左右キーどちらも押した瞬間かを調べる)
        bool key3 = Input.GetKeyDown(KeyRight) && Input.GetKeyDown(KeyLeft);
        //上の三つだとちゃんと床などに設置してからキーを押さないとジャンプできない(棒が少し地面でバウンドしてる時にバウンドのたびにタイミングよく押さないとジャンプできない)ので反応が悪くなるためこれを使うしかない
        bool key4 = Input.GetKey(KeyRight) && Input.GetKey(KeyLeft);
        if (onFloor && (key1 || key2 || key3 || key4 || playerKey[id - 1].jump))
        {
            // ジャンプの方向を求める

            float rotZ = transform.eulerAngles.z;
            if (rotZ < 0) { rotZ += 360; }// 0 度未満なら正の値にする
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
            playerKey[id - 1].jump = false;
        }
    }

    // 移動
    void Move()
    {
        if (GameSetting.Playable && ButtonInGame.Paused != 1 || GameStart.InSelectPN) //プレイヤー数選択画面でも操作可能
        {
            if (Input.GetKey(KeyRight) || playerKey[id - 1].horizontal >= 0.1f) { StickRot -= RotSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyLeft) || playerKey[id - 1].horizontal <= -0.1f) { StickRot += RotSpeed * Time.deltaTime; }
            StickRot %= 360f;                               // 360 で割った時のあまりを求める
        }

    }

    // 感度調整
    void ChangeSensitivity()
    {
        if (GameStart.InSelectPN) //プレイヤー数選択画面でのみ操作可能
        {

            if (playerKey[id - 1].X == 0) { inputCrossX = false; }
            //十字ボタン(横)を一回倒すごとに感度ステージを一段階変更
            if (playerKey[id - 1].X >= 0.1f && inputCrossX == false) { RotStage += 1; inputCrossX = true; }
            if (playerKey[id - 1].X <= -0.1f && inputCrossX == false) { RotStage -= 1; inputCrossX = true; }
            RotSpeed = 120 + RotStage * 4; //120 + 4 * 10(RotStage初期値) = 160をベースに感度ステージごとに4変更
            SensText.text = RotStage.ToString();
            if (this.gameObject.name == "Stick1")
            {
                RotStage += TitleButtonClick.P1SensChange;
                TitleButtonClick.P1SensChange = 0;
                P1RotStage = RotStage;
                P1RotSpeed = RotSpeed;
            }
            else if (this.gameObject.name == "Stick2")
            {
                RotStage += TitleButtonClick.P2SensChange;
                TitleButtonClick.P2SensChange = 0;
                P2RotStage = RotStage;
                P2RotSpeed = RotSpeed;
            }
            else if (this.gameObject.name == "Stick3")
            {
                RotStage += TitleButtonClick.P3SensChange;
                TitleButtonClick.P3SensChange = 0;
                P3RotStage = RotStage;
                P3RotSpeed = RotSpeed;
            }
            else
            {
                RotStage += TitleButtonClick.P4SensChange;
                TitleButtonClick.P4SensChange = 0;
                P4RotStage = RotStage;
                P4RotSpeed = RotSpeed;
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
    /* コリジョン */
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Surface")) { onSurface = true; }
        if (other.gameObject.CompareTag("Player")) { onPlayer = true; }
        if (other.gameObject.CompareTag("Stick")) { onStick = true; }
        if (other.gameObject.CompareTag("Surface")) { SoundEffect.BonTrigger = 1;/*効果音*/}
        if (onPinball && other.gameObject.CompareTag("Surface")) { Stickrbody2D.velocity = -Playerspeed * 2; } //ピンボールゾーンでの床との接触時反発
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pinball")) { onPinball = true; }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Surface")) { onSurface = false; }
        if (other.gameObject.CompareTag("Player")) { onPlayer = false; }
        if (other.gameObject.CompareTag("Stick")) { onStick = false; }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pinball")) { onPinball = false; }
    }
    //Stage3のピンボールゾーンのオブジェクトに触れると加速
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
    /* /コリジョン */

    /*感度ステージ変更ボタン*/
    public void GainSensP1()
    {
        if (P1RotStage < 20)
        {
            P1RotStage++;
        }
    }
    public void GainSensP2()
    {
        if (P2RotStage < 20)
        {
            P2RotStage++;
        }
    }
    public void GainSensP3()
    {
        if (P3RotStage < 20)
        {
            P3RotStage++;
        }
    }
    public void GainSensP4()
    {
        if (P4RotStage < 20)
        {
            P4RotStage++;
        }
    }
    public void LoseSensP1()
    {
        if (P1RotStage < 1)
        {
            P1RotStage--;
        }
    }
    public void LoseSensP2()
    {
        if (P2RotStage < 1)
        {
            P2RotStage--;
        }
    }
    public void LoseSensP3()
    {
        if (P3RotStage < 1)
        {
            P3RotStage--;
        }
    }
    public void LoseSensP4()
    {
        if (P4RotStage < 1)
        {
            P4RotStage--;
        }
    }
    /*/感度ステージ変更ボタン*/

    /*コントローラー入力*/

    //コントローラー1-4の種類を判別
    private void getControllerType()
    {
        string[] joystickNames = Input.GetJoystickNames();
        connected = joystickNames.Length; //コントローラーの接続台数を反映
        for (int i = 0; i < joystickNames.Length; i++)
        {
            Controllers[i] = CheckControllerName(joystickNames[i]);
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
        for (int i = 0; i < connected; i++)
        {
            if (Controllers[i] == "XBOX") //「i + 1」台目のコントローラーがxboxならば以下のボタン入力を受け付ける
            {
                //InputSystemのボタン名を配列に格納しておく
                string[] keyNametemp =
                {
                      /*1*/"XStart_" + (i+1).ToString(), /*2*/"XBack_" + (i+1).ToString(), /*3*/"XJump_" + (i+1).ToString(), /*4*/"Next_" + (i+1).ToString(), /*5*/"Plus_" + (i+1).ToString(), /*6*/"Minus_" + (i+1).ToString(), /*7*/"Horizontal_" + (i+1).ToString(), /*8*/"XCrossX_" + (i+1).ToString(), /*9*/"XCrossY_" + (i+1).ToString()
                    };
                keyName = keyNametemp;
            }
            else if (Controllers[i] == "Logicool") //「i + 1」台目のコントローラーがLogicoolならば以下のボタン入力を受け付ける
            {
                string[] keyNametemp =
                {
                      /*1*/"Logi/PSStart_" + (i+1).ToString(), /*2*/"Logi/PSBack_" + (i+1).ToString(), /*3*/"Logi/PSJump_" + (i+1).ToString(), /*4*/"Next_" + (i+1).ToString(), /*5*/"Plus_" + (i+1).ToString(), /*6*/"Minus_" + (i+1).ToString(), /*7*/"Horizontal_" + (i+1).ToString(), /*8*/"LogiCrossX_" + (i+1).ToString(), /*9*/"LogiCrossY_" + (i+1).ToString()
                    };
                keyName = keyNametemp;
            }
            else if (Controllers[i] == "PS") //「i + 1」台目のコントローラーがPSならば以下のボタン入力を受け付ける
            {
                string[] keyNametemp =
                {
                      /*1*/"Logi/PSStart_" + (i+1).ToString(), /*2*/"Logi/PSBack_" + (i+1).ToString(), /*3*/"Logi/PSJump_" + (i+1).ToString(), /*4*/"Next_" + (i+1).ToString(), /*5*/"Plus_" + (i+1).ToString(), /*6*/"Minus_" + (i+1).ToString(), /*7*/"Horizontal_" + (i+1).ToString(), /*8*/"PSCrossX_" + (i+1).ToString(), /*9*/"PSCrossY_" + (i+1).ToString()
                    };
                keyName = keyNametemp;
            }
            else  //「i + 1」台目のコントローラーが上記以外のコントローラーならば以下のボタン入力を受け付ける(Logicoolに合わせる)
            {
                string[] keyNametemp =
                {
                      /*1*/"Logi/PSStart_" + (i+1).ToString(), /*2*/"Logi/PSBack_" + (i+1).ToString(), /*3*/"Logi/PSJump_" + (i+1).ToString(), /*4*/"Next_" + (i+1).ToString(), /*5*/"Plus_" + (i+1).ToString(), /*6*/"Minus_" + (i+1).ToString(), /*7*/"Horizontal_" + (i+1).ToString(), /*8*/"LogiCrossX_" + (i+1).ToString(), /*9*/"LogiCrossY_" + (i+1).ToString()
                    };
                keyName = keyNametemp;
            }
            // スタートボタン
            if (Input.GetButtonDown(keyName[0])) { playerKey[i].start = true; }
            if (Input.GetButton(keyName[0])) { playerKey[i].startHold = true; }

            // 戻るボタン(X)
            if (Input.GetButtonDown(keyName[1])) { playerKey[i].back = true; }
            if (Input.GetButton(keyName[1])) { playerKey[i].backHold = true; }

            // ジャンプボタン(B)
            if (Input.GetButtonDown(keyName[2])) { playerKey[i].jump = true; }
            if (Input.GetButton(keyName[2])) { playerKey[i].jumpHold = true; }

            // 次へボタン(Y)              
            if (Input.GetButtonDown(keyName[3])) { playerKey[i].next = true; }
            if (Input.GetButton(keyName[3])) { playerKey[i].nextHold = true; }
            if (Input.GetButtonUp(keyName[3])) { playerKey[i].nextHold = false; }

            // プラスボタン(R1)
            if (Input.GetButtonDown(keyName[4])) { playerKey[i].plus = true; }
            if (Input.GetButton(keyName[4])) { playerKey[i].plusHold = true; }

            // マイナスボタン(L1)
            if (Input.GetButtonDown(keyName[5])) { playerKey[i].minus = true; }
            if (Input.GetButton(keyName[5])) { playerKey[i].minusHold = true; }

            // LStick
            if (Input.GetAxis(keyName[6]) != 0) { playerKey[i].horizontal = Input.GetAxis(keyName[6]); }

            // 十字ボタン横
            if (Input.GetAxis(keyName[7]) != 0) { playerKey[i].X = Input.GetAxis(keyName[7]); }

            // 十字ボタン縦
            if (Input.GetAxis(keyName[8]) != 0) { playerKey[i].Y = Input.GetAxis(keyName[8]); }
        }
    }
    void CheckControllerState()
    {
        bool anyButtonInput = false;
        bool keyORMouseInput = false;
        bool Bbutton = false;
        for (int i = 0; i < connected; i++)
        {
            Bbutton = playerKey[i].jump;
            anyButtonInput = playerKey[i].start || playerKey[i].next || playerKey[i].back || playerKey[i].plus || playerKey[i].minus || playerKey[i].X != 0 || playerKey[i].Y != 0 || playerKey[i].horizontal != 0;
            keyORMouseInput = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
        }
        if (anyButtonInput)
        {
            usingController = true;
        }
        if (keyORMouseInput)
        {
            usingController = false;
        }
    }
    /*/コントローラー入力*/

    /* ボタンレスポンス */



    /* /ボタンレスポンス */
}

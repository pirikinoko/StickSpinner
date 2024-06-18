using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using Photon.Pun;


public class Controller : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public int id;                                      // プレイヤー番号(1～4)
    [SerializeField]
    KeyCode KeyLeft, KeyRight, KeyJump, KeyDown;                          // 左右キー(キーボード使用時)
    [SerializeField]float rotSpeed = 160f;                              // 棒の回転速度
    [SerializeField]
    float CoolTime_ = 0.2f;                             // ジャンプのクールタイム初期化用
    float coolTime = 0.2f;                              // ジャンプのクールタイム
    Text SensText;
    GameSetting gameSetting;
    float stickRot = 0f;                                　　　// 棒の角度
    float[] stickRots = new float[GameStart.maxPlayer];
    float jumpforce = 8.3f;                            　　　 // Y軸ジャンプ力
    bool onFloor, onSurface, onPlayer, onStick;   // 接触している時は true
    bool inputCrossX;                               　　　 　// 十字ボタンの入力があるときはtrue
    float delay = 0.15f;
    float selfDeathTimer = 1.0f;
    public float rotZ { get; set; }
    bool delayFlag = false;
    bool isRespowing = false;
    private Coroutine countdownCoroutine;
    private GameObject nameTag;        //ネームタグ
    SpriteRenderer stickSprite;       // 棒スプライト
    Rigidbody2D stickRb;              // 棒のRigidbody  
    SpriteRenderer parentSprite;      // プレイヤーの顔
    private Vector3 playerPos, pausedPos, latestPos; 　　　　　　　　 　//プレイヤー,棒の位置
    private Vector2 Playerspeed, speedWhenPaused, deadPlayerPos;　　　　//プレイヤー速度,ポーズ直前のプレイヤー速度
    private bool isSaveDone; 　　　　　　　　　　　　　　　　　   // ポーズ処理に使用
    private Body body;
    GameObject bodyObj;
    int startTrigger= 0;
    float coolDown = 0.1f;
    //ゴースト
    Vector2 ghostStartPos;
    bool inCoroutine = false;
    Collider2D ghostCollider;
    void Start()
    {
        if (GameStart.gameMode1 == "Online" && SceneManager.GetActiveScene().name != "Title")
        {
            KeyLeft = KeyCode.LeftArrow;
            KeyRight = KeyCode.RightArrow;
            KeyJump = KeyCode.UpArrow;
            KeyDown = KeyCode.DownArrow;
        }

        // 親スプライト・スティックスプライトを得る
        parentSprite = transform.parent.gameObject.GetComponent<SpriteRenderer>();
        stickSprite = GetComponent<SpriteRenderer>();
        bodyObj = transform.parent.gameObject;
        //ゴースト
        ghostStartPos = bodyObj.transform.position;
        isRespowing = false;
        stickRot = 0f;
        coolTime = 0.0f;
        startTrigger = 0;
        onSurface = false;
        isSaveDone = false;
        onPlayer = false;
        onStick = false;
        inCoroutine = false;
        stickRb = GetComponent<Rigidbody2D>();


    }

    void StartInUpdate() 
    {
        if (SceneManager.GetActiveScene().name == "Stage")
        {
            gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
            nameTag = gameSetting.nameTags[id - 1];
        }
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            stickRb.MoveRotation(0);
        }

    }
    void GhostJump()
    {
        float jumpDirection;
        if (rotZ < 180) { jumpDirection = 6; }
        else { jumpDirection = 18; }
        jumpDirection = (jumpDirection - rotZ / 15) * 1.15f;
        stickRb.velocity = new Vector2(jumpDirection, jumpforce);
    }

    IEnumerator GhostMove()
    {
        float speed = 600;
        inCoroutine = true;
        bodyObj.transform.position = ghostStartPos;
        stickRot = 359;
        while (stickRot > 120)
        {
            stickRot -= speed * Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        GhostJump();
        yield return new WaitForSeconds(0.2f);
        while (stickRot < 230)
        {
            stickRot += speed * Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        GhostJump();
        yield return new WaitForSeconds(0.5f);
        while (stickRot > 3)
        {
            stickRot -= speed * Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
            Debug.Log("StickRot == " + stickRot);
        }
        yield return new WaitForSeconds(0.8f);
        inCoroutine = false;
    }
    void MoveAnime() 
    {
        if(id == 5) 
        {
            if (!inCoroutine)
            {
                StartCoroutine(GhostMove());
            }
        }
        if (id == 6 && PhotonNetwork.InRoom)
        {
            float speed = 120;
            stickRot -= speed * Time.deltaTime;
            if(this.transform.position.x > ghostStartPos.x + 4.5f) 
            {
                Vector2 newstartPos = ghostStartPos;
                newstartPos.x -= 0.8f;
                bodyObj.transform.position = newstartPos;
                bodyObj.transform.GetChild(1).gameObject.transform.position = newstartPos;
                bodyObj.transform.GetChild(1).gameObject.transform.position = newstartPos;
            }
        }
        rotZ = transform.eulerAngles.z;
        if (rotZ < 0) { rotZ += 360; }// 0 度未満なら正の値にする
        if (rotZ > 180) { rotZ -= 180; }//上に向いているほうの棒の角度のみ取得
        if (stickRot > 360) { stickRot -= 360; }
        if (stickRot < 0) { stickRot += 360; }
        stickRb.MoveRotation(stickRot);
    }

    // 入力は Update で行う
    void Update()
    {
        if (GameSetting.allJoin)
        {
            BoxCollider2D thisCollider = this.GetComponent<BoxCollider2D>();

            if (ghostCollider == null)
            {

                GameObject ghostObject = GameObject.Find("Ghost");
                ghostCollider = ghostObject != null ? ghostObject.GetComponent<Collider2D>() : null;
                GameObject ghostStickObject = GameObject.Find("Stick5");
                Collider2D ghostStickCollider = ghostStickObject != null ? ghostStickObject.GetComponent<Collider2D>() : null;
                if ((GameStart.gameMode2 == "Nomal" && GameStart.stage == 1) && thisCollider != null && ghostCollider != null)
                {
                    // IgnoreCollisionはCollider型を使用し、Physics2D.IgnoreCollisionを使用する
                    Physics2D.IgnoreCollision(thisCollider, ghostCollider);
                    Physics2D.IgnoreCollision(thisCollider, ghostStickCollider);
                }
            }

        }
        if (nameTag == null) 
        {
            nameTag = GameObject.Find("P" + id.ToString() + "NameTag");
        }
        if(id > 4) 
        {
            MoveAnime();
            return;
        }

        if (GameSetting.allJoin && startTrigger == 0)
        {
            StartInUpdate();
            startTrigger = 1;
        }
        //rotZの設定
        rotZ = transform.eulerAngles.z;
        if (rotZ < 0) { rotZ += 360; }// 0 度未満なら正の値にする
        if (rotZ > 180) { rotZ -= 180; }//上に向いているほうの棒の角度のみ取得

        body = bodyObj.GetComponent<Body>();// 親から Body を取得する
        onFloor = onSurface | onPlayer | onStick | body.onSurface | body.onPlayer | body.onStick; // 何かに接触している時は true

        if (!(isRespowing))
        {
            if (GameSetting.Playable && !gameSetting.isPaused)
            {
                selfDeath();
                Jump();
            }
            Move();
        }   
        ExitDelay();


  
    }

    // 移動は FixedUpdateで行う※Inputの入力が入りにくくなる
    void FixedUpdate()
    {
        if (GameStart.gameMode1 == "Online" && GameSetting.setupEnded && SceneManager.GetActiveScene().name != "Title")
        {
            if (photonView.IsMine)
            {
                if (!isRespowing) 
                {
                    photonView.RPC(nameof(MoveStickRotation), RpcTarget.All, id, stickRot);
                }
            }
            else 
            {
                stickRb = GetComponent<Rigidbody2D>();
                stickRb.MoveRotation(stickRots[id - 1]);            // 角度反映 これはポーズ時も行う
            }
        }
        else
        {
            stickRb = GetComponent<Rigidbody2D>();
            stickRb.MoveRotation(stickRot);            // 角度反映 これはポーズ時も行う
        }
        if (GameStart.gameMode1 == "Online") { return; }
        // プレイヤー速度取得
        Playerspeed = ((transform.parent.gameObject.transform.position - latestPos) / Time.deltaTime);
        if (gameSetting.isPaused && !isSaveDone)
        {
            pausedPos = transform.parent.gameObject.transform.position;
            speedWhenPaused = Playerspeed;
            isSaveDone = true; 
        }
        if (gameSetting.isPaused)
        {
            stickRb.velocity = new Vector2(0, 0);
            transform.parent.gameObject.transform.position = pausedPos;
        }
        if (gameSetting.isPaused && isSaveDone)
        {
            stickRb.velocity = new Vector2(speedWhenPaused.x * 2.1f, speedWhenPaused.y * 2.1f);
            isSaveDone = false;
        }
        latestPos = transform.parent.gameObject.transform.position;
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
        if (GameSetting.Playable && !gameSetting.isPaused) //プレイヤー数選択画面でも操作可能
        {
            if (!NetWorkMain.isOnline || (NetWorkMain.isOnline && photonView.IsMine))
            {
                bool jumpKey = Input.GetKeyDown(KeyJump);
                if (onFloor && (jumpKey || ControllerInput.jump[id - 1] ) )
                {
                    // 棒の回転値に合わせて飛ぶ方向を求める
                    float jumpDirection;                       
                    if (rotZ < 180) { jumpDirection = 6; }
                    else { jumpDirection = 18; }
                    jumpDirection = (jumpDirection - rotZ / 15) * 1.15f;

                    //棒が真横を向いているときはジャンプできない
                    if (!(rotZ > 179) && !(rotZ < 1))
                    {
                        stickRb.velocity = new Vector2(jumpDirection, jumpforce);
                        onFloor = false; onPlayer = false; onStick = false; onSurface = false; body.onSurface = false; body.onPlayer = false; body.onStick = false; 
                        //効果音鳴らす
                        SoundEffect.soundTrigger[5] = 1;
                    }

                    // クールタイム(この時間は入力を受け付けない)
                    coolTime = CoolTime_;

                }
            }
        
        }
       
    }
    [PunRPC]
    void MoveStickRotation(int id , float rot)
    {
        stickRb = GameObject.Find("Stick" + id).GetComponent<Rigidbody2D>();
        stickRb.MoveRotation(rot);
        stickRots[id - 1] = rot;    
    }


    // 移動
    void Move()
    {
        if (GameSetting.Playable && !gameSetting.isPaused) //プレイヤー数選択画面でも操作可能
        {
            if (!NetWorkMain.isOnline)
            {
                if (Input.GetKey(KeyRight) || ControllerInput.LstickX[id - 1] > 0) { stickRot -= rotSpeed * Time.deltaTime; }
                if (Input.GetKey(KeyLeft) || ControllerInput.LstickX[id - 1] < 0) { stickRot += rotSpeed * Time.deltaTime; }
            }
            else
            {
                if (photonView.IsMine)
                {
                    if (Input.GetKey(KeyCode.RightArrow) || ControllerInput.LstickX[0] > 0) { stickRot -= rotSpeed * Time.deltaTime; }
                    if (Input.GetKey(KeyCode.LeftArrow) || ControllerInput.LstickX[0] < 0) { stickRot += rotSpeed * Time.deltaTime; }

                    if (stickRot > 360)
                    {
                        stickRot -= 360;
                    }
                    if (stickRot < 0)
                    {
                        stickRot += 360;
                    }
                }
            }

        }
    }



    // 死亡
    public void StartDead()
    {
        if (GameStart.gameMode1 == "Single" && GameStart.gameMode2 == "Arcade")
        {
            return;
        }
        if (GameStart.gameMode1 == "Online")
        {
            if (photonView.IsMine)
            {
                photonView.RPC(nameof(RPCStartDead), RpcTarget.All);
                photonView.RPC(nameof(MoveStickRotation), RpcTarget.All, id, 0);
            }
        }
        else 
        {
            isRespowing = true;
            stickSprite.enabled = false;
            parentSprite.enabled = false;
            body.eyeActive = false;
            GameMode.isDead[id - 1] = true;
            stickRot = 0f;
            //効果音
            SoundEffect.soundTrigger[1] = 1;
            //エフェクト
            Vector2 effPos = this.transform.position;
            GameObject effectPrefab = (GameObject)Resources.Load("DeathEffect1");
            GameObject effectObj = Instantiate(effectPrefab, effPos, Quaternion.identity);
            StartCoroutine(Respown());
        }
  
    }
    [PunRPC] void RPCStartDead() 
    {
        isRespowing = true;
        stickSprite.enabled = false;
        parentSprite.enabled = false;
        body.eyeActive = false;
        GameMode.isDead[id - 1] = true;
        stickRot = 0f;
        //効果音
        SoundEffect.soundTrigger[1] = 1;
        //エフェクト
        Vector2 effPos = this.transform.position;
        GameObject effectPrefab = (GameObject)Resources.Load("DeathEffect1");
        GameObject effectObj = Instantiate(effectPrefab, effPos, Quaternion.identity);
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
        gameSetting.deadTimer[id - 1].SetActive(true);
        gameSetting.deadTimer[id - 1].transform.position = gameObject.transform.position + new Vector3(0, 0.5f, 0);
        Text deadText = gameSetting.deadTimer[id - 1].GetComponent<Text>();
        float countdown = 3.0f; // 開始するカウントダウンの数
        while (countdown > 0)
        {
            if (Mathf.Floor(countdown) < Mathf.Floor(countdown + Time.deltaTime))
            {
                SoundEffect.soundTrigger[3] = 1;
            }
            deadText.text = Mathf.Ceil(countdown).ToString(); // カウントダウンの整数部分を表示
            countdown -= Time.deltaTime; // Time.deltaTime を使用して時間を減少させる
            yield return null; // 次のフレームまで待機
        }

        deadText.text = ""; // カウントダウン終了後に空の文字列を設定

        //当たり判定O
        this.GetComponent<BoxCollider2D>().enabled = true;
        bodyObj.GetComponent<BoxCollider2D>().enabled = true;
        //位置固定解除
        bodyRb2D.constraints = RigidbodyConstraints2D.None; bodyRb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        //チェックポイントにリスポーン
        bodyObj.transform.position = GameSetting.respownPos[id - 1];

        nameTag.SetActive(true);
        stickSprite.enabled = true;
        parentSprite.enabled = true;
        body.eyeActive = true;
        GameMode.isDead[id - 1] = false;
        isRespowing = false;
    }

    bool animActive;
    void selfDeath()
    {
        if (GameStart.gameMode1 == "Single" && GameStart.gameMode2 == "Arcade")
        {
            return;
        }
        Vector2 animPos = this.transform.position;
        animPos.y += 1;
        if (Input.GetKey(KeyDown) || ControllerInput.backHold[id - 1])
        {
            selfDeathTimer -= Time.deltaTime;
        }
        else
        {
            selfDeathTimer = 1.0f;
        }

        if (selfDeathTimer < 0)
        {
            StartDead();
            GameObject objToDelete = GameObject.Find("HoldAnim" + id.ToString());
            if (objToDelete != null)
            {
                Destroy(objToDelete);
            }
        }

        if (Input.GetKeyDown(KeyDown) || ControllerInput.back[id - 1])
        {
            GameObject animPrefab = (GameObject)Resources.Load("HoldDeathEffect");
            GameObject animObj = Instantiate(animPrefab, animPos, Quaternion.identity);
            animObj.name = "HoldAnim" + id.ToString();
            animActive = true;
            return;
        }

        GameObject targetObj = GameObject.Find("HoldAnim" + id.ToString());
        if (targetObj != null)
        {
            targetObj.transform.position = animPos;
        }

        if (Input.GetKeyUp(KeyDown) || (ControllerInput.usingController && animActive == true && ControllerInput.backHold[id - 1] == false))
        {
            GameObject objToDelete = GameObject.Find("HoldAnim" + id.ToString());
            if (objToDelete != null)
            {
                Destroy(objToDelete);
            }
            animActive = false;
        }

    }


    // コリジョン
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(id == 5) { return; }
        if (other.gameObject.CompareTag("Surface")) { SoundEffect.soundTrigger[0] = 1; }
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        // 最初の接触点を取得
        ContactPoint2D contactPoint = other.GetContact(0);
        Vector2 contactPosition = contactPoint.point;
        float distance = 0.2f;
        int layer = 1 << LayerMask.NameToLayer("Default");
        Vector2 rayOrigin = contactPosition;
        rayOrigin.y -= 0.05f;
        Vector2 rayDirectionDown = Vector2.down;
        // Raycast の結果を格納する RaycastHit2D 変数
        RaycastHit2D hitVertical = Physics2D.Raycast(rayOrigin, rayDirectionDown, distance, layer);

        if (hitVertical.collider != null)
        {
            if (other.gameObject.CompareTag("Surface")) { onSurface = true; delay = 0.1f; delayFlag = false; }
            if (other.gameObject.CompareTag("Player")) { onPlayer = true; }
            if (other.gameObject.CompareTag("Stick")) { onStick = true; }
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Surface")) { delay = 0.1f; delayFlag = true; }
        if (other.gameObject.CompareTag("Player")) { onPlayer = false; }
        if (other.gameObject.CompareTag("Stick")) { onStick = false; }
    }
    void ExitDelay()    //床との設置判定に余裕を
    {
        if (delayFlag)
        {
            delay -= Time.deltaTime;
            if (delay < 0)
            {
                onSurface = false;
                delay = 0.1f;
                delayFlag = false;
            }
        }
    }
    //ゴーストとプレイヤーの当たり判定を無視
    void IgnoreCollisionByTag(string tag)
    {
        GameObject[] objectsToIgnore = GameObject.FindGameObjectsWithTag(tag);

        foreach (var obj in objectsToIgnore)
        {
            Collider objCollider = obj.GetComponent<Collider>();

            if (objCollider != null)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), objCollider);
            }
        }
    }
}


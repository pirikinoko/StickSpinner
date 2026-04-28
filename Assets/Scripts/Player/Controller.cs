using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controller : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameTag;

    [SerializeField]
    private TextMeshProUGUI _respownCountText;

    [SerializeField]
    public int id;

    [SerializeField]
    private KeyCode KeyLeft;

    [SerializeField]
    private KeyCode KeyRight;

    [SerializeField]
    private KeyCode KeyJump;

    [SerializeField]
    private KeyCode KeyDown;

    [SerializeField]
    private float rotSpeed = 160f;

    [SerializeField]
    private float CoolTime_ = 0.2f;

    [SerializeField]
    private Image _indicator;


    private float stickRot { get; set; }

    private float coolTime = 0.2f;
    private GamePlayHandler gameSetting;
    private const float jumpforce = 8.3f;

    private bool onFloor;
    private bool onSurface;
    private bool onPlayer;
    private bool onStick;

    private const float selfDeathDuration = 1.0f;
    public float rotZ { get; set; }
    private bool delayFlag = false;
    private bool isRespowing = false;
    private float elapsedFromOnSurface = 0;

    private SpriteRenderer stickSprite;
    private Rigidbody2D stickRb;
    private SpriteRenderer parentSprite;

    private Vector3 pausedPos;
    private Vector3 latestPos;

    private Vector2 Playerspeed;
    private Vector2 speedWhenPaused;

    private bool isSaveDone;

    private Body body;
    private GameObject bodyObj;

    private int startTrigger = 0;

    private CancellationTokenSource cts;
    private Tween fillAmoutTween;

    private SessionManager _sessionManager;

    void Start()
    {
        _sessionManager = SessionManager.Instance;
        _indicator.fillAmount = 0;

        if (_sessionManager.PlayingType == "Online" && SceneManager.GetActiveScene().name != "Title")
        {
            KeyLeft = KeyCode.LeftArrow;
            KeyRight = KeyCode.RightArrow;
            KeyJump = KeyCode.UpArrow;
            KeyDown = KeyCode.DownArrow;
        }

        parentSprite = transform.parent.gameObject.GetComponent<SpriteRenderer>();
        stickSprite = GetComponent<SpriteRenderer>();
        bodyObj = transform.parent.gameObject;

        cts = new CancellationTokenSource();
        stickRb = GetComponent<Rigidbody2D>();

        _respownCountText.gameObject.SetActive(false);
    }

    void TriggerOnce()
    {
        if (SceneManager.GetActiveScene().name == "Stage")
        {
            gameSetting = GameObject.Find("PlaySceneManager").GetComponent<GamePlayHandler>();
        }
        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            stickRb.MoveRotation(0);
        }
    }

    private void Update()
    {
        if (GamePlayHandler.AllJoin && startTrigger == 0)
        {
            TriggerOnce();
            startTrigger = 1;
        }

        rotZ = transform.eulerAngles.z;
        if (rotZ < 0) { rotZ += 360; }
        if (rotZ > 180) { rotZ -= 180; }

        body = bodyObj.GetComponent<Body>();
        onFloor = onSurface | onPlayer | onStick | body.onSurface | body.onPlayer | body.onStick;

        if (!isRespowing)
        {
            if (GamePlayHandler.Playable && !gameSetting.IsPaused)
            {
                Jump();
                if (Input.GetKeyDown(KeyDown) || ControllerInputManager.back[id - 1])
                {
                    if (fillAmoutTween == null && !(_sessionManager.PlayingType == "Single" && _sessionManager.GameMode == "Arcade"))
                    {
                        TweenFillAmountAsync(cts.Token).Forget();
                    }
                }
            }
            if (fillAmoutTween != null && (Input.GetKeyUp(KeyDown) || !ControllerInputManager.back[id - 1]))
            {
                cts.Cancel();
            }

            Move();
        }
        ExitDelay();
    }

    private async UniTask TweenFillAmountAsync(CancellationToken cancellationToken)
    {
        cts.Cancel();
        fillAmoutTween = _indicator.DOFillAmount(1.0f, selfDeathDuration)
           .SetEase(Ease.Linear)
            .OnComplete(() => StartDead());

        try
        {
            await fillAmoutTween.ToUniTask(cancellationToken: cts.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Tween was cancelled");
        }
    }

    void FixedUpdate()
    {
        stickRb = GetComponent<Rigidbody2D>();
        stickRb.MoveRotation(stickRot);

        if (_sessionManager.PlayingType == "Online" || gameSetting == null) { return; }

        PauseControl();
    }

    private void PauseControl()
    {
        Playerspeed = (transform.parent.gameObject.transform.position - latestPos) / Time.deltaTime;

        if (gameSetting.IsPaused && !isSaveDone)
        {
            pausedPos = transform.parent.gameObject.transform.position;
            speedWhenPaused = Playerspeed;
            isSaveDone = true;
        }

        if (gameSetting.IsPaused)
        {
            stickRb.velocity = new Vector2(0, 0);
            transform.parent.gameObject.transform.position = pausedPos;
        }

        if (gameSetting.IsPaused && isSaveDone)
        {
            stickRb.velocity = new Vector2(speedWhenPaused.x * 2.1f, speedWhenPaused.y * 2.1f);
            isSaveDone = false;
        }
        latestPos = transform.parent.gameObject.transform.position;
    }

    void Jump()
    {
        coolTime -= Time.deltaTime;

        if (!GamePlayHandler.Playable || gameSetting.IsPaused || coolTime > 0)
        {
            return;
        }

        if (_sessionManager.PlayingType == "Online" && Runner.LocalPlayer.PlayerId != id - 1)
        {
            return;
        }

        bool jumpKey = Input.GetKeyDown(KeyJump);

        if (onFloor && (jumpKey || ControllerInputManager.jump[id - 1]))
        {
            float jumpDirection = rotZ < 180 ? 6 : 18;
            jumpDirection = (jumpDirection - (rotZ / 15)) * 1.15f;

            if (!(rotZ > 179) && !(rotZ < 1))
            {
                stickRb.velocity = new Vector2(jumpDirection, jumpforce);
                onFloor = false; onPlayer = false; onStick = false; onSurface = false; body.onSurface = false; body.onPlayer = false; body.onStick = false;

                SoundEffect.soundTrigger[5] = 1;
            }
            coolTime = CoolTime_;
        }
    }

    void Move()
    {
        if (GamePlayHandler.Playable && !gameSetting.IsPaused)
        {
            if (_sessionManager.PlayingType != "Online")
            {
                if (Input.GetKey(KeyRight) || ControllerInputManager.LstickX[id - 1] > 0) { stickRot -= rotSpeed * Time.deltaTime; }
                if (Input.GetKey(KeyLeft) || ControllerInputManager.LstickX[id - 1] < 0) { stickRot += rotSpeed * Time.deltaTime; }
            }
            else
            {
                if (Runner.LocalPlayer.PlayerId == id - 1)
                {
                    if (Input.GetKey(KeyCode.RightArrow) || ControllerInputManager.LstickX[0] > 0) { stickRot -= rotSpeed * Time.deltaTime; }
                    if (Input.GetKey(KeyCode.LeftArrow) || ControllerInputManager.LstickX[0] < 0) { stickRot += rotSpeed * Time.deltaTime; }
                    RPC_ApplyRotation(stickRot);
                }
            }

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

    [Rpc]
    private void RPC_ApplyRotation(float rot)
    {
        stickRot = rot;
    }

    public void StartDead()
    {
        if (_sessionManager.PlayingType == "Single" && _sessionManager.GameMode == "Arcade")
        {
            return;
        }

        if (_sessionManager.PlayingType == "Online")
        {
            RPC_OnDeath();
        }
        else
        {
            OnDeath();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_OnDeath()
    {
        OnDeath();
    }

    void OnDeath()
    {
        isRespowing = true;
        stickSprite.enabled = false;
        parentSprite.enabled = false;
        body.eyeActive = false;
        GameRuleManager.IsDead[id - 1] = true;
        stickRot = 0f;

        SoundEffect.soundTrigger[1] = 1;

        Vector2 effPos = this.transform.position;
        GameObject effectPrefab = (GameObject)Resources.Load("DeathEffect1");
        GameObject effectObj = Instantiate(effectPrefab, effPos, Quaternion.identity);
        StartCoroutine(Respown());
    }


    IEnumerator Respown()
    {
        this.GetComponent<BoxCollider2D>().enabled = false;
        bodyObj.GetComponent<BoxCollider2D>().enabled = false;

        Rigidbody2D bodyRb2D = bodyObj.GetComponent<Rigidbody2D>();
        bodyRb2D.constraints = RigidbodyConstraints2D.FreezeAll;

        _nameTag.gameObject.SetActive(false);
        _respownCountText.gameObject.SetActive(true);
        _respownCountText.gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0.5f, 0);

        float countdown = 3.0f;
        while (countdown > 0)
        {
            if (Mathf.Floor(countdown) < Mathf.Floor(countdown + Time.deltaTime))
            {
                SoundEffect.soundTrigger[3] = 1;
            }
            _respownCountText.text = Mathf.Ceil(countdown).ToString();
            countdown -= Time.deltaTime;
            yield return null;
        }

        _respownCountText.text = "";

        this.GetComponent<BoxCollider2D>().enabled = true;
        bodyObj.GetComponent<BoxCollider2D>().enabled = true;
        bodyRb2D.constraints = RigidbodyConstraints2D.None;
        bodyRb2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        bodyObj.transform.position = GamePlayHandler.RespownPos[id - 1];

        _nameTag.gameObject.SetActive(true);
        stickSprite.enabled = true;
        parentSprite.enabled = true;
        body.eyeActive = true;
        GameRuleManager.IsDead[id - 1] = false;
        isRespowing = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (id == 5) { return; }
        if (other.gameObject.CompareTag("Surface")) { SoundEffect.soundTrigger[0] = 1; }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        ContactPoint2D contactPoint = other.GetContact(0);
        Vector2 contactPosition = contactPoint.point;
        float distance = 0.2f;
        int layer = 1 << LayerMask.NameToLayer("Default");
        Vector2 rayOrigin = contactPosition;
        rayOrigin.y -= 0.05f;
        Vector2 rayDirectionDown = Vector2.down;

        RaycastHit2D hitVertical = Physics2D.Raycast(rayOrigin, rayDirectionDown, distance, layer);

        if (hitVertical.collider != null)
        {
            if (other.gameObject.CompareTag("Surface")) { onSurface = true; elapsedFromOnSurface = 0.1f; delayFlag = false; }
            if (other.gameObject.CompareTag("Player")) { onPlayer = true; }
            if (other.gameObject.CompareTag("Stick")) { onStick = true; }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Surface")) { elapsedFromOnSurface = 0.1f; delayFlag = true; }
        if (other.gameObject.CompareTag("Player")) { onPlayer = false; }
        if (other.gameObject.CompareTag("Stick")) { onStick = false; }
    }

    void ExitDelay()
    {
        if (delayFlag)
        {
            elapsedFromOnSurface -= Time.deltaTime;
            if (elapsedFromOnSurface < 0)
            {
                onSurface = false;
                elapsedFromOnSurface = 0.1f;
                delayFlag = false;
            }
        }
    }
}


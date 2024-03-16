using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;



public class Body : MonoBehaviour
{

    [SerializeField]
    public int id;                                          // プレイヤー番号(1～4)

    Controller controller;

    [SerializeField]
    GameObject leftEye, rightEye;
    Collider2D ghostCollider;
    Vector2 leftPos, rightPos, leftPosGoal, rightPosGoal;

    SpriteRenderer leftEyeSprite, rightEyeSprite;
    public bool eyeActive { set; get; }

    // 内部で使うもの
    public bool  onFloor  { set; get; } 
    public bool  onSurface  { set; get; }
    public bool  onPlayer   { set; get; }
    public bool  onStick   { set; get; }
    public bool  onPinball { set; get; }

    // コリジョン
    private void OnCollisionStay2D(Collision2D other)
    {
        ContactPoint2D contactPoint = other.GetContact(0); // 最初の接触点を取得
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
            if (other.gameObject.CompareTag("Surface")) { onSurface = true; }
            if (other.gameObject.CompareTag("Player")) { onPlayer = true; }
            if (other.gameObject.CompareTag("Stick")) { onStick = true; }
            if (other.gameObject.CompareTag("Pinball")) { onPinball = true; }
        }
   
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Surface")){ onSurface = false;}
        if(other.gameObject.CompareTag("Player")){  onPlayer  = false;}
        if(other.gameObject.CompareTag("Stick")){   onStick   = false;}
        if(other.gameObject.CompareTag("Pinball")){ onPinball = false; }
    }
    void Start() 
    {
        if (GameStart.gameMode1 == "Online" && SceneManager.GetActiveScene().name != "Title")
        {
            this.GetComponent<PhotonRigidbody2DView>().enabled = true;
        }
        eyeActive = true;
    }
    void Update()
    {
        if (onPinball)　　//ピンボールゾーンでも摩擦、跳ね返りの調節
        {
            var material = GetComponent<Rigidbody2D>().sharedMaterial;
            material.friction = 1f;
            material.bounciness = 5;
        }
        else
        {
            var material = GetComponent<Rigidbody2D>().sharedMaterial;
            material.friction = 0.3f;
            material.bounciness = 0;
        }
        EyePosition();

        if(GameSetting.allJoin) 
        {
            BoxCollider2D thisCollider = this.GetComponent<BoxCollider2D>();

            if (ghostCollider == null) 
            {

                GameObject ghostObject = GameObject.Find("Ghost");
                ghostCollider = ghostObject != null ? ghostObject.GetComponent<Collider2D>() : null;
                GameObject ghostStickObject = GameObject.Find("Stick5");
                Collider2D ghostStickCollider = ghostStickObject != null ? ghostStickObject.GetComponent<Collider2D>() : null;
                if ((GameStart.gameMode2 == "Nomal" && GameStart.Stage == 1) && thisCollider != null && ghostCollider != null)
                {
                    // IgnoreCollisionはCollider型を使用し、Physics2D.IgnoreCollisionを使用する
                    Physics2D.IgnoreCollision(thisCollider, ghostCollider);
                    Physics2D.IgnoreCollision(thisCollider, ghostStickCollider);
                }
            }
          
        }
    
    }

    void EyePosition() 
    {
        controller = transform.GetChild(0).gameObject.GetComponent<Controller>();
        leftPosGoal = transform.position;
        rightPosGoal = transform.position;
        //目の位置
        leftPosGoal.x -= 0.13f;
        leftPosGoal.y += 0.03f;
        rightPosGoal.x += 0.13f;
        rightPosGoal.y += 0.03f;
        //角度によって移動
        leftPosGoal.x += (180 - controller.rotZ) / 1000;
        rightPosGoal.x -=  controller.rotZ / 1000;
        //少しずつ反映
        leftPos = Vector2.Lerp(leftPos, leftPosGoal, 80 * Time.deltaTime) ;
        rightPos = Vector2.Lerp(rightPos, rightPosGoal, 80 * Time.deltaTime);

        leftEye.transform.position = leftPos;
        rightEye.transform.position = rightPos;
        leftEyeSprite = leftEye.GetComponent<SpriteRenderer>();
        rightEyeSprite = rightEye.GetComponent<SpriteRenderer>();
        if (!eyeActive) 
        {
            leftEyeSprite.enabled = false;
            rightEyeSprite.enabled = false;
        }
        else 
        {
            leftEyeSprite.enabled = true;
            rightEyeSprite.enabled = true;
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

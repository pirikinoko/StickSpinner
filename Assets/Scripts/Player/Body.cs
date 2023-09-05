using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class Body : MonoBehaviour
{

    [SerializeField]
    public int id;                                          // プレイヤー番号(1～4)

    Controller controller;

    [SerializeField]
    GameObject leftEye, rightEye;

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
        if(other.gameObject.CompareTag("Surface")){ onSurface = true;}
        if(other.gameObject.CompareTag("Player")){  onPlayer  = true;}
        if(other.gameObject.CompareTag("Stick")){   onStick   = true;}
        if(other.gameObject.CompareTag("Pinball")){ onPinball = true; }
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
            material.friction = 0.2f;
            material.bounciness = 0;
        }
        EyePosition();
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
        leftPos = Vector2.Lerp(leftPos, leftPosGoal, 60 * Time.deltaTime) ;
        rightPos = Vector2.Lerp(rightPos, rightPosGoal, 60 * Time.deltaTime);

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class Body : MonoBehaviour
{

    [SerializeField]
    public int id;                                          // プレイヤー番号(1～4)

    [SerializeField]
    GameObject DEATH;                                       // 死亡エフェクト

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
    private void OnTriggerEnter2D(Collider2D other)　//トゲに衝突時の処理
    {
        if (other.gameObject.CompareTag("thorn"))
        {  
            Instantiate(DEATH, this.gameObject.transform.position, Quaternion.identity);
        }
    }
    void Update()
    {
        if (onPinball)　　//ピンボールゾーンでも摩擦、跳ね返りの調節
        {
            var material = GetComponent<Rigidbody2D>().sharedMaterial;
            material.friction = 0f;
            material.bounciness = 5;
        }
        else
        {
            var material = GetComponent<Rigidbody2D>().sharedMaterial;
            material.friction = 1f;
            material.bounciness = 0;
        }
    }
}

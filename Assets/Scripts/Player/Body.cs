using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



    
    public class Body : MonoBehaviour
    {
        public GameObject DEATH;

        // 内部で使うもの
        public bool  onFloor  { set; get; }
        public bool  onSurface  { set; get; }
        public bool  onPlayer   { set; get; }
        public bool  onStick   { set; get; }
        public bool  onPinball { set; get; }

        // コリジョン
        private void OnCollisionEnter2D(Collision2D other)
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
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("thorn"))
            {
                Instantiate(DEATH, this.gameObject.transform.position, Quaternion.identity); //パーティクル用ゲームオブジェクト生成        
            }
        }
        void Update()
        {
            if (onPinball)
            {
                //Physics Material2Dを取得
                var material = GetComponent<Rigidbody2D>().sharedMaterial;
                material.friction = 0f;
                material.bounciness = 5;
            }
            else
            {
                //Physics Material2Dを取得
                var material = GetComponent<Rigidbody2D>().sharedMaterial;
                material.friction = 2f;
                material.bounciness = 0;
            }
        }
    }

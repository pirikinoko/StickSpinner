using UnityEngine;

//
//  プレイヤーの死亡処理は Controller.cs へ移動
//


public class Thorn : MonoBehaviour
{

    private SessionManager _sessionManager;

    private void Start()
    {
        _sessionManager = SessionManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーとの判定
        if (other.gameObject.CompareTag("Player"))
        {
            Body bdy = other.gameObject.GetComponent<Body>();
            // Stick1 をとる
            Controller cnt = other.gameObject.transform.GetChild(0).GetComponent<Controller>();
            //当たったプレイヤーを死亡させる
            cnt.StartDead();
            int id = bdy.Id;
            if (_sessionManager.PlayingType == "Single" && _sessionManager.GameMode == "Arcade")
            {
                //無限モードの場合はゲームオーバー
                GameRuleManager.IsGameEnded = true;
            }

        }
    }
}

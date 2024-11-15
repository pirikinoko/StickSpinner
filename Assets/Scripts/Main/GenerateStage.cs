using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Numerics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine.Tilemaps;

public class GenerateStage : MonoBehaviour
{
    GameSetting gameSetting;
    [SerializeField] GameObject  checkLine, surface, thorns, parentObject, leftWall, rightWall;
    [SerializeField] GameObject[] frames;
    GameObject[] obj = new GameObject[objUnit];
    GameObject[] objForCheckLength;
    Vector2 playerPos, leftWallPos, rightWallPos, surfacePos;
    const int Floor = 0, Wall = 1, Right = 0, Left = 1, objUnit = 30;
    public static float maxHeight;
    float startHeight;
    string[,] objects = { { "Floor1", "Floor2", "Floor3", "Floor4" }, { "Wall1", "Wall2", "Wall3", "Wall4" } };
    float[,] eachLength = new float[2, 4];
    float xMax = 0, xMin = 0, yMax = 0, yMin = 0, playerYPrev, sizeX, sizeY, posX = -30, posY = 0;
    bool[] objActive = new bool[objUnit];
    Vector3[] objPos = new Vector3[objUnit];
    Vector2 checkLinePos;
    public float deadLine { get; set; }
    bool isCalucurateEnded;
    int[] objectType = new int[objUnit];
    int currentObj = 0, prev, prev2, count = 0, target = 0, objLength, objLengthPrev, objDirection = 0, enemyCount = 0, startCount, startTrigger;
    public static float[] collisionPos = new float[4];

    // Start is called before the first frame update
    void Start()
    {
        startTrigger = 0;
    }

    void AfterAllJoin() 
    {
        if (GameStart.gameMode1 != "Single") { return; }
        if (GameStart.gameMode2 != "Arcade") { return; }
        gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
        maxHeight = 0;

        startHeight = gameSetting.players[0].transform.position.y;
        surfacePos = surface.transform.position;
        thorns.gameObject.SetActive(false);
        leftWallPos = leftWall.transform.position;
        rightWallPos = rightWall.transform.position;    
        playerYPrev = gameSetting.players[0].transform.position.y; ;
        currentObj = 0;
        objectType[0] = 0;
        count = 0;
        target = 0;
        enemyCount = 0;
        isCalucurateEnded = false;
        deadLine = -10;
        SetObjectsForCalucuratingObjectLength();    
        for (int i = 0; i < 20; i++)
        {
            objActive[i] = false;
        }
        // 二次元配列をdefault値で初期化する
        for (int i = 0; i < collisionPos.Length; i++)
        {
            collisionPos[i] = 0;    
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (GameStart.gameMode1 != "Single" || GameStart.gameMode2 != "Arcade")
        {
            return;
        }
        if(GameSetting.allJoin && startTrigger == 0) 
        {
            AfterAllJoin();
            startTrigger = 1;
        }
        StartCalucurating();
        if (gameSetting.isCountDownEnded)
        {
            DeleteObject();
            SetNumbers();
            DecideNextObject();
        }
        PlayerHeightManagement();
    }
    void SetObjectsForCalucuratingObjectLength() 
    {
        //生成するオブジェクト数を保存
        int length1 = objects.GetLength(0);
        int length2 = objects.GetLength(1);
        int totalFloors = objects.GetLength(1); 
        //オブジェクト計測用の枠を設ける
        sizeX = 2.5f;
        sizeY = totalFloors * 1f + 1;
        float thickness = 0.05f;
        checkLinePos = new Vector2(posX + (sizeX / 2), posY - (sizeY / 2));
        checkLine.transform.position = checkLinePos;
        checkLine.transform.localScale = new Vector2(thickness, sizeY);
        //枠の４辺の位置と大きさをセット
        Vector2[] frameSet =
        {
            //フレーム位置
            new Vector2(posX, posY), //up
            new Vector2(posX, posY - sizeY), //down
            new Vector2(posX - (sizeX / 2), posY - ( sizeY / 2)), //left
            new Vector2(posX + (sizeX / 2), posY - ( sizeY / 2)), //right
            //フレームサイズ
            new Vector2(sizeX, thickness), //up
            new Vector2(sizeX, thickness), //down
            new Vector2(thickness, sizeY), //left
            new Vector2(thickness, sizeY) //right
        };
        for (int i = 0; i < 4; i++)
        {
            frames[i].transform.position = frameSet[i];
            frames[i].transform.localScale = frameSet[i + 4];
        }
        //上から順番にオブジェクトを配置し，計測の準備をする
        int spacing = 4;
        objForCheckLength = new GameObject[totalFloors];
        for (int j = 0; j < 1; j++)
        {
            for (int k = 0; k < length2; k++)
            {
                Vector2 generatePos = new Vector2(posX, (posY - (j * spacing) - (k + 1)));
                objForCheckLength[(j * 4) + k] = Instantiate((GameObject)Resources.Load(objects[j, k]), generatePos, Quaternion.identity);
                if (j == 1)
                {
                    objForCheckLength[(j * 4) + k].GetComponent<Transform>().Rotate(0f, 0f, 90f);
                }
            }
        }
    }
    void StartCalucurating() 
    {
        //チェックラインを動かしてそれぞれのオブジェクトの長さを計測
        if (!gameSetting.isCountDownEnded)
        {
            checkLinePos.x -= 1.0f * Time.deltaTime;
            checkLine.transform.position = checkLinePos;
            int length1 = objects.GetLength(0);
            int length2 = objects.GetLength(1);

            for (int i = 0; i < collisionPos.Length; i++)
            {
                if (collisionPos[i] == 0)
                {
                    return;
                }
            }

            if (!isCalucurateEnded)
            {
                for (int i = 0; i < 1; i++)
                {
                    for (int j = 0; j < length2; j++)
                    {
                        //衝突位置をもとにオブジェクトの長さを求める
                        float distansFromRightFrame = (posX + (sizeX / 2)) - collisionPos[(i * 4) + j];
                        eachLength[i, j] = sizeX - (distansFromRightFrame * 2);
                        UnityEngine.Debug.Log($"eachLength:{eachLength[i, j]}");
                    }
                }
                isCalucurateEnded = true;
            }
        }
    }
    void PlayerHeightManagement() 
    {
        playerPos = GameObject.Find("Player1").transform.position;
        leftWallPos.y = playerPos.y;
        rightWallPos.y = playerPos.y;
        if (playerPos.y > 2.5f)
        {
            thorns.gameObject.SetActive(true);
            surfacePos.y += 0.6f * Time.deltaTime;
            surface.transform.position = surfacePos;
        }
        if (playerPos.y > 10)
        {
            thorns.gameObject.SetActive(true);
            leftWall.transform.position = leftWallPos;
            rightWall.transform.position = rightWallPos;
        }
        if (GameMode.isGameOver == false && playerPos.y > maxHeight + startHeight)
        {
            maxHeight = playerPos.y;
            maxHeight -= startHeight;
        }
    }
    void DecideNextObject() 
    {
        if (objActive[currentObj] == false)
        {
            // 次に生成するオブジェクトの種類を決定
            objectType[currentObj] = UnityEngine.Random.Range(0, 1);  //床のみ;
            int maxLength = 3;
            if (objectType[currentObj] == Floor)
            {
                maxLength = 4;
            }
            objLength = UnityEngine.Random.Range(1, maxLength + 1);
            SetObjectPos(currentObj);
            GenerateObjects(currentObj);
            objActive[currentObj] = true;
            currentObj++;
            if (currentObj == objUnit) { currentObj = 0; }
            count++;
            objLengthPrev = objLength;
        }
    }
    void GenerateObjects(int targetNum)
    {
        GameObject prefabObj = (GameObject)Resources.Load(objects[objectType[targetNum], objLength - 1]);
        obj[targetNum] = Instantiate(prefabObj, objPos[targetNum], Quaternion.identity, parentObject.transform);
        string[] objDirectionName = { "Right", "Left" };
        obj[targetNum].name = objects[objectType[targetNum], objLength - 1] + objDirectionName[objDirection] + "-" + targetNum.ToString();
    }
    void SetObjectPos(int targetNum)
    {
        //一番最初のオブジェクト場合の設定
        if (count == 0)
        {
            objectType[targetNum] = Floor;　//最初は床オブジェクトを生成
            objPos[targetNum] = new Vector3(-6, surfacePos.y -1.1f, 0);
            objectType[0] = 0;
            return;
        }

        //次に生成するオブジェクトの方向を決定
        objDirection = UnityEngine.Random.Range(0, 2);  //0・・Left  1・・Right 

        int attempt = 0; // 追加: ループカウンタ
        const int maxAttempts = 100; // 追加: 最大試行回数

        while (true)
        {
            attempt++;

            //前のオブジェクトからどれだけ離れた位置に次のオブジェクトを生成するか決定
            switch (objectType[targetNum])
            {
                case Floor: //床        
                    xMin = 0.8f; xMax = 1.5f;
                    yMin = 0.7f; yMax = 1.10f;
                    xMin += 0.7f * eachLength[objectType[targetNum], objLength- 1];
                    xMax += 0.7f * eachLength[objectType[targetNum], objLength - 1];
                    break;
            }

            // 新しいオブジェクトの位置を計算
            Vector3 newObjPos = new Vector3();
            newObjPos.y = UnityEngine.Random.Range(objPos[prev].y + yMin, objPos[prev].y + yMax);

            if (objDirection == Right)
            {
                newObjPos.x = UnityEngine. Random.Range(objPos[prev].x + xMin, objPos[prev].x + xMax);
            }
            else
            {
                newObjPos.x = UnityEngine. Random.Range(objPos[prev].x - xMin, objPos[prev].x - xMax);
            }

            if (newObjPos.x > rightWallPos.x - (eachLength[objectType[targetNum], objLength - 1] / 2))
            {
                objDirection = Left;
            }
            else if (newObjPos.x < leftWallPos.x + (eachLength[objectType[targetNum], objLength - 1] / 2))
            {
                objDirection = Right;
            }
            else
            {
                objPos[targetNum] = newObjPos;
                break;
            }
            if(attempt > 20) 
            {
                UnityEngine.Debug.Log("無限ループです");
                break;
            }
        }

    }


    void DeleteObject()
    {
        Vector3 playerPos = gameSetting.players[0].transform.position;
        if (playerPos.y > 2.5f && playerPos.y - objPos[target].y > 7 && obj[target] != null)
        {
            deadLine = objPos[target].y;
            deadLine -= 3;
            Destroy(obj[target]);
            objActive[target] = false;
            target++;
            if (target >= objUnit)
            {
                target = 0;
            }
        }
    }

    void SetNumbers()
    {
        prev = currentObj - 1;
        if (prev == -1)
        {
            prev = objUnit - 1;
        }
        prev2 = prev - 1;
        if (prev2 == -1)
        {
            prev2 = objUnit - 1;
        }
    }
}

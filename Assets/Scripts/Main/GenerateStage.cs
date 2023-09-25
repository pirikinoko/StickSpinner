using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Numerics;
using UnityEngine;

public class GenerateStage : MonoBehaviour
{
    [SerializeField] GameObject player, checkLine;
    [SerializeField] GameObject[] frames;
    GameObject[] obj = new GameObject[objUnit];
    GameObject[] objForCheckLength;
    const int Floor = 0, Wall = 1, Right = 0, Left = 1, objUnit = 30;
    const float rightLimit = -2f, leftLimit = 10f;
    string[,] objNames = { { "Floor1", "Floor2", "Floor3", "Floor4" }, { "Wall1", "Wall2", "Wall3", "Wall4" } };
    float[,] eachLength = new float[2, 4];
    float xMax = 0, xMin = 0, yMax = 0, yMin = 0, playerYPrev, sizeX, sizeY, posX = -10, posY = 0;
    bool[] objActive = new bool[objUnit];
    Vector3[] objPos = new Vector3[objUnit];
    Vector2 checkLinePos;
    public float deadLine { get; set; }
    int[] objectType = new int[objUnit];
    int currentObj = 0, prev, prev2, count = 0, target = 0, tileY, objLength, objDirection = 0, enemyCount = 0, startCount;
    public static float[] collisionPos = new float[30];

    // Start is called before the first frame update
    void Start()
    {
        //生成するオブジェクトの長さを測る
        int length1 = objNames.GetLength(0);
        int length2 = objNames.GetLength(1);
        int totalObjects = objNames.Length - 4; //床のみ
        sizeX = 2.5f;
        sizeY = totalObjects * 1f + 1;
        float thickness = 0.05f;
        checkLinePos = new Vector2(posX + (sizeX / 2), posY - (sizeY / 2));
        checkLine.transform.position = checkLinePos;
        checkLine.transform.localScale = new Vector2(thickness, sizeY);
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
        //オブジェクトを生成し長さを計測
        objForCheckLength = new GameObject[totalObjects];
        for (int j = 0; j < 1; j++)
        {
            for (int k = 0; k < length2; k++)
            {
                Vector2 generatePos = new Vector2(posX, (posY - (j * 4) - (k + 1)));
                GameObject testPrefab = (GameObject)Resources.Load(objNames[j, k]);
                objForCheckLength[(j * 4) + k] = Instantiate(testPrefab, generatePos, Quaternion.identity);
                if (j == 1)
                {
                    Transform wallTransform = objForCheckLength[(j * 4) + k].GetComponent<Transform>();
                    wallTransform.Rotate(0f, 0f, 90f);

                }
            }
        }

        playerYPrev = player.transform.position.y;
        tileY = 0;
        currentObj = 0;
        objectType[0] = 0;
        startCount = 0;
        count = 0;
        target = 0;
        enemyCount = 0;
        deadLine = -10;
        for (int i = 0; i < 20; i++)
        {
            objActive[i] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GameStart.gameMode1 != "Single" || GameStart.gameMode2 != "Arcade")
        {
            return;
        }
        //長さを計測
        if (GameSetting.startTime > 0)
        {
            checkLinePos.x -= 1.0f * Time.deltaTime;
            checkLine.transform.position = checkLinePos;
            int length1 = objNames.GetLength(0);
            int length2 = objNames.GetLength(1);
            if (GameSetting.startTime < 0.5f && startCount == 0)
            {
                for (int i = 0; i < 1; i++)
                {
                    for (int j = 0; j < length2; j++)
                    {
                        //衝突位置をもとにオブジェクトの長さを求める
                        float distansFromRightFrame = (posX + (sizeX / 2)) - collisionPos[(i * 4) + j];
                        eachLength[i, j] = sizeX - (distansFromRightFrame * 2);
                        Destroy(objForCheckLength[(i * 4) + j]);
                    }
                }
                startCount++;
            }
        }
        else
        {
            DeleteObject();
            SetNumbers();
            if (objActive[currentObj] == false)
            {
                // 次に生成するオブジェクトの種類を決定
                objectType[currentObj] = UnityEngine.Random.Range(0, 1); 　//床のみ;
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
            }
        }

    }
    void GenerateObjects(int targetNum)
    {
        GameObject prefabObj = (GameObject)Resources.Load(objNames[objectType[targetNum], objLength - 1]);
        obj[targetNum] = Instantiate(prefabObj, objPos[targetNum], Quaternion.identity);
        string[] objDirectionName = { "Right", "Left" };
        obj[targetNum].name = objNames[objectType[targetNum], objLength - 1] + objDirectionName[objDirection] + "-" + targetNum.ToString();
    }
    void SetObjectPos(int targetNum)
    {
        //一番最初のオブジェクトの設定
        if (count == 0)
        {
            objectType[targetNum] = Floor;　//最初は床オブジェクトを生成
            objPos[targetNum] = new Vector3(1.0f, -3.8f, 0);
            objectType[0] = 0;
            return;
        }

        //次に生成するオブジェクトの方向を決定
        objDirection = UnityEngine. Random.Range(0, 2);  //0・・Left  1・・Right 
        //前のオブジェクトが壁の時壁ジャンプの方向にオブジェクトを生成
        if (objectType[prev] == Wall && count > 1)
        {
            if (((objPos[prev].x - objPos[prev2].x) > 0))
            {
                objDirection = Left;
            }
            else { objDirection = Right; }
        }

        while (true)
        {
            //前のオブジェクトとの距離を設定
            switch (objectType[targetNum])
            {
                case Floor: //床        
                    xMin = 0.5f; xMax = 0.8f;
                    yMin = 0.55f; yMax = 0.60f;
                    //壁→床の時
                    if (count >= 1 && objectType[prev] == Wall)
                    {
                        xMin = 0.6f; xMax = 0.9f;
                        yMin = 0.3f; yMax = 0.4f;
                        yMin -= 0.1f * eachLength[objectType[targetNum], objLength - 1];
                        yMax -= 0.1f * eachLength[objectType[targetNum], objLength - 1];
                    }
                    xMin += 0.3f * eachLength[objectType[targetNum], objLength - 1];
                    xMax += 0.3f * eachLength[objectType[targetNum], objLength - 1];
                    break;
                case Wall:  //壁      
                    xMin = 0.6f; xMax = 0.8f;
                    yMin = 0.4f; yMax = 0.6f;
                    //床→壁の時
                    if (count >= 1 && objectType[prev] == Floor)
                    {
                        xMin = 0.7f; xMax = 0.9f;
                        yMin = 0.7f; yMax = 0.9f;
                    }
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

            if (newObjPos.x > rightLimit - (eachLength[objectType[targetNum], objLength - 1] / 2))
            {
                objDirection = Left;
                objectType[targetNum] = Floor;
                if (objectType[prev] == Wall)
                {
                    objectType[targetNum] = Wall;
                    objDirection = Right;
                }
            }
            else if (newObjPos.x < leftLimit + (eachLength[objectType[targetNum], objLength - 1] / 2))
            {
                objDirection = Right;
                objectType[targetNum] = Floor;
                if (objectType[prev] == Wall)
                {
                    objectType[targetNum] = Wall;
                    objDirection = Left;
                }
            }
            else
            {
                objPos[targetNum] = newObjPos;
                break;
            }
        }

    }


    void DeleteObject()
    {
        Vector3 playerPos = player.transform.position;
        if (playerPos.y > 2.5f && playerPos.y - objPos[target].y > 4 && obj[target] != null)
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

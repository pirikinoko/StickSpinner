using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutOutline : MonoBehaviour
{
    GameObject gameObj;
    Transform objTransform;
    float lineSize = 0.07f;
    string[] outLineType = { "OutLineSquare", "circle", "OutLineTriangle" };
    int count = 0, typenum = 0;
    BoxCollider2D boxCollider;
    float sizeX, sizeY;
    Vector2 colSize, outLineSize;
    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        typenum = 0;
        objTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(count == 1) { return; }
        if(GameSetting.startTime > 2.9f) { return; }

        gameObj = this.gameObject;
        Vector3 objSize = this.transform.localScale;
        outLineSize = objSize;
        
        if (this.name.Contains("Wall") || this.name.Contains("Flo") || this.name.Contains("Slo"))
        {
            typenum = 0;
            objSize.x -= lineSize;
            objSize.y -= lineSize;
            //boxCollider = GetComponent<BoxCollider2D>();
            //sizeX = boxCollider.size.x + lineSize;
            //sizeY = boxCollider.size.y + lineSize;
            //colSize = new Vector2(sizeX, sizeY);
            //boxCollider.size = colSize;
        }
        else if (this.name.Contains("thorn"))
        {
            typenum = 2;
            objSize.x -= lineSize / 2;
            objSize.y -= lineSize / 2;
        }
        this.transform.localScale = objSize;
        // アウトライン生成
        Vector2 objPos = this.transform.position;
        GameObject outlinePrefab = (GameObject)Resources.Load(outLineType[typenum]);
        GameObject outline = Instantiate(outlinePrefab, objPos, Quaternion.identity);
        outline.transform.localScale = outLineSize;
        outline.gameObject.name = gameObject.name + "outline";
        // オブジェクトの向き
        Vector3 worldAngle = objTransform.eulerAngles;
        outline.transform.rotation = Quaternion.Euler(worldAngle);
        count++;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scBridge : MonoBehaviour
{

    public GameObject[] bridges;
    public GameObject bridgeTurn;

    GameObject newBridge;

    GameObject childBridge;

    GameObject oldBridge;

    GameObject bridge;
    int dir = 0;
    Quaternion quatAng;
    // Start is called before the first frame update
    void Start()
    {
        newBridge = GameObject.Find("StartBridge");
        oldBridge = GameObject.Find("OldBridge");
        childBridge = newBridge;

        MakeBridge("FORWARD");
    }

    // // Update is called once per frame
    void Update()
    {
        //MakeBridge();
    }

    void MakeBridge(string sDir)
    {
        DeleteOldBridge();
        CalcRotation(sDir);
        MakeNewBridge();
    }

    void DeleteOldBridge()
    {
    Destroy (oldBridge); //예전 다리 삭제 
    oldBridge = newBridge; //현재의 다리 저장
    //다리 시작점 만들기
    newBridge = new GameObject ("StartBridge"); 
    //스테이지에 있는 StartBridge를 새로 만들어서 newBridge에 저장한다.
    Debug.Log("okay");
    }

    void CalcRotation(string sDir)
    {
        switch (sDir) 
        {
            case "LEFT":
            dir--; //왼쪽으로 이동
            break;
            case "RIGHT":
            dir++; //오른쪽으로 이동 
            break;
        }
        //회전방향을 0~3으로 제한
            if (dir < 0) dir = 3;
            if (dir > 3) dir = 0;
        //회전각을 쿼터니언으로 변환
        quatAng.eulerAngles = new Vector3 (0, dir * 90, 0);

    }
    void MakeNewBridge()
    {
        for (int i = 0; i < 10; i++)
        {
            bridge = bridges [0]; //기본다리 (다리의 시작점은 기본다리1이다) 
            //Debug.Log(i);

            SelectBridge (i);
            
            Vector3 pos = Vector3.zero;

            Vector3 localPos = childBridge.transform.localPosition;
            //다리의 localPosition (맨 마지막으로 만들어진 다리의 localPosition으로 이 위치를 기준으로 새로운 다리를 만든다)
            switch (dir) 
            {//주인공의 회전방향에 따라 각각의 위치에 다리를 만든다. 
                case 0:
                pos = new Vector3 (localPos.x, 0, localPos.z + 10);
                break;
                case 1:
                pos = new Vector3 (localPos.x + 10, 0, localPos.z);
                break;
                case 2:
                pos = new Vector3 (localPos.x, 0, localPos.z - 10);
                break;
                case 3:
                pos = new Vector3 (localPos.x - 10, 0, localPos.z); break;
            }
            Debug.Log(bridge);
            //새로운 다리를 만들고 부모설정
            childBridge = Instantiate (bridge, pos, quatAng) as GameObject;
            childBridge.transform.parent = newBridge.transform; //(새로 만든 다리의 부모를 설정한다)
        } 
    }
    void SelectBridge(int n)
    {
        switch (n) 
        {
            
            case 9:
            case 1:
            case 3:
            case 5:
            case 7:
            default:
            break; 
        
            //마지막 다리는 교차로
            bridge = bridgeTurn;
            break;
            //홀수번째 다리는 장애물이 있는 다리
            bridge = bridges [Random.Range (0, bridges.Length)];
            break;
            //default:    //짝수번째 다리에는 동전을 만든다.
            // if (Random.Range (0, 100) > 50) 
            // {
            //         canCoin = true;
            // }
        }
    }
   
}

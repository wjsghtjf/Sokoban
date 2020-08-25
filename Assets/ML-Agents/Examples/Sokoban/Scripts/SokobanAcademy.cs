using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MLAgents;


public class SokobanAcademy : Academy
{
    [HideInInspector]
    public List<GameObject> actorObjs;
    [HideInInspector]
    public int[] players;

    public GameObject trueAgent;

    public int gridSize;
    private int numBoxes; //리셋 파라미터에서 설정한 num boxes 값을 받아오도록한다.
    public GameObject camObject;
    Camera cam;
    Camera agentCam;

    public GameObject boxPref;
    public GameObject agentPref;
    public GameObject goalPref;
    public GameObject pitPref;
    GameObject[] objects;

    GameObject plane;
    GameObject sN;
    GameObject sS;
    GameObject sE;
    GameObject sW;

    public override void InitializeAcademy()
    {
        gridSize = (int)resetParameters["gridSize"];
        cam = camObject.GetComponent<Camera>();

        objects = new GameObject[4] { agentPref, goalPref, pitPref, boxPref };

        agentCam = GameObject.Find("agentCam").GetComponent<Camera>();

        actorObjs = new List<GameObject>();

        plane = GameObject.Find("Plane");
        sN = GameObject.Find("sN");
        sS = GameObject.Find("sS");
        sW = GameObject.Find("sW");
        sE = GameObject.Find("sE");
    }

    public void SetEnvironment()
    {
        gridSize = (int)resetParameters["gridSize"];
        cam.transform.position = new Vector3(-((int)resetParameters["gridSize"] - 1) / 2f,
                                             (int)resetParameters["gridSize"] * 1.25f,
                                             -((int)resetParameters["gridSize"] - 1) / 2f);
        cam.orthographicSize = ((int)resetParameters["gridSize"] + 5f) / 2f;

        List<int> playersList = new List<int>();
        numBoxes = (int)resetParameters["numBoxes"];//리셋 파라미터에서 설정한 num boxes 값을 받아오도록한다.

        for (int i = 0; i < numBoxes; i++)
        {
            playersList.Add(3);
        }
        for (int i = 0; i < (int)resetParameters["numObstacles"]; i++)
        {
            playersList.Add(2);
        }
        for (int i = 0; i < (int)resetParameters["numGoals"]; i++)
        {
            playersList.Add(1);
        }
        players = playersList.ToArray();

        plane.transform.localScale = new Vector3(gridSize / 10.0f, 1f, gridSize / 10.0f);
        plane.transform.position = new Vector3((gridSize - 1) / 2f, -0.5f, (gridSize - 1) / 2f);
        sN.transform.localScale = new Vector3(1, 1, gridSize + 2);
        sS.transform.localScale = new Vector3(1, 1, gridSize + 2);
        sN.transform.position = new Vector3((gridSize - 1) / 2f, 0.0f, gridSize);
        sS.transform.position = new Vector3((gridSize - 1) / 2f, 0.0f, -1);
        sE.transform.localScale = new Vector3(1, 1, gridSize + 2);
        sW.transform.localScale = new Vector3(1, 1, gridSize + 2);
        sE.transform.position = new Vector3(gridSize, 0.0f, (gridSize - 1) / 2f);
        sW.transform.position = new Vector3(-1, 0.0f, (gridSize - 1) / 2f);
        agentCam.orthographicSize = (gridSize) / 2f;
        agentCam.transform.position = new Vector3((gridSize - 1) / 2f, gridSize + 1f, (gridSize - 1) / 2f);

    }

    public override void AcademyReset()
    {
        foreach (GameObject actor in actorObjs)
        {
            DestroyImmediate(actor);
        }
        SetEnvironment();

        actorObjs.Clear();

        HashSet<int> numbers = new HashSet<int>(); //오브젝트의 위치를 저장할 numbers 라는 hashset
        while (numbers.Count < numBoxes)
        {
            int x = Random.Range(0, gridSize - 2) + 1;
            int y = Random.Range(0, gridSize - 2) + 1;
            numbers.Add(x + gridSize * y);
        }
        while (numbers.Count < players.Length + 1)
        {
            numbers.Add(Random.Range(0, gridSize * gridSize));
        }
        int[] numbersA = Enumerable.ToArray(numbers);

        for (int i = 0; i < players.Length; i++)
        {
            int x = (numbersA[i]) / gridSize;
            int y = (numbersA[i]) % gridSize;
            GameObject actorObj = Instantiate(objects[players[i]]); //3이면 박스..그런거
            actorObj.transform.position = new Vector3(x, -0.25f, y);
            actorObjs.Add(actorObj);  //number의좌표애 player종류의 오브젝트가 실제로 들어가는것. 즉 실제 오브젝트
        }

        int x_a = (numbersA[players.Length]) / gridSize;
        int y_a = (numbersA[players.Length]) % gridSize;
        trueAgent.transform.position = new Vector3(x_a, -0.25f, y_a);

    }

    public int RemoveBoxGoal(GameObject Box, GameObject Goal) //박스가 충돌했을떄
    {
        DestroyImmediate(Box);
        DestroyImmediate(Goal);
        actorObjs.Remove(Box);
        actorObjs.Remove(Goal); //부딛힌놈과 박스 둘다 삭제
        int nBox = 0;
        foreach (GameObject obj in actorObjs)
        {
            if (obj.CompareTag("box")) nBox++; //남은 박스 개수 반환, 후에 agent와 함꼐 사용됨.
        }
        return nBox;
    }

}

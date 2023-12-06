using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class JengaTower : MonoBehaviour
{
    [SerializeField] int towerHeight;
    [SerializeField] int towerWidth;
    [SerializeField] JengaBlock block;
    [SerializeField] Transform[] towerPositions;
    [SerializeField] Material[] masteryMaterials;
    float blockHeight;
    float blockWidth;
    float heightOffset;
    int startingWidth = -1;
    List<StudentInfo> studentInfos = new List<StudentInfo>();
    Dictionary<string, List<JengaBlock>> towers = new Dictionary<string, List<JengaBlock>>();
    
    void Start()
    {
        blockHeight = block.transform.localScale.y;
        blockWidth = block.transform.localScale.x / 3;
        heightOffset = block.transform.localScale.y / 2;

        GenerateRequest();

        JengaBlock.OnBlockMoved += JengaBlock_OnBlockMoved;
    }

    private void OnDestroy()
    {
        JengaBlock.OnBlockMoved -= JengaBlock_OnBlockMoved;
    }

    public void GenerateRequest()
    {
        StartCoroutine(ProcessRequest(K.URL));
    }

    IEnumerator ProcessRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                JSONNode root = JSONNode.Parse(request.downloadHandler.text);
                foreach (JSONNode info in root)
                {
                    StudentInfo studentInfo = new StudentInfo();
                    studentInfo.id = info["id"];
                    studentInfo.subject = info["subject"];
                    studentInfo.grade = info["grade"];
                    studentInfo.mastery = info["mastery"];
                    studentInfo.domainid = info["domainid"];
                    studentInfo.domain = info["domain"];
                    studentInfo.cluster = info["cluster"];
                    studentInfo.standardid = info["standardid"];
                    studentInfo.standarddescription = info["standarddescription"];
                    studentInfos.Add(studentInfo);
                }
                CreateTowers();
            }
        }
    }

    private void CreateTowers()
    {
        towers.Add(K.SixthGrade, GenerateTower(studentInfos.FindAll(item => item.grade == K.SixthGrade), towerPositions[0].position));
        towers.Add(K.SeventhGrade, GenerateTower(studentInfos.FindAll(item => item.grade == K.SeventhGrade), towerPositions[1].position));
        towers.Add(K.EighthGrade, GenerateTower(studentInfos.FindAll(item => item.grade == K.EighthGrade), towerPositions[2].position));
        CheckTowerRigidBody(K.SeventhGrade);
        CheckTowerRigidBody(K.SixthGrade);
        CheckTowerRigidBody(K.EighthGrade);
    }

    List<JengaBlock> GenerateTower(List<StudentInfo> information, Vector3 offset)
    {
        List<JengaBlock> newTower = new List<JengaBlock>();
        int height = 0;
        int width = startingWidth;
        foreach (StudentInfo info in information)
        {
            JengaBlock newBlock = Instantiate(block);
            newBlock.transform.position = GetBlockPosition(height, width) + offset;
            if (height % 2 == 0)
            {
                newBlock.transform.RotateAround(offset, Vector3.up, 90);
            }
            SetBlockMaterial(newBlock, info.mastery);
            newBlock.StudentInfo = info;
            width++;
            if (width >= towerWidth + startingWidth)
            {
                height++;
                width = startingWidth;
            }
            newTower.Add(newBlock);
        }
        return newTower;
    }

    void SetBlockMaterial(JengaBlock block, int mastery)
    {
        block.gameObject.GetComponent<Renderer>().material = masteryMaterials[mastery];
    }

    Vector3 GetBlockPosition(int height, int width)
    {
        return new Vector3(0, height * blockHeight + heightOffset, width * blockWidth);
    }

    void JengaBlock_OnBlockMoved(string grade)
    {
        CheckTowerRigidBody(grade);
    }

    void CheckTowerRigidBody(string grade)
    {
        int count = 0;
        foreach (var block in towers[grade])
        {
            if (!block.IsTowerReady())
                count++;
        }
        if (count <= 0)
        {
            foreach (var block in towers[grade])
            {
                block.EnabledRigidBody();
            }
        }
    }
}
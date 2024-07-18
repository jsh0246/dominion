using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.AI;
using TMPro;

// ������ �ϳ��� �������� ���� ��ĥ���ִٰ� �����ϰ�, (���߿��� ��ó ������ �������� ������� �ް��� �����ϴ� �������� �����ٱ�?)
// 


// �Ķ��� ���忡�� �������� �������� �����ϴ°ž�


// (((���� ���忡�� ���� ���̴�)))
// ���� ���� ���� ������ ���� ���
// 1. ���� ������� ���Ǿ��ٰ� �����Ѵ�.
// 2. ���� ���� ������� ���� ���Ѱ����� ���� �󸶳� ������� ����� �� ������?(��簡 �����ұ�)
// 3. ���� ������� �Ÿ���ʷ� ����ߴµ� ���� �������...��..�� �����ϰǵ�, �����̸� ������� �����ǵ� ���� ������ ������� ����Ѵٰ� �ϴ°���
//      ��� �Ÿ��� ���� ���� �� ������̶�� ����� �� ������?..�� �Ƹ� ���� ��ġ�뿡 ���� ���� ���� ������� ���� ����� �� �ִ� ������̶�� �ϸ� �ϴ��� �� �� ����.
// 4. 



public class EnemyAgent2 : MonoBehaviour
{
    private GameObject[] playerList, enemyList, regionList;
    private List<Dictionary<int, float>> dist, distMine;
    private Dictionary<int, float> regionInfluencebyDist;
    private Dictionary<int, float> sortedRegionInfluencebyDist;

    // ������ ������
    private List<List<SelectableUnit>> regionsNearestUnits;
    private Dictionary<int, float> regionValue;

    private List<bool> isStopped;



    private void Start()
    {
        regionList = GameObject.FindGameObjectsWithTag("Region");
        dist = new List<Dictionary<int, float>>();
        distMine = new List<Dictionary<int, float>>();
        regionInfluencebyDist = new Dictionary<int, float>();

        regionValue = new Dictionary<int, float>();
        isStopped = new List<bool>();

        // ������ ������


        GetList();
        InitializeDataStructure();

        StartCoroutine(AutoMove());
        //StartCoroutine(MakeMovable());
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //AutoMoving();
    }

    private void AutoMoving()
    {
        CalculateEnemyInfluence();
        RegionInfluencbyDist();
        SortByInfluence();

        MyTurn();
    }

    private IEnumerator AutoMove()
    {
        while (true)
        {
            CalculateEnemyInfluence();
            RegionInfluencbyDist();
            SortByInfluence();

            //MyTurn();

            //NearestUnits();
            NNUnits();

            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator MakeMovable()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            for (int i = 0; i < playerList.Length; i++)
            {
                if (Vector3.zero == playerList[i].GetComponent<NavMeshAgent>().velocity)
                {
                    isStopped[i] = true;
                }
            }
        }
    }

    private void GetList()
    {
        enemyList = GameObject.FindGameObjectsWithTag("Unit 1");
        playerList = GameObject.FindGameObjectsWithTag("Unit 2");
    }

    private void InitializeDataStructure()
    {
        // dist
        for (int i = 0; i < regionList.Length; i++)
        {
            dist.Add(new Dictionary<int, float>());

            for (int j = 0; j < enemyList.Length; j++)
            {
                dist[i].Add(j, 0f);
            }
        }

        for (int i = 0; i < regionList.Length; i++)
        {
            // regionInfluencebyDist
            regionInfluencebyDist.Add(i, 0f);

            // regionsNearestUnits
            //regionsNearestUnits.Add(new List<SelectableUnit>());

            regionValue.Add(i, 0f);
        }

        for(int i=0; i<playerList.Length; i++)
        {
            isStopped.Add(true);
        }

    }

    private void CalculateEnemyInfluence()
    {
        for (int i = 0; i < regionList.Length; i++)
        {
            for (int j = 0; j < enemyList.Length; j++)
            {
                dist[i][j] = InverseDistXDamage(enemyList[j], regionList[i]);
            }
        }
    }

    // Sum�ϴ� �Լ�
    private void RegionInfluencbyDist()
    {
        for (int i = 0; i < regionList.Length; i++)
        {
            //float sum = 0;
            regionInfluencebyDist[i] = 0;
            for (int j = 0; j < enemyList.Length; j++)
            {
                //sum += dist[i].ElementAt(j).Value;
                regionInfluencebyDist[i] += dist[i].ElementAt(j).Value;
            }

            //regionInfluencebyDist.Add(i, sum);
            //regionInfluencebyDist[i] = sum;
        }
    }

    private void CalculateMyInfluence()
    {
        for (int i = 0; i < regionList.Length; i++)
        {
            distMine.Add(new Dictionary<int, float>());

            for (int j = 0; j < playerList.Length; j++)
            {
                distMine[i].Add(j, InverseDistXDamage(playerList[j], regionList[i]));
            }
        }
    }

    private void SortByInfluence()
    {
        // ��Ʈ �������� heap set ���� ����� �ڷᱸ�� ���°� ������?
        sortedRegionInfluencebyDist = regionInfluencebyDist.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        //for (int i = 0; i < sortedRegionInfluencebyDist.Count; i++)
        //{
        //    print(sortedRegionInfluencebyDist.ElementAt(i).Key + " : " + sortedRegionInfluencebyDist.ElementAt(i).Value);
        //}
    }

    private void MyTurn()
    {
        for(int i=0; i<playerList.Length; i++)
        {
            playerList[i].GetComponent<SelectableUnit>().MoveTo(regionList[sortedRegionInfluencebyDist.ElementAt(0).Key].transform.position);

            // We go to... 
            //print(sortedRegionInfluencebyDist.ElementAt(0).Key);
        }


        // �ϳ��� ã�ư���
    }

    private void NNUnits()
    {
        regionsNearestUnits = new List<List<SelectableUnit>>();

        for (int i = 0; i < regionList.Length; i++)
        {
            regionsNearestUnits.Add(new List<SelectableUnit>());
        }



        for (int i = 0; i < enemyList.Length; i++)
        {
            float min = 999999f;
            int minIdx = -1;

            for (int j = 0; j < regionList.Length; j++)
            {
                float length = Dist(enemyList[i], regionList[j]);
                if (length < min)
                {
                    min = length;
                    minIdx = j;
                }
            }

            regionsNearestUnits[minIdx].Add(enemyList[i].GetComponent<SelectableUnit>());
            //regionValue[i] += InverseDistXDamage(regionList[i], )
        }

        for (int i = 0; i < regionsNearestUnits.Count; i++)
        {
            regionValue[i] = 0f;
            for (int j = 0; j < regionsNearestUnits[i].Count; j++)
            {
                regionValue[i] += InverseDistXDamage(regionsNearestUnits[i][j].gameObject, regionList[i]);
            }

            //print(i + " : " + regionValue[i]);
        }

        regionValue = regionValue.OrderBy(item=>item.Value).ToDictionary(x=>x.Key, x=>x.Value);

        for (int i = 0;i<regionValue.Count;i++)
        {
            print(regionValue[i]);
        }

        // regionValue�� ���� ������� �����ؼ� �׸���� ��Ƽ� �Ǵµ��� �ĵ���
        // �����ϰ� �����ض�

        for(int i=0; i<regionValue.Count; i++)
        {
            // j�� player�� ���鼭 �ϳ��� ��ġ�ض�
            // �ٵ� ������ ����� �ȵǴ°Ű���? �ѳ� �߰��غ��ϱ� ������ �̻��� ,, dictionary �ٽ� ������
        }
    }


    //private void NearestUnits()
    //{
    //    regionsNearestUnits = new List<List<SelectableUnit>>();

    //    for (int i = 0; i < regionList.Length; i++)
    //    {
    //        regionsNearestUnits.Add(new List<SelectableUnit>());
    //    }



    //    for (int i = 0; i < enemyList.Length; i++)
    //    {
    //        float min = 999999f;
    //        int minIdx = -1;

    //        for (int j = 0; j < regionList.Length; j++)
    //        {
    //            float length = Dist(enemyList[i], regionList[j]);
    //            if (length < min)
    //            {
    //                min = length;
    //                minIdx = j;
    //            }
    //        }

    //        regionsNearestUnits[minIdx].Add(enemyList[i].GetComponent<SelectableUnit>());
    //        //regionValue[i] += InverseDistXDamage(regionList[i], )
    //    }

    //    for (int i = 0; i < regionsNearestUnits.Count; i++)
    //    {
    //        regionValue[i] = 0f;
    //        for (int j = 0; j < regionsNearestUnits[i].Count; j++)
    //        {
    //            regionValue[i] += InverseDistXDamage(regionsNearestUnits[i][j].gameObject, regionList[i]);
    //        }

    //        //print(i + " : " + regionValue[i]);
    //    }


    //    // ���� �߰� �ؾߵ�
    //    int t = 0;

    //    for (int i = 0; i < regionValue.Count; i++)
    //    {
    //        if (regionsNearestUnits[i].Count > 0)
    //        {
    //            //float enemyValue = InverseDistXDamage(regionsNearestUnits[i][0].gameObject, regionList[i]);
    //            float playerValue = 0f;


    //            for (int j = 0; j < playerList.Length; j++)
    //            {


    //                if (isStopped[j])
    //                //if (playerList[j].GetComponent<NavMeshAgent>().velocity == Vector3.zero)
    //                {
    //                    playerValue += InverseDistXDamage(regionsNearestUnits[i][0].gameObject, regionList[i]);
    //                    //playerValue = InverseDistXDamage(regionsNearestUnits[i][0].gameObject, regionList[i]);


    //                    if (playerValue >= regionValue[i])
    //                    //if (Mathf.Abs(playerValue - regionValue[i]) < 1f)
    //                    {
    //                        // for-loop �ؾߵɰŰ�����

    //                        // move
    //                        playerList[j].GetComponent<SelectableUnit>().MoveTo(regionList[i].transform.position);
    //                        isStopped[j] = false;

    //                        //t = j + 1;

    //                        break;
    //                    }
    //                }


    //            }
                
    //            //print(playerValue);
    //        }
              
            


    //    }






    //    //for (int i = 0; i < regionsNearestUnits.Count; i++)
    //    //{
    //    //    print(i + " region : ");
    //    //    for (int j = 0; j < regionsNearestUnits[i].Count; j++)
    //    //    {
    //    //        print(regionsNearestUnits[i][j].name);
    //    //    }
    //    //}

    //    //for (int i = 0; i < regionsNearestUnits.Count; i++)
    //    //{
    //    //    print(i + " : " + regionsNearestUnits[i].Count);
    //    //}
    //}


    private float Dist(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    private float InverseDist(GameObject a, GameObject b)
    {
        // �Ÿ��� ������ ����¿� �ݺ���ϹǷ� ���������ְ� ���ڰ� �ʹ� �۾Ƽ� x100 ���� �����
        //return Vector3.Distance(a.transform.position, b.transform.position);
        return 1 / Vector3.Distance(a.transform.position, b.transform.position) * 100;
    }

    // �Ÿ��� ������ ������ ���ݷ��� �� ����
    // �����ε��� �Ű������� �޶���
    private float InverseDistXDamage(float inverseDist, GameObject unit)
    {
        int weight = 1;

        return inverseDist * unit.GetComponent<SelectableUnit>().bullet.GetComponent<Bullet>().damage * weight;
    }

    private float InverseDistXDamage(GameObject a, GameObject b)
    {
        int weight = 1;

        return InverseDist(a, b) * a.GetComponent<SelectableUnit>().bullet.GetComponent<Bullet>().damage * weight;
    }

    // ���� ������ ���Ǿ����� �ش� �������� ���� ����� ���¼����� �ش� ������ �����º��� ���� ���������� ��ġ�Ѵ�
    // ���� ���� �������� ���� ���µ��� �߷��� ������
    // ������ �����ɶ�����
    // ó������ �� ��ü������ ���� ���ϸ�...���»����̳� ������ ��ٷȴٰ� ����(������ �ö����� �����ٴѴ�)
    // ������ ������ �񱳸� ��� �ϴ�? (1/�Ÿ�) x ���·�
    // �Ÿ��ǿ����� �պ��ٴ� ���°����� ����ġ�� �־�߰ڴ�. ���� ũ��Ƽ���ѵ�
}
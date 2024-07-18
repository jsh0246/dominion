using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.AI;
using TMPro;

// 유닛은 하나의 리전에만 영향 끼칠수있다고 가정하고, (나중에는 근처 리전에 제곱같은 영향력이 급격히 증가하는 보정값을 더해줄까?)
// 


// 파랑의 입장에서 빨간색을 적군으로 간주하는거야


// (((적의 입장에서 나인 것이다)))
// 현재 가장 쉽게 가보는 구현 상상
// 1. 적의 영향력은 계산되었다고 가정한다.
// 2. 내가 적의 영향력이 가장 약한곳부터 가면 얼마나 영향력을 행사할 수 있을까?(행사가 가능할까)
// 3. 적의 영향력은 거리비례로 계산했는데 나의 영향력은...음..난 움직일건데, 움직이면 영향력이 쎄질건데 내가 리전에 영향력을 행사한다고 하는것은
//      어느 거리에 있을 때를 내 영향력이라고 계산할 수 있을까?..음 아마 적의 위치쯤에 내가 있을 때의 영향력을 내가 행사할 수 있는 영향력이라고 하면 일단은 될 것 같다.
// 4. 



public class EnemyAgent2 : MonoBehaviour
{
    private GameObject[] playerList, enemyList, regionList;
    private List<Dictionary<int, float>> dist, distMine;
    private Dictionary<int, float> regionInfluencebyDist;
    private Dictionary<int, float> sortedRegionInfluencebyDist;

    // 구현중 변수들
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

        // 구현중 변수들


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

    // Sum하는 함수
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
        // 소트 하지말고 heap set 같은 방식의 자료구조 쓰는게 좋을까?
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


        // 하나씩 찾아가기
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

        // regionValue가 약한 순서대로 정렬해서 그리디로 모아서 되는데로 쳐들어가라
        // 간단하게 생각해라

        for(int i=0; i<regionValue.Count; i++)
        {
            // j를 player를 돌면서 하나씩 배치해라
            // 근데 정렬이 제대로 안되는거같다? 한놈 추가해보니까 정렬이 이상함 ,, dictionary 다시 공부좀
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


    //    // 정렬 추가 해야됨
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
    //                        // for-loop 해야될거같은데

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
        // 거리는 거점의 영향력에 반비례하므로 역수취해주고 숫자가 너무 작아서 x100 정도 해줬다
        //return Vector3.Distance(a.transform.position, b.transform.position);
        return 1 / Vector3.Distance(a.transform.position, b.transform.position) * 100;
    }

    // 거리의 역수와 유닛의 공격력의 곱 리턴
    // 오버로딩은 매개변수만 달라짐
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

    // 약한 거점이 계산되었으니 해당 거점에서 가장 가까운 병력순으로 해당 거점의 전투력보다 강한 전투병력을 배치한다
    // 다음 약한 거점으로 남은 병력들을 추려서 보낸다
    // 병력이 소진될때까지
    // 처음부터 내 전체병력이 존나 약하면...병력생산이나 지원을 기다렸다가 간다(병력이 올때까지 도망다닌다)
    // 병력의 전투력 비교를 어떻게 하니? (1/거리) x 공력력
    // 거리의역수의 합보다는 병력개수도 가중치에 넣어야겠다. 가장 크리티컬한듯
}
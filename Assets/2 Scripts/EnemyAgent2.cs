using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections;
using Unity.VisualScripting;

public class EnemyAgent2 : MonoBehaviour
{
    private GameObject[] playerList, enemyList, regionList;
    private List<Dictionary<int, float>> dist, distMine;
    private Dictionary<int, float> regionInfluencebyDist;
    Dictionary<int, float> sortedRegionInfluencebyDist;




    private void Start()
    {
        regionList = GameObject.FindGameObjectsWithTag("Region");
        dist = new List<Dictionary<int, float>>();
        distMine = new List<Dictionary<int, float>>();
        regionInfluencebyDist = new Dictionary<int, float>();

        GetList();
        InitializeDataStructure();

        StartCoroutine(AutoMove());
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

            MyTurn();

            yield return new WaitForSeconds(1f);
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
                dist[i].Add(j, 0);
            }
        }

        // regionInfluencebyDist
        for (int i = 0; i < regionList.Length; i++)
        {
            regionInfluencebyDist.Add(i, 0);
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

        // 2024 7. 3.
        // sortedRegionInfluencebyDist 출력해봐서 5개의 리전의 적의 영향력 수치화해서 그리디로 분배하기 반복문 한번 넣어야할듯
    }

    private void MyTurn()
    {
        for(int i=0; i<playerList.Length; i++)
        {
            playerList[i].GetComponent<SelectableUnit>().MoveTo(regionList[sortedRegionInfluencebyDist.ElementAt(0).Key].transform.position);
            print(sortedRegionInfluencebyDist.ElementAt(0).Key);
        }
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
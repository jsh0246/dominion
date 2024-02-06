using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections;

public class EnemyAgent2 : MonoBehaviour
{
    private GameObject[] playerList, enemyList, regionList;
    private List<Dictionary<int, float>> dist, distSorted, distMine;
    //private float[] regionInfluencebyDist;
    private Dictionary<int, float> regionInfluencebyDist;
    private float[] regionInfluencebyVoronoi;

    private Dictionary<int, float> regionPowerSum;
    private List<List<int>> regionInfluenceEnemyAsc;

    Dictionary<int, float> sortedDict;

    private float refreshDelay = 5f;
    private float time;

    private void Start()
    {
        regionList = GameObject.FindGameObjectsWithTag("Region");
        dist = new List<Dictionary<int, float>>();
        distSorted = new List<Dictionary<int, float>>();
        distMine = new List<Dictionary<int, float>>();
        //regionInfluencebyDist = new float[regionList.Length];
        regionInfluencebyDist = new Dictionary<int, float>();
        regionInfluencebyVoronoi = new float[regionList.Length];

        regionPowerSum = new Dictionary<int, float>();
        regionInfluenceEnemyAsc = new List<List<int>>();

        GetList();
        CalculateEnemyInfluence();
        RegionInfluencbyDist();
        SortByInfluence();
        AutoMoving();

        // 각각의 거점과 플레이어의 거리합
        //DistanceToRegionNumber();
        //RegionInfluencbyDist();

        // 각각의 거점과 가장 가까운 거리의 플레이어의 거리만 유효한 거리합
        //DistanceToRegionAscOrder();
        //RegionInfluenceByVoronoi();


        // 플레이어 전투력에 대한 내용 구현





        // 24. 2. 6. 새로 시작
        // 컴퓨터 관점에서 적(플레이어)은 리전에 영향력을 행사하고 있다.
        // 얼마나 행사하고 있는가를 수치적으로 표현
        //
        // 가정 1) 각 적은 모든 리전에 거리인버스x공격력만큼 영향력을 행사하고 있다
        // 가정 2) 각 적은 가장 가까운 리전에만 거리인버스x공격력만큼 영향력을 행사하고 있다, 또는 가장 가까운 (n)개에만 영향력을 행사하고 있다
        // 
        // 영향력을 분석하여 그 영향력보다 약간 높은 전투병력을 배치한다.
        // 없으면 만들어서, 시간을 보고 시간에 따라


        //GetList();
        //StartCoroutine(AutoMove());
    }

    private void Update()
    {

    }

    private void AutoMoving()
    {
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


    private void MakeList()
    {

    }

    // 리전마다 플레이어들의 영향력을 나열
    private void CalculateEnemyInfluence()
    {
        for (int i = 0; i < regionList.Length; i++)
        {
            dist.Add(new Dictionary<int, float>());
            //float powerSum = 0;

            for (int j = 0; j < enemyList.Length; j++)
            {
                dist[i].Add(j, InverseDistXDamage(enemyList[j], regionList[i]));

                //powerSum += playerList[j].GetComponent<SelectableUnit>().bullet.GetComponent<Bullet>().damage;

                //dist[i][j] = InverseDistXDamage(playerList[j], regionList[i]);

                //print("Region " + (i + 1) + ", Player " + (j + 1) + " : " +  dist[i][j]);
            }

            //var sortedEnemies = from x in dist[i] orderby x.Value ascending select x;

            //for(int j=0; j<playerList.Length; j++)
            //{
            //    print(sortedEnemies.ElementAt(j).Key + " : " + sortedEnemies.ElementAt(j).Value);
            //}
            //regionPowerSum.Add(i, powerSum);
            //print(regionPowerSum[i]);
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

        // Sort

        for(int i=0; i<regionList.Length; i++)
        {
            var ss = from x in distMine[i] orderby x.Value ascending select x;
            ditMine[i] = distMine[i].OrderBy(x => x.Value).ToDictionary(x=>key)

            for(int j=0; j<)
        }
    }

    private void DistanceToRegionNumber()
    {
        for (int i = 0; i < dist.Count; i++)
        {
            var items = from pair in dist[i] select pair;

            // 출력
            //foreach (KeyValuePair<int, float> pair in items)                  
            //{
            //    print(pair.Key + " " + pair.Value);
            //}
        }
    }

    // i번쨰 리전에 가해지는 영향력의 합 배열 regionInfluencebyDist
    private void RegionInfluencbyDist()
    {
        //for(int i=0; i<regionList.Length; i++)
        //{
        //    for(int j=0; j<playerList.Length; j++)
        //    {
        //        print(dist[i].ElementAt(j).Key + " " + dist[i].ElementAt(j).Value);


        //    }
        //}

        for (int i = 0; i < regionList.Length; i++)
        {
            float sum = 0;
            for (int j = 0; j < enemyList.Length; j++)
            {
                sum += dist[i].ElementAt(j).Value;
                //regionInfluencebyDist[i] += dist[i].ElementAt(j).Value;
            }

            regionInfluencebyDist.Add(i, sum);

            //print(i + " : " + regionInfluencebyDist[i]);

            // 출력
            //print(regionInfluencebyDist[i]);
        }
    }

    private void SortByInfluence()
    {
        sortedDict = regionInfluencebyDist.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        for (int i = 0; i < sortedDict.Count; i++)
        {
            print(sortedDict.ElementAt(i).Key + " : " + sortedDict.ElementAt(i).Value);
        }
    }

    // 분산 할 배열을 미리 다 정해야 할듯
    // 동적으로 해야 하는데..
    private void DisperseMyArmy()
    {
        for (int i = 0; i < regionList.Length; i++)
        {
            float playerInfluencePartialSum = 0;

            for(int j=0; j<playerList.Length; j++)
            {
                playerInfluencePartialSum += distMine[i][j];

                if(playerInfluencePartialSum > sortedDict[i])
                {

                }
            }
            

        }
    }

    private void MyTurn()
    {
        for(int i=0; i<playerList.Length; i++)
        {
            playerList[i].GetComponent<SelectableUnit>().MoveTo(regionList[sortedDict.ElementAt(0).Key].transform.position);
        }
    }

    private void DistanceToRegionAscOrder()
    {

        for (int i = 0; i < dist.Count; i++)
        {
            //var items = from pair in dist[i] orderby pair.Value ascending select pair;
            var items = from pair in dist[i] orderby pair.Value descending select pair;

            distSorted.Add(items.ToDictionary(pair => pair.Key, pair => pair.Value));


            //items 출력
            //foreach (KeyValuePair<int, float> pair in items)
            //{
            //    print(pair.Key + " " + pair.Value);
            //}

            //disSorted 출력
            //foreach (KeyValuePair<int, float> pair in distSorted[i])
            //{
            //    print(pair.Key + " " + pair.Value);
            //}
        }
    }

    private void RegionInfluenceByVoronoi()
    {
        int[] troops = new int[5];
        // 여기서 거점과 가까운 놈이 몇놈인지 계산?
        for (int i = 0; i < playerList.Length; i++)
        {
            regionInfluencebyVoronoi[distSorted[i].ElementAt(0).Key] += distSorted[i].ElementAt(0).Value;
            troops[distSorted[i].ElementAt(0).Key]++;
        }

        for (int i = 0; i < regionInfluencebyVoronoi.Length; i++)
        {
            //print(regionInfluencebyVoronoi[i]);
            regionInfluencebyVoronoi[i] *= troops[i];
            //print(regionInfluencebyVoronoi[i]);
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


    private void SortWeakRegion()
    {

    }
}
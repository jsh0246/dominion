using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyAgent : MonoBehaviour
{
    private GameObject[] playerList, enemyList, regionList;
    private List<Dictionary<int, float>> dist, distSorted;
    private float[] regionInfluencebyDist;
    private float[] regionInfluencebyVoronoi;

    private float refreshDelay = 5f;
    private float time;

    private void Start()
    {
        regionList = GameObject.FindGameObjectsWithTag("Region");
        dist = new List<Dictionary<int, float>>();
        distSorted = new List<Dictionary<int, float>>();
        regionInfluencebyDist = new float[regionList.Length];
        regionInfluencebyVoronoi = new float[regionList.Length];

        GetList();
        CalDist();

        // 각각의 거점과 플레이어의 거리합
        DistanceToRegionNumber();
        RegionInfluencbyDist();

        // 각각의 거점과 가장 가까운 거리의 플레이어의 거리만 유효한 거리합
        DistanceToRegionAscOrder();
        RegionInfluenceByVoronoi();


        // 플레이어 전투력에 대한 내용 구현
    }

    private void Update()
    {
        //time += Time.deltaTime;
        //if(time > refreshDelay)
        //{
        //    GetList();
        //    CalculateDistance();

        //    time = 0;
        //}
    }

    private void GetList()
    {
        playerList = GameObject.FindGameObjectsWithTag("Unit 1");
        enemyList = GameObject.FindGameObjectsWithTag("Unit 2");
    }

    private void CalDist()
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            dist.Add(new Dictionary<int, float>(playerList.Length));

            for (int j = 0; j < regionList.Length; j++)
            {
                // 거리의 역수만 사용
                //dist[i].Add(j, InverseDist(playerList[i], regionList[j]));

                // 거리의 역수와 공력력의 곱 사용
                // 변수이름은 dist이지만 dist에다가 각 유닛의 공격력을 곱했음, 귀찮아서 변수명 안바꿈, 공격력 없이 쓸수도 있으니까
                dist[i].Add(j, InverseDistXDamage(playerList[i], regionList[j]));
                

                // 컬렉션 초기화 단순화 하기전, 제너릭
                //dist[i][j] = new Dictionary<int, float>();
                //dist[i][j].Add(j, Dist(playerList[i], regionList[j]));

                // 플레이어 --- 리전 --- 거리 를 연결하는 제너릭 만들기
            }
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

    private void RegionInfluencbyDist()
    {
        for (int i = 0; i < regionList.Length; i++)
        {
            for (int j = 0; j < playerList.Length; j++)
            {
                regionInfluencebyDist[i] += dist[j].ElementAt(i).Value;
            }

            // 출력
            //print(regionInfluencebyDist[i]);
        }
    }

    private void DistanceToRegionAscOrder()
    {
        for (int i = 0; i < dist.Count; i++)
        {
            //var items = from pair in dist[i] orderby pair.Value ascending select pair;
            var items = from pair in dist[i] orderby pair.Value descending select pair;

            distSorted.Add(items.ToDictionary(pair => pair.Key, pair => pair.Value));


            // items 출력
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
            print(regionInfluencebyVoronoi[i]);
            regionInfluencebyVoronoi[i] *= troops[i];
            print(regionInfluencebyVoronoi[i]);
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
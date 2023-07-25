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

        // ������ ������ �÷��̾��� �Ÿ���
        DistanceToRegionNumber();
        RegionInfluencbyDist();

        // ������ ������ ���� ����� �Ÿ��� �÷��̾��� �Ÿ��� ��ȿ�� �Ÿ���
        DistanceToRegionAscOrder();
        RegionInfluenceByVoronoi();


        // �÷��̾� �����¿� ���� ���� ����
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
                // �Ÿ��� ������ ���
                //dist[i].Add(j, InverseDist(playerList[i], regionList[j]));

                // �Ÿ��� ������ ���·��� �� ���
                // �����̸��� dist������ dist���ٰ� �� ������ ���ݷ��� ������, �����Ƽ� ������ �ȹٲ�, ���ݷ� ���� ������ �����ϱ�
                dist[i].Add(j, InverseDistXDamage(playerList[i], regionList[j]));
                

                // �÷��� �ʱ�ȭ �ܼ�ȭ �ϱ���, ���ʸ�
                //dist[i][j] = new Dictionary<int, float>();
                //dist[i][j].Add(j, Dist(playerList[i], regionList[j]));

                // �÷��̾� --- ���� --- �Ÿ� �� �����ϴ� ���ʸ� �����
            }
        }
    }

    private void DistanceToRegionNumber()
    {
        for (int i = 0; i < dist.Count; i++)
        {
            var items = from pair in dist[i] select pair;

            // ���
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

            // ���
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


            // items ���
            //foreach (KeyValuePair<int, float> pair in items)
            //{
            //    print(pair.Key + " " + pair.Value);
            //}

            //disSorted ���
            //foreach (KeyValuePair<int, float> pair in distSorted[i])
            //{
            //    print(pair.Key + " " + pair.Value);
            //}
        }
    }

    private void RegionInfluenceByVoronoi()
    {
        int[] troops = new int[5];
        // ���⼭ ������ ����� ���� ������� ���?
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


    private void SortWeakRegion()
    {
        
    }
}
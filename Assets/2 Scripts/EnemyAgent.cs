using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyAgent : MonoBehaviour
{
    private GameObject[] playerList, enemyList, regionList;
    private List<Dictionary<int, float>[]> dist;
    //
    private List<Dictionary<int, float>> _dist;
    private float[] regionInfluence;

    private float refreshDelay = 5f;
    private float time;

    /* ������ �ڷᱸ���� Ȯ�� ����
    private List<Dictionary<int, float>[]> map;
    List<int> m;
    ������ �ڷᱸ���� Ȯ�� ���� */

    private void Start()
    {
        regionList = GameObject.FindGameObjectsWithTag("Region");
        dist = new List<Dictionary<int, float>[]>();
        _dist = new List<Dictionary<int, float>>();
        regionInfluence = new float[regionList.Length];

        /* ������ �ڷᱸ���� Ȯ�� ���� 
        m = new List<int>();
        m.Add(5);
        m.Add(4);
        print(m[0] + m[1]);

        map = new List<Dictionary<int, float>[]>();
        map.Add(new Dictionary<int, float>[5]);
        map[0][0] = new Dictionary<int, float>();
        map[0][0].Add(1, 1f);
        map[0][0].Add(2, 2f);
        map[0][0].Add(3, 3f);

        foreach(KeyValuePair<int, float> kvp in map[0][0])
        {
            print(kvp);
        }
        ������ �ڷᱸ���� Ȯ�� ���� */




    //for(int i=0; i<regionList.Length; i++)
    //{
    //    print(regionList[i].GetComponent<Region>().regionNumber);
    //}
        GetList();
        //CalculateDistance();
        //CalDist();
        DistanceToRegionAscOrder();
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

    private void CalculateDistance()
    {


        for(int i=0; i<playerList.Length; i++)
        {
            dist.Add(new Dictionary<int, float>[5]);
            for(int j=0; j<regionList.Length; j++)
            {
                dist[i][j] = new Dictionary<int, float>
                {
                    { j, Dist(playerList[i], regionList[j]) }
                };

                // �÷��� �ʱ�ȭ �ܼ�ȭ �ϱ���, ���ʸ�
                //dist[i][j] = new Dictionary<int, float>();
                //dist[i][j].Add(j, Dist(playerList[i], regionList[j]));

                // �÷��̾� --- ���� --- �Ÿ� �� �����ϴ� ���ʸ� �����
            }
        }
    }

    // �ּ�ó����
    private void CalDist()
    {
        //Dictionary<int, float> d = new Dictionary<int, float>();
        //d.Add(5, 5f);


        //List<Dictionary<int, float>> dic = new List<Dictionary<int, float>>();
        //dic.Add(new Dictionary<int, float>());
        //dic[0].Add(5, 5f);


        _dist.Add(new Dictionary<int, float>(playerList.Length));

        for (int i = 0; i < playerList.Length; i++)
        {
            for (int j = 0; j < regionList.Length; j++)
            {
                _dist[i].Add(j, Dist(playerList[i], regionList[j]));
                

                // �÷��� �ʱ�ȭ �ܼ�ȭ �ϱ���, ���ʸ�
                //dist[i][j] = new Dictionary<int, float>();
                //dist[i][j].Add(j, Dist(playerList[i], regionList[j]));

                // �÷��̾� --- ���� --- �Ÿ� �� �����ϴ� ���ʸ� �����
            }
        }
    }

    private void DistanceToRegionAscOrder()
    {
        //for(int i=0; i<_dist.Count; i++)
        //{
        //    for (int j=0; j < _dist[i].Count; j++)
        //    {
        //        var items = from pair in _dist[i] orderby pair.Value ascending select pair;

        //        foreach (KeyValuePair<int, float> pair in items)
        //        {
        //            print(pair.Key + " " + pair.Value);
        //        }
        //    }

        //}

        for (int i = 0; i < _dist.Count; i++)
        {
            var items = from pair in _dist[i] orderby pair.Value ascending select pair;

            //foreach (KeyValuePair<int, float> pair in items)
            //{
            //    print(pair.Key + " " + pair.Value);
            //}
        }
    }

    private void RegionInfluenceByPlayer()
    {
        //for(int i=0; i<regionList.Length; i++)
        //{
        //    for(int j=0; j<)
        //}
    }



    private float Dist(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }
}

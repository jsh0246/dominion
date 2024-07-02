using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections;
using Unity.VisualScripting;

public class EnemyAgent2Backup : MonoBehaviour
{
    private GameObject[] playerList, enemyList, regionList;
    private List<Dictionary<int, float>> dist, distSorted, distMine;
    //private float[] regionInfluencebyDist;
    private Dictionary<int, float> regionInfluencebyDist;
    private float[] regionInfluencebyVoronoi;

    private Dictionary<int, float> regionPowerSum;
    private List<List<int>> regionInfluenceEnemyAsc;

    Dictionary<int, float> sortedRegionInfluencebyDist;

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


        /* ������ �ٽ� �Լ� ���� */

        // 2024. 7. 2.
        // �Լ��� ���� ȣ���ؾ� �ϴµ� add�� ���� �־, update���� �� ������ ȣ���, �׷��� �ش� �ڷᱸ�� ���븸 ����� ���� �����ϴ� ������� ��ħ
        // ���⼭�� ���븸 ����

        GetList();
        InitializeDataStructure();


        //CalculateEnemyInfluence();
        //RegionInfluencbyDist();
        //SortByInfluence();
        //AutoMoving();

        /* ������ �ٽ� �Լ� �� */



        // ������ ������ �÷��̾��� �Ÿ���
        //DistanceToRegionNumber();
        //RegionInfluencbyDist();

        // ������ ������ ���� ����� �Ÿ��� �÷��̾��� �Ÿ��� ��ȿ�� �Ÿ���
        //DistanceToRegionAscOrder();
        //RegionInfluenceByVoronoi();


        // �÷��̾� �����¿� ���� ���� ����





        // 24. 2. 6. ���� ����
        // ��ǻ�� �������� ��(�÷��̾�)�� ������ ������� ����ϰ� �ִ�.
        // �󸶳� ����ϰ� �ִ°��� ��ġ������ ǥ��
        //
        // ���� 1) �� ���� ��� ������ �Ÿ��ι���x���ݷ¸�ŭ ������� ����ϰ� �ִ�
        // ���� 2) �� ���� ���� ����� �������� �Ÿ��ι���x���ݷ¸�ŭ ������� ����ϰ� �ִ�, �Ǵ� ���� ����� (n)������ ������� ����ϰ� �ִ�
        // 
        // ������� �м��Ͽ� �� ����º��� �ణ ���� ���������� ��ġ�Ѵ�.
        // ������ ����, �ð��� ���� �ð��� ����


        //GetList();
        //StartCoroutine(AutoMove());
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        AutoMoving();
    }

    private void AutoMoving()
    {
        //CalculateEnemyInfluence();
        //RegionInfluencbyDist();
        //SortByInfluence();

        CalculateEnemyInfluence2();
        RegionInfluencbyDist2();
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


    private void MakeList()
    {

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

        // sortedRegionInfluencebyDist
        // �Ұž��� �׳� ����ϱ�
    }

    private void CalculateEnemyInfluence2()
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
    private void RegionInfluencbyDist2()
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


    // �������� �÷��̾���� ������� ����
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

        //for(int i=0; i<regionList.Length; i++)
        //{
        //    var ss = from x in distMine[i] orderby x.Value ascending select x;
        //    distMine[i] = distMine[i].OrderBy(x => x.Value).ToDictionary(x => key);

        //    for(int j=0; j<)
        //}
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

    // i���� ������ �������� ������� �� �迭 regionInfluencebyDist
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

            // ���
            //print(regionInfluencebyDist[i]);
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

    // �л� �� �迭�� �̸� �� ���ؾ� �ҵ�
    // �������� �ؾ� �ϴµ�..
    private void DisperseMyArmy()
    {
        for (int i = 0; i < regionList.Length; i++)
        {
            float playerInfluencePartialSum = 0;

            for (int j = 0; j < playerList.Length; j++)
            {
                playerInfluencePartialSum += distMine[i][j];

                if (playerInfluencePartialSum > sortedRegionInfluencebyDist[i])
                {

                }
            }


        }
    }

    private void EnemyResponse()
    {

    }

    private void MyTurn()
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i].GetComponent<SelectableUnit>().MoveTo(regionList[sortedRegionInfluencebyDist.ElementAt(0).Key].transform.position);
            print(sortedRegionInfluencebyDist.ElementAt(0).Key);
        }
    }

    private void DistanceToRegionAscOrder()
    {

        for (int i = 0; i < dist.Count; i++)
        {
            //var items = from pair in dist[i] orderby pair.Value ascending select pair;
            var items = from pair in dist[i] orderby pair.Value descending select pair;

            distSorted.Add(items.ToDictionary(pair => pair.Key, pair => pair.Value));


            //items ���
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
            //print(regionInfluencebyVoronoi[i]);
            regionInfluencebyVoronoi[i] *= troops[i];
            //print(regionInfluencebyVoronoi[i]);
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
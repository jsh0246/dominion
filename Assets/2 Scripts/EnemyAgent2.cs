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

        // 2024 7. 3.
        // sortedRegionInfluencebyDist ����غ��� 5���� ������ ���� ����� ��ġȭ�ؼ� �׸���� �й��ϱ� �ݺ��� �ѹ� �־���ҵ�
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
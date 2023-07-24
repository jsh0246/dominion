using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGenerator : MonoBehaviour
{
    public GameObject unit1, unit2;
    public int num1, num2;

    // Start is called before the first frame update
    void Start()
    {
        UnitRandomGenerator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UnitRandomGenerator()
    {
        for(int i=0; i< num1; i++)
        {
            Instantiate(unit1, new Vector3(Random.Range(-140, -50), 2.5f, Random.Range(-40, 40)), Quaternion.identity);
        }

        for(int i=0; i<num2; i++)
        {
            Instantiate(unit2, new Vector3(Random.Range(-140, -50), 2.5f, Random.Range(-40, 40)), Quaternion.identity);
        }
    }
}

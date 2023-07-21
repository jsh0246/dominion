using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    // OnCollisionEnter vs OnTriggerEnter, collider isTrigger, Rigidbody ���� ���ǿ� ���� Ȱ��� �����ϱ�
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Floor"))
    //    {
    //        Destroy(gameObject, 3);
    //    }
    //    else if (collision.gameObject.CompareTag("Wall"))
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject, 3);
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}

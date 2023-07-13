using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    private int health;
    private int maxHealth;
    private float range;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            health -= bullet.damage;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class SelectableUnit : MonoBehaviour
{
    private NavMeshAgent Agent;
    [SerializeField] private SpriteRenderer SelectionSprite;
    [SerializeField] private Slider CapturingSlider;

    private void Awake()
    {
        SelectionManager.Instance.AvailableUnits.Add(this);
        Agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 Position)
    {
        Agent.SetDestination(Position);
    }

    public void Onselected()
    {
        SelectionSprite.gameObject.SetActive(true);
    }

    public void OnDeselected()
    {
        SelectionSprite.gameObject.SetActive(false);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Region"))
    //    {
    //        Debug.Log("Enter");
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Region"))
    //    {
    //        Debug.Log("Stay");
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Region"))
    //    {
    //        Debug.Log("Exit");
    //    }
    //}
}
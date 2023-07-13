using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class SelectableUnit : MonoBehaviour
{
    public float range;

    private NavMeshAgent Agent;
    [SerializeField] private SpriteRenderer SelectionSprite;
    [SerializeField] private Texture2D attackCursor;


    private void Awake()
    {
        range = 20f;

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

    private void OnMouseOver()
    {
        if(SelectionManager.Instance.SelectedUnits.Count > 0)
        {
            Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
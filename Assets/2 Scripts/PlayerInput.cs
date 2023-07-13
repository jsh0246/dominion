using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Camera Camera;
    [SerializeField] private RectTransform SelectionBox;
    [SerializeField] private LayerMask UnitLayers;
    [SerializeField] private LayerMask FloorLayers;
    [SerializeField] private float DragDelay = 0.1f;

    private float MouseDownTime;
    private Vector2 StartMousePosition;

    // 임시 변수
    Vector3 dist;
    Vector3 _unit;

    private void Update()
    {
        HandleSelectionInputs();
        HandleMovementInputs();
        TargetMove();

        Debug.DrawRay(_unit, dist, Color.yellow);
    }

    private void TargetMove()
    {
        if (Input.GetMouseButtonUp(1) && SelectionManager.Instance.SelectedUnits.Count > 0)
        {
            if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Unit 2")))
            {
                foreach (SelectableUnit unit in SelectionManager.Instance.SelectedUnits)
                {
                    Vector3 distance = hitInfo.point - unit.transform.position;
                    dist = distance;
                    print(distance);

                    Vector3 rangedPosition = unit.transform.position + distance.normalized * unit.range;
                    unit.MoveTo(distance);

                    _unit = unit.transform.position;

                    

                    //print("target move out");

                    //Debug.Log("Unit range " + unit.range + ", distance " + distance.magnitude + ", " + hitInfo.collider.gameObject);

                    //if (unit.range < distance.magnitude)
                    //{
                    //    unit.MoveTo(distance.normalized * (distance.magnitude - unit.range));
                    //    //print("target move in");
                    //} else
                    //{
                    //    //print("target move else");
                    //}
                }
            }
        }
    }

    private void HandleMovementInputs()
    {
        if(Input.GetMouseButtonUp(1) && SelectionManager.Instance.SelectedUnits.Count > 0)
        {
            if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit _hitInfo, Mathf.Infinity, LayerMask.GetMask("Unit 2")))
            {
                return;
            }

            if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, FloorLayers))
            {
                foreach(SelectableUnit unit in SelectionManager.Instance.SelectedUnits)
                {
                    //print("just move");
                    print("JUSTMOVE: " + hitInfo.collider.gameObject);
                    unit.MoveTo(hitInfo.point);
                }
            }
        }
    }

    private void HandleSelectionInputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectionBox.sizeDelta = Vector2.zero;
            SelectionBox.gameObject.SetActive(true);
            StartMousePosition = Input.mousePosition;
            MouseDownTime = Time.time;
        }
        else if (Input.GetMouseButton(0) && MouseDownTime + DragDelay < Time.time)
        {
            ResizeSelectionBox();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SelectionBox.sizeDelta = Vector2.zero;
            SelectionBox.gameObject.SetActive(false);

            if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, Mathf.Infinity, UnitLayers) && hitInfo.collider.TryGetComponent<SelectableUnit>(out SelectableUnit unit))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    if (SelectionManager.Instance.IsSelected(unit))
                    {
                        SelectionManager.Instance.Deselect(unit);
                    }
                    else
                    {
                        SelectionManager.Instance.Select(unit);
                    }
                }
                else
                {   
                    SelectionManager.Instance.DeselectAll();
                    SelectionManager.Instance.Select(unit);
                }
            }
            else if (MouseDownTime + DragDelay > Time.time)
            {
                SelectionManager.Instance.DeselectAll();
            }

            MouseDownTime = 0;
        }
    }

    private void ResizeSelectionBox()
    {
        float width = Input.mousePosition.x - StartMousePosition.x;
        float height = Input.mousePosition.y - StartMousePosition.y;

        SelectionBox.anchoredPosition = StartMousePosition + new Vector2(width / 2, height / 2);
        SelectionBox.sizeDelta = new Vector2(Math.Abs(width), Mathf.Abs(height));

        Bounds bounds = new Bounds(SelectionBox.anchoredPosition, SelectionBox.sizeDelta);

        for (int i = 0; i < SelectionManager.Instance.AvailableUnits.Count; i++)
        {
            if (UnitIsInSelectionBox(Camera.WorldToScreenPoint(SelectionManager.Instance.AvailableUnits[i].transform.position), bounds))
            {
                SelectionManager.Instance.Select(SelectionManager.Instance.AvailableUnits[i]);
            }
            else
            {
                SelectionManager.Instance.Deselect(SelectionManager.Instance.AvailableUnits[i]);
            }
        }
    }

    private bool UnitIsInSelectionBox(Vector2 Position, Bounds Bounds)
    {
        return Position.x > Bounds.min.x && Position.x < Bounds.max.x && Position.y > Bounds.min.y && Position.y < Bounds.max.y;
    }
}

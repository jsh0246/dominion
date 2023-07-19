using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    public GameObject[] objects;
    public bool canBuild;

    [SerializeField] private Toggle gridToggle;
    [SerializeField] private Material[] mat;

    private Buildings[] primaryBuildings;

    private RaycastHit hit;
    private GameObject pendingObject;
    private Vector3 pos;
    private bool isBuilding;
    private float gridSize;
    private bool gridOn;
    private float rotateAmount;
    private Buildings buildings;

    private void Start()
    {
        isBuilding = false;
        rotateAmount = 45f;
        gridSize = 0.5f;
        canBuild = true;

        primaryBuildings = new Buildings[2];
    }

    private void Update()
    {
        // BuildingPlaceing() 여기 있으면 에러난다 왜?ㅠ
        //BuildingPlacing();
    }

    private void FixedUpdate()
    {
        BuildingPointRayCasting();
        BuildingPlacing();
    }

    private void BuildingPlacing()
    {
        if(pendingObject != null)
        {
            if (gridOn)
            {
                pendingObject.transform.position = RoundToNearestGridVector3(pos.x, pos.y, pos.z);
            }
            else
            {
                pendingObject.transform.position = pos;
            }

            if(Input.GetMouseButton(0) && canBuild)
            {
                Build();
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                RotateBuilding();
            }

            if(pendingObject != null)
                UpdateMaterials();

            if(Input.GetMouseButton(1))
            {
                BuildCancel();
            }
        }
    }

    private void Build()
    {
        buildings.GetComponent<Buildings>().RecoverMaterial();
        buildings.GetComponent<Buildings>().RecoverTransparency();

        pendingObject = null;
        isBuilding = false;
    }

    private void BuildingPointRayCasting()
    {
        if (isBuilding)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Floor")))
            {
                pos = new Vector3(hit.point.x, 0, hit.point.z);
            }
        }
    }

    public void BuildObject(int index)
    {
        isBuilding = true;
        pendingObject = Instantiate(objects[index], pos, Quaternion.identity);
        buildings = pendingObject.GetComponent<Buildings>();
    }

    public void ToggleGrid()
    {
        if(gridToggle.isOn)
        {
            gridOn = true;
        } else
        {
            gridOn = false;
        }
    }

    private float RoundToNearestGrid(float pos)
    {
        float xDiff = pos % gridSize;
        pos -= xDiff;

        if(xDiff > (gridSize / 2))
        {
            pos += gridSize;
        }

        return pos;
    }

    private Vector3 RoundToNearestGridVector3(float x, float y, float z)
    {
        return new Vector3(RoundToNearestGrid(x), RoundToNearestGrid(y), RoundToNearestGrid(z));
    }

    private void RotateBuilding()
    {
        pendingObject.transform.Rotate(Vector3.up, rotateAmount);
    }

    private void UpdateMaterials()
    {
        //Buildings buildings = pendingObject.GetComponent<Buildings>();

        if (canBuild)
        {
            print("canbuild");
            //pendingObject.GetComponent<Buildings>().RecoverMaterial();
            buildings.RecoverMaterial();
        }
        else
        {
            print("cannotBuild");
            //pendingObject.GetComponent<Buildings>().ChangeMaterial(mat[1]);
            buildings.ChangeMaterial(mat[1]);
        }

        //pendingObject.GetComponent<Buildings>().ChangeTransparency();
        buildings.ChangeTransparency();
    }

    private void BuildCancel()
    {
        Destroy(pendingObject);
    }


}

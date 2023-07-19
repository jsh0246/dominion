using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEditor;
using static Unity.VisualScripting.Metadata;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Buildings : MonoBehaviour
{
    [SerializeField] private GameObject[] Units;
    [SerializeField] private Transform spawnPointFrom;
    [SerializeField] private Transform spawnPointTo;
    [SerializeField] private Texture2D primaryCursor;

    private Button makeBtn;
    private MeshRenderer[] meshes;
    private Material[] originalMeshes;
    private NavMeshAgent agent;
    private BuildingManager buildingManager;

    private void Start()
    {
        if (gameObject.name.CompareTo("Factory") == 0 || gameObject.name.CompareTo("Factory(Clone)") == 0)
        {
            makeBtn = GameObject.Find("Tank").GetComponent<Button>();
            makeBtn.onClick.AddListener(MakeTank);

        }
        else if (gameObject.name.CompareTo("Barrack") == 0 || gameObject.name.CompareTo("Barrack(Clone)") == 0)
        {
            makeBtn = GameObject.Find("Infantry").GetComponent<Button>();
            makeBtn.onClick.AddListener(MakeTank);
        }


        buildingManager = GameObject.Find("GameManager").GetComponent<BuildingManager>();
        //primaryCursor = 

        meshes = GetComponentsInChildren<MeshRenderer>();
        originalMeshes = new Material[meshes.Length];

        SaveMaterial();
    }

    public void MakeInfantry()
    {
        Instantiate(Units[0], transform.position, transform.rotation);
    }

    public void MakeTank()
    {
        GameObject tank = Instantiate(Units[0], spawnPointFrom.position, Quaternion.identity);

        agent = tank.GetComponent<NavMeshAgent>();
        agent.SetDestination(spawnPointTo.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Building") || other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Region") || other.gameObject.CompareTag("Unit 1") || other.gameObject.CompareTag("Unit 2"))
        {
            buildingManager.canBuild = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Building") || other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Region") || other.gameObject.CompareTag("Unit 1") || other.gameObject.CompareTag("Unit 2"))
        {
            buildingManager.canBuild = true;
        }
    }

    public void SaveMaterial()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            originalMeshes[i] = meshes[i].material;
        }
    }

    public void RecoverMaterial()
    {
        MeshRenderer[] children = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < children.Length; i++)
        {
            children[i].material = originalMeshes[i];
        }
    }

    public void RecoverTransparency()
    {
        MeshRenderer[] children = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer mesh in children)
        {
            for (int i = 0; i < mesh.materials.Length; i++)
            {
                ChangeRenderMode.changeRenderMode(mesh.material, ChangeRenderMode.BlendMode.Opaque);
                Color color = mesh.materials[i].color;
                color.a = 1f;

                mesh.materials[i].color = color;
            }
        }
    }

    public void ChangeTransparency()
    {
        MeshRenderer[] children = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer mesh in children)
        {
            for(int i=0; i<mesh.materials.Length; i++)
            {
                ChangeRenderMode.changeRenderMode(mesh.material, ChangeRenderMode.BlendMode.Transparent);
                Color color = mesh.materials[i].color;
                color.a = 0.2f;

                mesh.materials[i].color = color;
            }
        }
    }

    public void ChangeMaterial(Material newMat)
    {
        MeshRenderer[] children = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer mesh in children)
        {
            mesh.material = newMat;
            //var mats = new Material[mesh.materials.Length];
            //for (int i = 0; i < mesh.materials.Length; i++)
            //{
            //    mats[i] = newMat;
            //}
            //mesh.materials = mats;
        }
    }

    private void OnMouseOver()
    {
        UnityEngine.Cursor.SetCursor(primaryCursor, Vector2.zero, CursorMode.Auto);
    }

    private void OnMouseExit()
    {
        UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void OnMouseUp()
    {
        makeBtn.onClick.RemoveAllListeners();
        makeBtn.onClick.AddListener(() => MakeTank());
    }
}

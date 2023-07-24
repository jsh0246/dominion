using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public Button formationBtn;
    public bool isOrdered;

    private Sprite[] formation;

    private void Start()
    {
        formation = new Sprite[2];
        formation[0] = Resources.Load<Sprite>("or");
        formation[1] = Resources.Load<Sprite>("nor");

        isOrdered = true;
    }

    public void ChangeUnitFormation()
    {
        isOrdered = !isOrdered;

        if (isOrdered)
            formationBtn.image.sprite = formation[0];
        else
            formationBtn.image.sprite = formation[1];
    }
}

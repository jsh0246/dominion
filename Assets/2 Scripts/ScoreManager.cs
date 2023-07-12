using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int Score1P;
    private int Score2P;

    public int CapturingNum1P;
    public int CapturingNum2P;

    [SerializeField] private TextMeshProUGUI ScoreText1P;
    [SerializeField] private TextMeshProUGUI ScoreText2P;
    [SerializeField] private Region[] region;

    private void Start()
    {
        Score1P = 0;
        Score2P = 0;

        CapturingNum1P = 0;
        CapturingNum2P = 0;
    }

    private void Update()
    {
        CheckStatus();
        Cal();
    }

    private void Cal()
    {
        if (CapturingNum1P > 0 || CapturingNum2P > 0)
        {
            Score1P += CapturingNum1P * 1 * (int)(Time.deltaTime * 100f);
            Score2P += CapturingNum2P * 1 * (int)(Time.deltaTime * 100f);

            ScoreText1P.text = "1P Score : " + Score1P;
            ScoreText2P.text = "2P Score : " + Score2P;
        }
    }

    public void Capture1P()
    {
        ++CapturingNum1P;
    }

    public void Capture2P()
    {
        ++CapturingNum2P;
    }

    private void CheckStatus()
    {
        CapturingNum1P = CapturingNum2P = 0;

        foreach (Region r in region)
        {
            if (r.WhoOwnsThis() == 1) ++CapturingNum1P;
            else if (r.WhoOwnsThis() == 2) ++CapturingNum2P;
        }
    }

}
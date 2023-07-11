using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int TeamAScore;
    private int TeamBScore;

    private int CapturingNum1P;
    private int CapturingNum2P;

    private void Start()
    {
        TeamAScore = 0;
        TeamBScore = 0;

        CapturingNum1P = 0;
        CapturingNum2P = 0;
    }

    private void Cal()
    {

    }

    public void Capture1P()
    {
        ++CapturingNum1P;
        print(CapturingNum1P);
    }

    public void Capture2P()
    {
        ++CapturingNum2P;
    }

}
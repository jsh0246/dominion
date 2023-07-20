using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Region2 : MonoBehaviour
{
    //private enum Possession { None, Competing, Unit_1P, Unit_2P, Unit_1C, Unit_2C };
    private enum Possession { None, Unit_1P, Unit_2P, Unit_1C, Unit_2C };

    [SerializeField] private Slider CapturingSlider;
    [SerializeField] Possession Poss;
    [SerializeField] private GameObject particles;

    public HashSet<SelectableUnit> OnRegionUnits1P;
    public HashSet<SelectableUnit> OnRegionUnits2P;

    private ScoreManager scoreManager;

    private Coroutine up, down;
    private Gradient redGrad, blueGrad;

    private float gauge1P, gauge2P;

    private void Start()
    {
        redGrad = new Gradient();
        blueGrad = new Gradient();

        redGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.magenta, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 1.0f), new GradientAlphaKey(0.0f, 1.0f) });
        blueGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.cyan, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 1.0f), new GradientAlphaKey(0.0f, 1.0f) });

        Poss = Possession.None;
        up = down = null;

        OnRegionUnits1P = new HashSet<SelectableUnit>();
        OnRegionUnits2P = new HashSet<SelectableUnit>();

        // 초기화는 다른 방법?
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        print("enter");

        if (other.CompareTag("Unit 1"))
        {
            OnRegionUnits1P.Add(other.gameObject.GetComponent<SelectableUnit>());

            if(OnRegionUnits1P.Count > 0 && OnRegionUnits2P.Count >0)
            {

            }

        } else if(other.CompareTag("Unit 2"))
        {
            OnRegionUnits2P.Add(other.gameObject.GetComponent<SelectableUnit>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        print("exit");

        if (other.CompareTag("Unit 1"))
        {
            OnRegionUnits1P.Remove(other.gameObject.GetComponent<SelectableUnit>());
        }
        else if (other.CompareTag("Unit 2"))
        {
            OnRegionUnits2P.Remove(other.gameObject.GetComponent<SelectableUnit>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        print("STAy");

        if (other.CompareTag("Unit 1"))
        {

        }
        else if (other.CompareTag("Unit 2"))
        {

        }
    }



    IEnumerator Slider1PGaugeUp()
    {
        while (CapturingSlider.value < 100)
        {
            CapturingSlider.value += Time.deltaTime * 1000f;

            // 점령 표시 활성화
            if (CapturingSlider.value >= 100)
            {
                if (Poss == Possession.Unit_1P)
                {
                    Poss = Possession.Unit_1C;

                    particles.SetActive(true);
                    var col = particles.gameObject.GetComponent<ParticleSystem>().colorOverLifetime;
                    col.color = redGrad;
                }
                else if (Poss == Possession.Unit_2P)
                {
                    Poss = Possession.Unit_2C;

                    particles.SetActive(true);
                    var col = particles.gameObject.GetComponent<ParticleSystem>().colorOverLifetime;
                    col.color = blueGrad;
                }

                // break or yield return null?
                //yield return null;
                break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    //IEnumerator Slide2PGaugeUp()
    //{

    //}

    //IEnumerator Slider1PGaugeDown()
    //{
    //}

    //IEnumerator Slider2PGaugeDown()
    //{
    //}

    public int WhoOwnsThis()
    {
        if (Poss == Possession.Unit_1C) return 1;
        else if (Poss == Possession.Unit_2C) return 2;
        else return 0;
    }
}
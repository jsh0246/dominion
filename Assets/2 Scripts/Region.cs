using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.ShaderData;

public class Region : MonoBehaviour
{
    [SerializeField] private Slider CapturingSlider;
    private enum Possession { None, Competing, Unit_1P, Unit_2P, Unit_1C, Unit_2C};
    [SerializeField] Possession Poss;

    public HashSet<SelectableUnit> OnRegionUnits1P;
    public HashSet<SelectableUnit> OnRegionUnits2P;

    public ScoreManager ScoreManager;

    private Coroutine up, down;

    private void Start()
    {
        Poss = Possession.None;
        up = down = null;

        OnRegionUnits1P = new HashSet<SelectableUnit>();
        OnRegionUnits2P = new HashSet<SelectableUnit>();

        // 초기화는 다른 방법?
        ScoreManager = GameObject.FindObjectOfType<ScoreManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Unit 1"))
        {
            OnRegionUnits1P.Add(other.gameObject.GetComponent<SelectableUnit>());
            ScoreManager.Capture1P();

            switch (Poss)
            {
                case Possession.None:
                    Poss = Possession.Unit_1P;

                    StopAllCoroutines();
                    up = StartCoroutine(SliderGaugeUp());

                    break;
                case Possession.Competing:

                    


                    break;
                case Possession.Unit_1P:

                    StopAllCoroutines();
                    up = StartCoroutine(SliderGaugeUp());

                    break;
                case Possession.Unit_2P:
                    Poss = Possession.Competing;
                    StopAllCoroutines();
                    break;

                case Possession.Unit_1C:

                    break;
                case Possession.Unit_2C:
                    // 퍼센트 떨어뜨리면서 0이면 None으로
                    break;

            }

            
            

            //if (up != null)
            //    StopCoroutine(up);
            //if (down != null)
            //    StopCoroutine(down);

            //StopAllCoroutines();
            //up = StartCoroutine(SliderGaugeUp());
        } else if(other.CompareTag("Unit 2"))
        {
            OnRegionUnits2P.Add(other.gameObject.GetComponent<SelectableUnit>());

            switch (Poss)
            {
                case Possession.None:
                    Poss = Possession.Unit_2P;

                    StopAllCoroutines();
                    up = StartCoroutine(SliderGaugeUp());

                    break;
                case Possession.Competing:

                    

                    break;
                case Possession.Unit_1P:

                    Poss = Possession.Competing;
                    StopAllCoroutines();

                    break;
                case Possession.Unit_2P:

                    StopAllCoroutines();
                    up = StartCoroutine(SliderGaugeUp());

                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Unit 1"))
        {
            OnRegionUnits1P.Remove(other.gameObject.GetComponent<SelectableUnit>());

            // 1P가 나갔는데
            if(OnRegionUnits1P.Count == 0)
            {
                // 2P가 남아있다
                if (OnRegionUnits2P.Count > 0)
                {
                    Poss = Possession.Unit_2P;

                    StopAllCoroutines();
                    up = StartCoroutine(SliderGaugeUp());
                
                // 2P가 남아있지 않다. 아무도 없다
                } else if(OnRegionUnits2P.Count  == 0)
                {
                    // 1P가 100 채우고 나갔다
                    if(Poss == Possession.Unit_1C)
                    {

                    // 1P 가 중간에 나갔다
                    } else
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    }
                }
            }

            //switch (Poss)
            //{
            //    case Possession.Competing:
            //        break;
            //    case Possession.Unit_1P:
            //        break;
            //    case Possession.Unit_2P:
            //        break;
            //    case Possession.Unit_1C:
            //        break;
            //    case Possession.Unit_2C:
            //        break;
            //}





            //StopCoroutine(nameof(SliderGaugeUp));
            //StopCoroutine(nameof(SliderGaugeDown));

            //StopCoroutine("SliderGaugeUp");
            //StopCoroutine("SliderGaugeDown");

            //if (up != null)
            //    StopCoroutine(up);
            //if (down != null)
            //    StopCoroutine(down);

            //StopAllCoroutines();
            //down = StartCoroutine(SliderGaugeDown());
        }
        else if(other.CompareTag("Unit 2"))
        {
            OnRegionUnits2P.Remove(other.gameObject.GetComponent<SelectableUnit>());

            if (OnRegionUnits2P.Count == 0)
            {
                if (OnRegionUnits1P.Count > 0)
                {
                    Poss = Possession.Unit_1P;

                    StopAllCoroutines();
                    up = StartCoroutine(SliderGaugeUp());
                }
                else if (OnRegionUnits1P.Count == 0)
                {
                    // 2P가 100 채우고 나갔다
                    if (Poss == Possession.Unit_2C)
                    {

                    // 2P 가 중간에 나갔다
                    }
                    else
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    }
                }
            }

            //switch (Poss)
            //{
            //    case Possession.Competing:
            //        break;
            //    case Possession.Unit_1P:
            //        break;
            //    case Possession.Unit_2P:
            //        break;
            //    case Possession.Unit_1C:
            //        break;
            //    case Possession.Unit_2C:
            //        break;
            //}
        }
    }

    IEnumerator SliderGaugeUp()
    {
        while (CapturingSlider.value < 100)
        {
            CapturingSlider.value += Time.deltaTime * 1000f;

            if(CapturingSlider.value >= 100)
            {
                if (Poss == Possession.Unit_1P) Poss = Possession.Unit_1C;
                else if(Poss == Possession.Unit_2P) Poss = Possession.Unit_2C;

                // break or yield return null?
                //yield return null;
                break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator SliderGaugeDown()
    {
        while (CapturingSlider.value > 0)
        {
            CapturingSlider.value -= Time.deltaTime * 1000f;

            if(CapturingSlider.value <= 0)
            {
                Poss = Possession.None;
                //yield return null;
                break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
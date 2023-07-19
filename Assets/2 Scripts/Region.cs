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
    private enum Possession { None, Competing, Unit_1P, Unit_2P, Unit_1C, Unit_2C };

    [SerializeField] private Slider CapturingSlider;
    [SerializeField] Possession Poss;
    [SerializeField] private GameObject particles;

    public HashSet<SelectableUnit> OnRegionUnits1P;
    public HashSet<SelectableUnit> OnRegionUnits2P;

    private ScoreManager scoreManager;

    private Coroutine up, down;
    private Gradient redGrad, blueGrad;

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

        // �ʱ�ȭ�� �ٸ� ���?
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        print("enter");
        //print(OnRegionUnits1P.Count + " " + OnRegionUnits2P.Count);

        if (other.CompareTag("Unit 1"))
        {
            OnRegionUnits1P.Add(other.gameObject.GetComponent<SelectableUnit>());

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
                    // 1P->1P ��ȭ����
                    // ó������ �ö󰣳༮�̾����� ������ ��, �ֳ��ϸ� ���¿� ��ȭ�� ���� ���̹Ƿ�?...(��Ȯ�� ������ ����)

                    if (OnRegionUnits1P.Count == 1)
                    {
                        StopAllCoroutines();
                        up = StartCoroutine(SliderGaugeUp());
                    }

                    break;
                case Possession.Unit_2P:


                    Poss = Possession.Competing;
                    StopAllCoroutines();
                    break;

                case Possession.Unit_1C:
                    // ���� ��


                    break;
                case Possession.Unit_2C:
                    // �ۼ�Ʈ ����߸��鼭 0�̸� None����

                    // ���� 2P�� ������ 2C����, 2P�� ������ 1P�� �ٲٰ� ������ ���� 0�Ǹ� �ٽ� �ö�
                    // Poss = Possession.Unit_1P;

                    // ���⼭ ���� �ٲٰ� ����߸��°� �ؾ� �ҵ�

                    if (OnRegionUnits2P.Count == 0)
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    }

                    //StopAllCoroutines();
                    //down = StartCoroutine(SliderGaugeDown());
                    break;
            }
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
                    // 2P->2P ��ȭ����

                    if (OnRegionUnits1P.Count == 1)
                    {
                        StopAllCoroutines();
                        up = StartCoroutine(SliderGaugeUp());
                    }

                    break;
                case Possession.Unit_1C:
                    // �ۼ�Ʈ ����߸��鼭 0�̸� None����
                    //Poss = Possession.Unit_2P;
                    //if(OnRegionUnits1P.Count ==0)
                    //{
                    //    Poss = Possession.Unit_1P;
                    //} else if(OnRegionUnits1P.Count > 0)
                    //{
                    //    Poss = Possession.Competing;
                    //}

                    // 1P�� ������ 1C ���� ������ �ٿ� ����
                    // 1P�� ������ gauge down������ 0������ 1C����, 0�� �����ϸ� 2P�� ����


                    //if(OnRegionUnits1P.Count > 0 && OnRegionUnits2P.Count > 0)
                    //{

                    //} else
                    //{
                    //    StopAllCoroutines();
                    //    down = StartCoroutine(SliderGaugeDown());
                    //}

                    if(OnRegionUnits1P.Count == 0)
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    }

                    
                    break;
                case Possession.Unit_2C:
                    
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        print("exit");
        //print(OnRegionUnits1P.Count + " " + OnRegionUnits2P.Count);

        if (other.CompareTag("Unit 1"))
        {
            OnRegionUnits1P.Remove(other.gameObject.GetComponent<SelectableUnit>());

            // 1P�� �����µ�
            if(OnRegionUnits1P.Count == 0)
            {
                // 2P�� �����ִ�
                if (OnRegionUnits2P.Count > 0)
                {
                    Poss = Possession.Unit_2P;

                    StopAllCoroutines();
                    up = StartCoroutine(SliderGaugeUp());
                
                // 2P�� �������� �ʴ�. �ƹ��� ����
                } else if(OnRegionUnits2P.Count  == 0)
                {
                    // 1P�� 100 ä��� ������
                    if(Poss == Possession.Unit_1C)
                    {

                    // 1P �� �߰��� ������
                    } else
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    }
                }
            }
        }
        else if(other.CompareTag("Unit 2"))
        {
            OnRegionUnits2P.Remove(other.gameObject.GetComponent<SelectableUnit>());

            // ���⵵ �̻���
            // 2P�� �����µ�
            if (OnRegionUnits2P.Count == 0)
            {
                // 1P�� �����ִ�
                if (OnRegionUnits1P.Count > 0)
                {
                    if (Poss != Possession.Unit_1C)
                    {
                        Poss = Possession.Unit_1P;

                        StopAllCoroutines();
                        up = StartCoroutine(SliderGaugeUp());
                    }
                }
                else if (OnRegionUnits1P.Count == 0)
                {
                    // 2P�� 100 ä��� ������
                    if (Poss == Possession.Unit_2C)
                    {

                    // 2P �� �߰��� ������
                    }
                    else
                    {
                        // �Ʒ� �����Ұ�
                        // 1P�� ������ 2P�� �����µ� �߰��� �����Ŷ��
                        // 1C ������ ��������
                        // 2P ������ �ٽ� ������
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        print("STAy");

        if (Poss == Possession.None)
        {
            if (OnRegionUnits1P.Count > 0 && OnRegionUnits2P.Count == 0)
            {
                Poss = Possession.Unit_1P;
                up = StartCoroutine(SliderGaugeUp());
            }
            else if (OnRegionUnits1P.Count == 0 && OnRegionUnits2P.Count > 0)
            {
                Poss = Possession.Unit_2P;
                up = StartCoroutine(SliderGaugeUp());
            }
        }
    }



    IEnumerator SliderGaugeUp()
    {
        while (CapturingSlider.value < 100)
        {
            CapturingSlider.value += Time.deltaTime * 1000f;

            if(CapturingSlider.value >= 100)
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

    IEnumerator SliderGaugeDown()
    {
        while (CapturingSlider.value > 0)
        {
            //if(Poss == Possession.Unit_1C)
            //{
            //    Poss = Possession.Unit_1C;
            //}

            CapturingSlider.value -= Time.deltaTime * 1000f;
            particles.SetActive(false);

            if (OnRegionUnits1P.Count > 0 && OnRegionUnits2P.Count == 0)
            {
                if (Poss != Possession.Unit_1C && Poss != Possession.Unit_2C)
                {
                    Poss = Possession.Unit_1P;
                }
            }
            else if (OnRegionUnits1P.Count == 0 && OnRegionUnits2P.Count > 0)
            {
                if (Poss != Possession.Unit_1C && Poss != Possession.Unit_2C)
                {
                    Poss = Possession.Unit_2P;
                }
            }
            else if (OnRegionUnits1P.Count > 0 && OnRegionUnits2P.Count > 0)
            {
                if (Poss == Possession.Unit_1C || Poss == Possession.Unit_2C)
                {
                    // ���� �����µ� 1C�̰ų� 2C�̸� 0�� �ɶ����� �� ���¸� �������ش�. (�����ߴ����� �������� 0�ɶ������� ���θ� �������ش�)
                    ;
                }
                else
                {
                    Poss = Possession.Competing;
                }
            }

            if (CapturingSlider.value <= 0)
            {
                Poss = Possession.None;

                //yield return null;
                break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public int WhoOwnsThis()
    {
        if (Poss == Possession.Unit_1C) return 1;
        else if (Poss == Possession.Unit_2C) return 2;
        else return 0;
    }
}
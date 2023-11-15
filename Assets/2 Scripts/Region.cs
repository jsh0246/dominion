using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Region : MonoBehaviour
{
    public int regionNumber;
    // Competing�� �ʿ���� �� �����ϰ� ����°� ���Ƽ� ����
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
        //print("enter");
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
                //case Possession.Competing:
                //    if (OnRegionUnits1P.Count > 0 && OnRegionUnits2P.Count == 0)
                //    {
                //        Poss = Possession.Unit_1P;
                //    }

                //    break;
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

                    // ������ ���ڹ���
                    if (OnRegionUnits2P.Count == 0 && OnRegionUnits1P.Count == 1)
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    }
                    else if (OnRegionUnits2P.Count > 0)
                    {
                        StopAllCoroutines();
                    }

                    //Poss = Possession.Competing;

                    break;

                case Possession.Unit_1C:
                    // 1P�� ������ ���º������ up
                    // 1p 2p ������ �ۼ�Ʈ ���߱⸸ ��, 100���α��� ������ ����Ƽ���� ��

                    if (OnRegionUnits1P.Count == 1 && OnRegionUnits2P.Count == 0)
                    {
                        StopAllCoroutines();
                        up = StartCoroutine(SliderGaugeUp());
                    } else if (OnRegionUnits1P.Count > 0 && OnRegionUnits2P.Count > 0)
                    {
                        StopAllCoroutines();
                    }



                    // ���� ��




                    break;
                case Possession.Unit_2C:
                    // �ۼ�Ʈ ����߸��鼭 0�̸� None����

                    // ���� 2P�� ������ 2C����, 2P�� ������ 1P�� �ٲٰ� ������ ���� 0�Ǹ� �ٽ� �ö�
                    // Poss = Possession.Unit_1P;

                    // ���⼭ ���� �ٲٰ� ����߸��°� �ؾ� �ҵ�


                    /* ------------------------- ���ڹ��� ����  ------------------------ */
                    // �ι�° OnRegionUnits1P.Count == 1�� ���ǻ����� ������ ���ڹ�����
                    // �� ó������ ������ 2P���� �ߵ��ϰ� ���� ������ 2P���� �ߵ����� �ʵ��� �ϱ� ������
                    // �ּ��� ���������� �ִµ� Region.cs�� ==1 ���ǻ����� ������ ���ڹ��� ������
                    /* ------------------------- ���ڹ��� ����  ------------------------ */

                    if (OnRegionUnits2P.Count == 0 && OnRegionUnits1P.Count == 1)
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    }

                    //StopAllCoroutines();
                    //down = StartCoroutine(SliderGaugeDown());
                    break;
            }
        } else if (other.CompareTag("Unit 2"))
        {
            OnRegionUnits2P.Add(other.gameObject.GetComponent<SelectableUnit>());

            switch (Poss)
            {
                case Possession.None:
                    Poss = Possession.Unit_2P;

                    StopAllCoroutines();
                    up = StartCoroutine(SliderGaugeUp());

                    break;
                //case Possession.Competing:
                //    if (OnRegionUnits1P.Count == 0 && OnRegionUnits2P.Count > 0)
                //    {
                //        Poss = Possession.Unit_2P;
                //    }


                //    break;
                case Possession.Unit_1P:

                    if (OnRegionUnits1P.Count == 0 && OnRegionUnits2P.Count == 1)
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    } else if (OnRegionUnits1P.Count > 0)
                    {
                        StopAllCoroutines();
                    }

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

                    if (OnRegionUnits1P.Count == 0)
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    }


                    break;
                case Possession.Unit_2C:

                    // ���� 1C�� ���� ����

                    if (OnRegionUnits2P.Count == 1 && OnRegionUnits1P.Count == 0)
                    {
                        StopAllCoroutines();
                        up = StartCoroutine(SliderGaugeUp());
                    }
                    else if (OnRegionUnits1P.Count > 0 && OnRegionUnits2P.Count > 0)
                    {
                        StopAllCoroutines();
                    }

                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //print("exit");
        //print(OnRegionUnits1P.Count + " " + OnRegionUnits2P.Count);

        if (other.CompareTag("Unit 1"))
        {
            OnRegionUnits1P.Remove(other.gameObject.GetComponent<SelectableUnit>());

            // 1P�� �����µ�
            if (OnRegionUnits1P.Count == 0)
            {
                // 2P�� �����ִ�
                if (OnRegionUnits2P.Count > 0)
                {
                    //if(Poss == Possession.Competing)
                    //{
                    //    Poss = Possession.Unit_2P;

                    //    StopAllCoroutines();
                    //    up = StartCoroutine(SliderGaugeUp());
                    //}

                    // 1P�� ������ 1C ���¿��ٸ� �������� �������� 0������ 1C�������ش�
                    //if (Poss == Possession.Unit_1C)
                    //{
                    //    StopAllCoroutines();
                    //    down = StartCoroutine(SliderGaugeDown());
                    //} 

                    //if(Poss != Possession.Unit_2C)
                    //{
                    //    Poss = Possession.Unit_2P;

                    //    StopAllCoroutines();
                    //    down = StartCoroutine(SliderGaugeDown());
                    //} else if(Poss == Possession.Unit_2C)
                    //{
                    //    ;
                    //}

                    //Poss = Possession.Unit_2P;

                    //StopAllCoroutines();
                    //up = StartCoroutine(SliderGaugeUp());


                    if (Poss == Possession.Unit_1P)
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    }
                    else if (Poss == Possession.Unit_2P)
                    {
                        // �ö󰡴��߿� 1�� ���Դ� ������ �����ٰ� �ϸ� ���� ���� �����̴� ��ƽ�� ��� ����Ѵ�
                        // �̾ �ε巴�� ����ϴ� ���? +10�� �ƴϰ� �����ϰ� �ø��� �Ƿ���? ������ �ҿ�����
                        StopAllCoroutines();
                        up = StartCoroutine(SliderGaugeUp());
                        //down = StartCoroutine(SliderGaugeDown());
                    }
                    else if (Poss == Possession.Unit_1C)
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    }
                    else if (Poss == Possession.Unit_2C)
                    {
                        ;
                    }



                    // 2P�� �������� �ʴ�. �ƹ��� ����
                } else if (OnRegionUnits2P.Count == 0)
                {
                    // 1P�� 100 ä��� ������
                    if (Poss == Possession.Unit_1C)
                    {
                    }
                    // 1�� 2C���¿��� ������
                    else if (Poss == Possession.Unit_2C)
                    {
                        StopAllCoroutines();
                    }

                    // 1P �� �߰��� ������
                    else
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    }
                }
            }
        }
        else if (other.CompareTag("Unit 2"))
        {
            OnRegionUnits2P.Remove(other.gameObject.GetComponent<SelectableUnit>());

            // ���⵵ �̻���
            // 2P�� �����µ�
            if (OnRegionUnits2P.Count == 0)
            {
                // 1P�� �����ִ�
                if (OnRegionUnits1P.Count > 0)
                {

                    //if (Poss == Possession.Competing)
                    //{
                    //    Poss = Possession.Unit_1P;

                    //    StopAllCoroutines();
                    //    up = StartCoroutine(SliderGaugeUp());
                    //}

                    // 2P�� ������ 2C���¿��ٸ� �������� �������� 0������ 2C�������ش�

                    if (Poss == Possession.Unit_1P)
                    {
                        StopAllCoroutines();
                        up = StartCoroutine(SliderGaugeUp());
                    } else if (Poss == Possession.Unit_2P)
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    } else if (Poss == Possession.Unit_1C)
                    {
                        ;
                    } else if (Poss == Possession.Unit_2C)
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    }
                }
                else if (OnRegionUnits1P.Count == 0)
                {

                    // 2�� 1C���¿��� ������
                    if (Poss == Possession.Unit_1C)
                    {
                        StopAllCoroutines();
                    }
                    // 2P�� 100 ä��� ������
                    else if (Poss == Possession.Unit_2C)
                    {

                    }
                    // 2P �� �߰��� ������
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
        //print("STAy");

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

            // ���� ǥ�� Ȱ��ȭ
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

    IEnumerator SliderGaugeDown()
    {
        while (CapturingSlider.value > 0)
        {
            //if(Poss == Possession.Unit_1C)
            //{
            //    Poss = Possession.Unit_1C;
            //}

            CapturingSlider.value -= Time.deltaTime * 1000f;
            // 1C 2C ���¿����� 0�� ������ ��ƼŬ�� ��������
            // ���� ������ �ö󰡴ϱ� �ٷ� ������ �ʴ°�
            // �Ʒ� SetActive()�� �ٷ� ����°�
            //particles.SetActive(false);

            //if (OnRegionUnits1P.Count > 0 && OnRegionUnits2P.Count == 0)
            //{
            //    if (Poss != Possession.Unit_1C && Poss != Possession.Unit_2C)
            //    {
            //        Poss = Possession.Unit_1P;
            //    }
            //}
            //else if (OnRegionUnits1P.Count == 0 && OnRegionUnits2P.Count > 0)
            //{
            //    if (Poss != Possession.Unit_1C && Poss != Possession.Unit_2C)
            //    {
            //        Poss = Possession.Unit_2P;
            //    }
            //}
            //else if (OnRegionUnits1P.Count > 0 && OnRegionUnits2P.Count > 0)
            //{
            //    if (Poss == Possession.Unit_1C || Poss == Possession.Unit_2C)
            //    {
            //        // ���� �����µ� 1C�̰ų� 2C�̸� 0�� �ɶ����� �� ���¸� �������ش�. (�����ߴ����� �������� 0�ɶ������� ���θ� �������ش�)
            //        ;
            //    }
            //    else
            //    {
            //        //Poss = Possession.Competing;
            //    }
            //}

            if (CapturingSlider.value <= 0)
            {
                particles.SetActive(false);
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



    // ====================================
    // ------------------------------------
    // ------------------------------------

    //// ������ ������ �������� ���� ��ȭ
    //private void OnTriggerEnter(Collider other)
    //{
    //    // 1P�� �������� ��
    //    if (other.CompareTag("Unit 1"))
    //    {
    //        // �������� �ִ� 1P ������ ����Ʈ�� ���� �߰�
    //        OnRegionUnits1P.Add(other.gameObject.GetComponent<SelectableUnit>());

    //        switch (Poss)
    //        {
    //            // ���� �������� �������� ��������
    //            case Possession.None:
    //                Poss = Possession.Unit_1P;

    //                StopAllCoroutines();
    //                up = StartCoroutine(SliderGaugeUp());

    //                break;
    //            // ���� �������� 1P ������ 1P->1P�̹Ƿ� ��ȭ����
    //            // ó������ �ö󰣳༮�̾����� ������ ��, �ֳ��ϸ� ���¿� ��ȭ�� ���� ���̹Ƿ�?...(��Ȯ�� ������ ����)
    //            case Possession.Unit_1P:
    //                if (OnRegionUnits1P.Count == 1)
    //                {
    //                    StopAllCoroutines();
    //                    up = StartCoroutine(SliderGaugeUp());
    //                }

    //                break;
    //            // ���� �������� 2P������
    //            case Possession.Unit_2P:

    //                // �������� 2P�����ѵ�, �������� 2P�� ���� ��Ȳ, ������ �����
    //                if (OnRegionUnits2P.Count == 0 && OnRegionUnits1P.Count == 1)
    //                {
    //                    StopAllCoroutines();
    //                    down = StartCoroutine(SliderGaugeDown());
    //                }
    //                // �������� 2P�����ѵ�, �������� 2P�� �����ϴ� ��Ȳ, �������� ��ȭ���� ��������(�Ƹ� ������)
    //                else if (OnRegionUnits2P.Count > 0)
    //                {
    //                    StopAllCoroutines();
    //                }

    //                break;
    //            // ���� �������� ������ 1P�� ������(1C, ��ƼŬ ����Ʈ �۵���)�̾����� 
    //            case Possession.Unit_1C:
    //                // 1P�� �����ϴ� ��Ȳ�̸� �������� ���
    //                // ���⼭ �������� �ް��� ���ϴ� ��Ȳ�� �߻��ϴ� ����(�����ʿ�)
    //                if (OnRegionUnits1P.Count == 1 && OnRegionUnits2P.Count == 0)
    //                {
    //                    StopAllCoroutines();
    //                    up = StartCoroutine(SliderGaugeUp());
    //                }
    //                // 1P 2P ȥ���� ��Ȳ, ��� �ڷ�ƾ�� ������Ű�� �������·� ��ȯ
    //                else if (OnRegionUnits1P.Count > 0 && OnRegionUnits2P.Count > 0)
    //                {
    //                    StopAllCoroutines();
    //                }
    //                break;
    //            // ���� �������� ������ 2P�� ������(2C, ��ƼŬ ����Ʈ �۵���)�̾�����
    //            case Possession.Unit_2C:
    //                // 1P�� �����ϴ� ��Ȳ�̸� �������� �ٿ�
    //                // 2P�� ������ ������ ������ ó�� ���� ��Ȳ
    //                if (OnRegionUnits2P.Count == 0 && OnRegionUnits1P.Count == 1)
    //                {
    //                    StopAllCoroutines();
    //                    down = StartCoroutine(SliderGaugeDown());
    //                }
    //                break;
    //        }
    //    }
    //    else if (other.CompareTag("Unit 2"))
    //    {
    //        // ...
    //        // 1P�� ���ڸ� �ٲ㼭 �����ϰ� ����
    //    }
    //}

    //// ������ �������� ���������� ���� ��ȭ
    //private void OnTriggerExit(Collider other)
    //{
    //    // 1P�� ���������� ��
    //    if (other.CompareTag("Unit 1"))
    //    {
    //        // �������� �ִ� 1P ������ ����Ʈ�� ���� ����
    //        OnRegionUnits1P.Remove(other.gameObject.GetComponent<SelectableUnit>());

    //        // 1P�� �����µ�
    //        if (OnRegionUnits1P.Count == 0)
    //        {
    //            // 2P�� �����ִ�
    //            if (OnRegionUnits2P.Count > 0)
    //            {
    //                // ������ ��Ȳ�� 1P��� ������ �ٿ�
    //                if (Poss == Possession.Unit_1P)
    //                {
    //                    StopAllCoroutines();
    //                    down = StartCoroutine(SliderGaugeDown());
    //                }
    //                // ������ ��Ȳ�� 2P��� ������ ���
    //                // 2P�� �������� �ִµ� 1P�� ���Դ� ���� ��Ȳ
    //                else if (Poss == Possession.Unit_2P)
    //                {
    //                    // �Ʒ� 2���� �������� �ް��� ��ȭ�� �ҷ����� �� ����.
    //                    // ������ ��ȭ���� �ٿ��� �ð������� �� Ƽ���� �ϰų�, �ٺ��� ������ �ʿ��� ��
    //                    StopAllCoroutines();
    //                    up = StartCoroutine(SliderGaugeUp());
    //                }
    //                // ������ ��Ȳ�� 1C��� ������ �ٿ�
    //                else if (Poss == Possession.Unit_1C)
    //                {
    //                    StopAllCoroutines();
    //                    down = StartCoroutine(SliderGaugeDown());
    //                }
    //                // ������ ��Ȳ�� 2C��� ��ȭ����
    //                else if (Poss == Possession.Unit_2C)
    //                {
    //                    ;
    //                }
    //            }
    //            // 2P�� ���� ��Ȳ�̾���
    //            else if (OnRegionUnits2P.Count == 0)
    //            {
    //                // 1P�� 100 ä��� ������, ��ȭ����
    //                if (Poss == Possession.Unit_1C)
    //                {
    //                    ;
    //                }
    //                // 1P�� 2C���¿��� ������, 1P�� �ø��� ������ ��ȭ�� �ߴܽ�Ŵ
    //                else if (Poss == Possession.Unit_2C)
    //                {
    //                    StopAllCoroutines();
    //                }

    //                // 1P�� ������ �ø��ٰ� �߰��� ������, �����ϰ� ������ ���� ��Ȳ�̹Ƿ� ������ �ٿ�
    //                else
    //                {
    //                    StopAllCoroutines();
    //                    down = StartCoroutine(SliderGaugeDown());
    //                }
    //            }
    //        }
    //    }
    //    else if (other.CompareTag("Unit 2"))
    //    {
    //        // ...
    //        // 1P�� ���ڸ� �ٲ㼭 �����ϰ� ����
    //    }
    //}

    //// ������ �������� �ӹ��� �� �ϴ� �ൿ
    //private void OnTriggerStay(Collider other)
    //{
    //    // ������ �������� ����
    //    if (Poss == Possession.None)
    //    {
    //        // ���� ������ ���ָ� �����Ҷ� �������� �ø���. �����ǵ� ��ȯ��Ų��.
    //        if (OnRegionUnits1P.Count > 0 && OnRegionUnits2P.Count == 0)
    //        {
    //            Poss = Possession.Unit_1P;
    //            up = StartCoroutine(SliderGaugeUp());
    //        }
    //        else if (OnRegionUnits1P.Count == 0 && OnRegionUnits2P.Count > 0)
    //        {
    //            Poss = Possession.Unit_2P;
    //            up = StartCoroutine(SliderGaugeUp());
    //        }
    //    }
    //}

    //// ���� �������� �ø��� �Լ�
    //IEnumerator SliderGaugeUp()
    //{
    //    // ������ ���� 100 �̸��϶��� �ø���
    //    while (CapturingSlider.value < 100)
    //    {
    //        CapturingSlider.value += Time.deltaTime * 1000f;

    //        // ���� ǥ�� Ȱ��ȭ(��ƼŬ ����Ʈ)
    //        if (CapturingSlider.value >= 100)
    //        {
    //            if (Poss == Possession.Unit_1P)
    //            {
    //                Poss = Possession.Unit_1C;

    //                particles.SetActive(true);
    //                var col = particles.gameObject.GetComponent<ParticleSystem>().colorOverLifetime;
    //                col.color = redGrad;
    //            }
    //            else if (Poss == Possession.Unit_2P)
    //            {
    //                Poss = Possession.Unit_2C;

    //                particles.SetActive(true);
    //                var col = particles.gameObject.GetComponent<ParticleSystem>().colorOverLifetime;
    //                col.color = blueGrad;
    //            }

    //            break;
    //        }

    //        yield return new WaitForSeconds(1f);
    //    }
    //}

    //// ���� �������� ������ �Լ�
    //IEnumerator SliderGaugeDown()
    //{
    //    // ������ ���� 0 �̻��϶��� ������
    //    while (CapturingSlider.value > 0)
    //    {
    //        CapturingSlider.value -= Time.deltaTime * 1000f;

    //        // ���� ǥ�� ��Ȱ��ȭ(��ƼŬ ����Ʈ)
    //        if (CapturingSlider.value <= 0)
    //        {
    //            particles.SetActive(false);
    //            Poss = Possession.None;

    //            break;
    //        }

    //        yield return new WaitForSeconds(1f);
    //    }
    //}
}
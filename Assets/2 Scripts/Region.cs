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

        // 초기화는 다른 방법?
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
                //case Possession.Competing:
                //    if (OnRegionUnits1P.Count > 0 && OnRegionUnits2P.Count == 0)
                //    {
                //        Poss = Possession.Unit_1P;
                //    }

                //    break;
                case Possession.Unit_1P:
                    // 1P->1P 변화없음
                    // 처음으로 올라간녀석이었으면 게이지 업, 왜냐하면 상태에 변화가 생긴 것이므로?...(정확한 설명을 못함)

                    if (OnRegionUnits1P.Count == 1)
                    {
                        StopAllCoroutines();
                        up = StartCoroutine(SliderGaugeUp());
                    }

                    break;
                case Possession.Unit_2P:

                    // 뒤조건 따닥방지
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
                    // 1P만 있으면 상태변경없이 up
                    // 1p 2p 있으면 퍼센트 멈추기만 함, 100프로까지 점령한 어드밴티지를 줌

                    if(OnRegionUnits1P.Count == 1 && OnRegionUnits2P.Count == 0)
                    {
                        StopAllCoroutines();
                        up = StartCoroutine(SliderGaugeUp());
                    } else if(OnRegionUnits1P.Count > 0 && OnRegionUnits2P.Count > 0)
                    {
                        StopAllCoroutines();
                    }



                    // 점수 업

                    


                    break;
                case Possession.Unit_2C:
                    // 퍼센트 떨어뜨리면서 0이면 None으로

                    // 위에 2P가 있으면 2C유지, 2P가 없으면 1P로 바꾸고 게이지 감소 0되면 다시 올라감
                    // Poss = Possession.Unit_1P;

                    // 여기서 상태 바꾸고 떨어뜨리는걸 해야 할듯


                    /* ------------------------- 따닥방지 설명  ------------------------ */
                    // 두번째 OnRegionUnits1P.Count == 1의 조건삽입의 이유는 따닥방지임
                    // 맨 처음으로 들어오는 2P에만 발동하고 따라 들어오는 2P에는 발동하지 않도록 하기 위함임
                    // 주석을 못넣을수도 있는데 Region.cs의 ==1 조건삽입의 이유는 따닥방지 이유임
                    /* ------------------------- 따닥방지 설명  ------------------------ */

                    if (OnRegionUnits2P.Count == 0 && OnRegionUnits1P.Count == 1)
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
                    } else if(OnRegionUnits1P.Count > 0)
                    {
                        StopAllCoroutines();
                    }

                    break;
                case Possession.Unit_2P:
                    // 2P->2P 변화없음

                    if (OnRegionUnits1P.Count == 1)
                    {
                        StopAllCoroutines();
                        up = StartCoroutine(SliderGaugeUp());
                    }

                    break;
                case Possession.Unit_1C:
                    // 퍼센트 떨어뜨리면서 0이면 None으로
                    //Poss = Possession.Unit_2P;
                    //if(OnRegionUnits1P.Count ==0)
                    //{
                    //    Poss = Possession.Unit_1P;
                    //} else if(OnRegionUnits1P.Count > 0)
                    //{
                    //    Poss = Possession.Competing;
                    //}

                    // 1P가 있으면 1C 유지 게이지 다운 안함
                    // 1P가 없으면 gauge down이지만 0까지는 1C유지, 0에 도착하면 2P로 변경


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

                    // 위의 1C와 내용 동일

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
        print("exit");
        //print(OnRegionUnits1P.Count + " " + OnRegionUnits2P.Count);

        if (other.CompareTag("Unit 1"))
        {
            OnRegionUnits1P.Remove(other.gameObject.GetComponent<SelectableUnit>());

            // 1P가 나갔는데
            if(OnRegionUnits1P.Count == 0)
            {
                // 2P가 남아있다
                if (OnRegionUnits2P.Count > 0)
                {
                    //if(Poss == Possession.Competing)
                    //{
                    //    Poss = Possession.Unit_2P;

                    //    StopAllCoroutines();
                    //    up = StartCoroutine(SliderGaugeUp());
                    //}

                    // 1P가 나갈때 1C 상태였다면 게이지는 내리지만 0까지는 1C유지해준다
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
                        // 올라가는중에 1이 들어왔다 빠르게 나갔다고 하면 따닥 같이 슬라이더 한틱이 즉시 상승한다
                        // 이어서 부드럽게 상승하는 방법? +10이 아니고 세세하게 올리면 되려나? 따닥은 불완전함
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



                    // 2P가 남아있지 않다. 아무도 없다
                } else if(OnRegionUnits2P.Count  == 0)
                {
                    // 1P가 100 채우고 나갔다
                    if (Poss == Possession.Unit_1C)
                    {
                    }
                    // 1이 2C상태에서 나갔다
                    else if(Poss == Possession.Unit_2C)
                    {
                        StopAllCoroutines();
                    }

                    // 1P 가 중간에 나갔다
                    else
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

            // 여기도 이상해
            // 2P가 나갔는데
            if (OnRegionUnits2P.Count == 0)
            {
                // 1P가 남아있다
                if (OnRegionUnits1P.Count > 0)
                {

                    //if (Poss == Possession.Competing)
                    //{
                    //    Poss = Possession.Unit_1P;

                    //    StopAllCoroutines();
                    //    up = StartCoroutine(SliderGaugeUp());
                    //}

                    // 2P가 나갈때 2C상태였다면 게이지는 내리지만 0까지는 2C유지해준다

                    if(Poss == Possession.Unit_1P)
                    {
                        StopAllCoroutines();
                        up = StartCoroutine(SliderGaugeUp());
                    } else if(Poss == Possession.Unit_2P)
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    } else if(Poss == Possession.Unit_1C)
                    {
                        ;
                    } else if(Poss == Possession.Unit_2C)
                    {
                        StopAllCoroutines();
                        down = StartCoroutine(SliderGaugeDown());
                    }
                }
                else if (OnRegionUnits1P.Count == 0)
                {

                    // 2가 1C상태에서 나갔다
                    if(Poss == Possession.Unit_1C)
                    {
                        StopAllCoroutines();
                    }
                    // 2P가 100 채우고 나갔다
                    else if (Poss == Possession.Unit_2C)
                    {

                    }
                    // 2P 가 중간에 나갔다
                    else
                    {
                        // 아래 구현할것
                        // 1P가 없을때 2P가 나갔는데 중간에 나간거라면
                        // 1C 였을때 현상유지
                        // 2P 였을때 다시 떨어짐
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

            // 점령 표시 활성화
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
            // 1C 2C 상태에서는 0이 됐을때 파티클을 지워보자
            // 아직 점수는 올라가니까 바로 지우지 않는것
            // 아래 SetActive()는 바로 지우는것
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
            //        // 뭔가 나갔는데 1C이거나 2C이면 0이 될때까지 그 상태를 보존해준다. (점령했던것을 게이지가 0될때까지는 공로를 인정해준다)
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
}
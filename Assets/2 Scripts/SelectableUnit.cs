using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class SelectableUnit : MonoBehaviour
{
    public float range;
    
    [SerializeField] private SpriteRenderer SelectionSprite;
    [SerializeField] private Texture2D attackCursor;
    [SerializeField] private Transform bulletPosition;
    [SerializeField] private GameObject bullet;
    [SerializeField] private RectTransform healthBarGroup;
    [SerializeField] private RectTransform healthBar;

    public NavMeshAgent Agent;

    private float attackDelay;
    private float attackTime;
    public int health;
    private int maxHealth;


    private Vector3 d;

    private void Awake()
    {
        //range = 10f;
        SelectionManager.Instance.AvailableUnits.Add(this);
        Agent = GetComponent<NavMeshAgent>();

        maxHealth = health = 30;
        attackTime = attackDelay = 2f;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, d * 5, Color.yellow);

        AttackTimer();
    }

    private void AttackTimer()
    {
        attackTime += Time.deltaTime;
    }

    public void Attack(Vector3 target, Vector3 dir)
    {
        //GameObject instantBullet = Instantiate(bullet, bulletPosition.position, bullet.transform.rotation);
        //GameObject instantBullet = Instantiate(bullet, bulletPosition.position, bullet.transform.rotation * Quaternion.Euler(dir.x, 0, dir.z));
        //GameObject instantBullet = Instantiate(bullet, transform.position + dir * 2, bullet.transform.rotation * Quaternion.LookRotation(dir));

        //attackTime += Time.deltaTime;
        if (attackTime > attackDelay)
        {
            GameObject instantBullet = Instantiate(bullet, transform.position + dir * 2, Quaternion.LookRotation(dir));
            instantBullet.transform.rotation.SetLookRotation(dir);

            Rigidbody bulletRigidbody = instantBullet.GetComponent<Rigidbody>();

            bulletRigidbody.AddForce(new Vector3(dir.x, 0, dir.z) * 20f, ForceMode.Impulse);
            //bulletRigidbody.velocity = new Vector3(dir.x, 0, dir.z) * 20f;
            //bulletRigidbody.AddForce(target, ForceMode.Impulse);

            d = dir;

            attackTime = 0f;

            print("ATTACK");
        }

    }

    public bool MoveTo(Vector3 Position)
    {
        return Agent.SetDestination(Position);
    }

    public void Onselected()
    {
        SelectionSprite.gameObject.SetActive(true);
    }

    public void OnDeselected()
    {
        SelectionSprite.gameObject.SetActive(false);
    }

    private void OnMouseOver()
    {
        if(SelectionManager.Instance.SelectedUnits.Count > 0)
        {
            Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Unit 1 Bullet") && (gameObject.name.CompareTo("Player 2") == 0 || gameObject.name.CompareTo("Player 2(Clone)") == 0)) {
            Bullet bullet = other.GetComponent<Bullet>();
            health -= bullet.damage;

            // 위에 체력표 바꾸기
            // 체력 감소 애니메이션
            // 체력 회복 방법?
            if (health < maxHealth)
            {
                healthBarGroup.gameObject.SetActive(true);
                healthBar.localScale = new Vector3((health / (float)maxHealth), 1, 1);
            }

            Destroy(other.gameObject);

            if(health <= 0)
            {


                SelectionManager.Instance.AvailableUnits.Remove(this);


                // OntriggerExit() 를 직접 부르는 걸 어떻게 하지
                // 임시방편 처리함
                gameObject.GetComponent<MeshRenderer>().enabled = false;
                gameObject.transform.Translate(new Vector3(1000f, 1000f, 1000f));



                Destroy(gameObject, 0.1f);
            }
        }
    }
}
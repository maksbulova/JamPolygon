using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private SphereCollider senceCol;

    public float maxHealth;
    private float health;
    public float senceRange, senceAngle;
    public float hitRange, hitRate, hitDamage;



    public enum EnemyState
    {
        patrol,
        attack
    }
    public EnemyState state;

    public Transform[] patrolWay;

    private GameObject player;

    private void Start()
    {
        health = maxHealth;

        senceCol = GetComponent<SphereCollider>();
        senceCol.radius = senceRange;

        agent = GetComponent<NavMeshAgent>();

        StartCoroutine(Patrol());
    }

    private void OnValidate()
    {

        GetComponent<SphereCollider>().radius = senceRange;

        // отображает угол обзора
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.blue, 1);
        float radAngle = (senceAngle * Mathf.PI)/ 180;
        Debug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Sin(radAngle), 0, Mathf.Cos(radAngle)) * senceRange, Color.red, 1);
        Debug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Sin(-radAngle), 0, Mathf.Cos(-radAngle)) * senceRange, Color.red, 1);
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerController plr))
        {
            player = plr.gameObject;
            StartCoroutine(CheckDirectView());
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            player = null;

            StopCoroutine(CheckDirectView());
            if (state != EnemyState.patrol)
                StartCoroutine(Patrol());
        }
    }


    public void RecieveDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }


    private IEnumerator CheckDirectView()
    {
        Debug.Log("чек директ вью");
        // пока где-то рядом есть игрок
        while (player != null)
        {
            // луч в сторону игрока, потом проверка в поле ли он зрения и не за препятствием ли
            Vector3 dir = player.transform.position - transform.position;
            Ray ray = new Ray(transform.position, dir);

            // заметил игрока
            // пока преследует игрока обзор шире чтоб совсем нелепых побегов не было
            if (Physics.Raycast(ray, out RaycastHit hit, senceRange) && hit.collider.gameObject == player && (Vector3.Angle(transform.forward, dir) < (state == EnemyState.attack ? senceAngle * 2 : senceAngle)))
            {
                if (state != EnemyState.attack)
                {
                    StartCoroutine(Attack());
                }

            }
            // вне угла зрения или за укрытием
            else
            {
                if (state != EnemyState.patrol)
                {
                    StartCoroutine(Patrol());
                }
            }

            yield return null;
        }
    
    }


    private bool reloaded = true;

    private IEnumerator Reload()
    {
        reloaded = false;  

        yield return new WaitForSeconds(hitRate);

        reloaded = true;
    }

    private IEnumerator Attack()
    {
        Debug.Log("в погоню!");


        StateController plr = player.GetComponent<StateController>();
        // Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        agent.stoppingDistance = hitRange;
        state = EnemyState.attack;
        while (state == EnemyState.attack && (player != null))
        {
            agent.SetDestination(player.transform.position);
            
            if (reloaded && Vector3.Distance(transform.position, player.transform.position) <= hitRange)
            {
                Debug.Log("Удар");
                plr.RecieveDamage(hitDamage);

                StartCoroutine(Reload());

            }

            yield return null;
        }
    }


    private IEnumerator Patrol()
    {
        agent.stoppingDistance = 0;
        Debug.Log("патруль");
        yield return new WaitForSeconds(1);
        state = EnemyState.patrol;
        int i = 0;

        while (state == EnemyState.patrol && patrolWay.Length > 0)
        {
            
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                Transform nextPoint = patrolWay[i % patrolWay.Length];
                agent.SetDestination(nextPoint.position);

                i++;
            }

            yield return null;
        }
    }


    





}

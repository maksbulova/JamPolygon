using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private SphereCollider senceCol;

    public float senceRange, senceAngle, hitRange, hitRate, hitDamage;

    public enum EnemyState
    {
        patrol,
        hunt,
        melee
    }
    public EnemyState state;

    public Transform[] patrolWay;

    private GameObject player;

    private void Start()
    {
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
            if (Physics.Raycast(ray, out RaycastHit hit, senceRange) && hit.collider.gameObject == player && (Vector3.Angle(transform.forward, dir) < (state == EnemyState.hunt ? senceAngle * 2 : senceAngle)))
            {
                if (state != EnemyState.hunt && state != EnemyState.melee)
                {
                    StartCoroutine(RunToPlayer());
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

    private IEnumerator RunToPlayer()
    {
        agent.stoppingDistance = 5;
        Debug.Log("в погоню!");
        state = EnemyState.hunt;
        while (state == EnemyState.hunt && (player != null))
        {
            // Vector3 v = (player.transform.position - transform.position).normalized * 2;
            agent.SetDestination(player.transform.position);
            
            if (Vector3.Distance(transform.position, player.transform.position) <= hitRange)
            {
                state = EnemyState.melee;
                StartCoroutine(Melee());
            }

            yield return null;
        }
    }

    private IEnumerator Melee()
    {
        Debug.Log("меле");
        state = EnemyState.melee;
        float t = hitRate;
        StateController plr = player.GetComponent<StateController>();
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        while (state == EnemyState.melee && player != null && Vector3.Distance(transform.position, player.transform.position) <= hitRange)
        {
            rb.MoveRotation(Quaternion.LookRotation(player.transform.position));
            
            if (t >= hitRate)
            {
                Debug.Log("Удар");
                plr.SetHealth(-hitDamage);
                t = 0;
            }
            t += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(RunToPlayer());

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

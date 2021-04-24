using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private SphereCollider senceCol;

    public float senceRange, senceAngle;

    public enum EnemyState
    {
        patrol,
        attack
        //flee
    }
    public EnemyState state;

    public Transform[] patrolWay;

    private Transform player;

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


        // отображает угол
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.blue, 1);
        float radAngle = (senceAngle * Mathf.PI)/ 180;
        Debug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Sin(radAngle), 0, Mathf.Cos(radAngle)) * senceRange, Color.red, 1);
        Debug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Sin(-radAngle), 0, Mathf.Cos(-radAngle)) * senceRange, Color.red, 1);

    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerController plr))
        {
            Debug.Log("что-то рядом?");
            player = plr.gameObject.transform;
            StartCoroutine(CheckDirectView());
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform == player)
        {
            Debug.Log("показалось");
            player = null;

            StopCoroutine(CheckDirectView());
        }
    }


    private IEnumerator CheckDirectView()
    {
        while (player != null)
        {
            Vector3 dir = player.position - transform.position;
            Ray ray = new Ray(transform.position, dir);

            if(Physics.Raycast(ray, out RaycastHit hit, senceRange))
            {
                // заметил
                if(hit.collider.gameObject == player && (Vector3.Angle(transform.forward, dir) < senceAngle))
                {
                    state = EnemyState.attack;
                    StartCoroutine(Attack());

                }
                // вне поля зрения или за укрытием
                else
                {
                    state = EnemyState.patrol;
                    StartCoroutine(Patrol());

                }
            }

            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        while (state == EnemyState.attack)
        {
            agent.SetDestination(player.position);
            yield return null;
        }
    }

    private IEnumerator Patrol()
    {
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

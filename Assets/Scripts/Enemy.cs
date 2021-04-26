using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(SphereCollider))]
public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private SphereCollider senceCol;

    [Header("Параметры моба")]
    public float maxHealth;
    private float health;
    public float senceRange, senceAngle;
    public float hitRange, hitReload, hitDamage;
    [Header("Дроп хилки (от 1 до 100)")]
    public int dropChanse;
    public GameObject dropLoot;

    public GameObject potBreak;

    private Rigidbody rb;

    public enum EnemyState
    {
        patrol,
        attack
    }
    private EnemyState state;

    public Transform[] patrolWay;

    public event EventHandler HitEvent;

    private GameObject player;

    private void Start()
    {
        health = maxHealth;

        senceCol = GetComponent<SphereCollider>();
        senceCol.isTrigger = true;
        senceCol.radius = senceRange / gameObject.transform.localScale.magnitude * 2;

        agent = GetComponent<NavMeshAgent>();

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        StartCoroutine(Patrol());
    }

    private void OnValidate()
    {
        
        GetComponent<SphereCollider>().radius = senceRange / gameObject.transform.localScale.magnitude * 2; // костыль из-за очень больших 3дмоделей

        // отображает угол обзора
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.blue, 1);
        float radAngle = (senceAngle * Mathf.PI)/ 180;
        Debug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(radAngle), 0, Mathf.Sin(radAngle)) * senceRange, Color.red, 1);
        Debug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(-radAngle), 0, Mathf.Sin(-radAngle)) * senceRange, Color.red, 1);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].otherCollider.GetComponent<Rigidbody>() != null)
        {
            Rigidbody RB = collision.contacts[0].otherCollider.GetComponent<Rigidbody>();
            float hit = RB.velocity.magnitude * RB.mass;

            StartCoroutine(RecieveDamage(hit));
        }
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


    public IEnumerator RecieveDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Death();
        }

        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Renderer>().material.color = Color.white;

    }


    public void Death()
    {
        StopAllCoroutines();

        GameObject pB = Instantiate(potBreak, transform.position, Quaternion.identity);

        if (dropLoot != null && UnityEngine.Random.Range(0, 100) < dropChanse)
        {
            Instantiate(dropLoot, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }


    private IEnumerator CheckDirectView()
    {
        // пока где-то рядом есть игрок
        while (player != null)
        {
            // луч в сторону игрока, потом проверка в поле ли он зрения и не за препятствием ли
            Vector3 dir = player.transform.position - transform.position;
            Ray ray = new Ray(transform.position, dir);

            // заметил игрока
            // пока преследует игрока обзор шире чтоб совсем нелепых побегов не было
            if (Physics.Raycast(ray, out RaycastHit hit, senceRange) && hit.collider.gameObject == player &&  (Vector3.Angle(transform.forward, dir) < (state == EnemyState.attack ? Mathf.Clamp(senceAngle * 2, 0, 360) : senceAngle)))
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

        yield return new WaitForSeconds(hitReload);

        reloaded = true;
    }

    private IEnumerator Attack()
    {
        Debug.Log("Тревога!");



        StateController plr = player.GetComponent<StateController>();
        // Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        agent.stoppingDistance = hitRange;
        state = EnemyState.attack;
        while (state == EnemyState.attack && (player != null))
        {


            agent.SetDestination(player.transform.position);
            
            if (reloaded && Vector3.Distance(transform.position, player.transform.position) <= hitRange)
            {
                // удар

                // HitEvent(this, EventArgs.Empty);
                plr.StartCoroutine(plr.RecieveDamage(hitDamage));

                StartCoroutine(Reload());

            }



            yield return null;
        }
    }


    private IEnumerator Patrol()
    {
        agent.stoppingDistance = 1;
        yield return new WaitForSeconds(1);
        state = EnemyState.patrol;
        int i = 0;

        while (state == EnemyState.patrol && patrolWay.Length > 0)
        {
            
            if (!agent.pathPending && agent.remainingDistance < 1.5f)
            {
                Transform nextPoint = patrolWay[i % patrolWay.Length];
                agent.SetDestination(nextPoint.position);

                i++;
            }

            yield return null;
        }
    }


    





}

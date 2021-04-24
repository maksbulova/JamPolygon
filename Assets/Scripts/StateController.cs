using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    public float maxHealth;
    public float maxFuel;
    [Tooltip("Единиц в секунду")]
    public float fuelConsumption;
    public Light lamp;

    private float health;
    private float fuelLevel;

    private bool lightOn;

    private void Start()
    {
        health = maxHealth;
        fuelLevel = maxFuel;

        lamp.gameObject.SetActive(false);
        lightOn = false;
    }

    private IEnumerator FuelConsumption()
    {
        while (lightOn && fuelLevel > 0)
        {
            fuelLevel -= fuelConsumption;
            yield return new WaitForSeconds(1);
        }
        // если топливо кончилось
        Light(false);
    }

    public void Refuel(float fuel)
    {
        fuelLevel += fuel;
        if (fuelLevel > maxFuel)
        {
            fuelLevel = maxFuel;
        }
    }

    public void Damage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Death();
        }
        else if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    private void Death()
    {
        Debug.Log("Вмер");
        Destroy(gameObject);
    }

    private void Light(bool on)
    {
        lightOn = on;

        if (lightOn)
        {
            if (fuelLevel > 0)
            {
                
                StartCoroutine(FuelConsumption());
            }
            else
            {
                lightOn = false;
            }
        }
        else
        {
            StopCoroutine(FuelConsumption());
        }
        lamp.gameObject.SetActive(lightOn);
    }

    private void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            Light(!lightOn);
        }
    }

}

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

    public float healRate;

    private void Start()
    {
        health = maxHealth;
        fuelLevel = maxFuel;

        lamp.enabled = false;
        lightOn = false;
    }


    // пока фонарь гоит каждую секунду тратится топливо
    private IEnumerator FuelConsumption()
    {
        while (lightOn && fuelLevel > 0)
        {
            fuelLevel -= fuelConsumption;
            yield return new WaitForSeconds(1);
        }
        // если топливо кончилось
        TurnLight(false);
    }


    // заправить фонарь
    public void Refuel(float fuel)
    {
        fuelLevel += fuel;
        if (fuelLevel > maxFuel)
        {
            fuelLevel = maxFuel;
        }
    }

    // сюда и хилка и дамаг, для дамага передается отрицательное значение
    public void SetHealth(float hp)
    {
        health += hp;
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
        Debug.Log("Гоблін вмер");
        Destroy(gameObject);
    }


    // переключение света
    private void TurnLight(bool on)
    {

        if (on && (fuelLevel > 0))
        {
            lightOn = true;
            StartCoroutine(FuelConsumption());
        }
        else
        {
            lightOn = false;
            StopCoroutine(FuelConsumption());
        }
        lamp.enabled = lightOn;
    }

    private void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            TurnLight(!lightOn);
        }

        if (Input.GetKeyDown("q") && medicine && (health < maxHealth))
        {
            medicine = false;
            SetHealth(healRate);
        }
    }


    // проверяется в скрипте двери
    private bool key;
    private bool medicine;

    public void RecieveLoot(Loot item)
    {
        switch (item.item)
        {
            case Loot.LootItem.questKey:
                key = true;
                break;

            case Loot.LootItem.medicine:
                medicine = true;
                break;

            case Loot.LootItem.lampOil:
                Refuel(item.fuelRecover);
                break;

            default:
                break;
        }
    }




}

using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateController : MonoBehaviour
{
    public float maxHealth;
    public float maxFuel;
    [Tooltip("Единиц в секунду")]
    public float fuelConsumption;
    public Light lamp;

    // ключ проверяется в двери
    private int keys;
    private bool medicine;

    private float health;
    private float fuelLevel;

    private bool lightOn;

    public float healRate;

    public Image keyImage, lampImage, medImage;
    public Text keyAmount;

    public event EventHandler DeathEvent;

    private void Start()
    {
        health = maxHealth;
        fuelLevel = maxFuel;

        lamp.enabled = false;
        lightOn = false;

        SetUI();
        StartCoroutine(HelthBar());
        StartCoroutine(FuelBar());
    }


    // пока фонарь гоит каждую секунду тратится топливо
    private IEnumerator FuelConsumption()
    {
        while (lightOn && fuelLevel > 0)
        {
            fuelLevel -= fuelConsumption;
            StartCoroutine(FuelBar());
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
        StartCoroutine(FuelBar());
    }

    // сюда и хилка и дамаг, для хилки передается отрицательное значение
    public IEnumerator RecieveDamage(float dmg)
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

        StartCoroutine(HelthBar());

        if (dmg > 0)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        

        yield return new WaitForSeconds(0.2f);

        GetComponent<Renderer>().material.color = Color.white;

    }

    private void Death()
    {
        Debug.Log("Гоблін вмер");
        // Destroy(gameObject);

        DeathEvent(this, EventArgs.Empty);
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
            StartCoroutine(RecieveDamage(-healRate));
            SetUI(medImage, medicine);
        }
    }



    public void RecieveLoot(Loot item)
    {
        switch (item.item)
        {
            case Loot.LootItem.questKey:
                keys += 1;
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

        SetUI();
    }

    private void SetUI(Image img, bool active)
    {
        if (active)
        {
            img.color = Color.white;
        }
        else
        {
            img.color = new Color(0, 0, 0, 127);
        }

    }

    private void SetUI()
    {
        SetUI(medImage, medicine);
        SetUI(keyImage, (keys > 0));
        keyAmount.text = keys.ToString();
    }

    public AnimationCurve hpCurve;
    public Image hpFull, hpEmpty;
    private IEnumerator HelthBar()
    {
        float oldScaled = hpFull.fillAmount;
        float hpPercent = health / maxHealth;
        float newScaled = hpPercent * 0.6f + 0.2f; 

        for (float t = 0; t < 1; t+=Time.fixedDeltaTime)
        {
            float tempScaled = Mathf.Lerp(oldScaled, newScaled, hpCurve.Evaluate(t));

            hpFull.fillAmount = tempScaled;
            hpEmpty.fillAmount = 1 - tempScaled;

            yield return new WaitForFixedUpdate();
        }

        hpFull.fillAmount = newScaled;
        hpEmpty.fillAmount = 1 - newScaled;

    }

    public Image lampFull, lampEmpty;
    private IEnumerator FuelBar()
    {
        float oldScaled = lampFull.fillAmount;
        float fuelPercent = fuelLevel / maxFuel;
        for (float t = 0; t < 1; t += Time.fixedDeltaTime)
        {
            float tempScaled = Mathf.Lerp(oldScaled, fuelPercent, t);

            lampFull.fillAmount = tempScaled;
            lampEmpty.fillAmount = 1 - tempScaled;

            yield return new WaitForFixedUpdate();
        }

        lampFull.fillAmount = fuelPercent;
        lampEmpty.fillAmount = 1 - fuelPercent;

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            Death();
        }
    }

}

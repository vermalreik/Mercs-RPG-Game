using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    //void Start()
    //{
    //health.transform.localScale = new Vector3(0.5f, 1f);
    // Para testear  la Health Bar la ponemos a la mitad
    // To set the Health to 50%, you just have to change its X scale to 0.5
    //}

    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f, 1f);
        //health.transform.localScale = new Vector3(0.5f, 1f, 1f); // o.O en el video le funciona con los 2 primeros valore pero es un Vector 3, yo le paso 3 valores o si no la z me la hace 0 y no se ve la barra
        // We set the variable hpNormalized as the scale
    }

    public IEnumerator SetHPSmooth(float newHp)
    {
        float curHp = health.transform.localScale.x;
        float changeAmt = curHp - newHp; // Amount of HP that we have to change

        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime; // this way it will only take a small portion of the change amount
            health.transform.localScale = new Vector3(curHp, 1f, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3(newHp, 1f, 1f);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class IndicatorScript : MonoBehaviour
{
    public GameObject debuff0;
    public GameObject debuff1;
    public GameObject debuff2;
    public GameObject debuff3;
    public static int debuffIndicator;

    void Start()
    {
        debuffIndicator = 0;
    }

    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "debuff0")
        {
            debuffIndicator = 1;
        }
        else if (collision.gameObject.name == "debuff1")
        {
            debuffIndicator = 2;
        }
        if (collision.gameObject.name == "debuff2")
        {
            debuffIndicator = 3;
        }
        else if (collision.gameObject.name == "debuff3")
        {
            debuffIndicator = 4;
        }
    }
}

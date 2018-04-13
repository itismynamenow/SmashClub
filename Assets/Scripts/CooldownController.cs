using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CooldownController : MonoBehaviour
{

    public static Image TimerBG;
    public static Image Timer1;
    public static Image Timer2;
    public static Image Timer3;
    public float Cool1;
    public float Cool2;
    public float Cool3;
    static bool Ready1;
    static bool Ready2;
    static bool Ready3;
    private int code;

    // Use this for initialization
    void Start()
    {
        code = Globals.getCharacter();
        TimerBG = GameObject.Find("Canvas").GetComponentInChildren<Transform>().Find("CooldownBG").GetComponentInChildren<Image>();
        Timer1 = TimerBG.transform.Find("Ready1").gameObject.GetComponentInChildren<Image>().transform.Find("Timer").gameObject.GetComponentInChildren<Image>();
        Timer2 = TimerBG.transform.Find("Ready2").gameObject.GetComponentInChildren<Image>().transform.Find("Timer").gameObject.GetComponentInChildren<Image>();
        Timer3 = TimerBG.transform.Find("Ready3").gameObject.GetComponentInChildren<Image>().transform.Find("Timer").gameObject.GetComponentInChildren<Image>();
        Cool1 = GameObject.Find("speed(Clone)").GetComponent<SpeedCharacter>().getDashCool();
        if (code == 1) { Cool1 = GameObject.Find("shoot(Clone)").GetComponent<ShootCharacter>().getDashCool(); }
        else if (code == 2) { Cool1 = GameObject.Find("Feisty(Clone)").GetComponent<FeistyKittyCharacter>().getDashCool(); }
        Cool2 = 2.0f;
        Cool3 = GameObject.Find("speed(Clone)").GetComponent<SpeedCharacter>().getAttackCool();
        if (code == 1) {Cool3 = GameObject.Find("shoot(Clone)").GetComponent<ShootCharacter>().getAttackCool(); }
        else if(code == 2) { Cool3 = GameObject.Find("Feisty(Clone)").GetComponent<FeistyKittyCharacter>().getAttackCool(); }
        Timer1.fillAmount = 0; Ready1 = true;
        Timer2.fillAmount = 0; Ready2 = true;
        Timer3.fillAmount = 0; Ready3 = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Timer1.fillAmount > 0)
        {
            Timer1.fillAmount -= 1.0f / Cool1 * Time.deltaTime;
        }
        else if (Timer1.fillAmount == 0)
        {
            Ready1 = true;
        }
        if (Timer2.fillAmount > 0)
        {
            Timer2.fillAmount -= 1.0f / Cool2 * Time.deltaTime;
        }
        else if (Timer2.fillAmount == 0)
        {
            Ready2 = true;
        }
        if (Timer3.fillAmount > 0)
        {
            Timer3.fillAmount -= 1.0f / Cool3 * Time.deltaTime;
        }
        else if (Timer3.fillAmount == 0)
        {
            Ready3 = true;
        }
        
        if (code == 1) {Cool1 = GameObject.Find("shoot(Clone)").GetComponent<ShootCharacter>().getDashCool();}
        else if (code == 2) {Cool1 = GameObject.Find("Feisty(Clone)").GetComponent<FeistyKittyCharacter>().getDashCool();}
        else {Cool1 = GameObject.Find("speed(Clone)").GetComponent<SpeedCharacter>().getDashCool();}
        Cool2 = 2.0f;
        if (code == 1) {Cool3 = GameObject.Find("shoot(Clone)").GetComponent<ShootCharacter>().getAttackCool();}
        else if (code == 2) {Cool3 = GameObject.Find("Feisty(Clone)").GetComponent<FeistyKittyCharacter>().getAttackCool();}
        else { Cool3 = GameObject.Find("speed(Clone)").GetComponent<SpeedCharacter>().getAttackCool(); }
    }

    public static void Cooldown1()
    {
        Ready1 = false;
        Timer1.fillAmount = 1;
    }
    public static void Cooldown2()
    {
        Ready2 = false;
        Timer2.fillAmount = 1;
    }
    public static void Cooldown3()
    {
        Ready3 = false;
        Timer3.fillAmount = 1;
    }
}


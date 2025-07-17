using UnityEngine;
using System.Collections;

public class DreamToReal : MonoBehaviour
{
    public Dream0Manager Dream0Script;
    public Dream1Manager Dream1Script;
    public Dream2Manager Dream2Script;
    public Dream3Manager Dream3Script;

    void Start()
    {
        StartCoroutine(InitializeScriptsAfterDelay());
    }

    IEnumerator InitializeScriptsAfterDelay()
    {
        yield return new WaitForSeconds(1f); // 他のスクリプトが生成されるのを待つ
        Dream0Script = GameObject.Find("Dream0Manager").GetComponent<Dream0Manager>();
        Dream1Script = GameObject.Find("Dream1Manager").GetComponent<Dream1Manager>();
        Dream2Script = GameObject.Find("Dream2Manager").GetComponent<Dream2Manager>();
        Dream3Script = GameObject.Find("Dream3Manager").GetComponent<Dream3Manager>();
    }

    void Update()
    {
        if (Dream0Script && Dream1Script && Dream2Script && Dream3Script)
        { 
            Dream0();
            Dream1();
            Dream2();
            Dream3();
            Dream4();
            Dream5();
        }
    }

    // スリッパの夢
    // isHot, isIce, isLight
    public GameObject R0StandLight;
    public GameObject R0Heater;
    public GameObject R0Fan;
    //public GameObject R0Character;

    void Dream0()
    {
        // スリッパが燃えると
        if (Dream0Script.isHot)
        {
            // 扇風機 off
            R0Fan.transform.Find("Wind").gameObject.SetActive(false);
            // ヒーター off
            R0Heater.transform.Find("Steam").gameObject.SetActive(false);
            Light HotLight = R0Heater.transform.Find("Pivot").gameObject.GetComponent<Light>();
            HotLight.enabled = false;
            // 汗 on
            //R0Character.transform.Find("Sweat").gameObject.SetActive(true);
        }
        else
        {
            // 汗 off
            //R0Character.transform.Find("Sweat").gameObject.SetActive(false);
        }

        // スリッパが凍ると
        if (Dream0Script.isIce)
        {
            // 扇風機 off
            R0Fan.transform.Find("Wind").gameObject.SetActive(false);
            // ヒーター off
            R0Heater.transform.Find("Steam").gameObject.SetActive(false);
            Light HotLight = R0Heater.transform.Find("Pivot").gameObject.GetComponent<Light>();
            HotLight.enabled = false;
            // 震え on
            //R0Character.transform.Find("Samui").gameObject.SetActive(true);
        }
        else
        {
            // 震え off
            //R0Character.transform.Find("Samui").gameObject.SetActive(false);
        }

        // スリッパが光ると
        if (Dream0Script.isLight)
        {
            // ライト on
            Light light = R0StandLight.transform.Find("Pivot").gameObject.GetComponent<Light>();
            light.enabled = true;
        }
    }

    // お姫様の夢
    // isHot, isIce, isInsect
    public GameObject R1Heater;
    public GameObject R1Fan;
    public GameObject R1Mosquito;
    //public GameObject R1Character;

    void Dream1()
    {
        // 暑い
        if (Dream1Script.isHot)
        {
            // 扇風機 off
            R1Fan.transform.Find("Wind").gameObject.SetActive(false);
            // ヒーター off
            R1Heater.transform.Find("Steam").gameObject.SetActive(false);
            Light HotLight = R1Heater.transform.Find("Pivot").gameObject.GetComponent<Light>();
            HotLight.enabled = false;
            // 汗 on
            //R1Character.transform.Find("Sweat").gameObject.SetActive(true);
        }
        else
        {
            // 汗 off
            //R1Character.transform.Find("Sweat").gameObject.SetActive(false);
        }

        // 寒い
        if (Dream1Script.isIce)
        {
            // 扇風機 off
            R1Fan.transform.Find("Wind").gameObject.SetActive(false);
            // ヒーター off
            R1Heater.transform.Find("Steam").gameObject.SetActive(false);
            Light HotLight = R1Heater.transform.Find("Pivot").gameObject.GetComponent<Light>();
            HotLight.enabled = false;
            // 震え on
            //R1Character.transform.Find("Samui").gameObject.SetActive(true);
        }
        else
        {
            // 震え off
            //R1Character.transform.Find("Samui").gameObject.SetActive(false);
        }

        // 虫
        if (Dream1Script.isInsect)
        {
            R1Mosquito.SetActive(true);
        }
        else
        {
            R1Mosquito.SetActive(false);
        }
    }

    // バス停の夢
    // isHot, isIce, (isBike), isFlower
    public GameObject R2Heater;
    public GameObject R2Fan;
    public GameObject R2Music;
    //public GameObject R2Character;

    void Dream2()
    {
        // 暑い
        if (Dream2Script.isHot)
        {
            // 扇風機 off
            R2Fan.transform.Find("Wind").gameObject.SetActive(false);
            // ヒーター off
            R2Heater.transform.Find("Steam").gameObject.SetActive(false);
            Light HotLight = R2Heater.transform.Find("Pivot").gameObject.GetComponent<Light>();
            HotLight.enabled = false;
            // 汗 on
            //R2Character.transform.Find("Sweat").gameObject.SetActive(true);
        }
        else
        {
            // 汗 off
            //R2Character.transform.Find("Sweat").gameObject.SetActive(false);
        }

        // 寒い
        if (Dream2Script.isIce)
        {
            // 扇風機 off
            R2Fan.transform.Find("Wind").gameObject.SetActive(false);
            // ヒーター off
            R2Heater.transform.Find("Steam").gameObject.SetActive(false);
            Light HotLight = R2Heater.transform.Find("Pivot").gameObject.GetComponent<Light>();
            HotLight.enabled = false;
            // 震え on
            //R2Character.transform.Find("Samui").gameObject.SetActive(true);
        }
        else
        {
            // 震え off
            //R2Character.transform.Find("Samui").gameObject.SetActive(false);
        }

        if (Dream2Script.isFlower)
        {
            // 音楽 on
            AudioSource audio = R2Music.GetComponent<AudioSource>();
            audio.enabled = true;
            // 音符 on
            GameObject notes = R2Music.transform.Find("notes").gameObject;
            notes.SetActive(true);
        }
    }

    // 風船で空を飛ぶ夢
    // isThunder, isSun, isMosquito
    public GameObject R3Heater;
    public GameObject R3Fan;
    public GameObject R3Mosquito;
    //public GameObject R3Character;

    void Dream3()
    {
        // 暑い
        if (Dream3Script.isSun)
        {
            // 扇風機 off
            R3Fan.transform.Find("Wind").gameObject.SetActive(false);
            // ヒーター off
            R3Heater.transform.Find("Steam").gameObject.SetActive(false);
            Light HotLight = R3Heater.transform.Find("Pivot").gameObject.GetComponent<Light>();
            HotLight.enabled = false;
            // 汗 on
            //R3Character.transform.Find("Sweat").gameObject.SetActive(true);
        }
        else
        {
            // 汗 off
            //R3Character.transform.Find("Sweat").gameObject.SetActive(false);
        }

        // 寒い
        if (Dream3Script.isThunder)
        {
            // 扇風機 off
            R3Fan.transform.Find("Wind").gameObject.SetActive(false);
            // ヒーター off
            R3Heater.transform.Find("Steam").gameObject.SetActive(false);
            Light HotLight = R3Heater.transform.Find("Pivot").gameObject.GetComponent<Light>();
            HotLight.enabled = false;
            // 震え on
            //R3Character.transform.Find("Samui").gameObject.SetActive(true);
        }
        else
        {
            // 震え off
            //R3Character.transform.Find("Samui").gameObject.SetActive(false);
        }

        // 虫
        if (Dream3Script.isMosquito)
        {
            R3Mosquito.SetActive(true);
        }
        else
        {
            R3Mosquito.SetActive(false);
        }
    }

    // 宇宙飛行士の夢
    void Dream4()
    {

    }
    
    // サッカー選手の夢
    void Dream5()
    {
        
    }
}

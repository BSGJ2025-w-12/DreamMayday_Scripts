using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class RealToDream : MonoBehaviour
{
    public PlayerControl playerScript;
    public Dream0Manager Dream0Script;
    public Dream1Manager Dream1Script;
    public Dream2Manager Dream2Script;
    public Dream3Manager Dream3Script;
    public int score = 0;

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
        if (playerScript.KilledMosquito[1] && Dream1Script.isInsect)
        {
            Dream1Script.isInsect = false;
            playerScript.KilledMosquito[1] = false;
            score++;
        }
        if (playerScript.KilledMosquito[3] && Dream3Script.isMosquito)
        {
            Dream3Script.isMosquito = false;
            playerScript.KilledMosquito[3] = false;
            score++;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        // performedコールバックだけ受け取る
        if (!context.performed) return;

        Dream0();
        Dream1();
        Dream2();
        Dream3();
        Dream4();
        Dream5();
    }

    // スリッパの夢
    //public CharacterMovementCircle[] characterMovement;
    // isHot, isIce, isLight

    void Dream0()
    {
        // スリッパ鎮火
        if (playerScript.canTurnFan[0] && Dream0Script.isHot)
        {
            //foreach (var chara in characterMovement)
            //{
            //    chara.ResetAnomaly();
            //}
            Dream0Script.isHot = false;
            score++;
        }

        // スリッパ解凍
        if (playerScript.canTurnHot[0] && Dream0Script.isIce)
        {
            //foreach (var chara in characterMovement)
            //{
            //    chara.ResetAnomaly();
            //}
            Dream0Script.isIce = false;
            score++;
        }

        // スリッパ消灯
        if (playerScript.canTurnLight[0] && Dream0Script.isLight)
        {
            //foreach (var chara in characterMovement)
            //{
            //    chara.ResetAnomaly();
            //}
            Dream0Script.isLight = false;
            score++;
        }
    }

    // お姫様の夢
    // isHot, isIce, isInsect
    void Dream1()
    {
        // 暑い解消
        if (playerScript.canTurnFan[1] && Dream1Script.isHot)
        {
            Dream1Script.isHot = false;
            score++;
        }

        // 寒い解消
        if (playerScript.canTurnHot[1] && Dream1Script.isIce)
        {
            Dream1Script.isIce = false;
            score++;
        }
    }

    // バス停の夢
    // isHot, isIce, (isBike), isFlower
    void Dream2()
    {
        // 暑い解消
        if (playerScript.canTurnFan[2] && Dream2Script.isHot)
        {
            Dream2Script.isHot = false;
            score++;
        }

        // 寒い解消
        if (playerScript.canTurnHot[2] && Dream2Script.isIce)
        {
            Dream2Script.isIce = false;
            score++;
        }

        if (playerScript.canTurnMusic[2] && Dream2Script.isFlower)
        {
            Dream2Script.isFlower = false;
            score++;
        }
    }

    // 風船で空を飛ぶ夢
    // isThunder, isSun, isMosquito
    void Dream3()
    {
        // 暑い解消
        if (playerScript.canTurnFan[3] && Dream3Script.isSun)
        {
            Dream3Script.isSun = false;
            score++;
        }

        // 寒い解消
        if (playerScript.canTurnHot[3] && Dream3Script.isThunder)
        {
            Dream3Script.isThunder = false;
            score++;
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

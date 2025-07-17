using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 環境状態の管理を担当するコントローラー。
/// 状態に応じて床・壁・演出・キャラクターを切り替える。
/// </summary>
public class Dream1Manager : MonoBehaviour, IDreamAnomaly
{


    //IDreamAnomaly用の設定1
    //異変の名前
    public string[] GetAnomalyNames()
    {
        return new string[] { "Hot", "Ice", "Insect" };
    }
    //名前によって異変のboolを起こす
    public void SetAnomalyState(string anomalyName, bool value)
    {
        // Debug.Log($"[Dream1Manager] SetAnomalyState called with {anomalyName}, value={value}");

        isHot = false;
        isIce = false;
        isInsect = false;

        switch (anomalyName)
        {
            case "Hot":
                if (value) isHot = true;
                break;
            case "Ice":
                if (value) isIce = true;
                break;
            case "Insect":
                if (value) isInsect = true;
                break;
            case "None":
                break;
            default:
                Debug.LogWarning($"Unknown anomaly name: {anomalyName}");
                return;
        }

        if (isHot)
        {
            SetType(AnormalyType.Hot);
        }
        else if (isIce)
        {
            SetType(AnormalyType.Ice);
        }
        else if (isInsect)
        {
            SetType(AnormalyType.Insect);
        }
        else
        {
            SetType(AnormalyType.Normal);
        }
    }

    //異変が起こっているかのboolを返す
    public bool IsAnomalyActive(string anomalyName)
    {
        switch (anomalyName)
        {
            case "Hot": return isHot;

            case "Ice": return isIce;

            case "Insect": return isInsect;

            default: return false;
        }

    }
    public enum AnormalyType { Normal, Ice, Hot, Insect }  // 異常状態の列挙
    public AnormalyType CurrentState = AnormalyType.Normal; // 現在の状態

    [Header("Base")]
    public GameObject FirePlacesFire; // 暖炉の炎
    public Collider FloorCollider;    // 床のコライダー
    public Collider WallL, WallR, WallB, WallF; // 壁コライダー（左・右・前・後）

    [Header("Ice")]
    public PhysicsMaterial IceMaterial; // 氷の物理マテリアル
    private PhysicsMaterial _originFloorMaterial; // 元の床マテリアル
    private PhysicsMaterial _originWallMaterial;  // 元の壁マテリアル
    public bool isIce = false;

    [Header("Hot")]
    public GameObject RedFilter;  // 赤いフィルター（熱状態）
    public GameObject Trainer;    // トレーナーキャラ
    private Coroutine _redFilterBlinkRoutine;
    private Coroutine _redFilterDelayRoutine;
    private Material _redFilterMaterial;
    public bool isHot = false;


    [Header("Environment")]
    public List<Renderer> FurnitureRenderers = new List<Renderer>();
    public Material NormalMaterial;

    [Header("Visual Materials")]
    public Material IceVMaterial; // 氷状態のビジュアルマテリアル
    private Material _originVFloorMaterial; // 元のビジュアルマテリアル
    private Renderer _floorRenderer;       // 床のRenderer

    [Header("Insects")]
    public List<InsectController> InsectList; // 登場する虫たち
    public PrincessController Pc;             // 主人公キャラ
    public TrainerController Tc;              // トレーナー制御
    public InsectController Ic;               // 主要な虫（演出用）
    public bool isInsect = false;

    [Header("Door")]
    public GameObject DoorObject; // ドア演出オブジェクト
    [SerializeField] private float DoorRotateSpeed = 2f;
    private Vector3 _originPos;
    private Quaternion _closeRotation = Quaternion.Euler(0f, 0f, 0f);
    private Quaternion _openRotation = Quaternion.Euler(0f, 70f, 0f);
    private Coroutine _doorRotateRoutine;
    private Coroutine _closeDoorRoutine;
    private bool _isDoorOpen = false;

    void Start()
    {
        _originPos = transform.position;
        //SetType(CurrentState); // 初期状態セット

        if (FloorCollider != null)
        {
            _originFloorMaterial = FloorCollider.material;
            _floorRenderer = FloorCollider.GetComponent<Renderer>();
            if (_floorRenderer != null)
            {
                _originVFloorMaterial = _floorRenderer.material;
            }
        }
        if (RedFilter != null)
        {
            Renderer r = RedFilter.GetComponent<Renderer>();
            if (r != null)
            {
                _redFilterMaterial = r.material;
            }

        }
        if (WallL != null)
        {
            _originWallMaterial = WallL.material;
        }
    }

    void Update()
    {
        CheckAndFixStateConsistency();
        // Debug.Log($"[STATE] isHot={isHot}, isIce={isIce}, isInsect={isInsect}, Current={CurrentState}");
        // デバッグ用キー操作
        if (Input.GetKeyDown(KeyCode.I))
        {  //SetType(AnormalyType.Ice);
            SetAnomalyState("Ice", true);
        }
        if (Input.GetKeyDown(KeyCode.N))
        //SetType(AnormalyType.Normal);
        {
            SetAnomalyState("None", false);
        }
        if (Input.GetKeyDown(KeyCode.H))
        //SetType(AnormalyType.Hot);
        {
            SetAnomalyState("Hot", true);
        }
        if (Input.GetKeyDown(KeyCode.B))
        //SetType(AnormalyType.Insect);
        {
            SetAnomalyState("Insect", true);
        }

    }
    private void CheckAndFixStateConsistency()
    {
        if (!isHot && !isIce && !isInsect && CurrentState != AnormalyType.Normal)
        {
            SetType(AnormalyType.Normal);
        }
        else if ((isHot && CurrentState != AnormalyType.Hot) ||
                 (isIce && CurrentState != AnormalyType.Ice) ||
                 (isInsect && CurrentState != AnormalyType.Insect))
        {
            if (isHot) SetType(AnormalyType.Hot);
            else if (isIce) SetType(AnormalyType.Ice);
            else if (isInsect) SetType(AnormalyType.Insect);
        }
    }
        /// <summary>
        /// 状態切り替え処理
        /// </summary>
        private void SetType(AnormalyType type)
    {
        


        switch (type)
        {
            case AnormalyType.Normal:
                EnterNormal();
                break;

            case AnormalyType.Ice:
                //isIce = true;
                EnterIce();
                break;

            case AnormalyType.Hot:
                //isHot = true;
                EnterHot();
                break;

            case AnormalyType.Insect:
                //isInsect = true;
                EnterInsect();
                break;
        }

        CurrentState = type;
    }

    private void EnterNormal()
    {

        if (FirePlacesFire != null) FirePlacesFire.SetActive(true);
        if (FloorCollider != null) FloorCollider.material = _originFloorMaterial;
        if (_floorRenderer != null && _originVFloorMaterial != null) _floorRenderer.material = _originVFloorMaterial;
        if (RedFilter != null) RedFilter.SetActive(false);
        StopRedFilterBlink();
        if (WallL != null) WallL.material = _originWallMaterial;
        if (WallR != null) WallR.material = _originWallMaterial;
        if (WallF != null) WallF.material = _originWallMaterial;
        if (WallB != null) WallB.material = _originWallMaterial;
        if (Pc != null) Pc.OnEnterNormal();
        if (Tc != null) Tc.ExitRoom();
        foreach (var insect in InsectList)
        {
            if (insect != null && !insect.gameObject.activeSelf)
                insect.gameObject.SetActive(true);
            insect.ExitRoom();
        }
        TryStartCloseDoor(3f);
        ChangeFurnitureMaterial(NormalMaterial);
    }

    private void EnterHot()
    {
        OpenDoor();
        if (_redFilterDelayRoutine != null)
            StopCoroutine(_redFilterDelayRoutine);
        _redFilterDelayRoutine = StartCoroutine(StartRedFilterBlinkAfterDelay(1.7f));

        if (Trainer != null) Trainer.SetActive(true);
        if (Tc != null && Tc.TSB != null) Tc.TSB.gameObject.SetActive(true);
        if (FirePlacesFire != null) FirePlacesFire.SetActive(true);
        if (FloorCollider != null) FloorCollider.material = _originFloorMaterial;
        if (_floorRenderer != null && _originVFloorMaterial != null) _floorRenderer.material = _originVFloorMaterial;
        if (WallL != null) WallL.material = _originWallMaterial;
        if (WallR != null) WallR.material = _originWallMaterial;
        if (WallF != null) WallF.material = _originWallMaterial;
        if (WallB != null) WallB.material = _originWallMaterial;

        if (Pc != null) Pc.OnEnterHot();
        if (Trainer != null && !Trainer.activeInHierarchy)
            Trainer.SetActive(true);
        if (Tc != null) Tc.EnterRoom();
        foreach (var insect in InsectList)
        {
            if (insect != null && !insect.gameObject.activeSelf)
                insect.gameObject.SetActive(true);
            insect.ExitRoom();
        }
        ChangeFurnitureMaterial(NormalMaterial);
    }

    private void EnterIce()
    {
        if (FirePlacesFire != null) FirePlacesFire.SetActive(false);
        if (FloorCollider != null) FloorCollider.material = IceMaterial;
        if (_floorRenderer != null && IceVMaterial != null) _floorRenderer.material = IceVMaterial;
        if (WallL != null) WallL.material = IceMaterial;
        if (WallR != null) WallR.material = IceMaterial;
        if (WallF != null) WallF.material = IceMaterial;
        if (WallB != null) WallB.material = IceMaterial;
        if (RedFilter != null) RedFilter.SetActive(false);
        StopRedFilterBlink();

        if (Pc != null) Pc.OnEnterIce();
        if (Tc != null) Tc.ExitRoom();
        foreach (var insect in InsectList)
        {
            if (insect != null && !insect.gameObject.activeSelf)
                insect.gameObject.SetActive(true);
            insect.ExitRoom();
        }
        TryStartCloseDoor(3f);
        ChangeFurnitureMaterial(IceVMaterial);
    }

    private void EnterInsect()
    {
        OpenDoor();
        foreach (var insect in InsectList)
        {
            if (insect != null) insect.EnterRoom();
        }
        if (RedFilter != null) RedFilter.SetActive(false);
        StopRedFilterBlink();
        if (FloorCollider != null) FloorCollider.material = _originFloorMaterial;
        if (_floorRenderer != null && _originVFloorMaterial != null) _floorRenderer.material = _originVFloorMaterial;
        if (WallL != null) WallL.material = _originWallMaterial;
        if (WallR != null) WallR.material = _originWallMaterial;
        if (WallF != null) WallF.material = _originWallMaterial;
        if (WallB != null) WallB.material = _originWallMaterial;
        if (Pc != null) Pc.OnEnterInsect();
        if (Ic != null) Ic.EnterRoom();
        ChangeFurnitureMaterial(NormalMaterial);
    }

    private IEnumerator StartDelayInsectEnter()
    {
        yield return new WaitForSeconds(1.5f);
        EnterInsect();
        if (Pc != null) Pc.OnEnterInsect();
        if (Ic != null) Ic.EnterRoom();
    }
    private void ChangeFurnitureMaterial(Material mat)
    {
        foreach (var renderer in FurnitureRenderers)
        {
            if (renderer != null)
                renderer.material = mat;
        }
    }
    private IEnumerator StartDelayTrainerEnter()
    {
        yield return new WaitForSeconds(1.5f);
        EnterHot();
        if (Pc != null) Pc.OnEnterHot();
        if (Trainer != null && !Trainer.activeInHierarchy) Trainer.SetActive(true);
        if (Tc != null) Tc.EnterRoom();
    }
    private void OpenDoor()
    {

        {
            if (DoorObject != null && !_isDoorOpen)
            {
                _isDoorOpen = true;
                if (_doorRotateRoutine != null)
                    StopCoroutine(_doorRotateRoutine);
                _doorRotateRoutine = StartCoroutine(RotateDoor(DoorObject.transform, _openRotation));
            }

        }
    }

    private void TryStartCloseDoor(float delay)
    {
        if (!_isDoorOpen) return;

        if (_closeDoorRoutine != null)
            StopCoroutine(_closeDoorRoutine);

        _closeDoorRoutine = StartCoroutine(CloseDoorAfterDelay(delay));
    }



    private IEnumerator CloseDoorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (DoorObject != null && _isDoorOpen)
        {
            _isDoorOpen = false;
            if (_doorRotateRoutine != null)
                StopCoroutine(_doorRotateRoutine);
            _doorRotateRoutine = StartCoroutine(RotateDoor(DoorObject.transform, _closeRotation));
        }
    }
    private IEnumerator RotateDoor(Transform doorTransform, Quaternion targetRotation)
    {
        while (Quaternion.Angle(doorTransform.rotation, targetRotation) > 0.1f)
        {
            doorTransform.rotation = Quaternion.Lerp(doorTransform.rotation, targetRotation, Time.deltaTime * DoorRotateSpeed);
            yield return null;
        }

        doorTransform.rotation = targetRotation;
    }
    private IEnumerator BlinkRedFilter(float alphaMin = 0.2f, float alphaMax = 0.6f, float speed = 1f)
    {
        if (_redFilterMaterial == null) yield break;

        Color baseColor = _redFilterMaterial.color;

        while (true)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                float a = Mathf.Lerp(alphaMin, alphaMax, t);
                _redFilterMaterial.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
                yield return null;
            }

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                float a = Mathf.Lerp(alphaMax, alphaMin, t);
                _redFilterMaterial.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
                yield return null;
            }
        }
    }

    private IEnumerator StartRedFilterBlinkAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (RedFilter != null)
            RedFilter.SetActive(true);

        StartRedFilterBlink();
    }
    private void StartRedFilterBlink()
    {
        if (_redFilterBlinkRoutine != null)
            StopCoroutine(_redFilterBlinkRoutine);

        if (_redFilterMaterial != null)
        {
            Color color = _redFilterMaterial.color;
            color.a = 0.2f;
            _redFilterMaterial.color = color;
        }

        _redFilterBlinkRoutine = StartCoroutine(BlinkRedFilter());
    }


    private void StopRedFilterBlink()
    {
        if (_redFilterBlinkRoutine != null)
        {
            StopCoroutine(_redFilterBlinkRoutine);
            _redFilterBlinkRoutine = null;
        }

        if (_redFilterMaterial != null)
        {
            Color color = _redFilterMaterial.color;
            color.a = 1f;
            _redFilterMaterial.color = color;
        }
    }

}

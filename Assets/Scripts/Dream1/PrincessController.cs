using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 主人公（プリンセス）の状態と演出制御を行うクラス。
/// 状態によってアニメーションや色、物理挙動、水たまりなどの表示を切り替える。
/// </summary>
public class PrincessController : MonoBehaviour
{
    // プリンセスの状態定義
    public enum PrincessState
    {
        Reading,    // 通常状態（読書）
        Sliding,    // 氷状態（滑り移動）
        Training,   // 熱状態（筋トレ）
        Standing,   // 虫状態時：棒立ち
        Scaring     // 虫状態時：怯えモーション
    }

    public PrincessState CurrentState = PrincessState.Reading; // 現在の状態
    

    [Header("Ice")]
    public float SlideSpeed = 5f;       // 氷状態時の滑る速度
    public float RotateSpeed = 90f;     // 氷状態時の回転速度

    [Header("Hot")]
    public GameObject Mizutamari1;      // 水たまり1段階目（表示タイミングで変化）
    public GameObject Mizutamari2;
    public GameObject Mizutamari3;
    private Coroutine _hotRoutine;       // 水たまりの変化制御Coroutine

    [Header("Normal")]
    private Vector3 _startPos;           // 初期位置
    private Quaternion _startRotation;   // 初期回転
    private Color _startColor;           // 初期のスプライト色

    private Animator _animator;          // アニメーション制御用
    public Rigidbody Rb;                // 主人公の物理挙動
    private SpriteRenderer _sr;          // 見た目の色変更用

    private Coroutine _hotSequenceRoutine;
    private Coroutine _insectSequenceRoutine;

    private void Awake()
    {
        _startPos = transform.position;
        _startRotation = transform.rotation;
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (Rb == null) Rb = GetComponent<Rigidbody>();
        _sr = GetComponent<SpriteRenderer>();
        if (_sr != null) _startColor = _sr.color;
    }

    void Update()
    {
        switch (CurrentState)
        {
            case PrincessState.Reading:
            case PrincessState.Training:
            case PrincessState.Standing:
            case PrincessState.Scaring:
                if (Rb != null)
                {
                    Rb.linearVelocity = Vector3.zero;
                    Rb.angularVelocity = Vector3.zero;
                }
                break;

            case PrincessState.Sliding:
                MaintainSlide();
                break;
        }
    }
    

    void MaintainSlide()
    {
        Rb.linearVelocity = new Vector3(
            Mathf.Sign(Rb.linearVelocity.x) * SlideSpeed,
            Rb.linearVelocity.y,
            Mathf.Sign(Rb.linearVelocity.z) * SlideSpeed);
        transform.Rotate(0f, -RotateSpeed * Time.deltaTime, 0f);
    }

    void TriggerAnim(string trigger)
    {
        _animator.ResetTrigger("Reading");
        _animator.ResetTrigger("Freezing");
        _animator.ResetTrigger("Training");
        _animator.ResetTrigger("Standing");
        _animator.ResetTrigger("Scaring");
        _animator.SetTrigger(trigger);
    }

    private void StopAllRoutines()
    {
        if (_hotRoutine != null)
        {
            StopCoroutine(_hotRoutine);
            _hotRoutine = null;
        }
        if (_hotSequenceRoutine != null)
        {
            StopCoroutine(_hotSequenceRoutine);
            _hotSequenceRoutine = null;
        }
        if (_insectSequenceRoutine != null)
        {
            StopCoroutine(_insectSequenceRoutine);
            _insectSequenceRoutine = null;
        }
    }

    public void OnEnterNormal()
    {
        StopAllRoutines();

        CurrentState = PrincessState.Reading;
        TriggerAnim("Reading");

        if (_sr != null)
            _sr.color = _startColor;

        if (Rb != null)
        {
            Rb.linearVelocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;
        }

        transform.rotation = _startRotation;
        transform.position = _startPos;

        if (Mizutamari1) Mizutamari1.SetActive(false);
        if (Mizutamari2) Mizutamari2.SetActive(false);
        if (Mizutamari3) Mizutamari3.SetActive(false);
        
    }

    public void OnEnterIce()
    {
        StopAllRoutines();

        CurrentState = PrincessState.Sliding;
        TriggerAnim("Freezing");

        if (_sr != null)
            _sr.color = new Color(0.23f, 1f, 1f, 1f);

        if (Mizutamari1) Mizutamari1.SetActive(false);
        if (Mizutamari2) Mizutamari2.SetActive(false);
        if (Mizutamari3) Mizutamari3.SetActive(false);

        Rb.linearVelocity = new Vector3(SlideSpeed, 0f, 0f);
    }

    public void OnEnterHot()
    {
        
        StopAllRoutines();
        CurrentState = PrincessState.Reading;
        TriggerAnim("Reading");

        if (_sr != null)
            _sr.color = _startColor;

        if (Rb != null)
        {
            Rb.linearVelocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;
        }

        transform.rotation = _startRotation;
        transform.position = _startPos;

        _hotRoutine = StartCoroutine(Mizutamari());
        _hotSequenceRoutine = StartCoroutine(EnterHotSequence());
    }

    public void OnEnterInsect()
    {
        StopAllRoutines();

        CurrentState = PrincessState.Reading;
        TriggerAnim("Reading");

        if (_sr != null)
            _sr.color = _startColor;

        if (Rb != null)
        {
            Rb.linearVelocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;
        }

        transform.rotation = _startRotation;
        transform.position = _startPos;

        if (Mizutamari1) Mizutamari1.SetActive(false);
        if (Mizutamari2) Mizutamari2.SetActive(false);
        if (Mizutamari3) Mizutamari3.SetActive(false);

        _insectSequenceRoutine = StartCoroutine(EnterInsectSequence());
      
    }

    IEnumerator EnterHotSequence()
    {
        yield return new WaitForSeconds(1.5f);

        CurrentState = PrincessState.Training;
        TriggerAnim("Training");

        if (Rb != null)
        {
            Rb.linearVelocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;
        }

        transform.rotation = _startRotation;
        transform.position = _startPos;
    }

    IEnumerator EnterInsectSequence()
    {
        yield return new WaitForSeconds(2f);
        CurrentState = PrincessState.Standing;
        TriggerAnim("Standing");
        yield return new WaitForSeconds(1f);

        CurrentState = PrincessState.Scaring;
        TriggerAnim("Scaring");
    }

    IEnumerator Mizutamari()
    {
        yield return new WaitForSeconds(2f);
        if (Mizutamari1) Mizutamari1.SetActive(true);
        if (Mizutamari2) Mizutamari2.SetActive(false);
        if (Mizutamari3) Mizutamari3.SetActive(false);

        yield return new WaitForSeconds(3f);
        if (Mizutamari1) Mizutamari1.SetActive(false);
        if (Mizutamari2) Mizutamari2.SetActive(true);

        yield return new WaitForSeconds(5f);
        if (Mizutamari3) Mizutamari3.SetActive(true);
        if (Mizutamari2) Mizutamari2.SetActive(false);
    }
}
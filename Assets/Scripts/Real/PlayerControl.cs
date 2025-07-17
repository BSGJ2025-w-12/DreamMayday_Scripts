using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement; // 追加

// プレイヤー操作に関するスクリプト
public class PlayerControl : MonoBehaviour
{
    // パブリック変数
    public GameObject Mark; // インタラクトできる時に表示されるマーク
    public GameObject EntranceCamera; // エントランス用カメラ
    public GameObject[] Camera; // 部屋用カメラ
    public GameObject[] Anchor; // 部屋入った時の位置
    public GameObject Timer; // タイマー用オブジェクト
    public bool[] canTurnLight; // ライト操作できるか
    public bool[] canTurnFan; // 扇風機操作できるか
    public bool[] canTurnHot; // ストーブ操作できるか
    public bool[] canTurnMusic; // 音声機器操作できるか
    public bool[] KilledMosquito; // 蚊は〇ね！
    public float playerSpeed; // プレイヤーのスピード
    public float smoothTime;　// 目標値に到達するまでの時間
    public float clapSEDelay = 0.2f; // 手を叩くSEの遅延時間
    public float clapDelay = 0.5f;
    public int NumPressNorth = 5; // NORTHボタンを押してタイトルに戻る回数

    // プライベート変数
    private Room[] room; // 構造体の配列 （CameraとAnchorをまとめる）
    private Animator animator; // プレイヤーのアニメーター
    private SpriteRenderer sprite; // プレイヤーのスプライト
    private Rigidbody2D rb; // プレイヤーのリジットボディ
    private bool[] canGo; // 部屋への移動を管理するbool変数
    private bool canGoEntrance; // エントランスへの移動を管理するbool変数
    private bool isFootstepPlaying = false; // 足音が再生中かどうかを管理する変数
    private float _playerSpeed; // プレイヤーのスピード
    private Vector3 localScale; // プレイヤーの大きさ
    private Timer timer;

    // NORTHボタン連打カウント用
    private int northPressCount = 0;
    private float lastNorthPressTime = 0f;
    private float northPressTimeout = 0.5f; // 1秒以内に10回押す

    // カメラ移動と場所移動を同時に扱うための構造体
    public struct Room
    {
        public GameObject camera; // カメラ
        public GameObject anchor; // 場所
    }



    // 実行時に1回呼び出される
    void Start()
    {
        // コンポーネント取得
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        timer = Timer.GetComponent<Timer>();

        // 部屋の設定
        room = new Room[Camera.Length];
        for (int i = 0; i < room.Length; i++)
        {
            room[i].camera = Camera[i];
            room[i].anchor = Anchor[i];
        }
        // 部屋と番号の割り当てをシャッフル
        ArrayShuffler.Shuffle(room);
        // bool変数の長さ設定
        canTurnLight = new bool[room.Length];
        canTurnFan = new bool[room.Length];
        canTurnHot = new bool[room.Length];
        canTurnMusic = new bool[room.Length];
        KilledMosquito = new bool[room.Length];
        canGo = new bool[room.Length];
        _playerSpeed = playerSpeed;
        localScale = transform.localScale;
    }



    // ---------------
    // プレイヤーの操作
    // ---------------

    // プレイヤーの入力量を保存しておく変数
    private Vector2 _velocity;

    // 移動（Lスティック）
    public void OnMove(InputAction.CallbackContext context)
    {
        // Lスティックの入力量を取得
        Vector2 axis = context.ReadValue<Vector2>();
        // 入力量を保存
        _velocity = new Vector2(axis.x, axis.y);
    }


    // 手を叩く（Westボタン）
    public void OnDash(InputAction.CallbackContext context)
    {
        // performedコールバックだけ受け取る
        if (!context.performed) return;
        // 手を叩いてる途中は手を叩けない
        if (_playerSpeed == 0) return;

        // 押すとダッシュ状態
        //if (context.started)
        //{
        //    _playerSpeed = 20.0f;
        //    Debug.Log("Dash");
        //}
        // 離すと歩き状態
        //else if (context.canceled)
        //{
        //    _playerSpeed = 10.0f;
        //    Debug.Log("Walk");
        //}

        StartCoroutine("Clap");
    }

    IEnumerator Clap()
    {
        // 手を叩いてる途中は動けない
        _playerSpeed = 0f;

        animator.SetTrigger("clap");
        // SEのみ遅延再生
        yield return new WaitForSeconds(clapSEDelay);
        SoundManager.Instance.PlaySE("Clap");

        //1秒停止
        yield return new WaitForSeconds(clapDelay);

        _playerSpeed = playerSpeed;
    }


    // 現在の速度を格納する変数
    private Vector2 currentVelocity;

    // キャラクターの移動
    void FixedUpdate()
    {
        // 入力量に基づいてキャラクターを移動させる
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, _velocity * _playerSpeed, ref currentVelocity, smoothTime);

        // 移動中なら足音ループ再生，停止中なら停止
        if (_velocity.magnitude * _playerSpeed > 0.1f)
        {
            if (!isFootstepPlaying)
            {
                SoundManager.Instance.PlayLoopSE("Step");
                isFootstepPlaying = true;
            }
        }
        else
        {
            if (isFootstepPlaying)
            {
                SoundManager.Instance.StopLoopSE("Step");
                isFootstepPlaying = false;
            }
        }
    }


    // 部屋に入った時の位置を保存しておく変数
    private Vector2 keepPosition;

    // インタラクト（Southボタン）
    public void OnInteract(InputAction.CallbackContext context)
    {
        // performedコールバックだけ受け取る
        if (!context.performed) return;
        // Debug.Log("Interact");

        if(timer.isResult) return; // 結果画面中は何もしない

        // 部屋へ移動
            for (int i = 0; i < canGo.Length; i++)
            {
                if (canGo[i] == true)
                {
                    SoundManager.Instance.PlaySE("Door");
                    // 部屋に入った時の位置を保存
                    keepPosition = transform.position;
                    // プレイヤーを移動
                    transform.position = room[i].anchor.transform.position;
                    // カメラを変更
                    EntranceCamera.SetActive(false);
                    room[i].camera.SetActive(true);
                }
            }

        // エントランスへ移動
        if (canGoEntrance)
        {
            SoundManager.Instance.PlaySE("Door");
            // プレイヤーを移動
            transform.position = keepPosition;
            // カメラを変更
            foreach (var r in room)
            {
                r.camera.SetActive(false);
            }
            EntranceCamera.SetActive(true);
        }

        // ライトのオン・オフ
        for (int i = 0; i < canTurnLight.Length; i++)
        {
            if (canTurnLight[i])
            {
                //ライトコンポーネントを取得
                Light light = target.transform.Find("Pivot").gameObject.GetComponent<Light>();
                // ライトがONなら
                if (light.enabled)
                {
                    SoundManager.Instance.PlaySE("LightSwitch");
                    // ライトを切る
                    light.enabled = false;
                }
            }
        }

        // 扇風機のオン・オフ
        for (int i = 0; i < canTurnFan.Length; i++)
        {
            if (canTurnFan[i])
            {
                SoundManager.Instance.PlaySE("ObjectSwitch");
                // 風の状態反転
                GameObject wind = target.transform.Find("Wind").gameObject;
                wind.SetActive(!wind.activeSelf);
            }
        }

        // 暖炉のオン・オフ
        for (int i = 0; i < canTurnHot.Length; i++)
        {
            if (canTurnHot[i])
            {
                SoundManager.Instance.PlaySE("ObjectSwitch");
                // 風の状態反転
                GameObject steam = target.transform.Find("Steam").gameObject;
                steam.SetActive(!steam.activeSelf);
                // ライトの状態反転
                Light HotLight = target.transform.Find("Pivot").gameObject.GetComponent<Light>();
                HotLight.enabled = !HotLight.enabled;
            }
        }

        // 音声機器のオン・オフ
        for (int i = 0; i < canTurnMusic.Length; i++)
        {
            if (canTurnMusic[i])
            {
                // 接触相手のAudioSourceコンポーネントを取得
                AudioSource audio = target.GetComponent<AudioSource>();
                // audioがONなら
                if (audio.enabled)
                {
                    SoundManager.Instance.PlaySE("LightSwitch");
                    // 音楽を切る
                    audio.enabled = false;
                    // 接触相手の子オブジェクト（音符）を取得して切る
                    GameObject notes = target.transform.Find("notes").gameObject;
                    notes.SetActive(false);
                }
            }
        }

    }



    // ------------------
    // プレイヤーの描画関連
    // ------------------

    void Update()
    {
        // プレイヤーの向き制御
        SetDirection();
        // プレイヤーの描画優先度変更
        sprite.sortingOrder = -(int)transform.position.y;

        if (timer.isResult && isFootstepPlaying) // 結果画面中は足音を鳴らさない
        {
            SoundManager.Instance.StopLoopSE("Step");
            isFootstepPlaying = false;
        }
    }


    // アニメーションの設定
    private void SetDirection()
    {
        // x軸方向の入力 > y軸方向の入力
        if (Mathf.Abs(_velocity.x) > Mathf.Abs(_velocity.y))
        {
            // 右に移動するアニメーション
            if (_velocity.x > 0)
            {
                transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
                animator.SetBool("up", false);
                animator.SetBool("side", true);
                animator.SetBool("down", false);
            }
            // 左に移動するアニメーション
            else if (_velocity.x < 0)
            {
                transform.localScale = new Vector3(localScale.x, localScale.y, localScale.z);
                animator.SetBool("up", false);
                animator.SetBool("side", true);
                animator.SetBool("down", false);
            }
        }
        // x軸方向の入力 < y軸方向の入力
        else if (Mathf.Abs(_velocity.x) < Mathf.Abs(_velocity.y))
        {
            // 上に移動するアニメーション
            if (_velocity.y > 0)
            {
                animator.SetBool("up", true);
                animator.SetBool("side", false);
                animator.SetBool("down", false);
            }
            // 下に移動するアニメーション
            else if (_velocity.y < 0)
            {
                animator.SetBool("up", false);
                animator.SetBool("side", false);
                animator.SetBool("down", true);
            }
        }
        // x軸方向の入力 = y軸方向の入力
        else
        {
            animator.SetBool("up", false);
            animator.SetBool("side", false);
            animator.SetBool("down", false);
        }
    }



    // -------------
    // 当たり判定関連
    // -------------

    // 接触相手のゲームオブジェクトを保存する変数
    private GameObject target;

    // 当たり判定と接触中
    void OnTriggerStay2D(Collider2D other)
    {
        // インタラクト可能かどうかの表示
        Mark.SetActive(true);
        // 接触相手のインスタンスを保存
        target = other.gameObject;

        // ドア
        if (target.tag == "Door")
        {
            if (target.name == "Door to Entrance")
            {
                canGoEntrance = true;
            }
            else
            {
                // 接触相手の名前の番号を取得
                string name = target.name;
                char number = name[name.Length - 1];
                int i = int.Parse(number.ToString());
                canGo[i] = true;
            }
        }
        // ライト
        else if (target.tag == "Light")
        {
            // 接触相手の名前の番号を取得
            string name = target.name;
            char number = name[name.Length - 1];
            int i = int.Parse(number.ToString());
            canTurnLight[i] = true;
            //ライトコンポーネントを取得
            Light light = target.transform.Find("Pivot").gameObject.GetComponent<Light>();
            // ライトがOFFなら
            if (!light.enabled)
            {
                Mark.SetActive(false);
            }
        }
        // 扇風機
        else if (target.tag == "Fan")
        {
            // 接触相手の名前の番号を取得
            string name = target.name;
            char number = name[name.Length - 1];
            int i = int.Parse(number.ToString());
            canTurnFan[i] = true;
        }
        // 暖炉
        else if (target.tag == "Hot")
        {
            // 接触相手の名前の番号を取得
            string name = target.name;
            char number = name[name.Length - 1];
            int i = int.Parse(number.ToString());
            canTurnHot[i] = true;
        }
        // 蓄音機
        else if (target.tag == "Music")
        {
            // 接触相手の名前の番号を取得
            string name = target.name;
            char number = name[name.Length - 1];
            int i = int.Parse(number.ToString());
            canTurnMusic[i] = true;
            // 接触相手のAudioSourceコンポーネントを取得
            AudioSource audio = target.GetComponent<AudioSource>();
            // audioがONなら
            if (!audio.enabled)
            {
                Mark.SetActive(false);
            }
        }
        // 蚊
        else if (target.tag == "Mosquito")
        {
            // 接触相手の名前の番号を取得
            string name = target.name;
            char number = name[name.Length - 1];
            int i = int.Parse(number.ToString());
            KilledMosquito[i] = true;
            target.SetActive(false);
        }
    }


    // 当たり判定から離れた
    void OnTriggerExit2D(Collider2D other)
    {
        // マークを非表示
        Mark.SetActive(false);
        // bool変数をfalseに変更
        canGoEntrance = false;
        for (int i = 0; i < canGo.Length; i++)
        {
            canGo[i] = false;
        }
        for (int i = 0; i < canTurnLight.Length; i++)
        {
            canTurnLight[i] = false;
        }
        for (int i = 0; i < canTurnFan.Length; i++)
        {
            canTurnFan[i] = false;
        }
        for (int i = 0; i < canTurnHot.Length; i++)
        {
            canTurnHot[i] = false;
        }
        for (int i = 0; i < canTurnMusic.Length; i++)
        {
            canTurnMusic[i] = false;
        }
    }

    // NORTHボタン（Input SystemでNorthボタンに割り当ててください）
    public void OnNorth(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float now = Time.time;
            if (now - lastNorthPressTime > northPressTimeout)
            {
                northPressCount = 0; // タイムアウトでリセット
            }
            lastNorthPressTime = now;
            northPressCount++;
            // Debug.Log($"North押下: {northPressCount}");

            if (northPressCount >= NumPressNorth)
            {
                // Debug.Log("Titleシーンに遷移します");
                SceneManager.LoadScene("Title");
                northPressCount = 0;
            }
        }
    }
}

public static class ArrayShuffler
{
    /// <summary>
    /// 配列の要素をランダムにシャッフルします（Fisher-Yatesアルゴリズム）。
    /// </summary>
    /// <typeparam name="T">配列の型</typeparam>
    /// <param name="array">シャッフルしたい配列</param>
    public static void Shuffle<T>(T[] array)
    {
        // Unityのランダム機能を使用
        for (int i = array.Length - 1; i > 0; i--)
        {
            // 0以上i以下のランダムな整数を取得
            int j = Random.Range(0, i + 1);

            // 要素を交換（スワップ）
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
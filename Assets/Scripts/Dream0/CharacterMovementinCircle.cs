using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterMovementCircle : MonoBehaviour
{
    public Dream0Manager Manager;
    // 移動設定
    public Vector3 centerPoint = Vector3.zero; // 円の中心点
    public float radius = 5f; // 円の半径
    public float rotationSpeed = 30f; // 1秒あたりの回転角度（度）
    public float startAngle = 0f; // 開始角度（度）

    public float moveSpeed = 2f; // 円周移動には直接使われないが、一貫性のため残す
    public float jumpHeight = 1f; // ジャンプの高さ
    public float jumpDuration = 0.5f; // ジャンプにかかる時間
    public float minJumpInterval = 2f; // ジャンプの最小間隔
    public float maxJumpInterval = 5f; // ジャンプの最大間隔

    [Header("ビジュアルと回転設定")]
    public Transform spriteVisualsTransform; // InspectorでスプライトのGameObjectをここに割り当てる！
    public Animator animator; // InspectorでスプライトのGameObjectにあるAnimatorをここに割り当てる
    public SpriteRenderer spriteRenderer; // InspectorでスプライトのGameObjectにあるSpriteRendererをここに割り当てる
    public float turnRotationDuration = 0.5f; // 180度回転にかかる時間

    private bool isJumping = false;
    private bool isTurning = false; // 回転アニメーション実行中フラグ

    private float currentAngle = 0f; // 現在の円周上の角度
    private Vector3 previousPosition; // 前フレームの位置を保存し、移動方向計算に使う

    // スプライトのGameObject (spriteVisualsTransform) の現在のY軸回転角度 (0度または180度)
    private float currentVisualYRotationAngle = 0f; 

    // --- 異変関連の追加 ---
    public enum AnomalyType
    {
        None,   // 異変なし
        Fire,   // 火: 移動中止、反転のみ
        Ice,    // 氷: 全て停止
        Light   // 光: 移動速度二倍
    }

    private AnomalyType currentAnomaly = AnomalyType.None; // 現在の異変タイプ
    private float originalRotationSpeed; // 元の回転速度を保存
    // --- 異変関連の追加終わり ---

    void Start()
    {
        
        // currentAngle を開始角度で初期化
        currentAngle = startAngle;

        // 元の回転速度を保存
        originalRotationSpeed = rotationSpeed; 

        // 必要なコンポーネントと参照が全て設定されているか確認
        if (spriteVisualsTransform == null)
        {
            Debug.LogError("Sprite Visuals Transform が割り当てられていません！SpriteRenderer/Animatorを含むGameObjectを割り当ててください。");
            enabled = false;
            return;
        }
        if (animator == null)
        {
            Debug.LogError("Animatorコンポーネントが割り当てられていません！Sprite Visuals GameObjectのAnimatorを割り当ててください。");
            enabled = false;
            return;
        }
        if (spriteRenderer == null)
        {
             Debug.LogError("SpriteRendererコンポーネントが割り当てられていません！Sprite Visuals GameObjectのSpriteRendererを割り当ててください。");
            enabled = false;
            return;
        }

        // 開始角度に基づいて初期位置を設定
        float initialX = centerPoint.x + radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float initialZ = centerPoint.z + radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
        transform.position = new Vector3(initialX, transform.position.y, initialZ);
        previousPosition = transform.position; // 初期位置を保存

        centerPoint = transform.parent.position + transform.parent.up * 0.53f;
        // 初期ビジュアルの向き（回転）を設定
        float initialTangentX;
        if (rotationSpeed > 0) // 反時計回り
        {
            initialTangentX = -Mathf.Sin(currentAngle * Mathf.Deg2Rad);
        }
        else // 時計回り
        {
            initialTangentX = Mathf.Sin(currentAngle * Mathf.Deg2Rad);
        }
        
        // 初期移動が右なら180度、左なら0度に回転（反転条件が逆）
        if (initialTangentX > 0.001f) // 右へ移動
        {
            spriteVisualsTransform.localRotation = Quaternion.Euler(0, 180, 0); // 左向き
            currentVisualYRotationAngle = 180f;
        }
        else if (initialTangentX < -0.001f) // 左へ移動
        {
            spriteVisualsTransform.localRotation = Quaternion.Euler(0, 0, 0); // 右向き
            currentVisualYRotationAngle = 0f;
        }

        StartCoroutine(RandomJumpScheduler());

        // ★変更: ゲーム開始時は通常状態なので IsAnomalyActive を false に設定
        SetAnimatorAnomalyBools(false); 
    }

    void Update()
    {
        // --- 異変の適用ロジック ---
        if (currentAnomaly == AnomalyType.Ice) // 氷: 全て停止
        {
            animator.SetBool("front", false); 
            isJumping = false;
            isTurning = false;
            return; // 以降の処理をスキップ
        }

        // ジャンプ中または回転中、または火の異変（移動中止）の場合は、移動を一時停止
        if (isTurning || isJumping || currentAnomaly == AnomalyType.Fire) // 火: 移動中止
        {
            // 火の異変中は反転のみを許可するために HandleVisualRotation() を呼び出す
            if (currentAnomaly == AnomalyType.Fire && !isTurning) 
            {
                if(!isTurning)
                {
                     float targetYRotation = (currentVisualYRotationAngle == 0f) ? 180f : 0f;
                     StartCoroutine(RotateVisuals(currentVisualYRotationAngle, targetYRotation));
                }
            }

            HandleAnimation(); // 必要であればZ軸アニメーションは継続させる
            return; // 移動、ジャンプ、反転の処理をスキップ
        }
        // --- 異変の適用ロジック終わり ---

        MoveAroundCenter();
        HandleVisualRotation(); // スプライトのGameObjectの回転を処理
        HandleAnimation();
    }

    void MoveAroundCenter()
    {
        previousPosition = transform.position; // 移動前に現在の位置を保存

        currentAngle += rotationSpeed * Time.deltaTime;

        // 角度を0-360度の範囲に保つ
        if (currentAngle >= 360f)
        {
            currentAngle -= 360f;
        }
        else if (currentAngle < 0f)
        {
            currentAngle += 360f;
        }

        // 円周上の新しいXとZ座標を計算
        float x = centerPoint.x + radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float z = centerPoint.z + radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

        // Y座標は現在のY座標を維持するか、地面の高さに固定
        transform.position = new Vector3(x, transform.position.y, z);
    }

    // スプライトのGameObjectのY軸回転を処理する
    void HandleVisualRotation()
    {
        // 現在位置と前の位置の差分（ワールド座標）
        Vector3 worldDelta = transform.position - previousPosition;

        // ローカル空間での移動量を取得
        Vector3 localDelta = transform.InverseTransformDirection(worldDelta);

        // ローカルX軸の移動方向
        float localMovementX = localDelta.x;

        float targetYRotation = currentVisualYRotationAngle; // 初期値：変化なしと仮定
        // X移動に基づいてターゲットの回転角度を決定（反転条件が逆）
        if (localMovementX > 0.001f) // 右へ移動
        {
            targetYRotation = 180f; // 左向き
        }
        else if (localMovementX < -0.001f) // 左へ移動
        {
            targetYRotation = 0f; // 右向き
        }
        
        // ターゲットの回転角度が現在と異なり、かつ回転中でなければ、回転コルーチンを開始
        if (Mathf.Abs(targetYRotation - currentVisualYRotationAngle) > 0.1f && !isTurning)
        {
            StartCoroutine(RotateVisuals(currentVisualYRotationAngle, targetYRotation));
        }
    }

    // スプライトのGameObjectをスムーズに回転させるコルーチン
    IEnumerator RotateVisuals(float startYAngle, float endYAngle)
    {
        isTurning = true; // 回転中フラグを設定
        float timer = 0f;

        Quaternion startRotation = Quaternion.Euler(0, startYAngle, 0);
        Quaternion endRotation = Quaternion.Euler(0, endYAngle, 0);

        while (timer < turnRotationDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / turnRotationDuration;
            // 子オブジェクトなので localRotation を使用
            spriteVisualsTransform.localRotation = Quaternion.Slerp(startRotation, endRotation, progress);
            yield return null;
        }

        // 最終的な回転を正確に設定
        spriteVisualsTransform.localRotation = endRotation;
        currentVisualYRotationAngle = endYAngle; // 現在の角度を更新
        isTurning = false; // 回転中フラグをクリア
    }

    void HandleAnimation()
    {
        if (animator == null) return;

        float currentZMovementDirection = transform.position.z - previousPosition.z;

        // 回転中は移動アニメーションを停止したり、特定のアニメーションを再生したりできる
        if (isTurning)
        {
            // 回転中はZ軸の移動アニメーションを更新しない
            return; 
        }

        // Z軸の移動方向に基づいてアニメーションを適用（条件を反転）
        if (currentZMovementDirection > 0.001f) // Z軸正方向へ移動（奥へ）
        {
            animator.SetBool("front", false); // "front" を false にする
        }
        else if (currentZMovementDirection < -0.001f) // Z軸負方向へ移動（手前へ）
        {
            animator.SetBool("front", true); // "front" を true にする
        }
    }

    IEnumerator RandomJumpScheduler()
    {
        while (true)
        {
            float randomInterval = Random.Range(minJumpInterval, maxJumpInterval);
            yield return new WaitForSeconds(randomInterval);

            // ジャンプ中または回転中でなければジャンプ
            // 氷の異変中はジャンプしない
            if (!isJumping && !isTurning && currentAnomaly != AnomalyType.Ice) 
            {
                StartCoroutine(Jump());
            }
        }
    }

    IEnumerator Jump()
    {
        isJumping = true;
        Vector3 startPosition = transform.position;
        float groundY = startPosition.y; 
        Vector3 apexPosition = new Vector3(startPosition.x, startPosition.y + jumpHeight, startPosition.z);
        
        float timer = 0f;

        // 上昇
        while (timer < jumpDuration / 2f)
        {
            timer += Time.deltaTime;
            float progress = timer / (jumpDuration / 2f);
            transform.position = Vector3.Lerp(startPosition, apexPosition, progress);
            yield return null;
        }

        // 下降
        timer = 0f;
        startPosition = transform.position; // 現在の位置から下降を開始
        Vector3 endPosition = new Vector3(startPosition.x, groundY, startPosition.z); // 目標の地面Y座標

        while (timer < jumpDuration / 2f)
        {
            timer += Time.deltaTime;
            float progress = timer / (jumpDuration / 2f);
            transform.position = Vector3.Lerp(startPosition, endPosition, progress);
            yield return null;
        }

        // 正確な地面の位置に調整
        transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
        isJumping = false;
    }

    // --- 異変を発生させるパブリックメソッド ---

    /// <summary>
    /// 全ての異変効果をリセットし、通常の状態に戻します。
    /// </summary>
    public void ResetAnomaly()
    {
        currentAnomaly = AnomalyType.None;
        rotationSpeed = originalRotationSpeed; // 回転速度を元に戻す
        animator.speed=1; 
        spriteRenderer.material.color=Color.white;
        SetAnimatorAnomalyBools(false); // ★変更: アニメーターのBoolを通常状態に設定
        Debug.Log("異変をリセットしました。");
    }

    /// <summary>
    /// 火の異変を発動します。移動が中止され、反転のみを繰り返します。
    /// </summary>
    public void ActivateFireAnomaly()
    {
        ResetAnomaly(); // 他の異変をリセット
        currentAnomaly = AnomalyType.Fire;
        animator.speed=1; 
        SetAnimatorAnomalyBools(true); // ★変更: アニメーターのBoolを異変状態に設定
        Debug.Log("火の異変が発動しました：移動中止、反転のみ。");
    }

    /// <summary>
    /// 氷の異変を発動します。移動、ジャンプ、反転全てが停止します。
    /// </summary>
    public void ActivateIceAnomaly()
    {
        ResetAnomaly(); // 他の異変をリセット
        currentAnomaly = AnomalyType.Ice;
        // 停止状態にするために、必要であればAnimatorのBoolを操作
        animator.speed=0; 
        spriteRenderer.material.color=Color.cyan;
        SetAnimatorAnomalyBools(true); // ★変更: アニメーターのBoolを異変状態に設定
        Debug.Log("氷の異変が発動しました：全て停止。");
    }

    /// <summary>
    /// 光の異変を発動します。移動速度が二倍になります。
    /// </summary>
    public void ActivateLightAnomaly()
    {
        ResetAnomaly(); // 他の異変をリセット
        currentAnomaly = AnomalyType.Light;
        rotationSpeed = originalRotationSpeed * 2f; // 回転速度を二倍にする
        animator.speed=2; 
        SetAnimatorAnomalyBools(true); // ★変更: アニメーターのBoolを異変状態に設定
        Debug.Log("光の異変が発動しました：移動速度二倍。");
    }

    /// <summary>
    /// AnimatorのIsAnomalyActive boolパラメータを設定します。
    /// </summary>
    /// <param name="isAnomaly">異変がアクティブならtrue、通常時ならfalse</param>
    private void SetAnimatorAnomalyBools(bool isAnomaly) // ★変更: IsNormalパラメータの操作を削除
    {
        if (animator != null)
        {
            animator.SetBool("IsAnomalyActive", isAnomaly);
            // animator.SetBool("IsNormal", !isAnomaly); // <- この行は削除
        }
    }
}
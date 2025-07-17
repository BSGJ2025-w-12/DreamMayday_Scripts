using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// トレーナーのセリフバブル表示制御（ベースキャラバブルから継承）
/// </summary>
public class TrainerSpeechBubble : BaseCharacterSpeechBubble
{
    public bool EnableDebugLogs = true;                  // デバッグ用ログ出力フラグ
    public List<string> HotSpeechLines = new();          // 熱状態時のセリフリスト

    /// <summary>
    /// 現在の状態に対応するセリフを取得（今回は常にHot用）
    /// </summary>
    /// <returns>セリフのリスト</returns>
    protected override List<string> GetCurrentSpeechLines()
    {
        return HotSpeechLines;
    }
}

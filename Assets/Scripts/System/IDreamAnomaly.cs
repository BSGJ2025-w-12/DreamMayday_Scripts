using UnityEngine; // MonoBehaviourを継承しない純粋なインターフェースなので不要ですが、他のスクリプトとの整合性のため残しておきます。

public interface IDreamAnomaly
{
    // このマネージャーが持っている異変の名前のリストを返す
    string[] GetAnomalyNames();

    // 指定された名前の異変を有効/無効にする
    // `anomalyName`でどの異変を操作するかを指定する
    void SetAnomalyState(string anomalyName, bool value);

    // 現在、特定の異変が有効かどうかをチェックする（オプション）
    bool IsAnomalyActive(string anomalyName);
}
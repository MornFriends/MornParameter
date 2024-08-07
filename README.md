# 概要
パラメータを 定数 / カーブ / ランダム で簡単に設定できるようになる自分用ライブラリ

### 動作確認環境

- Unity 2022.3.14f1

### 依存ライブラリ

特になし

# 使い方
```csharp
[SerializeField] private MornParameterFloat _float;
[SerializeField] private MornParameterInt _int;

private void Start()
{
    Debug.Log(_float.Value);
    Debug.Log(_int.Value);
}
```

### 表示例



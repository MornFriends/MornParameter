# 概要
パラメータを 定数 / カーブ / ランダム で簡単に設定できるようになる自分用ライブラリ

### 動作確認環境

- Unity 2022.3.14f1

### 依存ライブラリ

特になし

# 使い方
```csharp
[SerializeField] private MornParameterFloat _float1;
[SerializeField] private MornParameterFloat _float2;
[SerializeField] private MornParameterFloat _float3;
[SerializeField] private MornParameterInt _int1;
[SerializeField] private MornParameterInt _int2;
[SerializeField] private MornParameterInt _int3;

private void Hoge()
{
    Debug.Log(_float1.Value);
    Debug.Log(_float2.Value);
    Debug.Log(_float3.Value);
    Debug.Log(_int1.Value);
    Debug.Log(_int2.Value);
    Debug.Log(_int3.Value);
}
```

### 表示例
![image](https://github.com/user-attachments/assets/5a8bd1a6-7f11-417c-ac36-da22fa34e4ad)

---
name: metamall
description: "MetaMall プロジェクト固有のコード生成スキル。マスター・テーブルのペア生成、UseCase の新規作成、マスターシートのインポート設定を行う。"
---

# MetaMall プロジェクトスキル

---

## 1. マスター・テーブル ペア生成

新しいマスターデータとそのテーブルを作成する。

### ファイル構成

- `Assets/MetaMallData/Master/{Name}Master.cs`
- `Assets/MetaMallData/Master/{Name}Table.cs`

### Master テンプレート

報酬系（RewardType に属するもの）は `RewardBase` を継承する。それ以外は `MasterBase` を継承する。

```csharp
// RewardBase を継承する場合
using System;

[Serializable]
public class {Name}Master : RewardBase
{
}
```

```csharp
// MasterBase を継承する場合
using System;

[Serializable]
public class {Name}Master : MasterBase
{
}
```

### Table テンプレート

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class {Name}Table : ScriptableObject, IReadOnlyList<{Name}Master>
{
    [SerializeField] private List<{Name}Master> rows = new List<{Name}Master>();

    public int Count => rows.Count;
    public {Name}Master this[int index] => rows[index];

    public IEnumerator<{Name}Master> GetEnumerator() => rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
```

### 継承の判断基準

| 条件 | 継承元 |
|------|--------|
| RewardType の enum に含まれる | RewardBase |
| それ以外 | MasterBase |

### 作成後

- `uloop compile` でコンパイル確認
- コンパイルエラーがあれば修正して再コンパイル

---

## 2. UseCase 新規作成

新しい UseCase を作成し、UseCases.cs に登録する。

### ファイル

- 新規: `Assets/MetaMallUseCases/UseCase_{Name}.cs`
- 修正: `Assets/Core/UseCases.cs`

### UseCase テンプレート

```csharp
using UnityEngine;

public class UseCase_{Name} : MonoBehaviour
{
    Main main;

    public void Setup(Main main)
    {
        this.main = main;
    }
}
```

### UseCases.cs への登録手順

1. 適切な BoxGroup にフィールドを追加:
   ```csharp
   [BoxGroup("カテゴリ名")] public UseCase_{Name} {camelName};
   ```

2. `Setup()` メソッド内に初期化を追加:
   ```csharp
   {camelName}.Setup(main);
   ```

3. `#if UNITY_EDITOR` の `GenerateAllUseCases()` に追加:
   ```csharp
   {camelName} = GetOrCreate<UseCase_{Name}>("{Name}", order++);
   ```

### 既存の BoxGroup カテゴリ

- コア / システム / ショップ / メニュー / コンテンツ / キャラクター / アイテム / デバッグ

### 作成後

- `uloop compile` でコンパイル確認

---

## 3. マスターシートインポート設定

Google スプレッドシートからデータをインポートする設定を追加する。

### 仕組み

- `MasterSheetConfig.asset` に `MasterSheetEntry` を追加
- 各エントリは spreadsheetId, gid, outputTable, sheetCategory を持つ
- Unity メニュー `Window > MasterSheet` からインポート実行

### MasterSheetCategory 一覧

| 値 | カテゴリ |
|----|---------|
| 0 | Fighter |
| 1 | Reward |
| 2 | Stage |
| 3 | Story |
| 4 | System |
| 5 | Shop |
| 6 | Content |
| 7 | MainMenu |
| 8 | SubMenu |
| 9 | Localization |

### ImportSettings アセット

- 場所: `Assets/MetaMallData/Master/ImportSettings/`
- Table の ScriptableObject アセット (.asset) をここに配置
- `uloop execute-dynamic-code` で動的に作成可能

---
name: metamall-ui
description: "MetaMall の UI を新規作成するスキル。Overlay かそれ以外かを判断し、スクリプト・プレハブ・シーン登録・フィールド登録まで一貫して行う。"
---

# MetaMall UI 作成スキル

## フロー

1. ユーザーから UI 名と用途を受け取る
2. **Overlay かそうでないか指示がなければ必ずユーザーに聞く**
3. 該当するサブフォルダが存在しない場合はフォルダ名をユーザーに確認する
4. スクリプト作成
5. プレハブ作成（uloop execute-dynamic-code）
6. シーン登録 & Inspector フィールド設定（uloop execute-dynamic-code）
7. `uloop compile` でコンパイル確認

---

## ディレクトリ構成

```
Assets/MetaMallUI/
  Scripts/
    Base/          ← 汎用コンポーネント（MyButton, MyToggle 等）
    Core/          ← MetaMallUI, OverlayUIBase 等
    Common/        ← BadgeUI, RewardIconUI 等
    Menu/          ← MenuUIBase 継承のメニュー系
    Overlay/       ← OverlayUIBase 継承のオーバーレイ系
      Mail/        ← サブフォルダで機能ごとに分ける
      Formation/
      ...
    Character/
  Prefabs/
    Base/          ← 基本プレハブ（MyButton 等）
    Core/          ← OverlayUIBase.prefab 等のベーステンプレート
    Common/
    Menu/
    Overlay/       ← オーバーレイ用プレハブ
      Mail/
      Formation/
      ...
    Character/
    Debug/
```

---

## A. Overlay UI の作成手順

### A-1. スクリプト作成

場所: `Assets/MetaMallUI/Scripts/Overlay/{SubFolder}/{Name}UI.cs`

```csharp
using UnityEngine;

public class {Name}UI : OverlayUIBase
{
}
```

- `OverlayUIBase` を継承する
- namespace なし
- 必要に応じて `OnBeforeShow()`, `OnAfterShow()`, `OnAfterHide()` をオーバーライド

### A-2. プレハブ作成（uloop execute-dynamic-code）

場所: `Assets/MetaMallUI/Prefabs/Overlay/{SubFolder}/{Name}UI.prefab`

**ベースプレハブのバリアントとして生成する:**

```csharp
// Core/OverlayUIBase.prefab をベースにバリアントを作成
var basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MetaMallUI/Prefabs/Core/OverlayUIBase.prefab");
var variant = PrefabUtility.InstantiatePrefab(basePrefab) as GameObject;
variant.name = "{Name}UI";

// スクリプトをアタッチ（OverlayUIBase の代わりに派生クラスを使う）
// 注意: バリアントなので OverlayUIBase は既にある。派生クラスを追加する
variant.AddComponent<{Name}UI>();

// バリアントとして保存
var prefabPath = "Assets/MetaMallUI/Prefabs/Overlay/{SubFolder}/{Name}UI.prefab";
// フォルダが無ければ作成
var dir = System.IO.Path.GetDirectoryName(prefabPath);
if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
AssetDatabase.Refresh();
PrefabUtility.SaveAsPrefabAsset(variant, prefabPath);
DestroyImmediate(variant);
```

### A-3. シーンの OverlayStack 配下に配置（uloop execute-dynamic-code）

```csharp
// OverlayStack を見つける
var overlayStack = FindObjectOfType<OverlayStack>(true);

// プレハブをインスタンス化して OverlayStack の子にする
var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MetaMallUI/Prefabs/Overlay/{SubFolder}/{Name}UI.prefab");
var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, overlayStack.transform);

// RectTransform を画面全体に合わせる
var rt = instance.GetComponent<RectTransform>();
rt.anchorMin = Vector2.zero;
rt.anchorMax = Vector2.one;
rt.sizeDelta = Vector2.zero;
rt.anchoredPosition = Vector2.zero;
rt.localScale = Vector3.one;

EditorUtility.SetDirty(overlayStack);
```

### A-4. OverlayStack にフィールドを追加

`Assets/MetaMallUI/Scripts/Overlay/OverlayStack.cs` に public フィールドを追加:

```csharp
public {Name}UI {camelName}UI;
```

### A-5. Inspector にフィールドを登録（uloop execute-dynamic-code）

```csharp
var overlayStack = FindObjectOfType<OverlayStack>(true);
var ui = overlayStack.GetComponentInChildren<{Name}UI>(true);

// SerializedObject でフィールドにセット
var so = new SerializedObject(overlayStack);
so.FindProperty("{camelName}UI").objectReferenceValue = ui;
so.ApplyModifiedProperties();
EditorUtility.SetDirty(overlayStack);
```

---

## B. 非 Overlay UI の作成手順

### B-1. スクリプト作成

場所: `Assets/MetaMallUI/Scripts/{SubFolder}/{Name}UI.cs`

```csharp
using UnityEngine;

public class {Name}UI : MonoBehaviour
{
}
```

- MonoBehaviour を継承
- namespace なし

### B-2. プレハブ作成（uloop execute-dynamic-code）

場所: `Assets/MetaMallUI/Prefabs/{SubFolder}/{Name}UI.prefab`

```csharp
var go = new GameObject("{Name}UI", typeof(RectTransform));
go.AddComponent<{Name}UI>();

var rt = go.GetComponent<RectTransform>();
rt.anchorMin = Vector2.zero;
rt.anchorMax = Vector2.one;
rt.sizeDelta = Vector2.zero;

var prefabPath = "Assets/MetaMallUI/Prefabs/{SubFolder}/{Name}UI.prefab";
var dir = System.IO.Path.GetDirectoryName(prefabPath);
if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
AssetDatabase.Refresh();
PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
DestroyImmediate(go);
```

### B-3. シーン配置 & フィールド登録

配置先はUIの種類に応じてユーザーに確認する。
プレハブからインスタンス化して親オブジェクトの子にし、参照が必要なコンポーネントの SerializedObject にフィールドをセットする。

---

## Overlay デフォルト設定

- **CloseType**: デフォルトは `Button`
- **Blur**: デフォルトで `useBlur = true`。プレハブには `BlurBackground` コンポーネントを子オブジェクトとして配置し、`blurBackground` フィールドに参照をセットする。ベースプレハブ（OverlayUIBase.prefab）に既に含まれているのでバリアント作成時は自動で引き継がれる
- **TitleText**: プレハブ内の LocalizedText（TitleText）の key に `ui-title-{name}` を設定する（{name} は UI 名を小文字にしたもの。例: GetRewardUI → `ui-title-getreward`）
- **ローカライズ通知**: TitleText の key を設定したら、ユーザーに「ローカライズキー `ui-title-{name}` を作成してください」と伝える
- **操作対象**: Overlay の設定変更は必ず**プレハブ側**で行う。シーン上のインスタンスは直接操作しない（プレハブとの差分が生まれるため）
- **プレハブへの子オブジェクト追加**: `AssetDatabase.LoadAssetAtPath` ではなく `PrefabUtility.LoadPrefabContents` → 編集 → `SaveAsPrefabAsset` → `UnloadPrefabContents` のフローを使う。直接アセット参照に追加しても保存されない

## 表示制御ルール

- オブジェクトのアクティブ状態を外側から操作する際は `SetVisible(bool visible)` メソッドを使う
- 外側から `gameObject.SetActiveIfChanged()` や `gameObject.SetActive()` を直接呼ばない
- 対象クラスに `SetVisible` が無ければ追加する:
```csharp
public void SetVisible(bool visible)
{
    gameObject.SetActiveIfChanged(visible);
}
```
- クラス内部で自身の `gameObject` を操作するのは OK（`SetVisible` 内部など）

## コーディングルール

- **`SetActive` 使用禁止**: `gameObject.SetActive()` を直接呼ばない。代わりに `gameObject.SetActiveIfChanged()` を使う（`GameObjectExtensions` で定義済み）。現在の状態と同じなら何もしないので無駄な再描画を防げる
- **外側からの表示切替**: `SetVisible(bool)` メソッド経由で行う（上記「表示制御ルール」参照）

## 共通ルール

- **コンポーネント順序**: UI スクリプト（〇〇UI）はプレハブの Inspector で**できるだけ上部**に配置する（`ComponentUtility.MoveComponentUp` で RectTransform の直下まで移動）。Overlay に限らず全 UI プレハブ共通のルール
- **スクロール**: ScrollRect を自前で組み立てない。`Prefabs/Base/VerticalScroll.prefab` または `HorizontalScroll.prefab` をインスタンス化して使う。これらには MyScrollRect + ScrollRect + Viewport + Content が既に構成済み
- **フォルダが無い場合**: サブフォルダ名をユーザーに確認してから作成
- **命名規則**: `{Name}UI` （例: ShopDetailUI, MailUI）
- **コンパイル確認**: 全ステップ完了後に `uloop compile` を実行
- **Cell UI**: スクロール内のセルが必要な場合は `{Name}CellUI` を `CellUIBase`（`Scripts/Core/CellUIBase.cs`）を継承して作成する。実装例は `InventoryCellUI`（`Scripts/Overlay/Inventory/InventoryCellUI.cs`）を参照。`CellUIBase` は Title（TMP_Text）、Button（MyButton）、BadgeUI、Background（Image）、選択色（`NormalColor` / `SelectedColor`）を提供する
- **CloseButton**: Overlay には "CloseButton" という名前の MyButton を子に配置すると、Reset() で自動的に Hide() が接続される
- **レイヤー**: UI プレハブ内のすべての GameObject は **UI レイヤー（layer = 5）** に設定する。子オブジェクトを動的に生成する場合も `gameObject.layer = 5` を忘れないこと
- **Canvas Sorting Layer**: Overlay の Canvas の Sorting Layer は **UI** に設定する（Default のままにしない）。`canvas.sortingLayerName = "UI";`

## 既存のサブフォルダ一覧

### Overlay
Mail, Formation, CustomCharacter, Debug, Item, Container, Reward, Inventory

### その他
Base, Core, Common, Menu, Character

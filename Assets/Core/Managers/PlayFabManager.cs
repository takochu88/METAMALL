using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabManager : MonoBehaviour
{
    public bool IsLoggedIn { get; private set; }
    public string PlayFabId { get; private set; }

    public void Login(Action onSuccess = null, Action<string> onError = null)
    {
        var customId = SystemInfo.deviceUniqueIdentifier;

        // Server API でアカウント作成＋ログイン
        var serverRequest = new PlayFab.ServerModels.LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true,
        };

        PlayFabServerAPI.LoginWithCustomID(serverRequest,
            serverResult =>
            {
                // Server ログイン成功後、Client API でもログインしてセッションを取得
                var clientRequest = new LoginWithCustomIDRequest
                {
                    CustomId = customId,
                    CreateAccount = false,
                };

                PlayFabClientAPI.LoginWithCustomID(clientRequest,
                    clientResult =>
                    {
                        IsLoggedIn = true;
                        PlayFabId = clientResult.PlayFabId;
                        Debug.Log($"[PlayFab] ログイン成功: {PlayFabId}");
                        onSuccess?.Invoke();
                    },
                    clientError =>
                    {
                        Debug.LogError($"[PlayFab] Client ログイン失敗: {clientError.GenerateErrorReport()}");
                        onError?.Invoke(clientError.GenerateErrorReport());
                    });
            },
            serverError =>
            {
                Debug.LogError($"[PlayFab] Server ログイン失敗: {serverError.GenerateErrorReport()}");
                onError?.Invoke(serverError.GenerateErrorReport());
            });
    }

    /// <summary>
    /// TitleNews（運営メール）を取得する
    /// </summary>
    public void GetTitleNews(Action<System.Collections.Generic.List<TitleNewsItem>> onSuccess, Action<string> onError = null)
    {
        var request = new GetTitleNewsRequest { Count = 50 };
        PlayFabClientAPI.GetTitleNews(request,
            result =>
            {
                Debug.Log($"[PlayFab] TitleNews 取得成功 ({result.News.Count} 件)");
                onSuccess?.Invoke(result.News);
            },
            error =>
            {
                Debug.LogError($"[PlayFab] TitleNews 取得失敗: {error.GenerateErrorReport()}");
                onError?.Invoke(error.GenerateErrorReport());
            });
    }

    /// <summary>
    /// Title Data を取得する
    /// </summary>
    public void GetTitleData(Action<System.Collections.Generic.Dictionary<string, string>> onSuccess, Action<string> onError = null)
    {
        var request = new GetTitleDataRequest();
        PlayFabClientAPI.GetTitleData(request,
            result =>
            {
                Debug.Log($"[PlayFab] TitleData 取得成功 ({result.Data.Count} 件)");
                onSuccess?.Invoke(result.Data);
            },
            error =>
            {
                Debug.LogError($"[PlayFab] TitleData 取得失敗: {error.GenerateErrorReport()}");
                onError?.Invoke(error.GenerateErrorReport());
            });
    }

    // ── Stability（スタミナ） ────────────────────────────

    /// <summary> スタミナの現在値・上限・回復時間をサーバーから取得する </summary>
    public void FetchStability(Action onSuccess = null, Action<string> onError = null)
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result =>
            {
                string code = StabilityData.CurrencyCode;
                int amount = result.VirtualCurrency.TryGetValue(code, out var val) ? val : 0;
                int max = 100;
                int seconds = 0;

                if (result.VirtualCurrencyRechargeTimes.TryGetValue(code, out var recharge))
                {
                    max = recharge.RechargeMax;
                    seconds = recharge.SecondsToRecharge;
                }

                Mgr.Save.StabilityData.Apply(amount, max, seconds);
                Debug.Log($"[PlayFab] Stability 取得成功: {amount}/{max}");
                onSuccess?.Invoke();
            },
            error =>
            {
                Debug.LogError($"[PlayFab] Stability 取得失敗: {error.GenerateErrorReport()}");
                onError?.Invoke(error.GenerateErrorReport());
            });
    }

    /// <summary> スタミナを消費する </summary>
    public void ConsumeStability(int amount, Action onSuccess = null, Action<string> onError = null)
    {
        var request = new SubtractUserVirtualCurrencyRequest
        {
            VirtualCurrency = StabilityData.CurrencyCode,
            Amount = amount,
        };
        PlayFabClientAPI.SubtractUserVirtualCurrency(request,
            result =>
            {
                Mgr.Save.StabilityData.SetBalance(result.Balance);
                Debug.Log($"[PlayFab] Stability 消費成功: -{amount} → 残{result.Balance}");
                onSuccess?.Invoke();
            },
            error =>
            {
                Debug.LogError($"[PlayFab] Stability 消費失敗: {error.GenerateErrorReport()}");
                onError?.Invoke(error.GenerateErrorReport());
            });
    }

    /// <summary> スタミナを追加する（アイテム回復等） </summary>
    public void AddStability(int amount, Action onSuccess = null, Action<string> onError = null)
    {
        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = StabilityData.CurrencyCode,
            Amount = amount,
        };
        PlayFabClientAPI.AddUserVirtualCurrency(request,
            result =>
            {
                Mgr.Save.StabilityData.SetBalance(result.Balance);
                Debug.Log($"[PlayFab] Stability 追加成功: +{amount} → 残{result.Balance}");
                onSuccess?.Invoke();
            },
            error =>
            {
                Debug.LogError($"[PlayFab] Stability 追加失敗: {error.GenerateErrorReport()}");
                onError?.Invoke(error.GenerateErrorReport());
            });
    }
}

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
}

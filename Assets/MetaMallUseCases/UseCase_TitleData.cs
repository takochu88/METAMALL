using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayFab TitleNews の取得・キャッシュ・パース配布を一元管理する。
/// </summary>
public class UseCase_TitleData : MonoBehaviour
{
    private Main main;
    private readonly List<PlayFab.ClientModels.TitleNewsItem> cachedNews = new();
    private readonly List<Action<IReadOnlyList<PlayFab.ClientModels.TitleNewsItem>>> listeners = new();
    private bool fetched;

    public bool IsFetched => fetched;
    public IReadOnlyList<PlayFab.ClientModels.TitleNewsItem> CachedNews => cachedNews;

    public void Setup(Main main)
    {
        this.main = main;
    }

    /// <summary> Fetch 完了時のリスナーを登録する。 </summary>
    public void AddListener(Action<IReadOnlyList<PlayFab.ClientModels.TitleNewsItem>> listener)
    {
        listeners.Add(listener);
    }

    /// <summary> TitleNews を取得してキャッシュする。ログイン直後に1回呼ぶ。 </summary>
    public void Fetch(Action onComplete = null)
    {
        if (!Mgr.PlayFab.IsLoggedIn)
        {
            Debug.LogWarning("[TitleData] PlayFab 未ログイン");
            onComplete?.Invoke();
            return;
        }

        Mgr.PlayFab.GetTitleNews(
            newsList =>
            {
                cachedNews.Clear();
                cachedNews.AddRange(newsList);
                fetched = true;
                Debug.Log($"[TitleData] 取得成功: {cachedNews.Count} 件");
                NotifyListeners();
                onComplete?.Invoke();
            },
            error =>
            {
                Debug.LogError($"[TitleData] 取得失敗: {error}");
                onComplete?.Invoke();
            });
    }

    /// <summary> キャッシュから指定型の Body をパースして返す。 </summary>
    public List<T> Parse<T>() where T : ServerTitleOneDataBase
    {
        var results = new List<T>();
        foreach (var news in cachedNews)
        {
            var item = TryParse<T>(news);
            if (item != null) results.Add(item);
        }
        return results;
    }

    /// <summary> 手動リフレッシュ </summary>
    public void Refresh(Action onComplete = null)
    {
        fetched = false;
        Fetch(onComplete);
    }

    private void NotifyListeners()
    {
        for (int i = 0; i < listeners.Count; i++)
            listeners[i]?.Invoke(cachedNews);
    }

    private static T TryParse<T>(PlayFab.ClientModels.TitleNewsItem news) where T : ServerTitleOneDataBase
    {
        if (string.IsNullOrEmpty(news.Body)) return null;
        try
        {
            return JsonUtility.FromJson<T>(news.Body);
        }
        catch
        {
            return null;
        }
    }
}

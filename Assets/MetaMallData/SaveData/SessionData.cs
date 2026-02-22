/// <summary>
/// セッション中のみ保持される一時データ（永続化しない）。
/// </summary>
public class SessionData
{
    public int inventoryTypeIndex;
    public int selectedCharacterIndex = -1;
    public int configTypeIndex;
}

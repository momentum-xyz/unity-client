using Cysharp.Threading.Tasks;
using Odyssey;
using UnityEngine;
using Odyssey.Networking;

public interface IMinimapDriver
{
    void ShowHideMinimap(bool show);
    bool IsExpanded();
    void Clear();
}

public class MinimapDriver : MonoBehaviour, IRequiresContext, IMinimapDriver
{
    [Header("References")]
    [SerializeField]
    Minimap Minimap;

    void OnEnable()
    {
        Minimap.Driver = this;

        _c.Get<IWispManager>().OnWispAdded += OnWispAdded;
        _c.Get<IWispManager>().OnWispRemoved += OnWispRemoved;
        _c.Get<ISpawner>().OnPlayerAvatarSpawned += OnPlayerAvatarSpawned;
        _c.Get<ISpawner>().OnObjectSpawned += OnObjectSpwaned;
        _c.Get<ISpawner>().OnBeforeObjectDestroyed += OnBeforeObjectDestroyed;
        _c.Get<IReactBridge>().ToggleMinimap_Event += OnToggleMinimap;
    }


    void OnDisable()
    {
        Minimap.Driver = null;

        _c.Get<IWispManager>().OnWispAdded -= OnWispAdded;
        _c.Get<IWispManager>().OnWispRemoved -= OnWispRemoved;
        _c.Get<ISpawner>().OnPlayerAvatarSpawned -= OnPlayerAvatarSpawned;
        _c.Get<ISpawner>().OnObjectSpawned -= OnObjectSpwaned;
        _c.Get<ISpawner>().OnBeforeObjectDestroyed -= OnBeforeObjectDestroyed;
        _c.Get<IReactBridge>().ToggleMinimap_Event -= OnToggleMinimap;
    }

    public void Init(IMomentumContext context)
    {
        _c = context;
    }

    #region API
    public void ShowHideMinimap(bool show)
    {
        Minimap.ShowHide(show);
    }

    public void TeleportToUser(string id)
    {
        _c.Get<ITeleportSystem>().OnTeleportToUser(id);
    }

    public void TeleportToSpace(string id)
    {
        _c.Get<ITeleportSystem>().OnTeleportToSpace(id);
    }

    public async UniTask<string> GetUsername(string id)
    {
        await UniTask.SwitchToMainThread();
        var userMetadata = await _c.Get<IBackendService>().GetUserData(id);
        await UniTask.SwitchToMainThread();
        return userMetadata.name;
    }

    public void Clear()
    {
        Minimap.Clear();
    }

    #endregion API

    #region Event Handlers
    private void OnPlayerAvatarSpawned(GameObject obj)
    {
        Minimap.PlayerAvatarSpawned(obj);
    }

    private void OnWispAdded(WispData obj)
    {
        Minimap.WispSpawned(obj);
    }

    private void OnWispRemoved(WispData obj)
    {
        Minimap.WispRemoved(obj);
    }

    private void OnObjectSpwaned(WorldObject obj)
    {
        Minimap.WorldObjectSpawned(obj);
    }

    private void OnBeforeObjectDestroyed(WorldObject obj)
    {
        Minimap.WorldObjectDestroyed(obj);
    }

    public bool IsExpanded()
    {
        return Minimap.IsExpanded();
    }

    [ContextMenu("Toggle Minimap")]
    private void OnToggleMinimap()
    {
        Minimap.ToggleVisibility();
    }
    #endregion

    IMomentumContext _c;
}

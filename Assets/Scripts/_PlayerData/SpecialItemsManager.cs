
using UnityEngine;

public class SpecialItemsManager : MonoBehaviour
{
    #region Singleton Syntax

    private static SpecialItemsManager _instance;
    public static SpecialItemsManager Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    public Chests_SO Chests_SO { get { return _chests_SO; } }
    [SerializeField] private Chests_SO _chests_SO;
    public Keys_Shards_Scrolls_SO Keys_Shards_Scrolls_SO { get { return _keys_Shards_Scrolls_SO; } }
    [SerializeField] private Keys_Shards_Scrolls_SO _keys_Shards_Scrolls_SO;

    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            lock (_lock)
            {
                if (Instance == null)
                {
                    _instance = this; // LATER TO ADD DONTDESTROY ON LOAD
                }
            }
        }
    }
}

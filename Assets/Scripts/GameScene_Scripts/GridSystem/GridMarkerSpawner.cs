using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GridMarkerSpawner : MonoBehaviour, ISerializationCallbackReceiver
{
    private static ObjectPool<GridMarker> _pool;

    [SerializeField] private GridMarker _gridMarkerPF;
    private static GridMarker GridMarkerPF;

    public static ObjectPool<GridMarker> Pool
    {
        get
        {
            if (_pool == null)
            {
                _pool = new ObjectPool<GridMarker>(createFunc: CreatePooledItem,
                                                   actionOnGet: GetPooledItem,
                                                   actionOnRelease: ReleasePooledItem,
                                                   actionOnDestroy: DestroyPooledItem, defaultCapacity: 4, maxSize: 10);
            }
            return _pool;
        }
    }

    private static GridMarker CreatePooledItem()
    {
        return Instantiate(GridMarkerPF);
    }

    private static void GetPooledItem (GridMarker gridMarker)
    {
        gridMarker.SubscribeToVerifivationCallback(true);
        gridMarker.gameObject.SetActive(true);
    }

    private static void ReleasePooledItem (GridMarker gridMarker)
    {
        gridMarker.SubscribeToVerifivationCallback(false);
        gridMarker.gameObject.SetActive(false);
    }

    private static void DestroyPooledItem (GridMarker gridMarker)
    {
        Destroy(gridMarker);
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        GridMarkerPF = _gridMarkerPF;
    }
}

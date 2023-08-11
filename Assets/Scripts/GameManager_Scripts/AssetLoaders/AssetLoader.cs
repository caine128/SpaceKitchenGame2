using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class AssetLoader<T_LoaderType, T_AssetType> :Singleton<T_LoaderType>
    where T_LoaderType : class
    where T_AssetType : UnityEngine.Object
{

    private static Dictionary<AssetReferenceT<T_AssetType>, (AsyncOperationHandle<T_AssetType> handle, int count)> handleAndCounts_Dict = new Dictionary<AssetReferenceT<T_AssetType>, (AsyncOperationHandle<T_AssetType>, int)>();
    
    private static Dictionary<AssetReferenceT<T_AssetType>, Queue<Action<T_AssetType>>> queuedLoadRequests_dict = new Dictionary<AssetReferenceT<T_AssetType>, Queue<Action<T_AssetType>>>();

    public  void LoadAdressable(AssetReferenceT<T_AssetType> atlasedSpriteRef_IN, Action<T_AssetType> assetLoadAction) //, Image requestingComponent_IN)
    {
        if (atlasedSpriteRef_IN.RuntimeKeyIsValid() != true)
        {
            Debug.Log("invalid key" + atlasedSpriteRef_IN.RuntimeKey.ToString());
            return;
        }

        
        if (handleAndCounts_Dict.TryGetValue(atlasedSpriteRef_IN, out (AsyncOperationHandle<T_AssetType> handle, int count) handleAndCount))
        {
            handleAndCounts_Dict[atlasedSpriteRef_IN] = (handleAndCount.handle, handleAndCount.count + 1);
            if (handleAndCount.handle.IsValid() && handleAndCount.handle.IsDone)
            {
                switch (handleAndCount.handle.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        assetLoadAction(handleAndCount.handle.Result);
                        break;
                    case AsyncOperationStatus.Failed:
                        Debug.Log("sprite load failed");
                        break;
                    case AsyncOperationStatus.None:
                        break;
                }
            }
            else
            {
                EnqueueLoadRequest(atlasedSpriteRef_IN, assetLoadAction);
            }
        }
        else
        {
            LocateAndLoad(atlasedSpriteRef_IN, assetLoadAction); //requestingComponent_IN);
        }
    }


    private  void EnqueueLoadRequest(AssetReferenceT<T_AssetType> atlasedSpriteRef_IN, Action<T_AssetType> assetLoadAction) //Image requestingComponent_IN)
    {
        if (!queuedLoadRequests_dict.ContainsKey(atlasedSpriteRef_IN))
        {
            queuedLoadRequests_dict[atlasedSpriteRef_IN] = new Queue<Action<T_AssetType>>();
        }

        queuedLoadRequests_dict[atlasedSpriteRef_IN].Enqueue(assetLoadAction);
    }


    private  void LocateAndLoad(AssetReferenceT<T_AssetType> atlasedSpriteRef_IN, Action<T_AssetType> assetLoadAction)//Image requestingComponent_IN)
    {
        var asyncHandle = atlasedSpriteRef_IN.LoadAssetAsync();
        handleAndCounts_Dict[atlasedSpriteRef_IN] = (handle: asyncHandle, count: 1);

        asyncHandle.Completed += (operation) =>
        {
            //assetLoadAction(operation.Result);
            ExecuteLoadAction(assetLoadAction, operation.Result);
            if (queuedLoadRequests_dict.TryGetValue(atlasedSpriteRef_IN, out Queue<Action<T_AssetType>> assetLoadActions) && assetLoadActions.Count > 0)
            {
                while (assetLoadActions.Count > 0)
                {
                    //assetLoadActions.Dequeue()(operation.Result);
                    ExecuteLoadAction(assetLoadActions.Dequeue(), operation.Result);
                }
            }
        };
    }

    protected  void ExecuteLoadAction(Action<T_AssetType> assetLoadAction, T_AssetType asset)
    {
        assetLoadAction(asset);
    }

 
    public  void UnloadAdressable(AssetReferenceT<T_AssetType> atlasedSpriteRef_IN)
    {

        if (handleAndCounts_Dict.ContainsKey(atlasedSpriteRef_IN))
        {
            var previousTuple = handleAndCounts_Dict[atlasedSpriteRef_IN];
            handleAndCounts_Dict[atlasedSpriteRef_IN] = (previousTuple.handle, previousTuple.count - 1);

            if (handleAndCounts_Dict[atlasedSpriteRef_IN].count == 0)
            {
                atlasedSpriteRef_IN.ReleaseAsset();
                handleAndCounts_Dict.Remove(atlasedSpriteRef_IN);

                if (queuedLoadRequests_dict.TryGetValue(atlasedSpriteRef_IN, out Queue<Action<T_AssetType>> assetLoadActions) && assetLoadActions.Count > 0)
                {
                    assetLoadActions.Clear();
                }
            }
        }
        else
        {
            Debug.Log("no adressable reference found to release for " + atlasedSpriteRef_IN);
        }


    }
}


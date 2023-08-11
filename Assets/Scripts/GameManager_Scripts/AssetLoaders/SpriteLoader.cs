using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;



public class SpriteLoader : AssetLoader<SpriteLoader, Sprite>
{
    private SpriteLoader()
    {
    }

    //private static Dictionary<AssetReferenceT<Sprite>, (AsyncOperationHandle<Sprite> handle, int count)> handleAndCounts_Dict = new Dictionary<AssetReferenceT<Sprite>, (AsyncOperationHandle<Sprite>, int)>();
    ////private static Dictionary<AssetReferenceT<Sprite>, AsyncOperationHandle<Sprite>> asyncOperationHandles_dict = new Dictionary<AssetReferenceT<Sprite>, AsyncOperationHandle<Sprite>>();
    ////private static Dictionary<AssetReferenceT<Sprite>, int> asyncOperationHandleCounts_dict = new Dictionary<AssetReferenceT<Sprite>, int>();

    //private static Dictionary<AssetReferenceT<Sprite>, Queue<Action<Sprite>>> queuedLoadRequests_dict = new Dictionary<AssetReferenceT<Sprite>, Queue<Action<Sprite>>> ();

    ////private static Dictionary<AssetReferenceT<Sprite>, Queue<Image>> queuedLoadRequests_dict = new Dictionary<AssetReferenceT<Sprite>, Queue<Image>>();
    ///*public static void ProcessAdressableSprites_Load(this Adressable_SpriteDisplay contentImage_IN , ref AssetReferenceT<Sprite> loadedSpriteRef_IN, ref AssetReferenceT<Sprite> newSpriteRef_IN)
    //{
    //    if(loadedSpriteRef_IN != null && loadedSpriteRef_IN == newSpriteRef_IN)
    //    {
    //        return;
    //    }

    //    else if(loadedSpriteRef_IN != null && loadedSpriteRef_IN != newSpriteRef_IN)
    //    {
    //        UnloadAdressable(loadedSpriteRef_IN);
    //    }
    //    loadedSpriteRef_IN = newSpriteRef_IN;
    //    LoadAdressable(loadedSpriteRef_IN, contentImage_IN.ImageContainer);
    //}*/
    ///*public static void ProcessAdressableSprite_Unload(this Adressable_SpriteDisplay contentImage_IN, ref AssetReferenceT<Sprite> loadedSpriteRef_IN)
    //{
    //    if(contentImage_IN.ImageContainer.sprite != null)
    //    {
    //        contentImage_IN.ImageContainer.sprite = null;
    //        UnloadAdressable(loadedSpriteRef_IN);
    //        loadedSpriteRef_IN = null;
    //    }
    //}*/

    //public static void LoadAdressable(AssetReferenceT<Sprite> atlasedSpriteRef_IN, Action<Sprite> assetLoadAction) //, Image requestingComponent_IN)
    //{
    //    if (atlasedSpriteRef_IN.RuntimeKeyIsValid() != true)
    //    {
    //        Debug.Log("invalid key" + atlasedSpriteRef_IN.RuntimeKey.ToString());
    //        return;
    //    }

    //   /* if (!asyncOperationHandleCounts_dict.ContainsKey(atlasedSpriteRef_IN))
    //    {
    //        asyncOperationHandleCounts_dict.Add(atlasedSpriteRef_IN, 0);
    //    }

    //    asyncOperationHandleCounts_dict[atlasedSpriteRef_IN]++;*/

    //    if (handleAndCounts_Dict.TryGetValue(atlasedSpriteRef_IN, out (AsyncOperationHandle<Sprite> handle, int count) handleAndCount))
    //    {
    //        handleAndCounts_Dict[atlasedSpriteRef_IN] = (handleAndCount.handle, handleAndCount.count + 1);
    //        if (handleAndCount.handle.IsValid() && handleAndCount.handle.IsDone)
    //        {
    //            switch (handleAndCount.handle.Status)
    //            {
    //                case AsyncOperationStatus.Succeeded:
    //                    //LoadFromExistingReference(handle.Result, requestingComponent_IN);
    //                    assetLoadAction(handleAndCount.handle.Result);
    //                    break;
    //                case AsyncOperationStatus.Failed:
    //                    Debug.Log("sprite load failed");
    //                    break;
    //                case AsyncOperationStatus.None:
    //                    break;
    //            }
    //        }
    //        else
    //        {
    //            EnqueueLoadRequest(atlasedSpriteRef_IN, assetLoadAction);
    //        }
    //    }
    //    else
    //    {
    //        LocateAndLoad(atlasedSpriteRef_IN, assetLoadAction); //requestingComponent_IN);
    //    }
    //}


    //private static void EnqueueLoadRequest(AssetReferenceT<Sprite> atlasedSpriteRef_IN,Action<Sprite> assetLoadAction) //Image requestingComponent_IN)
    //{
    //    if (!queuedLoadRequests_dict.ContainsKey(atlasedSpriteRef_IN))
    //    {
    //        queuedLoadRequests_dict[atlasedSpriteRef_IN] = new Queue<Action<Sprite>>();
    //    }

    //    queuedLoadRequests_dict[atlasedSpriteRef_IN].Enqueue(assetLoadAction);
    //    //queuedLoadRequests_dict[atlasedSpriteRef_IN].Enqueue(requestingComponent_IN);
    //}


    //private static void LocateAndLoad(AssetReferenceT<Sprite> atlasedSpriteRef_IN, Action<Sprite> assetLoadAction)//Image requestingComponent_IN)
    //{
    //    var asyncHandle = atlasedSpriteRef_IN.LoadAssetAsync();
    //    handleAndCounts_Dict[atlasedSpriteRef_IN] = (handle: asyncHandle, count: 1);
    //    //asyncOperationHandles_dict[atlasedSpriteRef_IN] = asyncHandle;

    //    asyncHandle.Completed += (operation) =>
    //    {
    //        //LoadFromExistingReference(operation.Result, assetLoadAction);//requestingComponent_IN);
    //        assetLoadAction(operation.Result);
    //        if(queuedLoadRequests_dict.TryGetValue(atlasedSpriteRef_IN, out Queue<Action<Sprite>> assetLoadActions) && assetLoadActions.Count>0)
    //        {
    //            while(assetLoadActions.Count > 0)
    //            {
    //                //LoadFromExistingReference(operation.Result, assetLoadActions.Dequeue());
    //                assetLoadActions.Dequeue()(operation.Result);
    //            }
    //        }
    //    };
    //}
    ///*private static void LoadFromExistingReference(Sprite sprite_IN, Action<Sprite> assetLoadAction)  //Image requestingComponent_IN)
    //{
    //    assetLoadAction(sprite_IN);
    //    //requestingComponent_IN.sprite = sprite_IN;
    //}*/


    //public static void UnloadAdressable(AssetReferenceT<Sprite> atlasedSpriteRef_IN)
    //{

    //    if(handleAndCounts_Dict.ContainsKey(atlasedSpriteRef_IN))
    //    {
    //        //asyncOperationHandleCounts_dict[atlasedSpriteRef_IN] -- ;
    //        var previousTuple = handleAndCounts_Dict[atlasedSpriteRef_IN];
    //        handleAndCounts_Dict[atlasedSpriteRef_IN] = (previousTuple.handle, previousTuple.count - 1);

    //        if (handleAndCounts_Dict[atlasedSpriteRef_IN].count == 0)
    //        {
    //            atlasedSpriteRef_IN.ReleaseAsset();
    //            //Addressables.Release(asyncOperationHandles_dict[atlasedSpriteRef_IN]);
    //            handleAndCounts_Dict.Remove(atlasedSpriteRef_IN);
    //            //asyncOperationHandles_dict.Remove(atlasedSpriteRef_IN);

    //            if(queuedLoadRequests_dict.TryGetValue(atlasedSpriteRef_IN, out Queue<Action<Sprite>> assetLoadActions) && assetLoadActions.Count>0)
    //            {
    //                assetLoadActions.Clear();
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("no adressable reference found to release for " + atlasedSpriteRef_IN);
    //    }


    //}
}

/*public static class SpriteLoader
{
    private static Dictionary<AssetReferenceAtlasedSprite, AsyncOperationHandle<Sprite>> asyncOperationHandles_dict = new Dictionary<AssetReferenceAtlasedSprite, AsyncOperationHandle<Sprite>>();
    private static Dictionary<AssetReferenceAtlasedSprite, int> asyncOperationHandleCounts_dict = new Dictionary<AssetReferenceAtlasedSprite, int>();

    private static Dictionary<AssetReferenceAtlasedSprite, Queue<Image>> queuedLoadRequests_dict = new Dictionary<AssetReferenceAtlasedSprite, Queue<Image>>();


    public static void ProcessAdressableSprites_Load(this Adressable_SpriteDisplay contentImage_IN, ref AssetReferenceAtlasedSprite loadedSpriteRef_IN, ref AssetReferenceAtlasedSprite newSpriteRef_IN)
    {
        if (loadedSpriteRef_IN != null && loadedSpriteRef_IN == newSpriteRef_IN)
        {
            return;
        }

        else if (loadedSpriteRef_IN != null && loadedSpriteRef_IN != newSpriteRef_IN)
        {
            UnloadAdressable(loadedSpriteRef_IN);
        }
        loadedSpriteRef_IN = newSpriteRef_IN;
        LoadAdressable(loadedSpriteRef_IN, contentImage_IN.ImageContainer);
    }

    public static void ProcessAdressableSprite_Unload(this Adressable_SpriteDisplay contentImage_IN, ref AssetReferenceAtlasedSprite loadedSpriteRef_IN)
    {
        if (contentImage_IN.ImageContainer.sprite != null)
        {
            contentImage_IN.ImageContainer.sprite = null;
            UnloadAdressable(loadedSpriteRef_IN);
            loadedSpriteRef_IN = null;
        }
    }

    public static void LoadAdressable(AssetReferenceAtlasedSprite atlasedSpriteRef_IN, Image requestingComponent_IN)
    {
        if (atlasedSpriteRef_IN.RuntimeKeyIsValid() != true)
        {
            Debug.Log("invalid key" + atlasedSpriteRef_IN.RuntimeKey.ToString());
            return;
        }

        if (!asyncOperationHandleCounts_dict.ContainsKey(atlasedSpriteRef_IN))
        {
            asyncOperationHandleCounts_dict.Add(atlasedSpriteRef_IN, 0);
        }

        asyncOperationHandleCounts_dict[atlasedSpriteRef_IN]++;

        if (asyncOperationHandles_dict.TryGetValue(atlasedSpriteRef_IN, out AsyncOperationHandle<Sprite> handle))
        {
            if (handle.IsValid() && handle.IsDone)
            {
                switch (handle.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        LoadFromExistingReference(handle.Result, requestingComponent_IN);
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
                EnqueueLoadRequest(atlasedSpriteRef_IN, requestingComponent_IN);
            }
        }
        else
        {
            LocateAndLoad(atlasedSpriteRef_IN, requestingComponent_IN);
        }
    }


    private static void EnqueueLoadRequest(AssetReferenceAtlasedSprite atlasedSpriteRef_IN, Image requestingComponent_IN)
    {
        if (!queuedLoadRequests_dict.ContainsKey(atlasedSpriteRef_IN))
        {
            queuedLoadRequests_dict[atlasedSpriteRef_IN] = new Queue<Image>();
        }

        queuedLoadRequests_dict[atlasedSpriteRef_IN].Enqueue(requestingComponent_IN);
    }


    private static void LocateAndLoad(AssetReferenceAtlasedSprite atlasedSpriteRef_IN, Image requestingComponent_IN)
    {
        var asyncHandle = atlasedSpriteRef_IN.LoadAssetAsync();
        asyncOperationHandles_dict[atlasedSpriteRef_IN] = asyncHandle;

        asyncHandle.Completed += (operation) =>
        {
            LoadFromExistingReference(operation.Result, requestingComponent_IN);
            if (queuedLoadRequests_dict.TryGetValue(atlasedSpriteRef_IN, out Queue<Image> requestingComponents) && requestingComponents.Count > 0)
            {
                while (requestingComponents.Count > 0)
                {
                    LoadFromExistingReference(operation.Result, requestingComponents.Dequeue());
                }
            }
        };
    }


    private static void LoadFromExistingReference(Sprite sprite_IN, Image requestingComponent_IN)
    {
        requestingComponent_IN.sprite = sprite_IN;
    }


    public static void UnloadAdressable(AssetReferenceAtlasedSprite atlasedSpriteRef_IN)
    {

        if (asyncOperationHandleCounts_dict.ContainsKey(atlasedSpriteRef_IN))
        {
            asyncOperationHandleCounts_dict[atlasedSpriteRef_IN]--;

            if (asyncOperationHandleCounts_dict[atlasedSpriteRef_IN] == 0)
            {
                atlasedSpriteRef_IN.ReleaseAsset();
                //Addressables.Release(asyncOperationHandles_dict[atlasedSpriteRef_IN]);
                asyncOperationHandles_dict.Remove(atlasedSpriteRef_IN);

                if (queuedLoadRequests_dict.TryGetValue(atlasedSpriteRef_IN, out Queue<Image> requestingComponentsQueue) && requestingComponentsQueue.Count > 0)
                {
                    requestingComponentsQueue.Clear();
                }
            }
        }
        else
        {
            Debug.Log("no adressable reference found to release for " + atlasedSpriteRef_IN);
        }


    }*/

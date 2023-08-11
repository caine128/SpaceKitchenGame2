
using UnityEngine;

public class GridMarker : MonoBehaviour, IVerificationCallbackReceiver
{
    private MeshRenderer meshRenderer;
    private static (float x, float z) offset;
    private readonly static (float x, float z) Scale = (.9f,.9f);

    public Vector3 AnchorPosition { get => transform.position; }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        transform.localScale = new Vector3(Scale.x, transform.localScale.y, Scale.z);

        var differenceForEachside = (BuildingGrid.cellSize - Scale.x) / 2;
        offset = ((Scale.x / 2) + differenceForEachside, (Scale.z / 2) + differenceForEachside);
    }

    public void Place(Vector3 position_IN)
    {
        transform.position = new Vector3(position_IN.x + offset.x, transform.position.y, position_IN.z + offset.z);
    }

    public void Subscribe(bool shouldSubscribe)
    {
        if (shouldSubscribe)
            BuildingGrid.Instance.OnValidate += VerificationCallback;
        else
        {
            BuildingGrid.Instance.OnValidate -= VerificationCallback;
        }
    }

    public void VerificationCallback(bool isVerified)
    {
        meshRenderer.material.color = isVerified ? Color.green : Color.red;
    }
}

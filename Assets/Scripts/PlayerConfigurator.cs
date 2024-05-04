using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
// Used for the Hat selection logic
public class PlayerConfigurator : MonoBehaviour
{
    [SerializeField]
    private Transform m_HatAnchor;

    [SerializeField]
    private AssetReference m_HatAssetReference;

    private AsyncOperationHandle<GameObject> m_HatLoadOpHandle;

    void Start()
    {           
        SetHat(string.Format("Hat{0:00}", GameManager.s_ActiveHat));
    }

    public void SetHat(string hatKey)
    {
        if (!m_HatAssetReference.RuntimeKeyIsValid())
        {
            return;
        }

        m_HatLoadOpHandle = m_HatAssetReference.LoadAssetAsync<GameObject>();
        m_HatLoadOpHandle.Completed += OnHatLoadComplete;

    }

    private void OnHatLoadComplete(AsyncOperationHandle<GameObject> asyncOperationHandle)
    {
        if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Instantiate(asyncOperationHandle.Result, m_HatAnchor);
        }
    }

    private void OnDisable()
    {
        m_HatLoadOpHandle.Completed -= OnHatLoadComplete;
    }

}

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceLocations;

// Used for the Hat selection logic
public class PlayerConfigurator : MonoBehaviour
{
    [SerializeField]
    private Transform m_HatAnchor;

    [SerializeField]
    private GameObject m_HatInstance;
    private AsyncOperationHandle<IList<IResourceLocation>> m_HatsLocationsOpHandle;

    private AsyncOperationHandle<GameObject> m_HatLoadOpHandle;

    private List<string> m_Keys = new List<string>() { "Hats", "Fancy" };
    private int m_LastHatIndex = -1; // Variable to store the index of the last selected hat


    void Start()
    {
        m_HatsLocationsOpHandle = Addressables.LoadResourceLocationsAsync(m_Keys, Addressables.MergeMode.Intersection);
        m_HatsLocationsOpHandle.Completed += OnHatLocationsLoadComplete;
    }

    private void LoadInRandomHat(IList<IResourceLocation> resourceLocations)
    {
        int randomIndex;

        // Ensure the new hat is different from the last hat
        do
        {
            randomIndex = Random.Range(0, resourceLocations.Count);
        } while (randomIndex == m_LastHatIndex);

        m_LastHatIndex = randomIndex; // Update the last hat index

        IResourceLocation randomHatPrefab = resourceLocations[randomIndex];

        m_HatLoadOpHandle = Addressables.LoadAssetAsync<GameObject>(randomHatPrefab);
        m_HatLoadOpHandle.Completed += OnHatLoadComplete;
    }

    private void OnHatLocationsLoadComplete(AsyncOperationHandle<IList<IResourceLocation>> asyncOperationHandle)
    {
        Debug.Log("AsyncOperationHandle Status: " + asyncOperationHandle.Status);

        if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            IList<IResourceLocation> results = asyncOperationHandle.Result;
            for (int i = 0; i < results.Count; i++)
            {
                Debug.Log("Hat: " + results[i].PrimaryKey);
            }

            LoadInRandomHat(results);
        }
    }
    private void OnHatLoadComplete(AsyncOperationHandle<GameObject> asyncOperationHandle)
    {
        if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            m_HatInstance = Instantiate(asyncOperationHandle.Result, m_HatAnchor);
        }
    }
    private void OnDisable()
    {
        m_HatLoadOpHandle.Completed -= OnHatLoadComplete;
        m_HatsLocationsOpHandle.Completed -= OnHatLocationsLoadComplete;
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            Destroy(m_HatInstance);

            Addressables.Release(m_HatLoadOpHandle);

            LoadInRandomHat(m_HatsLocationsOpHandle.Result);
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssetBundleLoading : MonoBehaviour
{
    public static AssetBundleLoading instance;
    public AssetBundle assetBundle;
    AsyncOperation async = null;

    private string filePath;
    private string bundle_name = "SpotDifference";

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(this);

    }

    public void CallBundle()
    {
        StartCoroutine(Load_Scene(bundle_name));

    }

    public void UnloadBundle()
    {
        if (assetBundle != null)
        {
            instance = null;
            assetBundle.Unload(true);
            Destroy(this.gameObject);
        }
    }

    private string GetBundleFilePath(string filepath)
    {
#if UNITY_EDITOR
        return filepath = System.IO.Path.Combine(Application.streamingAssetsPath, bundle_name);
#else
        return filepath = System.IO.Path.Combine(Application.persistentDataPath, bundle_name);
#endif
    }

    IEnumerator Load_Scene(string bundle_name)
    {
        yield return new WaitForSeconds(1.25f);
        //GetBundleFilePath(filePath);

#if UNITY_EDITOR
        filePath = System.IO.Path.Combine(Application.streamingAssetsPath, bundle_name);
#else
         filePath = System.IO.Path.Combine(Application.persistentDataPath, bundle_name);
#endif

        Debug.Log("Filepath : " + filePath);
        var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(filePath);

        yield return assetBundleCreateRequest;

        assetBundle = assetBundleCreateRequest.assetBundle;

        async = SceneManager.LoadSceneAsync(assetBundle.GetAllScenePaths()[0]);

        while (!async.isDone)
        {
            yield return null;
        }
    }
}
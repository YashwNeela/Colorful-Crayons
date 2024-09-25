using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssetBundleLoading : MonoBehaviour
{
    public static AssetBundleLoading instance;
    public AssetBundle assetBundle;
    AsyncOperation async = null;

    private string filePath;
    public string bundle_name = "SpotDifference";

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

   
    IEnumerator Load_Scene(string bundle_name)
    {
        yield return new WaitForSeconds(1.25f);
        //GetBundleFilePath(filePath);


        filePath = System.IO.Path.Combine(Application.streamingAssetsPath, bundle_name);


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
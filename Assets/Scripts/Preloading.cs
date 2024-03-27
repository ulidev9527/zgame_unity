using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Preloading : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError(Addressables.RuntimePath);

        var aa = Addressables.LoadAssetAsync<GameObject>("Assets/ZGame/ZGame.prefab");
        aa.Completed += obj =>
        {
            Debug.LogError(obj);
            Debug.LogError(obj.Result);
            if(obj.Result!=null){
                Instantiate(obj.Result);
            }
        };
    }

    // Update is called once per frame
    void Update()
    {

    }
}

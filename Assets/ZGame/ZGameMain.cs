using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame
{
    public class ZGameMain : MonoBehaviour
    {
        public static string Version = "1.0.0";
        private void Awake()
        {
            Debug.Log("ZGame Start");
            Debug.Log("ZGame Version " + Version);
        }

        // Start is called before the first frame update
        void Start()
        {
            gameObject.AddComponent<AssetLoader>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
using BepInEx;
using DevHoldableEngine;
using dingus.Behaviors;
using dingus.Networking;
using GorillaLocomotion.Swimming;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace dingus
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static AssetBundle bundle;
        public static GameObject dingusPrefab;
        public static GameObject localDingus;

        void Start() => Utilla.Events.GameInitialized += Init;

        private void Init(object sender, EventArgs e)
        {
            bundle = LoadAssetBundle("dingus.Resources.dingus");

            dingusPrefab = bundle.LoadAsset<GameObject>("dingus");
            localDingus = Instantiate(dingusPrefab);
            DontDestroyOnLoad(localDingus);

            localDingus.transform.position = new Vector3(-66.4f, 14.5f, -82.5f);

            var holdable = localDingus.AddComponent<DevHoldable>();
            holdable.Rigidbody = localDingus.GetComponent<Rigidbody>();
            holdable.PickUp = true;

            localDingus.AddComponent<DingusInjury>();
            localDingus.AddComponent<RigidbodyWaterInteraction>();
            localDingus.AddComponent<LocalDingus>();

            localDingus.layer = 8;

            gameObject.AddComponent<DingusManager>();

            DingusNetworkManager.SetHasDingus();
        }

        public AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }
    }
}

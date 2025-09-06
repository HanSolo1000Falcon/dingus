using BepInEx;
using DevHoldableEngine;
using dingus.Behaviors;
using dingus.Networking;
using GorillaLocomotion.Swimming;
using System;
using System.IO;
using System.Reflection;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.InputSystem;

namespace dingus
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static AssetBundle bundle;
        public static GameObject dingusPrefab;
        public static GameObject localDingus;
        public static Texture2D dingusIcon;



        private bool showGUI = true;
        private Rect guiRect = new Rect(10, 10, 240, 270);
        private Vector2 dragOffset;
        private bool dragging = false;
        private bool muted = false;
        private Vector3 ogDingusScale;


        void Start() => Utilla.Events.GameInitialized += Init;

        private void Init(object sender, EventArgs e)
        {
            bundle = LoadAssetBundle("dingus.Resources.dingus");
            dingusIcon = LoadTextureFromResource("dingus.Resources.dingusICON.png");


            dingusPrefab = bundle.LoadAsset<GameObject>("dingus");
            localDingus = Instantiate(dingusPrefab);
            DontDestroyOnLoad(localDingus);

            localDingus.transform.position = new Vector3(-66.4f, 14.5f, -82.5f);
            ogDingusScale = localDingus.transform.localScale;
            

            var holdable = localDingus.AddComponent<DevHoldable>();
            holdable.Rigidbody = localDingus.GetComponent<Rigidbody>();
            holdable.PickUp = true;

            localDingus.AddComponent<DingusInjury>();
            localDingus.AddComponent<RigidbodyWaterInteraction>();
            localDingus.AddComponent<LocalDingus>();

            localDingus.layer = 8;

            gameObject.AddComponent<DingusManager>();

            DingusNetworkManager.SetHasDingus();

            ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "dingus.cfg"), true);
            ConfigEntry<bool> dingusAudio = configFile.Bind("Dingus", "Audio", true, "Enable/Disable dingus Audio");

            foreach (var aSrc in localDingus.GetComponentsInChildren<AudioSource>())
            {
                aSrc.enabled = dingusAudio.Value;
                muted = !dingusAudio.Value;
            }
        }

        public AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }
        
        Texture2D LoadTextureFromResource(string resourcePath)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
            {
                if (stream == null) return null;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(buffer);
                return tex;
            }
        }

        internal void Update()
        {
            if (Keyboard.current.insertKey.wasPressedThisFrame)
                showGUI = !showGUI;
        }
        void OnGUI()
        {
            if (!showGUI) return;

            GUI.backgroundColor = Color.blue;
            guiRect = GUI.Window(41, guiRect, DrawWindow, "Dingus Controller");

            if (Event.current.type == EventType.MouseDown && guiRect.Contains(Event.current.mousePosition))
            {
                dragOffset = Event.current.mousePosition - new Vector2(guiRect.x, guiRect.y);
                dragging = true;
            }

            if (dragging && Event.current.type == EventType.MouseDrag)
            {
                guiRect.position = Event.current.mousePosition - dragOffset;
                Event.current.Use();
            }
        }

        void DrawWindow(int windowID)
        {
            if (dingusIcon != null)
            {
                GUI.DrawTexture(new Rect(-15, 20, 280, 100), dingusIcon);
            }

            GUI.backgroundColor = Color.green;
            if (GUI.Button(new Rect(10, 120, 220, 30), "Bring Dingus"))
            {
                if (localDingus != null && GorillaTagger.Instance != null)
                {
                    localDingus.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 0.3f, 0.5f);
                }
            }

            GUI.backgroundColor = Color.red;
            if (GUI.Button(new Rect(10, 160, 220, 30), "Reset Dingus"))
            {
                if (localDingus != null && GorillaTagger.Instance != null)
                {
                    localDingus.transform.position = new Vector3(-66.4f, 14.5f, -82.5f);
                    localDingus.transform.localScale = ogDingusScale;
                }
            }
            
            GUI.backgroundColor = Color.magenta;
            if (GUI.Button(new Rect(10, 200, 220, 30), "STFU Dingus"))
            {
                foreach (var aSrc in localDingus.GetComponentsInChildren<AudioSource>())
                    if (muted == false)
                    {
                        aSrc.enabled = false;
                        muted = true;
                    }
                    else
                    {
                        aSrc.enabled = true;
                        muted = false;
                    }
                    
            }

            GUI.backgroundColor = Color.blue;

            GUI.Label(new Rect(37.5f, 230, 220, 25), "Press [Insert] to Toggle GUI");

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

    }
}

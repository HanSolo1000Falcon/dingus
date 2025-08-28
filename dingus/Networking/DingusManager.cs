using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace dingus.Networking
{
    public class DingusManager : MonoBehaviourPunCallbacks
    {
        public static DingusManager Instance;

        private Dictionary<int, GameObject> remoteDinguses = new Dictionary<int, GameObject>();

        void Awake()
        {
            Instance = this;
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (DingusNetworkManager.PlayerHasDingus(targetPlayer))
                SpawnRemoteDingus(targetPlayer);
        }

        private void SpawnRemoteDingus(Player player)
        {
            if (player == PhotonNetwork.LocalPlayer) return;

            if (!remoteDinguses.ContainsKey(player.ActorNumber))
            {
                GameObject dingus = GameObject.Instantiate(Plugin.dingusPrefab);

                foreach (var src in dingus.GetComponentsInChildren<AudioSource>())
                    src.enabled = false;

                var rb = dingus.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                var coll = dingus.GetComponent<Collider>();
                if (coll != null) coll.enabled = false;

                dingus.AddComponent<RemoteDingus>();
                remoteDinguses[player.ActorNumber] = dingus;
            }
        }

        public bool TryGetRemoteDingus(int actorNumber, out GameObject dingus)
        {
            return remoteDinguses.TryGetValue(actorNumber, out dingus);
        }

        public void UpdateRemoteDingus(int actorNumber, Vector3 pos, Quaternion rot)
        {
            if (remoteDinguses.TryGetValue(actorNumber, out var dingus))
            {
                dingus.transform.position = pos;
                dingus.transform.rotation = rot;
            }
        }

        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (remoteDinguses.TryGetValue(otherPlayer.ActorNumber, out var dingus))
            {
                Destroy(dingus);
                remoteDinguses.Remove(otherPlayer.ActorNumber);
            }
        }

        public override void OnLeftRoom()
        {
            foreach (var dingus in remoteDinguses.Values)
            {
                Destroy(dingus);
            }
            remoteDinguses.Clear();
        }
    }
}

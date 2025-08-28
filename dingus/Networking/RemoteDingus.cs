using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

namespace dingus.Networking
{
    public class RemoteDingus : MonoBehaviour, IOnEventCallback
    {
        private Vector3 targetPos;
        private Quaternion targetRot;

        private float lerpSpeed = 15f;

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == 41)
            {
                object[] data = (object[])photonEvent.CustomData;

                int sender = photonEvent.Sender;
                Vector3 pos = (Vector3)data[0];
                Quaternion rot = (Quaternion)data[1];

                if (DingusManager.Instance.TryGetRemoteDingus(sender, out GameObject dingus) && dingus == gameObject)
                {
                    targetPos = pos;
                    targetRot = rot;
                }
            }
        }

        void Update()
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * lerpSpeed);
        }

        void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
        void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);
    }
}

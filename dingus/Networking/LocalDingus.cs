using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Realtime;

namespace dingus.Networking
{
    public class LocalDingus : MonoBehaviour
    {
        private float sendRate = 0.025f;
        private float timer = 0f;

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= sendRate)
            {
                timer = 0f;
                SendTransform();
            }
        }

        private void SendTransform()
        {
            if (!PhotonNetwork.InRoom) return;

            object[] data = new object[]
            {
                transform.position,
                transform.rotation
            };

            PhotonNetwork.RaiseEvent(
                41,
                data,
                new RaiseEventOptions { Receivers = ReceiverGroup.Others },
                SendOptions.SendUnreliable
            );
        }
    }
}

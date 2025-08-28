using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace dingus.Networking
{
    public static class DingusNetworkManager
    {
        public const string DINGUS_KEY = "Dingus";

        public static void SetHasDingus()
        {
                var props = new Hashtable
                {
                    { DINGUS_KEY, true }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public static bool PlayerHasDingus(Player player)
        {
            return player.CustomProperties.TryGetValue(DINGUS_KEY, out object value) && value is bool b && b;
        }
    }
}

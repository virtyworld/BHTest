using System.Collections;
using UnityEngine;

namespace Mirror.Examples.NetworkRoom
{
    public class PlayerScore : NetworkBehaviour
    {
        [SyncVar]
        public int index;

        [SyncVar]
        public uint score;

        private NetworkRoomManagerExt nrme;
        private bool isRestarting;

        private void Start()
        {
            NetworkRoomManagerExt nrme = FindObjectOfType<NetworkRoomManagerExt>();
        }

        void OnGUI()
        {
            GUI.Box(new Rect(10f + (index * 110), 10f, 100f, 25f), $"P{index}: {score.ToString("0000000")}");
            GUIStyle myStyle = new GUIStyle(GUI.skin.box ); 
            myStyle.fontSize = 35;
            if (score>=3)
            {
                GUI.Box(new Rect((Screen.width-500f)/2, (Screen.height-55f)/2, 500f, 55f), 
                    $"Player{index} is win in this match!",myStyle);
            }
        }

        private void FixedUpdate()
        {
            if (score >= 3 && !isRestarting)
            {
                StartCoroutine(RestartLvl());
            }
        }

        private IEnumerator RestartLvl()
        {
            isRestarting = true;
            yield return new WaitForSeconds(5f);
            NetworkManager.singleton.ServerChangeScene("OnlineScene");
        }
    }
}

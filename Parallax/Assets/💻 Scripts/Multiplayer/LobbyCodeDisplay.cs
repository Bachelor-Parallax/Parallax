using Unity.Netcode;
using UnityEngine;

public class LobbyCodeDisplay : MonoBehaviour
{
    void OnGUI()
    {
        if (Multiplayer.Instance == null) return;
        
        float w = 220f, h = 40f;
        float x = 10f, y = 10f;
        //float spacing = 10f;

        string code = Multiplayer.Instance.CurrentLobbyCode;

        if (!string.IsNullOrEmpty(code))
        {
            GUI.Label(new Rect(x, y, 300, h), "Lobby Code: " + code);
            if (GUI.Button(new Rect(x, y + (h + 10), w, h), "Leave lobby")) _ = Multiplayer.Instance.Disconnect(); 
        }
    }
}
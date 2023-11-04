using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon.StructWrapping;
using JetBrains.Annotations;

public class PlayerLocalControl : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    
    public List<Vector3> spawnpos;
    private List<bool> spawned = new List<bool>();
    // Start is called before the first frame update
    void Start()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(this.playerPrefab.name, choiceSpawnpos(spawnpos), Quaternion.identity, 0);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    private Vector3 choiceSpawnpos(List<Vector3> spawnpos)
    {
        PhotonView pv = playerPrefab.GetComponent<PhotonView>();

        if (pv != null)
        {
            return spawnpos[pv.ViewID % 4];
        }

        else return Vector3.one;
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = this.gameObject;
    }
}

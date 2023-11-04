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
    public GameObject cam;
    public PhotonView pv;
    
    public List<Vector3> spawnpos;
    
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
            Vector3 pos = choiceSpawnpos(spawnpos);
            PhotonNetwork.Instantiate(this.playerPrefab.name, pos, Quaternion.identity, 0);
            
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
            
            return spawnpos[(PhotonNetwork.LocalPlayer.ActorNumber)-1];
        }

        else return Vector3.one;
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = this.gameObject;
    }
}

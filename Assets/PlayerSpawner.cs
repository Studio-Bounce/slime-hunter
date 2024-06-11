using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

// Keeps track of all player spawn points / checkpoints
public class PlayerSpawner : PersistentObject
{
    [SerializeField] GameObject playerPrefab;
    GameObject playerInstance = null;

    public int currentCheckpointIdx = -1;
    public PlayerCheckpoint[] checkpoints;

    protected override void Awake()
    {
        base.Awake();
        if (checkpoints.Length == 0)
        {
            Debug.LogError("Nowhere to spawn the player");
        }
    }

    private void Start()
    {
        InstantiateOrMovePlayer();
    }

    void InstantiateOrMovePlayer()
    {
        int idx = (currentCheckpointIdx != -1) ? currentCheckpointIdx : 0;
        Vector3 playerPosition = checkpoints[idx].transform.position;
        playerPosition.y = 0;
        if (playerInstance == null)
        {
            // Instantiate the player at appropriate location
            playerInstance = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
        }
        else
        {
            // Move the player to latest location
            playerInstance.transform.position = playerPosition;
        }
    }

    public void ModifyRecentCheckpoint(PlayerCheckpoint checkpoint)
    {
        int idx = System.Array.IndexOf(checkpoints, checkpoint);
        if (idx == -1)
        {
            Debug.LogError("Invalid checkpoint");
        }
        else
        {
            currentCheckpointIdx = idx;
            // Save the checkpoint
            PersistenceManager.Instance.SaveGame();
        }
    }

    // ------------------- Save / Load -------------------
    public override void LoadSaveData(byte[] data)
    {
        using (var stream = new MemoryStream(data))
        {
            using (var reader = new BinaryReader(stream))
            {
                currentCheckpointIdx = reader.ReadInt32();
                InstantiateOrMovePlayer();
            }
        }
    }

    public override byte[] GetSaveData()
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(currentCheckpointIdx);
            }
            return stream.ToArray();
        }
    }
}

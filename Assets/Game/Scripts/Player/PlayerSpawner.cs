using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

// Keeps track of all player spawn points / checkpoints
public class PlayerSpawner : PersistentObject
{
    [SerializeField] GameObject playerPrefab;
    public GameObject playerInstance = null;

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
        if (playerInstance == null)
        {
            // Instantiate the player at appropriate location
            playerInstance = Instantiate(playerPrefab, playerPosition, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(playerInstance, SceneManager.GetSceneByName(GameManager.Instance.gameSceneName));
        }
        else
        {
            // Move the player to latest location
            // Player has a character controller so we can't move it directly
            if (playerInstance.TryGetComponent<CharacterController>(out var playerCC))
            {
                playerCC.enabled = false;
                playerInstance.transform.position = playerPosition;
                playerCC.enabled = true;
            }
            else
            {
                playerInstance.transform.position = playerPosition;
            }
        }
        StartCoroutine(PlayPlayerSpawnVFXWithDelay(0.5f));
    }

    IEnumerator PlayPlayerSpawnVFXWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (playerInstance.TryGetComponent<VisualEffect>(out var visualEffect))
        {
            visualEffect.Play();
        }
    }

    public void ModifyRecentCheckpoint(PlayerCheckpoint checkpoint)
    {
        int idx = System.Array.IndexOf(checkpoints, checkpoint);
        if (idx == -1)
        {
            Debug.LogError("Invalid checkpoint");
        }
        // No point in updating older checkpoints
        else if (idx > currentCheckpointIdx)
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

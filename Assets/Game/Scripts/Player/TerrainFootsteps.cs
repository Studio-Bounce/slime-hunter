using FMODUnity;
using UnityEngine;

public enum GroundType
{
    None,
    Grass,
    Dirt,
    Gravel
}

[RequireComponent(typeof(Collider))]
public class TerrainFootsteps : MonoBehaviour
{
    public LayerMask terrainLayer;

    // Define your ground types based on terrain texture indices
    public GroundType[] groundTypes;

    [Header("Footstep Sounds")]
    public float footstepInterval = 0.5f; // Time interval between footsteps
    private float footstepTimer = 0f;

    public Terrain currentTerrain;

    private void Update()
    {
        if (InputManager.Instance.Movement != Vector2.zero)
        {
            footstepTimer -= Time.unscaledDeltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }
    }

    private void PlayFootstep()
    {
        switch (GetGroundType(transform.position))
        {
            case GroundType.None:
                //Debug.Log("No ground detected");
                break;
            case GroundType.Dirt:
                RuntimeManager.PlayOneShot(AudioManager.Config.walkDirt);
                break;
            case GroundType.Grass:
                RuntimeManager.PlayOneShot(AudioManager.Config.walkGrass);
                break;
            case GroundType.Gravel:
                RuntimeManager.PlayOneShot(AudioManager.Config.walkGravel);
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with a terrain
        if ((terrainLayer & (1 << collision.gameObject.layer)) != 0)
        {
            currentTerrain = collision.gameObject.GetComponent<Terrain>();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Check if the collision is with a terrain
        if ((terrainLayer & (1 << collision.gameObject.layer)) != 0)
        {
            // Check if the exiting terrain is the currently referenced terrain
            if (currentTerrain == collision.gameObject.GetComponent<Terrain>())
            {
                currentTerrain = null; // Reset the current terrain
            }
        }
    }

    private GroundType GetGroundType(Vector3 worldPosition)
    {
        if (currentTerrain == null)
        {
            return GroundType.None;
        }

        Vector3 terrainPosition = worldPosition - currentTerrain.transform.position;
        Vector3 normalizedPosition = new Vector3(
            terrainPosition.x / currentTerrain.terrainData.size.x,
            terrainPosition.y / currentTerrain.terrainData.size.y,
            terrainPosition.z / currentTerrain.terrainData.size.z
        );

        int mapX = Mathf.RoundToInt(normalizedPosition.x * currentTerrain.terrainData.alphamapWidth);
        int mapZ = Mathf.RoundToInt(normalizedPosition.z * currentTerrain.terrainData.alphamapHeight);

        float[,,] splatmapData = currentTerrain.terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        float maxMix = 0f;
        int maxIndex = 0;

        for (int i = 0; i < splatmapData.GetLength(2); i++)
        {
            if (splatmapData[0, 0, i] > maxMix)
            {
                maxMix = splatmapData[0, 0, i];
                maxIndex = i;
            }
        }

        return groundTypes[maxIndex];
    }
}

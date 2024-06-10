using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public class EnemyGauntlet : MonoBehaviour
{
    [Header("Gauntlet Bounds")]
    public Vector2 boundSize;
    public float boundHeight = 3f;
    public string collisionTag = "Player";
    public GameObject wallPrefab;
    public float wallWidth = 2f;
    public float colliderOffset;
    public float boundSpawnDelay = 0f;
    public bool active = true;

    private BoxCollider _boxCollider;
    private GameObject[] _wallObjectPool = new GameObject[4];

    [Header("Gauntlet Enemy Waves")]
    public List<EnemyWaveProperties> enemyWaves = new List<EnemyWaveProperties>();
    private EnemyWave enemyWaveHandler;

    private bool gauntletStart = false;
    private int waveCounter = 0;

    public EnemyWaveProperties CurrentWaveProp
    {
        get { return enemyWaves[waveCounter]; }
    }

    private void Awake()
    {
        _boxCollider = gameObject.AddComponent<BoxCollider>();
    }

    private void Start()
    {
        Debug.Assert(wallPrefab != null, "Requires a prefab wall");

        _boxCollider.isTrigger = true;
        _boxCollider.center = new Vector3(_boxCollider.center.x, boundHeight / 4, _boxCollider.center.z);
        _boxCollider.size = new Vector3(boundSize.x - colliderOffset, boundHeight / 2, boundSize.y - colliderOffset);

        // Instantiate all walls ahead of time
        for (int i = 0; i < _wallObjectPool.Length; i++)
        {
            _wallObjectPool[i] = Instantiate(wallPrefab, transform);
            _wallObjectPool[i].SetActive(false);
        }

        GameObject go = new GameObject("EnemyWave");
        go.transform.SetParent(transform, false);
        enemyWaveHandler = go.AddComponent<EnemyWave>();
    }

    private void Update()
    {
        if (gauntletStart)
        {
            UpdateEnemySpawn();
        }
    }

    private void UpdateEnemySpawn()
    {
        if (enemyWaves.Count < 1) return;

        // Gauntlet Completed
        if (waveCounter > enemyWaves.Count - 1)
        {
            StartCoroutine(ReleaseWalls());
            return;
        }

        if (!enemyWaveHandler.Started)
        {
            Vector2 innerBounds = boundSize;
            innerBounds.x = innerBounds.x / 2 - colliderOffset;
            innerBounds.y = innerBounds.y / 2 - colliderOffset;
            enemyWaveHandler.StartWave(CurrentWaveProp, innerBounds);
        }

        if (enemyWaveHandler.Completed)
        {
            waveCounter++;
            enemyWaveHandler.Reset();
        }
    }

    IEnumerator SpawnWalls()
    {
        yield return new WaitForSeconds(boundSpawnDelay);

        Vector2Int sideDirection = new Vector2Int(1, 0);
        Vector2 positionOffset = new Vector2(boundSize.x / 2, boundSize.y / 2);
        Vector2Int rotationOffset = new Vector2Int(0, 90);

        for (int i = 0; i < _wallObjectPool.Length; i++)
        {
            Vector3 directionalOffset = new Vector3(positionOffset.x * sideDirection.y, -boundHeight, positionOffset.y * sideDirection.x);
            GameObject wall = _wallObjectPool[i];
            wall.transform.position = transform.position + directionalOffset;
            wall.transform.localScale = new Vector3(sideDirection.x == 0 ? boundSize.y : boundSize.x , boundHeight, wallWidth);
            wall.transform.rotation = Quaternion.Euler(new Vector3(0, (rotationOffset*sideDirection).magnitude, 0));
            wall.SetActive(true);

            // Rotate 90 deg to next side
            sideDirection = new Vector2Int(-sideDirection.y, sideDirection.x);
        }

        // Animate
        float timeElapsed = 0;
        float animationTime = 1;
        float startHeight = transform.position.y - boundHeight/2;
        float endHeight = transform.position.y;
        
        while (timeElapsed < animationTime)
        {
            float t = Easing.EaseOutBack(timeElapsed / animationTime);
            for (int i = 0; i < _wallObjectPool.Length; i++)
            {
                GameObject wall = _wallObjectPool[i];
                Vector3 startPosition = wall.transform.position;
                startPosition.y = startHeight;
                Vector3 endPosition = wall.transform.position;
                endPosition.y = endHeight;
                wall.transform.position = Vector3.LerpUnclamped(startPosition, endPosition, t);
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        gauntletStart = true;
    }

    IEnumerator ReleaseWalls()
    {
        for (int i = 0; i < _wallObjectPool.Length; i++)
        {
            GameObject wall = _wallObjectPool[i];
            float timeElapsed = 0;
            float animationTime = 1;
            Vector3 startPosition = wall.transform.position;
            Vector3 endPosition = wall.transform.position - new Vector3(0, boundHeight/2, 0);
            while (timeElapsed < animationTime)
            {
                float t = Easing.EaseInBack(timeElapsed / animationTime);
                wall.transform.position = Vector3.LerpUnclamped(startPosition, endPosition, t);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            _wallObjectPool[i].SetActive(false);
        }
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (active)
            {
                StartCoroutine(SpawnWalls()); // Will also start enemy spawning
                _boxCollider.enabled = false;
            }
        }

    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!active)
        {
            Handles.color = new Color(0, 0, 0, 1);
        }
        // Get the position of the object
        Vector3 position = transform.position;

        // Calculate half width and half length
        float halfWidth = boundSize.x / 2f;
        float halfLength = boundSize.y / 2f;

        // Calculate the four corners of the rect
        Vector3 topLeft = position + new Vector3(-halfWidth, 0, halfLength);
        Vector3 topRight = position + new Vector3(halfWidth, 0, halfLength);
        Vector3 bottomLeft = position + new Vector3(-halfWidth, 0, -halfLength);
        Vector3 bottomRight = position + new Vector3(halfWidth, 0, -halfLength);

        // Draw the square using Handles.DrawSolidRectangleWithOutline
        Handles.DrawSolidRectangleWithOutline(new Vector3[] { topLeft, topRight, bottomRight, bottomLeft }, Color.clear, Color.red);

        halfWidth = (boundSize.x - colliderOffset) / 2f;
        halfLength = (boundSize.y - colliderOffset) / 2f;

        // Calculate the four corners of the rect
        topLeft = position + new Vector3(-halfWidth, 0, halfLength);
        topRight = position + new Vector3(halfWidth, 0, halfLength);
        bottomLeft = position + new Vector3(-halfWidth, 0, -halfLength);
        bottomRight = position + new Vector3(halfWidth, 0, -halfLength);

        Handles.DrawSolidRectangleWithOutline(new Vector3[] { topLeft, topRight, bottomRight, bottomLeft }, new Color(1, 0, 0, 0.05f), Color.red);


    }
#endif
}

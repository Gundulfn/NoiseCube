using UnityEngine;

public class TerrainChunk
{
    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
    public Vector2 coord;

    const float colliderGenerationDistanceThreshold = 5f;
    float maxViewDst;

    GameObject meshObject;
    Vector2 sampleCenter;
    Bounds bounds;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    LODInfo[] detailLevels;
    LODMesh[] lodMeshes;
    int colliderLODIndex;
    int previousLODIndex = -1;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;
    Transform viewer;

    HeightMap heightMap;
    bool heightMapReceived;

    bool hasSetCollider;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material)
    {
        this.coord = coord;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;

        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;

        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();

        meshRenderer.material = material;

        sampleCenter = coord * meshSettings.meshWorlSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorlSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorlSize);

        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;

        SetVisible(false);

        lodMeshes = new LODMesh[detailLevels.Length];

        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            lodMeshes[i].updateCallback += UpdateTerrainChunk;

            if (i == colliderLODIndex)
            {
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }
    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCenter), OnHeightMapReceived);
    }

    public void OnHeightMapReceived(object heightMapObject)
    {
        this.heightMap = (HeightMap)heightMapObject;
        heightMapReceived = true;

        UpdateTerrainChunk();
    }

    Vector2 viewerPosition
    {
        get
        {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }

    public void UpdateTerrainChunk()
    {
        if (!heightMapReceived)
            return;

        float viewerDstFromTerrainChunk = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

        bool wasVisible = IsVisible();
        bool visible = viewerDstFromTerrainChunk <= maxViewDst;

        if (visible)
        {
            int lodIndex = 0;

            for (int i = 0; i < detailLevels.Length - 1; i++)
            {
                if (viewerDstFromTerrainChunk > detailLevels[i].visibleDstThreshold)
                {
                    lodIndex = i + 1;
                }
                else
                {
                    break;
                }
            }

            if (lodIndex != previousLODIndex)
            {
                LODMesh lodMesh = lodMeshes[lodIndex];

                if (lodMesh.hasMesh)
                {
                    previousLODIndex = lodIndex;
                    meshFilter.mesh = lodMesh.mesh;
                }
                else if (!lodMesh.hasRequestedMesh)
                {
                    lodMesh.RequestMesh(heightMap, meshSettings);
                }
            }
        }

        if (wasVisible != visible)
        {
            SetVisible(visible);

            if (onVisibilityChanged != null)
            {
                onVisibilityChanged(this, visible);
            }
        }

    }

    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

            if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
            {
                if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
                {
                    lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings);
                }
            }

            if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
            {
                if (lodMeshes[colliderLODIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                    hasSetCollider = true;
                }
            }
        }
    }

    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return meshObject.activeSelf;
    }
}

public class LODMesh
{
    int lod;
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    public event System.Action updateCallback;

    public LODMesh(int lod)
    {
        this.lod = lod;
    }

    void OnMeshDataReceived(object meshDataObject)
    {
        mesh = ((MeshData)meshDataObject).CreateMesh();
        hasMesh = true;

        updateCallback();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        hasRequestedMesh = true;
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadFace {

    public float scale;
    int resolution;
    Vector3 position;
    Vector3 left;
    Vector3 forward;

    public bool generated = false;
    public bool finalized = false;
    public bool merged = true;
    public GameObject gameObject;
    public QuadFace parent;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;
    public Mesh mesh;
    private Material _material;


    private Vector3[] _vertices;
    private int[] _triangles;
    public QuadFace[] faceTree;

    private void FinalizeFace()
    {
        if (finalized)
            return;

        var verts = mesh.vertices;

        for (int i = 0; i < verts.Length; i++)
            verts[i] = verts[i] * (1 + Planet.Evaluate(verts[i])) * 1000;

        mesh.vertices = verts;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();


        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;

         finalized = true;
    }
    // Use this for initialization
    public void RenderFace () {
        if (generated)
        {
            meshRenderer.enabled = true;
            meshCollider.enabled = true;
            return;
        }

        _vertices = new Vector3[resolution * resolution];
        _triangles = new int[(resolution - 1) * (resolution - 1) * 6];

        int triangleIndex = 0;

        for(int z = 0; z < resolution; z++ )
        {
            for(int x = 0; x < resolution; x++ )
            {
                int index = x + z * resolution;

                var px = (float)x / (resolution - 1);
                var pz = (float)z / (resolution - 1);
                var vx = left * px * scale;
                var vz = forward * pz * scale;

                Vector3 pointOnUnitSphere =  (position + vx + vz).normalized  ;
                _vertices[index] = -pointOnUnitSphere ;

                if (z != resolution - 1 && x != resolution - 1)
                {
                    _triangles[triangleIndex] = index;
                    _triangles[triangleIndex + 1] = index + 1;
                    _triangles[triangleIndex + 2] = index + resolution + 1;
                    _triangles[triangleIndex + 3] = index;
                    _triangles[triangleIndex + 4] = index + resolution + 1;
                    _triangles[triangleIndex + 5] = index + resolution;
                    triangleIndex += 6;
                }
            }
        }
        
        meshFilter.sharedMesh = mesh;
        meshRenderer.material = _material;
        meshCollider.sharedMesh = mesh;
        mesh.vertices = _vertices;
        mesh.triangles = _triangles;

        generated = true;
        FinalizeFace();
    }
	public QuadFace (QuadFace parent, int resolution, float scale, Vector3 normal, Vector3 left, Vector3 forward)
    {
        Init(parent, resolution, scale, normal, left, forward);
    }
    public QuadFace (int resolution, float scale, Vector3 normal, Vector3 left, Vector3 forward)
    {
        Init(null, resolution, scale, normal, left, forward);
    }
    void Init(QuadFace parent, int resolution, float scale, Vector3 normal, Vector3 left, Vector3 forward)
    {
        this.parent = parent;
        this.scale = scale;
        this.position = normal;
        this.resolution = resolution;
        this.left = left;
        this.forward = forward;

        faceTree = null;

        gameObject = new GameObject("CubePlane_" + scale + "_" + scale + "_" + position.ToString());
        gameObject.SetActive(true);
        position -= left * (scale / 2);
        position -= forward * (scale / 2);
        mesh = new Mesh();
        _material = new Material(Shader.Find("Standard"));
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = null;
        if (parent != null)
            gameObject.transform.parent = parent.gameObject.transform;
        RenderFace();
    }

    public void Split()
    {
        var subPos = position;
        subPos += left * (scale / 2);
        subPos += forward * (scale / 2);

        var stepLeft = (left * scale / 4);
        var stepForward = (forward * scale / 4);

        var hs = scale / 2;

        faceTree = new QuadFace[]
        {
            new QuadFace(this, resolution, hs, subPos - stepLeft + stepForward, left, forward),
            new QuadFace(this, resolution, hs, subPos + stepLeft + stepForward, left, forward),
            new QuadFace(this, resolution, hs, subPos - stepLeft - stepForward, left, forward),
            new QuadFace(this, resolution, hs, subPos + stepLeft - stepForward, left, forward)
        };
        Dispose();
    }

    public void Merge()
    {
        if (faceTree != null)
        {
            for (int t = 0; t < faceTree.Length; t++)
            {
                faceTree[t].Dispose();
            }
            faceTree = null;
            //RenderFace();
            meshRenderer.enabled = true;
        }

    }
    public void Dispose()
    {
        gameObject.SetActive(false);
        meshRenderer.enabled = false;
        generated = false;
        finalized = false;
    }
}

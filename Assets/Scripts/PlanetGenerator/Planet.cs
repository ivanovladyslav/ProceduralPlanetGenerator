using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet {

    QuadFace[] faceTree;
    public GameObject gameObject;
    [SerializeField]
    int res = 4;
    float scale = 1000;
    public float globalScale;
    static Noise noise = new Noise();

    public static float Evaluate(Vector3 point)
    {
        float noiseValue = (noise.Evaluate(point) + 1) * .3f;
        return noiseValue;
    }

    public Planet()
    {
        float hs = scale / 2;
        globalScale = scale;

        faceTree = new QuadFace[]
        {
            new QuadFace(res, scale, new Vector3(0, hs, 0), new Vector3(0, 0, 1), new Vector3(-1, 0, 0)),
            new QuadFace(res, scale, new Vector3(0, -hs, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0)),

            new QuadFace(res, scale, new Vector3(-hs, 0, 0), new Vector3(0, 0, 1), new Vector3(0, -1, 0)),
            new QuadFace(res, scale, new Vector3(hs, 0, 0), new Vector3(0, 0, -1), new Vector3(0, -1, 0)),

            new QuadFace(res, scale, new Vector3(0, 0, -hs), new Vector3(-1, 0, 0), new Vector3(0, -1, 0)),
            new QuadFace(res, scale, new Vector3(0, 0, hs), new Vector3(1, 0, 0), new Vector3(0, -1, 0))
        };

        gameObject = new GameObject("planet");
        for (int f = 0; f < faceTree.Length; f++)
            faceTree[f].gameObject.transform.parent = gameObject.transform;
    }
	

    private void GetActiveFaces(QuadFace face, ref List<QuadFace> active)
    {
        if (face.faceTree == null)
        {
            active.Add(face);
        }
        else
        {
            for (int f = 0; f < face.faceTree.Length; f++)
            {
                GetActiveFaces(face.faceTree[f], ref active);
            }
        }            
    }

    public IEnumerator Update(int f)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            yield break;

        List<QuadFace> activeTree = new List<QuadFace>();
        GetActiveFaces(faceTree[f], ref activeTree);

        for(int a = 0; a < activeTree.Count; a++)
        {
            float dist = Vector3.Distance(player.transform.position, activeTree[a].mesh.bounds.ClosestPoint(player.transform.position));

            if (dist < activeTree[a].scale)
            {
                activeTree[a].Split();
            }
            else
            {
                if(activeTree[a].parent != null && dist > activeTree[a].parent.scale * 4)
                    activeTree[a].parent.Merge();
                
            }
        }
    }
}

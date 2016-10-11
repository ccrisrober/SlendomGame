using UnityEngine;
using System.Collections;

public class StaticScript : MonoBehaviour {
    float theAlpha = 0.0f;

    Camera theCamera;
    Transform cameraTransform;

    private Mesh mesh;

    private Vector2[] uv;
    private Vector3[] verts;
    private int[] tris;
    private Vector3[] normals;

    public float distance = 1.0f;

    private Material theMaterial;

    EnemyScript theEnemy;
	// Use this for initialization
	void Start () {
        Startup();
        // find and store a reference to the enemy script (to use health as alpha for texture)
        if (theEnemy == null)
        {
            theEnemy = GameObject.Find("Enemy").GetComponent<EnemyScript>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        SetAlpha();

        ScrollUVs();
    }

    void SetAlpha()
    {
        theAlpha = (100.0f - theEnemy.health) * 0.01f;

        theMaterial.color = new Color(theMaterial.color.r, theMaterial.color.g, theMaterial.color.b, theAlpha);
    }


    void ScrollUVs()
    {
        float scrollX = Random.Range(-0.5f, 0.5f);
        float scrollY = Random.Range(-0.5f, 0.5f);

        // UVs
        for (int i = 0; i < 4; ++i)
	    {
            uv[i] = new Vector2(uv[i].x + scrollX, uv[i].y + scrollY);
        }

        mesh.uv = uv;
    }


    // ----

    void Startup()
    {
        if (theCamera == null)
        {
            theCamera = Camera.main;
        }
        cameraTransform = theCamera.transform;

        theMaterial = gameObject.GetComponent<Renderer>().material;
        theMaterial.color = Color.white;

        if (!mesh)
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "ScreenMesh";
        }

        Construct();

        //DebugVerts();
    }

    void Construct()
    {
        mesh.Clear();

        verts = new Vector3[4];
        uv = new Vector2[4];
        tris = new int[6];
        normals = new Vector3[4];

        // calculate verts based on camera FOV
        Vector3 pos = cameraTransform.position - transform.position;

        float halfFOV = (theCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float aspect = theCamera.aspect;
        //Debug.Log( " Screen.width " + Screen.width + " : Screen.height " + Screen.height + " : aspect " + aspect );

        float height = distance * Mathf.Tan(halfFOV);
        float width = height * aspect;

        //Debug.Log( " fieldOfView " + theCamera.fieldOfView + " : aspect " + aspect );

        // UpperLeft
        verts[0] = pos - (cameraTransform.right * width);
        verts[0] += cameraTransform.up * height;
        verts[0] += cameraTransform.forward * distance;

        // UpperRight
        verts[1] = pos + (cameraTransform.right * width);
        verts[1] += cameraTransform.up * height;
        verts[1] += cameraTransform.forward * distance;

        // LowerLeft
        verts[2] = pos - (cameraTransform.right * width);
        verts[2] -= cameraTransform.up * height;
        verts[2] += cameraTransform.forward * distance;

        // LowerRight
        verts[3] = pos + (cameraTransform.right * width);
        verts[3] -= cameraTransform.up * height;
        verts[3] += cameraTransform.forward * distance;


        // UVs
        uv[0] = new Vector2(0.0f, 1.0f);
        uv[1] = new Vector2(1.0f, 1.0f);
        uv[2] = new Vector2(0.0f, 0.0f);
        uv[3] = new Vector2(1.0f, 0.0f);

        // Triangles
        tris[0] = 0;
        tris[1] = 1;
        tris[2] = 2;
        tris[3] = 2;
        tris[4] = 1;
        tris[5] = 3;

        // Normals
        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        // assign mesh
        mesh.vertices = verts;
        mesh.uv = uv;
        mesh.triangles = tris;
        mesh.normals = normals;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}

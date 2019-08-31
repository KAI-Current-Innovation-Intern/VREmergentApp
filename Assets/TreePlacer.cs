using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TreePlacer : MonoBehaviour
{

    #region Class Variables

    [System.Serializable]
    public class TreeTypes
    {
        [SerializeField]
        public TextAsset TreeType;
        [SerializeField]
        [Range(0, 100)]
        public int spawnChance = 50;
        [SerializeField]
        public bool spawned = false;

    }

    private int density = 1;
    /// <summary>
    /// The size of the sphere in which trees can be placed, based of Unity transform units
    /// </summary>
    [SerializeField]
    private int radius = 10;
    /// <summary>
    /// Transparent material to be used on the placement sphere
    /// </summary>
    [SerializeField]
    private Material transMaterial;
    private GameObject radiusGO;
    private List<GameObject> trees;
    private GameObject parentMesh;
    public GameObject TreeSpawner;
    public GameObject TreeSpawnerPreFab;
    //public GameObject TreeSpawnerPrefab; 
    //private GameObject tree;
    #endregion

  

    public void Start()
    {


    }

    public void Update()
    {
    

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
             
                TreeSpawner = Instantiate(TreeSpawnerPreFab, new Vector3(hit.point.x, hit.point.y, hit.point.z), new Quaternion());

            }
           

        }


    }
            
            }


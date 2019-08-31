using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;

public class LSystemsGenerator : MonoBehaviour
{
    //number of trees that have been declared
    public static int NUM_OF_TREES = 8;
    //number of iterations to generate the tree
    public static int MAX_ITERATIONS = 10;

    public int title = 1;
    public int iterations = 4;
    //angle of rotation
    public float angle = 30f;
    public float width = 0.5f;
    //length travelled
    public float length = 1f;
    public float variance = 10f;
    public bool hasTreeChanged = false;
    private GameObject radiusGO;
    //actual tree
    public GameObject Tree = null;

    //create the prefabs of the trees
    [SerializeField] private GameObject treeParent;
    [SerializeField] private GameObject branch;
    [SerializeField] private GameObject leaf;
    [SerializeField] private HUDScript HUD;

    private const string axiom = "X";

    private Dictionary<char, string> rules;
    private Stack<TransformInfo> transformStack;
    private int titleLastFrame;
    private int iterationsLastFrame;
    private float angleLastFrame;
    private float widthLastFrame;
    private float lengthLastFrame;
    private string currentString = string.Empty;
    private Vector3 initialPosition = Vector3.zero;
    private float[] randomRotationValues = new float[100];
    private bool isGenerating = false;

    private Dictionary<char, string>[] seeds = new Dictionary<char, string>[]
     {
        //0
        new Dictionary<char, string>
        {
            { 'X', "[F[-X+F[+FX]][*-X+F[+FX]][/-X+F[+FX]-X]]" },
            { 'F', "FF" }
        },
        //1
        new Dictionary<char, string>
        {
            { 'X', "[*F-[/X+X*]+F[*+FX]-X]" },
            { 'F', "FF" }
        },
        //2
         new Dictionary<char, string>
        {
            { 'X', "[*-FX][/+FX][FX]" },
            { 'F', "FF" }
        },
        //3
         new Dictionary<char, string>
        {
            { 'X', "[*-FX]X[+FX][/+F-FX]" },
            { 'F', "FF" }
        },
        //4
        new Dictionary<char, string>
        {
            { 'X', "[*FF[/+XF-F+FX]--F+F-FX]" },
            { 'F', "FF" }
        },
       //5
         new Dictionary<char, string>
        {
            { 'X', "[*FX[+F[/-FX]FX][-F-FXFX]]" },
            { 'F', "FF" }
        },
       //6
         new Dictionary<char, string>
        {
            { 'X', "[*+FX]X[+FX][/+F-FX]" },
            { 'F', "FF" }
        },
       //7
         new Dictionary<char, string>
        {
            { 'X', "[F[+FX][*+FX][/+FX]]" },
            { 'F', "FF" }
        },

        //8
         new Dictionary<char, string>
        {
            { 'X', "F[*+X][/-X]FX" },
            { 'F', "FF" }
        },
        //9
        new Dictionary<char, string>
        {
            { 'X', "F-[[X*]+X]+F[+FX/]-X" },
            { 'F', "FF" }
        },


     };

    //private int rand_seed = UnityEngine.Random.Range(0, 9);
    //private int rand_iteration = UnityEngine.Random.Range(4, 10);
    //private float rand_angle = UnityEngine.Random.Range(25f, 35f);
    //private float rand_length = UnityEngine.Random.Range(1f, 10f);


    private void Start()
    {
        titleLastFrame = title;
        iterationsLastFrame = iterations;
        angleLastFrame = angle;
        widthLastFrame = width;
        lengthLastFrame = length;
        int rand_int = UnityEngine.Random.Range(0, seeds.Length);
        Debug.Log(rand_int);
        rules = seeds[rand_int];

        for (int i = 0; i < randomRotationValues.Length; i++)
        {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }

        transformStack = new Stack<TransformInfo>();

        StartCoroutine(GenerateLSystem());
    }


    IEnumerator GenerateLSystem()
    {
        int count = 0;

        //number of iterations
        while (count < 2)
        {
            if (!isGenerating)
            {
                isGenerating = true;
                StartCoroutine(Generate());
            }
            else
            {
                yield return new WaitForSeconds(10f);

            }
            count++;
        }

    }

    IEnumerator Generate()
    {
        Destroy(Tree);

        Tree = Instantiate(treeParent);

        currentString = axiom;

        StringBuilder sb = new StringBuilder();


        //rules for the game 

        for (int i = 0; i < iterations; i++)
        {
            foreach (char c in currentString)
            {
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }

            currentString = sb.ToString();
            sb = new StringBuilder();
        }

        Debug.Log(currentString);

        for (int i = 0; i < currentString.Length; i++)
        {
            switch (currentString[i])
            {
                case 'F':
                    initialPosition = transform.position;
                    transform.Translate(Vector3.up * 2 * length);

                    GameObject fLine = currentString[(i + 1) % currentString.Length] ==
                    'X' || currentString[(i + 3) % currentString.Length] == 'F'
                         && currentString[(i + 4) % currentString.Length] == 'X' ?
                     Instantiate(leaf) : Instantiate(branch);
                    fLine.transform.SetParent(Tree.transform);
                    fLine.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
                    fLine.GetComponent<LineRenderer>().SetPosition(1, transform.position);
                    fLine.GetComponent<LineRenderer>().startWidth = width;
                    fLine.GetComponent<LineRenderer>().endWidth = width;
                    yield return null;
                    break;

                case 'X':
                    break;

                case '+':
                    transform.Rotate(Vector3.back * angle * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;

                case '-':
                    transform.Rotate(Vector3.forward * angle * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;

                case '*':
                    transform.Rotate(Vector3.up * 120 * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;

                case '/':
                    transform.Rotate(Vector3.down * 120 * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;

                case '[':
                    transformStack.Push(new TransformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    break;

                case ']':
                    TransformInfo ti = transformStack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    break;

                default:
                    throw new InvalidOperationException("Invalid L-tree operation");
            }
        }

      //  Tree.transform.rotation = Quaternion.Euler(0, HUD.rotation.value, 0);
        isGenerating = false;
    }




    private void ResetRandomValues()
    {
        for (int i = 0; i < randomRotationValues.Length; i++)
        {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

    private void ResetFlags()
    {
        iterationsLastFrame = iterations;
        angleLastFrame = angle;
        widthLastFrame = width;
        lengthLastFrame = length;
    }

    private void ResetTreeValues()
    {
        iterations = 4;
        angle = 30f;
        width = 1f;
        length = 2f;
        variance = 10f;
    }

    IEnumerator TextFade()
    {
        Color c = HUD.warning.color;

        float TOTAL_TIME = 4f;
        float FADE_DURATION = .25f;

        for (float timer = 0f; timer <= TOTAL_TIME; timer += Time.deltaTime)
        {
            if (timer > TOTAL_TIME - FADE_DURATION)
            {
                c.a = (TOTAL_TIME - timer) / FADE_DURATION;
            }
            else if (timer > FADE_DURATION)
            {
                c.a = 1f;
            }
            else
            {
                c.a = timer / FADE_DURATION;
            }

            HUD.warning.color = c;

            yield return null;
        }
    }

    private void Update()
    {
        ////Creating Raycast hit info variable and the actual Ray, which points from screen to mouse position
        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out hit))
        //{
        //    //Sets position of gameobject to match whatever it hit on the collision
        //    radiusGO.transform.position = hit.point;
        //}
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    // parentMesh = new GameObject();
        //    //Once the user presses the key it will itterate through and generate the number of trees
        //    //GameObject tree = new GameObject();
        //    //LSystemsGenerator Script = tree.AddComponent<LSystemsGenerator>();
        //    //GameObject TreeSpawnerPrefab = Resources.Load<GameObject>("Assets/Prefabs/TreeSpawner");
        //    //GameObject TreeSpawner = Instantiate(TreeSpawnerPrefab);
        //    StartCoroutine(GenerateLSystem());


        //}
        //if (HUD.hasGenerateBeenPressed || Input.GetKeyDown(KeyCode.G))
        //{
        //    ResetRandomValues();
        //    HUD.hasGenerateBeenPressed = false;
        //    Generate();
        //}

        //if (HUD.hasResetBeenPressed || Input.GetKeyDown(KeyCode.R))
        //{
        //    ResetTreeValues();
        //    HUD.hasResetBeenPressed = false;
        //    HUD.Start();
        //    Generate();
        //}

        //if (titleLastFrame != title)
        //{
        //if (title >= 6)
        //{
        //    HUD.rotation.gameObject.SetActive(true);
        //}
        //else
        //{
        //    HUD.rotation.value = 0;
        //    HUD.rotation.gameObject.SetActive(false);
        //}

        //    switch (title)
        //    {
        //        case 1:
        //            SelectTreeOne();
        //            break;

        //        case 2:
        //            SelectTreeTwo();
        //            break;

        //        case 3:
        //            SelectTreeThree();
        //            break;

        //        case 4:
        //            SelectTreeFour();
        //            break;

        //        case 5:
        //            SelectTreeFive();
        //            break;

        //        case 6:
        //            SelectTreeSix();
        //            break;

        //        case 7:
        //            SelectTreeSeven();
        //            break;

        //        case 8:
        //            SelectTreeEight();
        //            break;

        //        default:
        //            SelectTreeOne();
        //            break;
        //    }

        //    titleLastFrame = title;
        //}

        //if (iterationsLastFrame != iterations)
        //{
        //    if (iterations >= 6)
        //    {
        //        HUD.warning.gameObject.SetActive(true);
        //        StopCoroutine("TextFade");
        //        StartCoroutine("TextFade");
        //    }
        //    else
        //    {
        //        HUD.warning.gameObject.SetActive(false);
        //    }
        //}

        //if (iterationsLastFrame != iterations ||
        //        angleLastFrame  != angle ||
        //        widthLastFrame  != width ||
        //        lengthLastFrame != length)
        //{
        //    ResetFlags();
        //    Generate();
        //}

    }

    private void SelectTreeOne()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F-[X+X]+F[+FX]-X]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeTwo()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[-FX][+FX][FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeThree()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[-FX]X[+FX][+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeFour()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[FF[+XF-F+FX]--F+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeFive()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[FX[+F[-FX]FX][-F-FXFX]]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeSix()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F[+FX][*+FX][/+FX]]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeSeven()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[*+FX]X[+FX][/+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeEight()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F[-X+F[+FX]][*-X+F[+FX]][/-X+F[+FX]-X]]" },
            { 'F', "FF" }
        };

        Generate();
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

public class GenGameObject : MonoBehaviour
{
    Camera mainCamera;
    GameObject Boat;
    GameObject River;
    GameObject Block;
    public float speed = 100F;

    GameSceneController my;

    GameObject[] boat = new GameObject[2];
    Stack<GameObject> Angel_start = new Stack<GameObject>();
    Stack<GameObject> Angel_end = new Stack<GameObject>();
    Stack<GameObject> Devil_start = new Stack<GameObject>();
    Stack<GameObject> Devil_end = new Stack<GameObject>();

    Vector3 shoreStartPos = new Vector3(-12.5F, 0, 0);
    Vector3 shoreEndPos = new Vector3(12, 0, 0);
    Vector3 AngelStartPos = new Vector3(-14.5F, 0.5F, 0);
    Vector3 AngelEndPos = new Vector3(14.5F, 0.5F, 0);
    Vector3 DevilStartPos = new Vector3(-14.5F, 0.5F, -3);
    Vector3 DevilEndPos = new Vector3(14.5F, 0.5F, -3);

    public void LoadResources()
    {
        Boat = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Boat"), shoreStartPos, Quaternion.identity);
        River = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/River"), Vector3.zero, Quaternion.identity);
        Block = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Block"), Vector3.zero, Quaternion.identity);

        for (int i = 0; i < 3; i++)
        {
            Angel_start.Push(Instantiate(Resources.Load("Prefabs/Angel")) as GameObject);
            Devil_start.Push(Instantiate(Resources.Load("Prefabs/Devil")) as GameObject);
        }
    }

    void Start()
    {
        my = GameSceneController.GetInstance();
        my.setGenGameObject(this);
        LoadResources();
        GameObject gameObject = GameObject.Find("MainCamera");
        mainCamera = gameObject.GetComponent<Camera>();
        mainCamera.transform.LookAt(Boat.transform);
    }

    void setCharacterPositions(Stack<GameObject> data, Vector3 pos)
    {
        GameObject[] store = data.ToArray();
        for (int i = 0; i < data.Count; i++)
        {
            store[i].transform.position = new Vector3(pos.x, pos.y, pos.z + i);
        }
    }

    void Update()
    {
        setCharacterPositions(Angel_start, AngelStartPos);
        setCharacterPositions(Angel_end, AngelEndPos);
        setCharacterPositions(Devil_start, DevilStartPos);
        setCharacterPositions(Devil_end, DevilEndPos);

        if (my.state == State.SEMOV)
        {
            Boat.transform.position = Vector3.MoveTowards(Boat.transform.position, shoreEndPos, speed * Time.deltaTime);
            if (Boat.transform.position == shoreEndPos)
            {
                my.state = State.END;
            }
        }
        else if (my.state == State.ESMOV)
        {
            Boat.transform.position = Vector3.MoveTowards(Boat.transform.position, shoreStartPos, speed * Time.deltaTime);
            if (Boat.transform.position == shoreStartPos)
            {
                my.state = State.START;
            }
        }
        else
        {
            check();
        }
    }

    int boatCapacity()
    {
        int num = 0;
        for (int i = 0; i < 2; i++)
        {
            if (boat[i] == null)
                num++;
        }
        return num;
    }

    void getOnTheBoat(GameObject other)
    {
        if (boatCapacity() != 0)
        {
            other.transform.parent = Boat.transform;
            if (boat[0] == null)
            {
                boat[0] = other;
                other.transform.localPosition = new Vector3(0, 0.5F, -0.3F);

            }
            else
            {
                boat[1] = other;
                other.transform.localPosition = new Vector3(0, 0.5F, 0.3F);
            }
        }
    }

    public void moveBoat()
    {
        if (boatCapacity() != 2)
        {
            if (my.state == State.START)
            {
                my.state = State.SEMOV;
            }
            else if (my.state == State.END)
            {
                my.state = State.ESMOV;
            }
        }
    }

    public void getOffTheBoat(int side)
    {
        if (boat[side] != null)
        {
            boat[side].transform.parent = null;
            if (my.state == State.END)
            {
                if (boat[side].tag == "Angel")
                {
                    Angel_end.Push(boat[side]);
                }
                else if (boat[side].tag == "Devil")
                {
                    Devil_end.Push(boat[side]);
                }
            }
            else if (my.state == State.START)
            {
                if (boat[side].tag == "Angel")
                {
                    Angel_start.Push(boat[side]);
                }
                else if (boat[side].tag == "Devil")
                {
                    Devil_start.Push(boat[side]);
                }
            }
        }
        boat[side] = null;
    }

    public void AngelStartOnBoat()
    {
        if (Angel_start.Count != 0 && boatCapacity() != 0 && my.state == State.START)
            getOnTheBoat(Angel_start.Pop());
    }

    public void AngelEndOnBoat()
    {
        if (Angel_start.Count != 0 && boatCapacity() != 0 && my.state == State.END)
            getOnTheBoat(Angel_start.Pop());
    }

    public void DevilStartOnBoat()
    {
        if (Devil_start.Count != 0 && boatCapacity() != 0 && my.state == State.START)
            getOnTheBoat(Devil_start.Pop());
    }

    public void DevilEndOnBoat()
    {
        if (Devil_start.Count != 0 && boatCapacity() != 0 && my.state == State.END)
            getOnTheBoat(Devil_end.Pop());
    }

    public void check()
    {
        int acon = 0;
        int dcon = 0;
        int angel_s = 0, devil_s = 0, angel_e = 0, devil_e = 0;

        if (Angel_end.Count == 3 && Devil_end.Count == 3)
        {
            my.state = State.WIN;
            return;
        }

        for (int i = 0; i < 2; i++)
        {
            if (boat[i] != null && boat[i].tag == "Angel")
                acon++;
            else if (boat[i] != null && boat[i].tag == "Devil")
                dcon++;
        }

        if (my.state == State.START)
        {
            angel_s = Angel_start.Count + acon;
            devil_s = Devil_start.Count + dcon;
            angel_e = Angel_end.Count;
            devil_e = Devil_end.Count;
        }
        else if (my.state == State.END)
        {
            angel_s = Angel_start.Count;
            devil_s = Devil_start.Count;
            angel_e = Angel_end.Count + acon;
            devil_e = Devil_end.Count + dcon;
        }

        if ((angel_s != 0 && angel_s < devil_s) || (angel_e != 0 && angel_e < devil_e))
        {
            my.state = State.LOSE;
        }
    }
}

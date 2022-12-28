using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    public GameObject originGO;
    public GameObject junctionGO;
    public GameObject northGO;
    public GameObject southGO;

    public Node origin;
    public Node junction;
    public Node north;
    public Node south;
    public Node curNode;

    void Start() 
    {
        originGO = GameObject.Find("Origin");
        junctionGO = GameObject.Find("Junction");
        northGO = GameObject.Find("North");
        southGO = GameObject.Find("South");

        origin = new Node(originGO, "origin");
        junction = new Node(junctionGO, "junction");
        north = new Node(northGO, "north");
        south = new Node(southGO, "south");

        curNode = origin;
        origin.right = junction;
        junction.up = north;
        junction.down = south;
        junction.left = origin;
        north.down = junction;
        south.up = junction;
    }
}

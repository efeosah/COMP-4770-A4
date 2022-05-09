using GameBrains.Actuators.Motion.Navigation.SearchGraph;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Extensions.Vectors;
using GameBrains.GameManagement;
using UnityEngine;
using System;

namespace A4
{
    // TODO A4: Implement and Test automatic generation of a corner graph search space.
    // TODO for A4 (optional): Check all the math formulas. I'm not 100% sure I got them right.
    // TODO for A4: Add appropriate testing.
    
    public class CornerGraphSearchSpace : ExtendedMonoBehaviour
    {
        #region Members and Properties
        
        // Reference the levelInfo. Should be set in the inspector or could set it up in Awake.
        [SerializeField] LevelInfo levelInfo;

        Graph graph;
        const float MaxCast = 42f; // Probably to small to connect all corner waypoints

        #endregion Members and Properties

        #region Start

        public override void Start()
        {
            base.Start();

            graph = FindObjectOfType<Graph>();
            graph.IsLocked = false; // make sure the graph is not locked or you can't add nodes and edges.

            GenerateCornerGraphSearchSpace(); 
        }

        #endregion Start

        #region Members

        public void GenerateCornerGraphSearchSpace()
        {
            if (graph == null || graph.NodeCollection == null)
            {
                Debug.LogWarning("GenerateCornerGraphSearchSpace: Graph missing.");
                return;
            }

            // TODO for A4: Add nodes at corners. Probably call a private helper method to determine the node placement.
            //we start from 1
            //no convex corners at the beginning of the map
            for (int i = 1; i < levelInfo.sizeV - 1; i++)
            {
                for (int j = 1; j < levelInfo.sizeW - 1; j++)
                {
                    //check to see if theres space one "cell" away from this edge
                    if (levelInfo.mapData[i, j] == 1)
                    {
                       
                        if (i + 1 > levelInfo.sizeV || j + 1 > levelInfo.sizeW || i - 1 < 0 || j - 1 < 0)
                        {
                            Debug.Log(i + "<<<>>>" + j);
                            break;
                        }


                        try
                        {

                            //point -> up, left
                            //quadrant 4
                            if (levelInfo.mapData[i - 1, j] == 1
                                && levelInfo.mapData[i, j - 1] == 1
                                && DiagonalCheck(i, j, 4)
                                && levelInfo.mapData[i, j+ 1] == 0
                                && levelInfo.mapData[i + 1, j] == 0)
                            {
                                AddNodeAt(i+4, j+4);
                                Debug.Log(">>>here");
                            }

                            //point -> down, right
                            //quadrant 2
                            if (levelInfo.mapData[i + 1, j] == 1
                                && levelInfo.mapData[i, j + 1] == 1
                                && DiagonalCheck(i, j, 2)
                                && levelInfo.mapData[i, j - 1] == 0
                                && levelInfo.mapData[i - 1, j] == 0)
                            {
                                AddNodeAt(i - 4, j - 4);
                                Debug.Log(">>>here");
                            }

                            //point -> down, left
                            //quadrant 1
                            if (levelInfo.mapData[i + 1, j] == 1
                                && levelInfo.mapData[i, j - 1] == 1
                                && DiagonalCheck(i, j, 1)
                                && levelInfo.mapData[i, j + 1] == 0
                                && levelInfo.mapData[i - 1, j] == 0)
                            {
                                AddNodeAt(i - 4, j + 4);
                                Debug.Log(">>>here");
                            }

                            //point -> up, right
                            //quadrant 3
                            if (levelInfo.mapData[i - 1, j] == 1
                                && levelInfo.mapData[i, j + 1] == 1
                                && DiagonalCheck(i, j, 3)
                                && levelInfo.mapData[i, j - 1] == 0
                                && levelInfo.mapData[i + 1, j] == 0)
                            {
                                AddNodeAt(i + 4, j - 4);
                                Debug.Log(">>>here");
                            }

                            //point -> up
                            //quadrant variant .left side .2
                            if (levelInfo.mapData[i - 1, j] == 0
                                && levelInfo.mapData[i, j - 1] == 0
                                && DiagonalCheck(i, j, 2)
                                && levelInfo.mapData[i, j + 1] == 0
                                && levelInfo.mapData[i + 1, j] == 1)
                            {
                                AddNodeAt(i - 4, j - 4);
                                Debug.Log(">>>here");
                            }
                            //quadrant variant .right side .1
                            if (levelInfo.mapData[i - 1, j] == 0
                                && levelInfo.mapData[i, j - 1] == 0
                                && DiagonalCheck(i, j, 1)
                                && levelInfo.mapData[i, j + 1] == 0
                                && levelInfo.mapData[i + 1, j] == 1)
                            {
                                AddNodeAt(i - 4, j + 4);
                                Debug.Log(">>>here");
                            }

                            //point -> down
                            //quadrant variant .left side .3
                            if (levelInfo.mapData[i + 1, j] == 0
                                && levelInfo.mapData[i, j - 1] == 0
                                && DiagonalCheck(i, j, 3)
                                && levelInfo.mapData[i, j + 1] == 0
                                && levelInfo.mapData[i - 1, j] == 1)
                            {
                                AddNodeAt(i + 4, j - 4);
                                Debug.Log(">>>here");
                            }

                            //quadrant variant .right side .4
                            if (levelInfo.mapData[i + 1, j] == 0
                                && levelInfo.mapData[i, j - 1] == 0
                                && DiagonalCheck(i, j, 4)
                                && levelInfo.mapData[i, j + 1] == 0
                                && levelInfo.mapData[i - 1, j] == 1)
                            {
                                AddNodeAt(i + 4, j + 4);
                                Debug.Log(">>>here");
                            }

                            //point -> left
                            //quadrant variant .top side .1
                            if (levelInfo.mapData[i + 1, j] == 0
                                && levelInfo.mapData[i, j - 1] == 1
                                && DiagonalCheck(i, j, 1)
                                && levelInfo.mapData[i, j + 1] == 0
                                && levelInfo.mapData[i - 1, j] == 0)
                            {
                                AddNodeAt(i - 4, j + 4);
                                Debug.Log(">>>here");
                            }

                            //quadrant variant .bottom side .4
                            if (levelInfo.mapData[i + 1, j] == 0
                                && levelInfo.mapData[i, j - 1] == 1
                                && DiagonalCheck(i, j, 4)
                                && levelInfo.mapData[i, j + 1] == 0
                                && levelInfo.mapData[i - 1, j] == 0)
                            {
                                AddNodeAt(i + 4, j + 4);
                                Debug.Log(">>>here");
                            }


                            //point -> right
                            //quadrant variant .top side .2
                            if (levelInfo.mapData[i + 1, j] == 0
                                && levelInfo.mapData[i, j - 1] == 0
                                && DiagonalCheck(i, j, 2)
                                && levelInfo.mapData[i, j + 1] == 1
                                && levelInfo.mapData[i - 1, j] == 0)
                            {
                                AddNodeAt(i - 4, j - 4);
                                Debug.Log(">>>here");
                            }

                            //quadrant variant .bottom side .3
                            if (levelInfo.mapData[i + 1, j] == 0
                                && levelInfo.mapData[i, j - 1] == 0
                                && DiagonalCheck(i, j, 3)
                                && levelInfo.mapData[i, j + 1] == 1
                                && levelInfo.mapData[i - 1, j] == 0)
                            {
                                AddNodeAt(i + 4, j - 4);
                                Debug.Log(">>>here");
                            }

                        }
                        catch (IndexOutOfRangeException e)
                        {
                            Debug.Log(e);
                            Debug.Log(i + ">>><<<" + j);
                        }


                    }
                }

            }


            //Debug.Log(levelInfo.mapData.Length);


            // I'm going to add edges using RaycastNodes which checks for LOS between nodes.
            // Change the default maximum range to check from 20 to something reasonable.
            Parameters.Instance.NodeCastMaximumDistance = MaxCast;
            graph.NodeCollection.RaycastNodes(); // Creates bidirectional edges between nodes with clear line of sight.

            // TODO for A4 (optional): Or you could manually add edges instead of raycasting. See example in GridSearchSpace.
            //foreach (var fromNode in graph.NodeCollection.Nodes)
            //{
            //    foreach (var toNode in graph.NodeCollection.Nodes)
            //    {
            //        if (fromNode != toNode)
            //        {
            //            if (VectorXZ.Distance(fromNode.Location, toNode.Location) <=
            //                MaxCast)
            //            {
            //                fromNode.AddConnection(toNode);
            //                toNode.AddConnection(fromNode); // if you want bidirectional
            //            }
            //        }
            //    }
            //}
        }

        #endregion Members

        #region Private/Protected Members

        // TODO for A4: You'll probably need some private helper methods here.


        bool DiagonalCheck(int i, int j, int quadrant)
        {

            int count;

            switch (quadrant) {

                case 4:

                    i += 1;
                    j += 1;


                    count = 4;

                    while (count != 0)
                    {
                        if (levelInfo.mapData[i, j] != 0)
                        {
                            return false;
                        }
                        count--;
                    }

                    break;

                case 2:

                    i -= 1;
                    j -= 1;


                    count = 4;

                    while (count != 0)
                    {
                        if (levelInfo.mapData[i, j] != 0)
                        {
                            return false;
                        }
                        count--;
                    }
                    break;


                case 1:

                    i -= 1;
                    j += 1;


                    count = 4;

                    while (count != 0)
                    {
                        if (levelInfo.mapData[i, j] != 0)
                        {
                            return false;
                        }
                        count--;
                    }
                    break;

                case 3:

                    i += 1;
                    j -= 1;


                    count = 4;

                    while (count != 0)
                    {
                        if (levelInfo.mapData[i, j] != 0)
                        {
                            return false;
                        }
                        count--;
                    }
                    break;
            }


            return true;
        }


        void AddNodeAt(int v, int w)
        {
            float x = levelInfo.MapDataVtoX(v);
            float z = levelInfo.MapDataWtoZ(w);
            graph.NodeCollection.AddNode(new VectorXZ(x, z));
        }

        #endregion Private/Protected Members
    }
}
using GameBrains.Actuators.Motion.Navigation.SearchGraph;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Extensions.Vectors;
using GameBrains.GameManagement;
using UnityEngine;

namespace A4
{
    public class GridSearchSpace : ExtendedMonoBehaviour
    {
        #region Members and Properties

        // Size of the cell, e.g., 4x4. Probably should be square.
        [SerializeField]int cellSizeX = 4;
        [SerializeField]int cellSizeZ = 4;

        // Reference the levelInfo. Should be set in the inspector or could set it up in Awake.
        [SerializeField] LevelInfo levelInfo;

        Graph graph;

        #endregion Members and Properties

        #region Start

        public override void Start()
        {
            Debug.Log(gameObject);
            base.Start();

            graph = FindObjectOfType<Graph>();
            graph.IsLocked = false; // make sure the graph is not locked or you can't add nodes and edges.

            GenerateGridSearchSpace(); 
        }

        #endregion Start

        #region Members

        public void GenerateGridSearchSpace()
        {
            // TODO: Check the math here.
            int countX = (int)((levelInfo.sizeV - 1) / 8f - cellSizeX / 2f);
            int countZ = (int)((levelInfo.sizeW - 1) / 8f - cellSizeZ / 2f);
            Debug.Log(countX + ">>>" + countZ);


            for (int x = 0; x <= countX; x += cellSizeX)
            {
                for (int z = 0; z <= countZ; z += cellSizeZ)
                {
                    // Add node at (x, z) if clear. Mirror to all four quadrants. Don't duplicate (0,z) or (x, 0).
                    AddNodeIfClear(x, z);
                    if (z != 0) { AddNodeIfClear(x, -z); }
                    if (x != 0) { AddNodeIfClear(-x, z); }
                    if ( x != 0 && z != 0) { AddNodeIfClear(-x, -z); }
                }
            }

            if (graph != null && graph.NodeCollection != null)
            {
                // I'm going to add edges using RaycastNodes which checks for LOS between nodes.
                // OMG, change the default maximum range to check from 20 to cell size. Otherwise,
                // you will produce some nice art. Go ahead, just for laughs, comment out the next line.
                Parameters.Instance.NodeCastMaximumDistance = Mathf.Max(cellSizeX, cellSizeZ);
                graph.NodeCollection.RaycastNodes(); // Creates bidirectional edges between nodes with clear line of sight.
                
                // You could manually add edges like this (be sure the graph is not locked):
                // foreach (var fromNode in graph.NodeCollection.Nodes)
                // {
                //     foreach (var toNode in graph.NodeCollection.Nodes)
                //     {
                //         if (fromNode != toNode)
                //         {
                //             if (VectorXZ.Distance(fromNode.Location, toNode.Location) <=
                //                 Mathf.Max(cellSizeX, cellSizeZ))
                //             {
                //                 fromNode.AddConnection(toNode);
                //                 toNode.AddConnection(fromNode); // if you want bidirectional
                //             }
                //         }
                //     }
                // }
            }
        }

        void AddNodeIfClear(int x, int z)
        {
            bool clear = CheckIfClearAt(x, z);
            string status = clear ? "clear" : "blocked";
            //Debug.Log($"{(x, z)}: is {status}");

            if (clear && graph != null && graph.NodeCollection != null)
            {
                
                graph.NodeCollection.AddNode(new VectorXZ(x, z));
            }
        }

        #endregion Members

        #region Private/Protected Members

        bool CheckIfClearAt(int x, int z)
        {
            // TODO: CS-check or Math-check this math. I only Engineer-checked it.
            int vMin = levelInfo.MapDataXtoV(x - cellSizeX / 2f);
            int vMax = levelInfo.MapDataXtoV(x + cellSizeX / 2f);
            
            int wMin = levelInfo.MapDataZtoW(z - cellSizeZ / 2f);
            int wMax = levelInfo.MapDataZtoW(z + cellSizeZ / 2f);

            for (int v = vMin; v <= vMax; v++)
            {
                for (int w = wMin; w <= wMax; w++)
                {
                    if (v < 0 || v >= levelInfo.sizeV || w < 0 || w >= levelInfo.sizeW)
                    {
                        // This should happen if we got the math right, but ...
                        Debug.LogError($"CheckIfClearAt(x={x}, z={z}): produced out of range (v={v}, w={w})");
                    }
                    
                    if (levelInfo.mapData[v, w] != 0) // not clear (not ground)
                    {
                        return false;
                    }
                }
            }

            return true; // clear (ground)
        }

        #endregion Private/Protected Members
    }
}
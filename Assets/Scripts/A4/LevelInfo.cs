using System.IO;
using System.Text;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.GameManagement;
using UnityEngine;

namespace A4
{
    public class LevelInfo : ExtendedMonoBehaviour
    {
        // TODO for A4 (optional): Check all the math formulas. I'm not 100% sure I got them right.
        // TODO for A4: Add appropriate testing.
        
        #region Members and Properties

        // Level is 100 x 75. x range is -50..50. z range is -37.5 to 37.5. Outer walls are 0.25 units thick.
        // Want to sample at 0.25 unit intervals so we need 400 x 300 (plus 3 because we include first and last)
        public int sizeV = 403;
        public int sizeW = 303;

        // TODO: Calculate dimensions from level.
        // Currently not used but above dimensions should be automatically calculated from the level.
        public GameObject ground;
        public GameObject level;

        // This is where we store the sample data. We raycast down to the first obstacle. If it is the ground, we
        // assume the 0.25 x 0.25 square around the sample point is clear, though of course a single raycast can't
        // guarantee that. You can always manually edit generated graphs to fix boo-boos.
        public int[,] mapData;

        #endregion Members and Properties

        #region Enable/Disable/Destroy

        public override void OnEnable()
        {
            base.OnEnable();
            
            mapData = new int[sizeV, sizeW];
            CreateMapData();
            
            // Optionally, we store in a text file for debugging or maybe saving level info so maybe we don't
            // have to recreate the map data. This is stored in the project folder, so be careful with the
            // filename so as not to overwrite something.
            StreamWriter writer = new StreamWriter("MapData.txt", false);
            writer.Write(ToString());
            writer.Close();
        }

        #endregion Enable/Disable/Destroy
        
        #region Methods
        
        // Convert to level x-coordinates (-50..50) from map data v-coordinates (0..400).
        public float MapDataVtoX(int v)
        {
            return (2 * v - (sizeV - 1)) / 8f;
        }
        
        // Convert to map data v-coordinates (0..400) from level x-coordinates (-50..50). 
        public int MapDataXtoV(float x)
        {
            return (int)((8 * x + (sizeV - 1)) / 2f);
        }
        
        // Convert to level z-coordinates (-37.5..37.5) from map data w-coordinates (0..300).
        public float MapDataWtoZ(int w)
        {
            return (2 * w - (sizeW - 1)) / 8f;
        }
        
        // Convert to map data w-coordinates (0..300) from level z-coordinates (-37.5..37.5). 
        public int MapDataZtoW(float z)
        {
            return (int)((8 * z + (sizeW - 1)) / 2f);
        }

        // Check if area is clear. Map granularity is 1/4 but waypoints need at least 1 unit clearance.
        public bool IsAreaClear(int centerV, int centerW, int oddSizeV, int oddSizeW)
        {
            if (oddSizeV % 2 == 0) { oddSizeV++; Debug.Log($"Increased oddSizeV to {oddSizeV}."); }
            if (oddSizeW % 2 == 0) { oddSizeW++; Debug.Log($"Increased oddSizeW to {oddSizeW}."); }

            for (int v = centerV - oddSizeV / 2; v <= centerV + oddSizeV / 2; v++)
            {
                for (int w = centerW - oddSizeW / 2; w <= centerW + oddSizeW / 2; w++)
                {
                    if (mapData[v, w] != 0) { return false; }
                }
            }

            return true;
        }

        // Convert the map data to a string for output
        public override string ToString()
        {
            var sb = new StringBuilder();
            
            // Number the columns W=0..sizeW-1
            sb.Append("    ");
            for (int w = 0; w < sizeW; w++)
            {
                sb.Append("W");
            }
            sb.AppendLine();
            sb.Append("    ");
            for (int w = 0; w < sizeW; w++)
            {
                sb.Append(w / 100 % 10);
            }
            sb.AppendLine();
            sb.Append($"    ");
            for (int w = 0; w < sizeW; w++)
            {
                sb.Append(w / 10 % 10);
            }
            sb.AppendLine();
            sb.Append($"    ");
            for (int w = 0; w < sizeW; w++)
            {
                sb.Append(w % 10);
            }
            sb.AppendLine();
            
            for (int v = 0; v < sizeV; v++)
            {
                sb.Append($"V{v:D3}"); // Number rows V=0..sizeV-1
                
                for (int w = 0; w < sizeW; w++)
                {
                    var sample = mapData[v, w];
                    switch (sample)
                    {
                        case 0:
                            sb.Append(" "); // clear
                            break;
                        case 1:
                            sb.Append("x"); // blocked by obstacle
                            break;
                        default:
                            sb.Append("?"); // off map or something weird, probably a bug.
                            break;
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        #endregion Methods

        #region Private and Protected Methods

        void CreateMapData()
        {
            for (int v = 0; v < sizeV; v++)
            {
                for (int w = 0; w < sizeW; w++)
                {
                    var x = MapDataVtoX(v);
                    var z = MapDataWtoZ(w);
                    mapData[v, w] = SampleLevelAt(x, z);
                }
            }
        }
        
        // Use raycast to see if we hit ground, obstacle, or weird at (x,z).
        int SampleLevelAt(float x, float z)
        {
            var distance = 30f;
            var samplePoint = new Vector3(x, distance, z);
            if (Physics.Raycast(samplePoint, Vector3.down, out RaycastHit hit,  2f * distance, Parameters.Instance.ObstacleLayerMask))
            {
                var groundLayerMask = Parameters.Instance.GroundLayerMask;
                var hitLayer = hit.collider.gameObject.layer;
                var hitGround = groundLayerMask == (groundLayerMask | (1 << hitLayer)); // true if hit ground
                return hitGround ? 0 : 1; // clear or blocked
            }

            // TODO: Generalize to reflect cost of terrain.
            // TODO: Is this an error? How did we not hit an obstacle or ground?
            return -1; // outside level or weird
        }

        #endregion Private and Protected Methods
    }
}
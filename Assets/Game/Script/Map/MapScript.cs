
using UnityEngine;
using Game.Script.Attribute;
using Mirror;

namespace Game.Script.Map
{
    
    [RequireComponent(typeof(Grid))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class MapScript : MonoBehaviour
    {
        [Label("原点起始偏移")] public Vector3 originOffset;
        [Label("X方向数量")]public int xGridNum = 100;

        [Label("Y方向数量")]public int yGridNum = 100;

        private Grid _grid;
        private Grid MyGrid
        {
            get
            {
                if (null == _grid)
                {
                    _grid = GetComponent<Grid>();
                }

                return _grid;
            }
        }
        
        public (uint, uint) DeCodeIndex(uint index)
        {
            uint retX = 0;
            uint retY = 0;

            retX = index >> 16;

            retY = index << 16 >> 16;

            return (retX, retY);
        }
        
        
        

        public Vector3 Offset => transform.position + originOffset;

        public (int, int) GetGridIndex(Vector3 worldPos)
        {

            int retX = -1;
            int retY = -1;

            Vector3 o = transform.position + originOffset;
            var cellSize = MyGrid.cellSize;
            
            var offset = (worldPos - o);

            offset.x /= cellSize.x;
            offset.y /= cellSize.y;

            if (offset.x >= 0 && offset.x < xGridNum)
            {
                retX = Mathf.FloorToInt(offset.x);
            }
            
            if (offset.y >= 0 && offset.y < yGridNum)
            {
                retY = Mathf.FloorToInt(offset.y);
            }
            return (retX, retY);
        }
    }
}
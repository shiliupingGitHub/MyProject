
using Cinemachine;
using UnityEngine;
using Game.Script.Attribute;
using Game.Script.Game;
using Mirror;

namespace Game.Script.Map
{
    
    [RequireComponent(typeof(Grid))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class MapScript : MonoBehaviour
    {
        [Label("X方向数量")]public int xGridNum = 100;

        [Label("Y方向数量")]public int yGridNum = 100;

        public bool showGrid = false;

        public CinemachineVirtualCamera virtualCamera;

        private Grid _grid;
        public Grid MyGrid
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

        private void Start()
        {
            GameInstance.Instance.MapScript = this;
        }
        

        public Vector3 Offset => transform.position;

        public (int, int) GetGridIndex(Vector3 worldPos)
        {

            int retX = -1;
            int retY = -1;

            Vector3 o = transform.position ;
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
using System;
using UnityEngine;

namespace Models
{
    public class Map
    {
        // For Adjacency and covers check
        // Calculating the lengths of vector projections to check the Adjacency
        public readonly Vector3[] checkVectorsArrCover = new Vector3[6]
        {
            new Vector3(0.9f, -1f, 0f),
            new Vector3(0.45f, -1f, 0.81f),
            new Vector3(0.45f, -1f, -0.81f),
            new Vector3(-0.9f, -1f, 0f),
            new Vector3(-0.45f, -1f, -0.81f),
            new Vector3(-0.45f, -1f, 0.81f)
        };

        public Tile[,] tiles;

        /// <summary>
        ///     Map Initialization
        /// </summary>
        /// <param name="width"></param>
        /// <param name="length"></param>
        /// <param name="height"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="zOffset"></param>
        public Map(int width, int length, int height, float xOffset, float yOffset, float zOffset)
        {
            tiles = new Tile[width, length];

            for (var i = 0; i < tiles.GetLength(0); i++)
            for (var j = 0; j < tiles.GetLength(1); j++)
                tiles[i, j] = new Tile();

            //LayerMask mask = LayerMask.GetMask("Floor");
            var mask = 1 << 8;
            for (var i = 0; i < tiles.GetLength(0); i++)
            for (var j = 0; j < tiles.GetLength(1); j++)
            {
                var hitInfo = new RaycastHit();
                var hit = Physics.Raycast(
                    GetPixelCoordsByTileIndexes(new Vector3(i, 0, j) + (height + 1) * Vector3.up, xOffset, yOffset,
                        zOffset), Vector3.down, out hitInfo, Mathf.Infinity, mask);
                if (hit)
                {
                    tiles[i, j].height = (int) GetTileIndexesByPixelCoords(hitInfo.point, xOffset, yOffset, zOffset).y;
                    Debug.DrawLine(
                        GetPixelCoordsByTileIndexes(new Vector3(i, tiles[i, j].height, j) + Vector3.up, xOffset,
                            yOffset, zOffset), hitInfo.point, Color.green, 10);
                    tiles[i, j].traversable = true;
                }
                else
                {
                    Debug.DrawRay(
                        GetPixelCoordsByTileIndexes(new Vector3(i, 0, j) + Vector3.up, xOffset, yOffset, zOffset),
                        Vector3.down, Color.red, 10);
                    tiles[i, j].traversable = false;
                }
            }


            for (var i = 0; i < tiles.GetLength(0); i++)
            for (var j = 0; j < tiles.GetLength(1); j++)
                if (tiles[i, j].traversable)
                    for (var k = 0; k < checkVectorsArrCover.Length; ++k)
                    {
                        height = tiles[i, j].height;
                        var hitInfo = new RaycastHit();
                        tiles[i, j].AdjacencyArray[k] = Physics.Raycast(
                            GetPixelCoordsByTileIndexes(new Vector3(i, height, j) + 3 * Vector3.up, xOffset, yOffset,
                                zOffset), checkVectorsArrCover[k], out hitInfo, 2.7f, mask);

                        if (tiles[i, j].AdjacencyArray[k])
                            Debug.DrawLine(
                                GetPixelCoordsByTileIndexes(new Vector3(i, height, j) + 3 * Vector3.up, xOffset,
                                    yOffset, zOffset), hitInfo.point, Color.green, 10);
                        else
                            Debug.DrawRay(
                                GetPixelCoordsByTileIndexes(new Vector3(i, height, j) + 3 * Vector3.up, xOffset,
                                    yOffset, zOffset), checkVectorsArrCover[k], Color.red, 10);
                    }
        }

        /// <summary>
        ///     Returns vector of pixel coordinates where selected tile is located in current map
        /// </summary>
        /// <param name="convertiblePoint"></param>
        /// <returns></returns>
        public Vector3 GetPixelCoordsByTileIndexes(Vector3 convertiblePoint, float xOffset, float yOffset,
            float zOffset)
        {
            var result = new Vector3
            (
                (float) (Math.Sqrt(3) * convertiblePoint.x) + (float) (Math.Sqrt(3) / 2 * convertiblePoint.z) + xOffset,
                (float) 0.5 * convertiblePoint.y + yOffset,
                (float) 1.5 * convertiblePoint.z + zOffset
            );
            return result;
        }

        /// <summary>
        ///     Returns vector of indexes of tile who is located in the selected point
        /// </summary>
        /// <param name="convertiblePoint"></param>
        /// <returns></returns>
        public Vector3 GetTileIndexesByPixelCoords(Vector3 convertiblePoint, float xOffset, float yOffset,
            float zOffset)
        {
            var result = new Vector3
            (
                Convert.ToInt32(Math.Round((convertiblePoint.x * Math.Sqrt(3) - convertiblePoint.z) / 3 - xOffset)),
                Convert.ToInt32(Math.Round(convertiblePoint.y * 2 - yOffset)),
                Convert.ToInt32(Math.Round(convertiblePoint.z * 2 / 3 - zOffset))
            );
            return result;
        }
    }
}
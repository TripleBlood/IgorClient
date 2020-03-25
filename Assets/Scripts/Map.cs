using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Models
{
    public class Map
    {
        // Calculating the lengths of vector projections to check the Adjacency
        private readonly Vector3[] CheckVectorsAdjArr =
        {
            new Vector3(0.45f, -1f, 0.81f), // North
            new Vector3(0.9f, -1f, 0f), // NorthEast
            new Vector3(0.45f, -1f, -0.81f), // SouthEast
            new Vector3(-0.45f, -1f, -0.81f), // South
            new Vector3(-0.9f, -1f, 0f), // SouthWest
            new Vector3(-0.45f, -1f, 0.81f) // NorthWest

            // TODO: Thinking, what is better
            // new Vector3(0f, -4f, 1f), // North
            // new Vector3(1f, -4f, 0f), // NorthEast
            // new Vector3(1f, -4f, -1f), // SouthEast
            // new Vector3(0f, -4f, -1f), // South
            // new Vector3(-1f, -4f, 0f), // SouthWest
            // new Vector3(-1f, -4f, 1f) // NorthWest
        };

        private readonly List<Tile> Tiles;
        private readonly Dictionary<int, Tile[]> TilesAdjArray;

        private int iter;

        /// <summary>
        ///     Map Initialization
        /// </summary>
        /// <param name="width"></param>
        /// <param name="length"></param>
        /// <param name="height"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="zOffset"></param>
        public Map(int width, int length, int height, float xOffset = 0, float yOffset = 0, float zOffset = 0)
        {
            Tiles = new List<Tile>();
            TilesAdjArray = new Dictionary<int, Tile[]>();

            for (var i = 0; i < width; i++)
            for (var j = 0; j < length; j++)
                Tiles.Add(new Tile(i, height + 1, j));

            CheckingTraversibility(xOffset, yOffset, zOffset);
            CreatingAdjList(xOffset, yOffset, zOffset);
        }

        // TODO: Create constructor without width, length and height

        /// <summary>
        ///     Tiles traversibility checking
        /// </summary>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="zOffset"></param>
        public void CheckingTraversibility(float xOffset = 0, float yOffset = 0, float zOffset = 0)
        {
            RaycastHit hitInfo;
            Color color;

            // LayerMask mask = LayerMask.GetMask("Floor");
            // var mask = 1 << 8;

            foreach (var tile in Tiles)
            {
                // Debug.Log(
                // $"traversibility ({tile.CoordX}; {tile.CoordZ})");

                var rayStarting = new Vector3(tile.CoordX, tile.Height, tile.CoordZ) + 2 * Vector3.up;
                tile.Traversable = Physics.Raycast(
                    GetPixelCoordsByTileIndexes(rayStarting, xOffset, yOffset, zOffset),
                    Vector3.down, out hitInfo, Mathf.Infinity);

                if (tile.Traversable)
                {
                    // Debug.Log(
                    // $"hit ({hitInfo.point.x}; {hitInfo.point.y}; {hitInfo.point.z})");

                    tile.Height = (int) GetTileIndexesByPixelCoords(hitInfo.point, xOffset, yOffset, zOffset).y;
                    color = Color.green;
                }
                else
                {
                    hitInfo.point = GetPixelCoordsByTileIndexes(
                        rayStarting + tile.Height * Vector3.down, xOffset, yOffset, zOffset);
                    color = Color.red;
                }

                Debug.DrawLine(
                    GetPixelCoordsByTileIndexes(
                        rayStarting, xOffset, yOffset, zOffset),
                    hitInfo.point, color, 5);
            }
        }

        /// <summary>
        ///     Returns map that is storing tiles adjacency
        /// </summary>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="zOffset"></param>
        public void CreatingAdjList(float xOffset = 0, float yOffset = 0, float zOffset = 0)
        {
            RaycastHit hitInfo;
            Color color;

            // LayerMask mask = LayerMask.GetMask("Floor");
            // var mask = 1 << 8;

            foreach (var tile in Tiles.Where(tile => tile.Traversable))
            {
                var rayStarting = new Vector3(tile.CoordX, tile.Height, tile.CoordZ) + 3 * Vector3.up;

                foreach (var rayVector in CheckVectorsAdjArr)
                {
                    var hit = Physics.Raycast(
                        GetPixelCoordsByTileIndexes(rayStarting, xOffset, yOffset, zOffset),
                        rayVector, out hitInfo, 2.7f);

                    // var hit = Physics.Raycast(
                    //     GetPixelCoordsByTileIndexes(rayStarting, xOffset, yOffset, zOffset),
                    //     GetPixelCoordsByTileIndexes(rayVector, xOffset, yOffset, zOffset), out hitInfo, 2.7f);

                    if (hit)
                    {
                        var neighborCoords = GetTileIndexesByPixelCoords(rayVector, xOffset, yOffset, zOffset);

                        Debug.Log(
                            $"({rayStarting.x}; {rayStarting.z}) + ({rayVector.x}; {rayVector.z}) -> ({neighborCoords.x}; {neighborCoords.z})");

                        try
                        {
                            var i = iter++;
                            var neighborTile = Tiles.Find(t =>
                                t.CoordX == (int) (rayStarting + neighborCoords).x &&
                                t.CoordZ == (int) (rayStarting + neighborCoords).z);

                            TilesAdjArray[i] = new[] {tile, neighborTile};

                            Debug.Log(
                                $"{i} record of adjacency is created ({tile.CoordX}, {tile.CoordZ}) -> ({neighborTile.CoordX}, {neighborTile.CoordZ})");
                        }
                        catch
                        {
                            Debug.Log(
                                $"Пустота - твой новый друг (неть) и сосед (на самом деле мы потеряли соседа ({neighborCoords.x}, {neighborCoords.z}))");
                        }

                        color = Color.green;
                        Debug.Log("");
                    }
                    else
                    {
                        hitInfo.point = GetPixelCoordsByTileIndexes(rayStarting, xOffset, yOffset, zOffset) + rayVector;
                        color = Color.red;
                    }

                    Debug.DrawLine(
                        GetPixelCoordsByTileIndexes(rayStarting, xOffset, yOffset, zOffset),
                        hitInfo.point, color, 5);
                }
            }
        }

        /// <summary>
        ///     Returns vector of pixel coordinates where selected tile is located in current map
        /// </summary>
        /// <param name="convertiblePoint"></param>
        /// <returns></returns>
        public Vector3 GetPixelCoordsByTileIndexes(Vector3 convertiblePoint,
            float xOffset = 0, float yOffset = 0, float zOffset = 0)
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
        public Vector3 GetTileIndexesByPixelCoords(Vector3 convertiblePoint,
            float xOffset = 0, float yOffset = 0, float zOffset = 0)
        {
            var result = new Vector3
            (
                // TODO: I sense smth with x coord might be wrong here...
                (float) Math.Round(((convertiblePoint.x - xOffset) * Math.Sqrt(3) - (convertiblePoint.z - zOffset)) /
                                   3),
                (float) Math.Round(2 * (convertiblePoint.y - yOffset)),
                (float) Math.Round(2 / 3 * (convertiblePoint.z - zOffset))
            );

            // Debug.Log($"{result.x} {result.y} {result.z}");
            return result;
        }
    }
}
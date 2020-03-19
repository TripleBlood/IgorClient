namespace Models
{
    public class Tile
    {
        public bool[] AdjacencyArray = new bool[6];
        // adjacencyMap
        // [
        // 	North		(+1; -1),
        // 	NorthEast	(+1; 0),
        // 	SouthEast	(0; +1),
        // 	South		(-1; +1),
        // 	SouthWest	(-1; 0),
        // 	NorthWest	(0; -1)
        // ]

        public int height;

        public bool traversable;
        // public bool occupied;
        // public GameObject CharacterOnTile;

        public Tile()
        {
            height = 0;
            // occupied = false;
            // CharacterOnTile = null;
        }
    }
}
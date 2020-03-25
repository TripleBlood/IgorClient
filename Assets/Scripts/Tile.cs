namespace Models
{
    public class Tile
    {
        public int CoordX;
        public int CoordZ;
        public int Height; // CoordY

        public bool Traversable;
        // public bool Occupied;
        // public GameObject CharacterOnTile;

        public Tile(int x, int z)
        {
            CoordX = x;
            Height = 0;
            CoordZ = z;
        }

        public Tile(int x, int y, int z)
        {
            CoordX = x;
            Height = y;
            CoordZ = z;
        }
    }
}
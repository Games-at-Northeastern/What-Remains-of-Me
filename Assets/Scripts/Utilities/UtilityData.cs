namespace UtilityData
{
    using UnityEngine;
    using System.Collections.Generic;

    #region 2D Iteration

    public class UData
    {
        public static Vector3Int[] NeighbourAdjacents() => new Vector3Int[]
            {
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.left,
            Vector3Int.down
            };

        public static Vector3Int[] NeighbourDiagonals() => new Vector3Int[]
            {
            Vector3Int.up + Vector3Int.right,
            Vector3Int.up + Vector3Int.left,
            Vector3Int.down + Vector3Int.right,
            Vector3Int.down + Vector3Int.left
            };

        public static Vector3Int[] NeighbourAdjacentDiagonals() => new Vector3Int[]
            {
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.up + Vector3Int.right,
            Vector3Int.up + Vector3Int.left,
            Vector3Int.down + Vector3Int.right,
            Vector3Int.down + Vector3Int.left
            };
    }

    #endregion
    public class Vector3IntSet : HashSet<Vector3Int>
    {
        public class V3IComparer : IEqualityComparer<Vector3Int>
        {
            public bool Equals(Vector3Int a, Vector3Int b) => GetHashCode(a) == GetHashCode(b);

            public int GetHashCode(Vector3Int v3i)
            {
                int hash = 0;
                hash = AppendInts(hash, MapToPositive(v3i.x));
                hash = AppendInts(hash, MapToPositive(v3i.y));
                hash = AppendInts(hash, MapToPositive(v3i.z));
                return hash;
            }

            private int AppendInts(int a, int b)
            {
                int digitsB = Mathf.FloorToInt(Mathf.Log10(b)) + 1;
                return (a * 10 * digitsB) + b;
            }

            private int MapToPositive(int num)
            {
                if (num < 0)
                {
                    return num * -2;
                }
                return 1 + ((num - 1) * 2);
            }
        }

        public Vector3IntSet() : base(new V3IComparer()) { }
        public Vector3IntSet(HashSet<Vector3Int> copy) : base(copy, new V3IComparer()) { }
    }
}

using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "2D/Tile Palette/Custom Tile")]
[System.Serializable]
public class CustomTile : Tile
{
    [SerializeField]
    public CustomTileSide
    TopSide,
    BottomSide,
    LeftSide,
    RightSide;
    [SerializeField] public string Name;

    public CustomTile(Sprite sprite, string name = "Custom tile")
    {
        base.sprite = sprite;
        TopSide = new();
        BottomSide = new();
        RightSide = new();
        LeftSide = new();
        Name = name;
    }

    // [other]
    //        [this]
    public bool CanGoBelowRightOfOther(CustomTile other)
    {
        if (other == null || this == null)
        {
            return true;
        }
        return CanGoDiagonal(TopSide, LeftSide, other.BottomSide, other.RightSide);
    }

    // [other]
    // [this]
    public bool CanGoBelowOther(CustomTile other)
    {
        if (other == null || this == null)
        {
            return true;
        }
        return TopSide.CanGoNextTo(other.BottomSide);
    }

    //      [other]
    // [this]
    public bool CanGoBelowLeftOfOther(CustomTile other)
    {
        if (other == null || this == null)
        {
            return true;
        }
        if (!other.GetType().Equals(GetType()))
        {
            return false;
        }
        return CanGoDiagonal(new(TopSide, true), RightSide, new(other.BottomSide, true), other.LeftSide);
    }

    // [other] [this]
    public bool CanGoRightOfOther(CustomTile other)
    {
        if (other == null || this == null)
        {
            return true;
        }
        return LeftSide.CanGoNextTo(other.RightSide);
    }

    // [this] [other]
    public bool CanGoLeftOfOther(CustomTile other)
    {
        if (other == null || this == null)
        {
            return true;
        }
        return RightSide.CanGoNextTo(other.LeftSide);
    }

    //       [this]
    // [other]
    public bool CanGoAboveRightOfOther(CustomTile other)
    {
        if (other == null || this == null)
        {
            return true;
        }
        return CanGoDiagonal(BottomSide, new(LeftSide, true), other.TopSide, new(other.RightSide, true));
    }

    // [this]
    // [other]
    public bool CanGoAboveOther(CustomTile other)
    {
        if (other == null || this == null)
        {
            return true;
        }
        return BottomSide.CanGoNextTo(other.TopSide);
    }

    // [this]
    //      [other]
    public bool CanGoAboveLeftOfOther(CustomTile other)
    {
        if (other == null || this == null)
        {
            return true;
        }
        return CanGoDiagonal(new(BottomSide, true), new(RightSide, true), new(other.TopSide, true), new(other.LeftSide, true));
    }

    // [other]
    //       [this]
    private static bool CanGoDiagonal(CustomTileSide thisTopSide, CustomTileSide thisLeftSide,
        CustomTileSide otherBottomSide, CustomTileSide otherRightSide)
    {
        if (otherBottomSide.FullBetween2ndAnd3rd && otherRightSide.FullBetween2ndAnd3rd)
        {
            if (otherBottomSide.Has3rdConnection)
            {
                return !thisTopSide.FullBetween1stAnd2nd
                    && !thisLeftSide.FullBetween1stAnd2nd
                    && !thisLeftSide.Has1stConnection;
            }
            else
            {
                return thisTopSide.FullBetween1stAnd2nd
                    && thisLeftSide.FullBetween1stAnd2nd
                    && !thisLeftSide.Has1stConnection;
            }
        }
        else
        {
            return !(thisTopSide.FullBetween1stAnd2nd
                    && thisLeftSide.FullBetween1stAnd2nd
                    && !thisLeftSide.Has1stConnection);
        }
    }


    [System.Serializable]
    public class CustomTileSide
    {
        [SerializeField]
        public bool
        Has1stConnection = false,
        FullBetween1stAnd2nd = true,
        Has2ndConnection = false,
        FullBetween2ndAnd3rd = true,
        Has3rdConnection = false;

        public CustomTileSide()
        {

        }

        internal CustomTileSide(CustomTileSide reference, bool invert = false)
        {
            if (invert)
            {
                Has1stConnection = reference.Has3rdConnection;
                FullBetween1stAnd2nd = reference.FullBetween2ndAnd3rd;
                Has2ndConnection = reference.Has2ndConnection;
                FullBetween2ndAnd3rd = reference.FullBetween1stAnd2nd;
                Has3rdConnection = reference.Has1stConnection;
            }
            else
            {
                Has1stConnection = reference.Has1stConnection;
                FullBetween1stAnd2nd = reference.FullBetween1stAnd2nd;
                Has2ndConnection = reference.Has2ndConnection;
                FullBetween2ndAnd3rd = reference.FullBetween2ndAnd3rd;
                Has3rdConnection = reference.Has3rdConnection;
            }
        }

        public bool CanGoNextTo(CustomTileSide other)
        {
            // make sure FullBetween1stAnd2nd and FullBetween2ndAnd3rd are equal
            if (FullBetween1stAnd2nd != other.FullBetween1stAnd2nd
                || FullBetween2ndAnd3rd != other.FullBetween2ndAnd3rd)
            {
                return false;
            }
            // continue knowing that FullBetween1stAnd2nd and FullBetween2ndAnd3rd are equal
            // if this is full between both 1 & 2 AND 2 & 3, all connections must be equal
            if (BothSectionsAreFull())
            {
                return Has1stConnection == other.Has1stConnection
                    && Has2ndConnection == other.Has2ndConnection
                    && Has3rdConnection == other.Has3rdConnection;
            }
            // if this is empty between both 1 & 2 AND 2 & 3, connections don't matter
            else if (BothSectionsAreEmpty())
            {
                return true;
            }
            // now left with 2nd-connection possiblities only
            // check for equal connections if section is full
            else if (FullBetween1stAnd2nd)
            {
                return Has1stConnection == other.Has1stConnection;
            }
            else if (FullBetween2ndAnd3rd)
            {
                return Has3rdConnection == other.Has3rdConnection;
            }
            return false;
        }

        public bool BothSectionsAreFull() =>
            FullBetween1stAnd2nd && FullBetween2ndAnd3rd;

        public bool BothSectionsAreEmpty() =>
            !FullBetween1stAnd2nd && !FullBetween2ndAnd3rd;
    }
}
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
    topBorderTile,
    sxBorderTile,
    dxBorderTile,
    botBorderTile,

    topSxangleTile,
    topDxangleTile,
    botSxangleTile,
    botDxangleTile,

    #region external angles

    exTopSxangleTile,
    exTopDxangleTile,
    exBotSxangleTile,
    exBotDxangleTile,

    /*
    *  o x x o
    *  x x x x 
    *  x x x x
    *  x x x x
    */
    exDoubleTopAngleTile,

    /*
    *  o x x x
    *  x x x x 
    *  x x x x
    *  o x x x
    */
    exDoubleLeftAngleTile,

    /*
    *  x x x o
    *  x x x x 
    *  x x x x
    *  x x x o
    */
    exDoubleRightAngleTile,

    /*
    *  x x x x
    *  x x x x 
    *  x x x x
    *  o x x o
    */
    exDoubleBotAngleTile,

    /*
     *  o x x x
     *  x x x x 
     *  x x x x
     *  x x x o
     */
    exDoubleTopRightAngleTile,

    /*
    *  x x x o
    *  x x x x 
    *  x x x x
    *  o x x x
    */
    exDoubleBotRightAngleTile,

    //Triple point
    exTripleTopSxAngleTile,
    exTripleTopDxAngleTile,
    exTripleBotSxAngleTile,
    exTripleBopDxAngleTile,


    //Quadruple point
    exAllAngleTile,

    #endregion

    #region borders + intern angles
    //Single angle
    topBorderLeftAngleTile,
    topBorderRightAngleTile,

    leftBorderTopAngleTile,
    leftBorderBotAngleTile,

    rightBorderTopAngleTile,
    rightBorderBotAngleTile,

    botBorderLeftAngleTile,
    botBorderRightAngleTile,

    //Double angle
    topBorderBotAnglesTile,
    leftBorderRightAnglesTile,
    rightBorderLeftAnglesTile,
    botBorderTopAnglesTile,

    #endregion

    #region angles + intern angles
    topSxangleAndAngleTile,
    topDxangleAndAngleTile,
    botSxangleAndAngleTile,
    botDxangleAndAngleTile,

    #endregion

    uUpTile,
    uBotTile,
    uLeftTile,
    uRightTile,

    floorTile,
    soloTile
}

public class TileTesting : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap collTilemap;

    public TileBase sxSide;
    public TileBase dxSide;
    public TileBase topSide;
    public TileBase botSide;

    public TileBase botSXAngle;
    public TileBase botDXAngle;

    public TileBase door;
    public TileBase floor;
    public TileBase exBotSXAngle;
    public TileBase exBotDXAngle;

    public int quadtype;
    private int[] indeces;

    #region borders
    int[] topBorderTiles = new int[] { 2, 2, 2, 2, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 };
    int[] sxBorderTiles = new int[] { 0, 6, 6, 6, 0, 6, 6, 6, 0, 6, 6, 6, 0, 6, 6, 6 };
    int[] dxBorderTiles = new int[] { 6, 6, 6, 1, 6, 6, 6, 1, 6, 6, 6, 1, 6, 6, 6, 1 };
    int[] botBorderTiles = new int[] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 3, 3, 3, 3 };
    #endregion

    #region angles
    int[] topSxangleTiles = new int[] { 0, 2, 2, 2, 0, 6, 6, 6, 0, 6, 6, 6, 0, 6, 6, 6 };
    int[] topDxangleTiles = new int[] { 2, 2, 2, 1, 6, 6, 6, 1, 6, 6, 6, 1, 6, 6, 6, 1 };
    int[] botSxangleTiles = new int[] { 0, 6, 6, 6, 0, 6, 6, 6, 0, 6, 6, 6, 4, 3, 3, 3 };
    int[] botDxangleTiles = new int[] { 6, 6, 6, 1, 6, 6, 6, 1, 6, 6, 6, 1, 3, 3, 3, 5 };
    #endregion

    #region external angles
    //Single point
    int[] exTopSxangleTiles = new int[] { 2, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 };
    int[] exTopDxangleTiles = new int[] { 6, 6, 6, 2, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 };
    int[] exBotSxangleTiles = new int[] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 6 };
    int[] exBotDxangleTiles = new int[] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 8 };

    //Double point

    /*
    *  o x x o
    *  x x x x 
    *  x x x x
    *  x x x x
    */
    int[] exDoubleTopAngleTiles = new int[] { 2, 6, 6, 2, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 };

    /*
    *  o x x x
    *  x x x x 
    *  x x x x
    *  o x x x
    */
    int[] exDoubleLeftAngleTiles = new int[] { 2, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 6 };

    /*
    *  x x x o
    *  x x x x 
    *  x x x x
    *  x x x o
    */
    int[] exDoubleRightAngleTiles = new int[] { 6, 6, 6, 2, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 8 };

    /*
    *  x x x x
    *  x x x x 
    *  x x x x
    *  o x x o
    */
    int[] exDoubleBotAngleTiles = new int[] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 8 };

    /*
     *  o x x x
     *  x x x x 
     *  x x x x
     *  x x x o
     */
    int[] exDoubleTopRightAngleTiles = new int[] { 2, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 8 };

    /*
    *  x x x o
    *  x x x x 
    *  x x x x
    *  o x x x
    */
    int[] exDoubleBotRightAngleTiles = new int[] { 6, 6, 6, 2, 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 6 };

    //Triple point
    int[] exTripleTopSxAngleTiles = new int[] { 2, 6, 6, 2, 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 6 };
    int[] exTripleTopDxAngleTiles = new int[] { 2, 6, 6, 2, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 8 };
    int[] exTripleBotSxAngleTiles = new int[] { 2, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 8 };
    int[] exTripleBopDxAngleTiles = new int[] { 6, 6, 6, 2, 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 8 };


    //Quadruple point
    int[] exAllAngleTiles = new int[] { 2, 6, 6, 2, 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 8 };

    #endregion

    #region borders + intern angles
    //Single angle
    int[] topBorderLeftAngleTiles = new int[] { 2, 2, 2, 2, 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 6 };
    int[] topBorderRightAngleTiles = new int[] { 2, 2, 2, 2, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 8 };

    int[] leftBorderTopAngleTiles = new int[] { 0, 6, 6, 2, 0, 6, 6, 6, 0, 6, 6, 6, 0, 6, 6, 6 };
    int[] leftBorderBotAngleTiles = new int[] { 0, 6, 6, 6, 0, 6, 6, 6, 0, 6, 6, 6, 0, 6, 6, 8 };

    int[] rightBorderTopAngleTiles = new int[] { 2, 6, 6, 1, 6, 6, 6, 1, 6, 6, 6, 1, 6, 6, 6, 1 };
    int[] rightBorderBotAngleTiles = new int[] { 6, 6, 6, 1, 6, 6, 6, 1, 6, 6, 6, 1, 7, 6, 6, 1 };

    int[] botBorderLeftAngleTiles = new int[] { 2, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 3, 3, 3, 3 };
    int[] botBorderRightAngleTiles = new int[] { 6, 6, 6, 2, 6, 6, 6, 6, 6, 6, 6, 6, 3, 3, 3, 3 };

    //Double angle
    int[] topBorderBotAnglesTiles = new int[] { 2, 2, 2, 2, 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 8 };
    int[] leftBorderRightAnglesTiles = new int[] { 0, 6, 6, 2, 0, 6, 6, 6, 0, 6, 6, 6, 0, 6, 6, 8 };
    int[] rightBorderLeftAnglesTiles = new int[] { 2, 6, 6, 1, 6, 6, 6, 1, 6, 6, 6, 1, 7, 6, 6, 1 };
    int[] botBorderTopAnglesTiles = new int[] { 2, 6, 6, 2, 6, 6, 6, 6, 6, 6, 6, 6, 3, 3, 3, 3 };

    #endregion

    #region angles + intern angles
    int[] topSxangleAndAngleTiles = new int[] { 0, 2, 2, 2, 0, 6, 6, 6, 0, 6, 6, 6, 0, 6, 6, 8 };
    int[] topDxangleAndAngleTiles = new int[] { 2, 2, 2, 1, 6, 6, 6, 1, 6, 6, 6, 1, 7, 6, 6, 1 };
    int[] botSxangleAndAngleTiles = new int[] { 0, 6, 6, 2, 0, 6, 6, 6, 0, 6, 6, 6, 4, 3, 3, 3 };
    int[] botDxangleAndAngleTiles = new int[] { 2, 6, 6, 1, 6, 6, 6, 1, 6, 6, 6, 1, 3, 3, 3, 5 };

    #endregion

    #region U tiles
    int[] uUpTiles = new int[] { 0, 6, 6, 1, 0, 6, 6, 1, 0, 6, 6, 1, 4, 3, 3, 5 };
    int[] uBotTiles = new int[] { 0, 2, 2, 1, 0, 6, 6, 1, 0, 6, 6, 1, 0, 6, 6, 1 };
    int[] uLeftTiles = new int[] { 2, 2, 2, 1, 6, 6, 6, 1, 6, 6, 6, 1, 3, 3, 3, 5 };
    int[] uRightTiles = new int[] { 0, 2, 2, 2, 0, 6, 6, 6, 0, 6, 6, 6, 4, 3, 3, 3 };
    #endregion

    int[] soloTiles = new int[] { 0, 2, 2, 1, 0, 6, 6, 1, 0, 6, 6, 1, 4, 3, 3, 5 };
    int[] insideTiles = new int[] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 };

    // Start is called before the first frame update
    void Start()
    {
        TileBase[] tiles = new TileBase[] { sxSide, dxSide, topSide, botSide, botSXAngle, botDXAngle, floor, exBotSXAngle, exBotDXAngle };

        setIndeces();

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.transform.position = Vector3.one;
        go.transform.localScale = Vector2.one * 4;

        float x = -1.5f;
        float y = 1.5f;

        int cont = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(go.transform.position.x + x, go.transform.position.y + y));
                if (indeces[cont] != 7)
                    tilemap.SetTile(cellPosition, tiles[indeces[cont]]);
                else
                    collTilemap.SetTile(cellPosition, tiles[indeces[cont]]);

                x++;
                cont++;
            }
            x = -1.5f;
            y--;
        }
    }

    public void setIndeces()
    {
        if (quadtype == 0)
            indeces = topBorderTiles;
        else if (quadtype == 1)
            indeces = sxBorderTiles;
        else if (quadtype == 2)
            indeces = dxBorderTiles;
        else if (quadtype == 3)
            indeces = botBorderTiles;
        else if (quadtype == 4)
            indeces = insideTiles;
        else if (quadtype == 5)
            indeces = topSxangleTiles;
        else if (quadtype == 6)
            indeces = topDxangleTiles;
        else if (quadtype == 7)
            indeces = botSxangleTiles;
        else if (quadtype == 8)
            indeces = botDxangleTiles;
        else if (quadtype == 9)
            indeces = uUpTiles;
        else if (quadtype == 10)
            indeces = uBotTiles;
        else if (quadtype == 11)
            indeces = uLeftTiles;
        else if (quadtype == 12)
            indeces = uRightTiles;
        else if (quadtype == 13)
            indeces = soloTiles;
        else if (quadtype == 14)
            indeces = exTopSxangleTiles;
        else if (quadtype == 15)
            indeces = exTopDxangleTiles;
        else if (quadtype == 16)
            indeces = exBotSxangleTiles;
        else if (quadtype == 17)
            indeces = exBotDxangleTiles;
    }
}

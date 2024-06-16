using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public enum TileType : int
{
    floorTile = 0,
    soloTile = 1,

    topBorderTile = 2,
    sxBorderTile = 3,
    dxBorderTile = 4,
    botBorderTile = 5,

    topSxangleTile = 6,
    topDxangleTile = 7,
    botSxangleTile = 8,
    botDxangleTile = 9,

    #region external angles

    exTopSxangleTile = 10,
    exTopDxangleTile = 11,
    exBotSxangleTile = 12,
    exBotDxangleTile = 13,

    /*
    *  o x x o
    *  x x x x 
    *  x x x x
    *  x x x x
    */
    exDoubleTopAngleTile = 14,

    /*
    *  o x x x
    *  x x x x 
    *  x x x x
    *  o x x x
    */
    exDoubleLeftAngleTile = 15,

    /*
    *  x x x o
    *  x x x x 
    *  x x x x
    *  x x x o
    */
    exDoubleRightAngleTile = 16,

    /*
    *  x x x x
    *  x x x x 
    *  x x x x
    *  o x x o
    */
    exDoubleBotAngleTile = 17,

    /*
     *  o x x x
     *  x x x x 
     *  x x x x
     *  x x x o
     */
    exDoubleTopRightAngleTile = 18,

    /*
    *  x x x o
    *  x x x x 
    *  x x x x
    *  o x x x
    */
    exDoubleBotRightAngleTile = 19,

    //Triple point
    exTripleTopSxAngleTile = 20,
    exTripleTopDxAngleTile = 21,
    exTripleBotSxAngleTile = 22,
    exTripleBopDxAngleTile = 23,


    //Quadruple point
    exAllAngleTile = 24,

    #endregion

    #region borders + intern angles
    //Single angle
    topBorderLeftAngleTile = 25,
    topBorderRightAngleTile = 26,

    leftBorderTopAngleTile = 27,
    leftBorderBotAngleTile = 28,

    rightBorderTopAngleTile = 29,
    rightBorderBotAngleTile = 30,

    botBorderLeftAngleTile = 31,
    botBorderRightAngleTile = 32,

    //Double angle
    topBorderBotAnglesTile = 33,
    leftBorderRightAnglesTile = 34,
    rightBorderLeftAnglesTile = 35,
    botBorderTopAnglesTile = 36,

    #endregion

    #region angles + intern angles
    topSxangleAndAngleTile = 37,
    topDxangleAndAngleTile = 38,
    botSxangleAndAngleTile = 39,
    botDxangleAndAngleTile = 40,

    #endregion

    #region border+border

    topbotborder = 41,
    leftrightborder = 42,

    #endregion

    uUpTile = 43,
    uLeftTile = 44,
    uRightTile = 45,
    uBotTile = 46
}

public class TileSetting : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap doorTilemap;
    public Tilemap collTilemap;

    public TileBase sxSide;
    public TileBase dxSide;
    public TileBase topSide;
    public TileBase botSide;

    public TileBase botSXAngle;
    public TileBase botDXAngle;

    public TileBase door;
    public TileBase verticalDoor;
    public TileBase floor;
    public TileBase exBotSXAngle;
    public TileBase exBotDXAngle;

    private TileBase[] tiles;

    private int[] indeces;
    private int[][] indecesMatrix;

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
    int[] exTripleBotDxAngleTiles = new int[] { 6, 6, 6, 2, 6, 6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 8 };


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
    /*
     * o o o o
     * o x x x
     * o x x x
     * o x x o
     */
    int[] topSxangleAndAngleTiles = new int[] { 0, 2, 2, 2, 0, 6, 6, 6, 0, 6, 6, 6, 0, 6, 6, 8 };
    int[] topDxangleAndAngleTiles = new int[] { 2, 2, 2, 1, 6, 6, 6, 1, 6, 6, 6, 1, 7, 6, 6, 1 };
    int[] botSxangleAndAngleTiles = new int[] { 0, 6, 6, 2, 0, 6, 6, 6, 0, 6, 6, 6, 4, 3, 3, 3 };
    int[] botDxangleAndAngleTiles = new int[] { 2, 6, 6, 1, 6, 6, 6, 1, 6, 6, 6, 1, 3, 3, 3, 5 };

    #endregion

    #region border+border
    int[] topBotTiles = new int[] { 2, 2, 2, 2, 6, 6, 6, 6, 6, 6, 6, 6, 3, 3, 3, 3 };
    int[] leftRightTiles = new int[] { 0, 6, 6, 1, 0, 6, 6, 1, 0, 6, 6, 1, 0, 6, 6, 1 };
    #endregion

    #region U tiles
    int[] uUpTiles = new int[] { 0, 6, 6, 1, 0, 6, 6, 1, 0, 6, 6, 1, 4, 3, 3, 5 };
    int[] uBotTiles = new int[] { 0, 2, 2, 1, 0, 6, 6, 1, 0, 6, 6, 1, 0, 6, 6, 1 };
    int[] uLeftTiles = new int[] { 2, 2, 2, 1, 6, 6, 6, 1, 6, 6, 6, 1, 3, 3, 3, 5 };
    int[] uRightTiles = new int[] { 0, 2, 2, 2, 0, 6, 6, 6, 0, 6, 6, 6, 4, 3, 3, 3 };
    #endregion

    int[] soloTiles = new int[] { 0, 2, 2, 1, 0, 6, 6, 1, 0, 6, 6, 1, 4, 3, 3, 5 };
    int[] floorTiles = new int[] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 };


    private void Awake()
    {
        tiles = new TileBase[] { sxSide, dxSide, topSide, botSide, botSXAngle, botDXAngle, floor, exBotSXAngle, exBotDXAngle };

        indecesMatrix = new int[47][];

        indecesMatrix[0] = floorTiles;
        indecesMatrix[1] = soloTiles;
        indecesMatrix[2] = topBorderTiles;
        indecesMatrix[3] = sxBorderTiles;
        indecesMatrix[4] = dxBorderTiles;
        indecesMatrix[5] = botBorderTiles;
        indecesMatrix[6] = topSxangleTiles;
        indecesMatrix[7] = topDxangleTiles;
        indecesMatrix[8] = botSxangleTiles;
        indecesMatrix[9] = botDxangleTiles;
        indecesMatrix[10] = exTopSxangleTiles;
        indecesMatrix[11] = exTopDxangleTiles;
        indecesMatrix[12] = exBotSxangleTiles;
        indecesMatrix[13] = exBotDxangleTiles;
        indecesMatrix[14] = exDoubleTopAngleTiles;
        indecesMatrix[15] = exDoubleLeftAngleTiles;
        indecesMatrix[16] = exDoubleRightAngleTiles;
        indecesMatrix[17] = exDoubleBotAngleTiles;
        indecesMatrix[18] = exDoubleTopRightAngleTiles;
        indecesMatrix[19] = exDoubleBotRightAngleTiles;
        indecesMatrix[20] = exTripleTopSxAngleTiles;
        indecesMatrix[21] = exTripleTopDxAngleTiles;
        indecesMatrix[22] = exTripleBotSxAngleTiles;
        indecesMatrix[23] = exTripleBotDxAngleTiles;
        indecesMatrix[24] = exAllAngleTiles;
        indecesMatrix[25] = topBorderLeftAngleTiles;
        indecesMatrix[26] = topBorderRightAngleTiles;
        indecesMatrix[27] = leftBorderTopAngleTiles;
        indecesMatrix[28] = leftBorderBotAngleTiles;
        indecesMatrix[29] = rightBorderTopAngleTiles;
        indecesMatrix[30] = rightBorderBotAngleTiles;
        indecesMatrix[31] = botBorderLeftAngleTiles;
        indecesMatrix[32] = botBorderRightAngleTiles;
        indecesMatrix[33] = topBorderBotAnglesTiles;
        indecesMatrix[34] = leftBorderRightAnglesTiles;
        indecesMatrix[35] = rightBorderLeftAnglesTiles;
        indecesMatrix[36] = botBorderTopAnglesTiles;
        indecesMatrix[37] = topSxangleAndAngleTiles;
        indecesMatrix[38] = topDxangleAndAngleTiles;
        indecesMatrix[39] = botSxangleAndAngleTiles;
        indecesMatrix[40] = botDxangleAndAngleTiles;
        indecesMatrix[41] = topBotTiles;
        indecesMatrix[42] = leftRightTiles;
        indecesMatrix[43] = uUpTiles;
        indecesMatrix[44] = uLeftTiles;
        indecesMatrix[45] = uRightTiles;
        indecesMatrix[46] = uBotTiles;
    }

    public void setTiles(Vector2 quadPosition, TileType type)
    {
        indeces = indecesMatrix[(int)type];

        float x = -1.5f;
        float y = 1.5f;

        int cont = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(quadPosition.x + x, quadPosition.y + y));
                if (indeces[cont] == 6)
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

    public void setDoorTile(Vector2 position, bool vertical)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(position.x, position.y));

        if (vertical)
            doorTilemap.SetTile(cellPosition, verticalDoor);
        else
            doorTilemap.SetTile(cellPosition, door);

    }

    public void createCorridor(Vector2 position, bool vertical)
    {
        if (vertical)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(position.x - 1, position.y));
            collTilemap.SetTile(cellPosition, sxSide);
            cellPosition = tilemap.WorldToCell(new Vector3(position.x, position.y));
            tilemap.SetTile(cellPosition, floor);
            cellPosition = tilemap.WorldToCell(new Vector3(position.x + 1, position.y));
            collTilemap.SetTile(cellPosition, dxSide);
        }
        else
        {
            Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(position.x, position.y + 1));
            collTilemap.SetTile(cellPosition, topSide);
            cellPosition = tilemap.WorldToCell(new Vector3(position.x, position.y));
            tilemap.SetTile(cellPosition, floor);
            cellPosition = tilemap.WorldToCell(new Vector3(position.x, position.y - 1));
            collTilemap.SetTile(cellPosition, botSide);
        }
    }

    public void changeDoorWalls(Vector2 doorPosition, int type)
    {
        Vector3Int cellPosition;
        if (type == 1)
        {
            cellPosition = tilemap.WorldToCell(new Vector3(doorPosition.x - 1, doorPosition.y));
            collTilemap.SetTile(cellPosition, exBotSXAngle);

            cellPosition = tilemap.WorldToCell(doorPosition);
            tilemap.SetTile(cellPosition, floor);
            collTilemap.SetTile(cellPosition, null);

            cellPosition = tilemap.WorldToCell(new Vector3(doorPosition.x + 1, doorPosition.y));
            collTilemap.SetTile(cellPosition, exBotDXAngle);
        }
        else if (type == 2)
        {

            cellPosition = tilemap.WorldToCell(new Vector3(doorPosition.x, doorPosition.y + 1));
            collTilemap.SetTile(cellPosition, topSide);

            cellPosition = tilemap.WorldToCell(doorPosition);
            tilemap.SetTile(cellPosition, floor);
            collTilemap.SetTile(cellPosition, null);

            cellPosition = tilemap.WorldToCell(new Vector3(doorPosition.x, doorPosition.y - 1));
            collTilemap.SetTile(cellPosition, exBotDXAngle);
        }
        else if (type == 3)
        {
            cellPosition = tilemap.WorldToCell(new Vector3(doorPosition.x, doorPosition.y + 1));
            collTilemap.SetTile(cellPosition, topSide);

            cellPosition = tilemap.WorldToCell(doorPosition);
            tilemap.SetTile(cellPosition, floor);
            collTilemap.SetTile(cellPosition, null);

            cellPosition = tilemap.WorldToCell(new Vector3(doorPosition.x, doorPosition.y - 1));
            collTilemap.SetTile(cellPosition, exBotSXAngle);
        }
        else
        {
            cellPosition = tilemap.WorldToCell(doorPosition);
            tilemap.SetTile(cellPosition, floor);
            collTilemap.SetTile(cellPosition, null);
        }

    }


}
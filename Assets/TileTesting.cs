using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.WSA;

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

    public TileBase exTopSXAngle;
    public TileBase exTopDXAngle;
    public TileBase exBotSXAngle;
    public TileBase exBotDXAngle;

    public int quadtype;
    private int[] indeces;

    int[] topBorderTiles = new int[] { 2, 2, 2, 2, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 };
    int[] sxBorderTiles = new int[] { 2, 2, 2, 2, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 };
    int[] dxBorderTiles = new int[] { 2, 2, 2, 2, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 };
    int[] botBorderTiles = new int[] { 2, 2, 2, 2, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 };


    int[] topSxangleTiles = new int[] { 0, 2, 2, 2, 0, 7, 7, 7, 0, 7, 7, 7, 0, 7, 7, 7 };

    int[] topDxangleTiles = new int[] { 0, 2, 2, 2, 0, 7, 7, 7, 0, 7, 7, 7, 0, 7, 7, 7 };
    int[] botSxangleTiles = new int[] { 0, 2, 2, 2, 0, 7, 7, 7, 0, 7, 7, 7, 0, 7, 7, 7 };
    int[] botDxangleTiles = new int[] { 0, 2, 2, 2, 0, 7, 7, 7, 0, 7, 7, 7, 0, 7, 7, 7 };

    int[] exTopSxangleTiles = new int[] { 0, 2, 2, 2, 0, 7, 7, 7, 0, 7, 7, 7, 0, 7, 7, 7 };
    int[] exTopDxangleTiles = new int[] { 0, 2, 2, 2, 0, 7, 7, 7, 0, 7, 7, 7, 0, 7, 7, 7 };
    int[] exBotSxangleTiles = new int[] { 0, 2, 2, 2, 0, 7, 7, 7, 0, 7, 7, 7, 0, 7, 7, 7 };
    int[] exBotDxangleTiles = new int[] { 0, 2, 2, 2, 0, 7, 7, 7, 0, 7, 7, 7, 0, 7, 7, 7 };


    int[] uUpTiles = new int[] { 0, 2, 2, 1, 0, 7, 7, 1, 0, 7, 7, 1, 4, 3, 3, 5 };
    int[] uBotTiles = new int[] { 0, 2, 2, 1, 0, 7, 7, 1, 0, 7, 7, 1, 4, 3, 3, 5 };
    int[] uLeftTiles = new int[] { 0, 2, 2, 1, 0, 7, 7, 1, 0, 7, 7, 1, 4, 3, 3, 5 };
    int[] uRightTiles = new int[] { 0, 2, 2, 1, 0, 7, 7, 1, 0, 7, 7, 1, 4, 3, 3, 5 };

    int[] soloTiles = new int[] { 0, 2, 2, 1, 0, 7, 7, 1, 0, 7, 7, 1, 4, 3, 3, 5 };




    // Start is called before the first frame update
    void Start()
    {
        TileBase[] tiles = new TileBase[] { sxSide, dxSide, topSide, botSide, botSXAngle, botDXAngle, door, floor, exTopSXAngle, exTopDXAngle, exBotSXAngle, exBotDXAngle };

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
            indeces = topSxangleTiles;
        else if (quadtype == 2)
            indeces = soloTiles;
    }
}

using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace autovoxel{

public enum FaceDirection
{
    Top,
    Bottom,
    Front,
    Back,
    Left,
    Right
}

public enum TileSide
{
    Top,
    Right,
    Bottom,
    Left
}

public static class FaceMappings
{
    public static readonly Dictionary<FaceDirection, FaceData> Data = new()
    {
        { FaceDirection.Front,  new FaceData(Vector3.forward,   new Vector3(-0.5f,  -0.5f, -0.5f))},
        { FaceDirection.Back,   new FaceData(Vector3.back,      new Vector3( 0.5f,  -0.5f,  0.5f))},
        { FaceDirection.Top,    new FaceData(Vector3.down,      new Vector3(-0.5f,   0.5f, -0.5f))},
        { FaceDirection.Bottom, new FaceData(Vector3.up,        new Vector3(-0.5f,  -0.5f,  0.5f))},
        { FaceDirection.Left,   new FaceData(Vector3.right,     new Vector3(-0.5f,  -0.5f,  0.5f))},
        { FaceDirection.Right,  new FaceData(Vector3.left,      new Vector3( 0.5f,  -0.5f, -0.5f))}
    };
}

public class FaceData
{
    public Vector3 Normal { get; }
    public Vector3 PosOffset { get; }

    public FaceData(Vector3 normal, Vector3 offset)
    {
        Normal = normal;
        PosOffset = offset;
    }
}

public class TileMappings
{
    private static readonly int NumRowTiles = 3;
    public static float UVTileSize => 1f / NumRowTiles;
    public static readonly Dictionary<FaceDirection, UVTileData> Data = new()
    {
        { FaceDirection.Top,    new UVTileData(new Vector2Int(0, 1), UVTileSize) },
        { FaceDirection.Bottom, new UVTileData(new Vector2Int(0, 0), UVTileSize) },
        { FaceDirection.Front,  new UVTileData(new Vector2Int(1, 1), UVTileSize) },
        { FaceDirection.Back,   new UVTileData(new Vector2Int(2, 1), UVTileSize) },
        { FaceDirection.Left,   new UVTileData(new Vector2Int(1, 0), UVTileSize) },
        { FaceDirection.Right,  new UVTileData(new Vector2Int(2, 0), UVTileSize) }
    };

    public static Vector2 GetUVBaseOffset(UVTileData tileData)
    {
        return new Vector2(tileData.UvBaseOffset.x, tileData.UvBaseOffset.y);
    }

    public static (FaceDirection, TileSide) GetAdjacentTileSide(FaceDirection faceDir, TileSide tileSide)
    {
        switch (faceDir)
        {
            case FaceDirection.Front:
                return tileSide switch
                {
                    TileSide.Top => (FaceDirection.Top, TileSide.Bottom),
                    TileSide.Right => (FaceDirection.Right, TileSide.Left),
                    TileSide.Bottom => (FaceDirection.Bottom, TileSide.Top),
                    TileSide.Left => (FaceDirection.Left, TileSide.Right),
                    _ => (FaceDirection.Front, TileSide.Top)
                };
            case FaceDirection.Back:
                return tileSide switch
                {
                    TileSide.Top => (FaceDirection.Top, TileSide.Top),
                    TileSide.Right => (FaceDirection.Left, TileSide.Left),
                    TileSide.Bottom => (FaceDirection.Bottom, TileSide.Bottom),
                    TileSide.Left => (FaceDirection.Right, TileSide.Right),
                    _ => (FaceDirection.Back, TileSide.Top)
                };
            case FaceDirection.Top:
                return tileSide switch
                {
                    TileSide.Top => (FaceDirection.Back, TileSide.Top),
                    TileSide.Right => (FaceDirection.Right, TileSide.Top),
                    TileSide.Bottom => (FaceDirection.Front, TileSide.Top),
                    TileSide.Left => (FaceDirection.Left, TileSide.Top),
                    _ => (FaceDirection.Top, TileSide.Top)
                };
            case FaceDirection.Bottom:
                return tileSide switch
                {
                    TileSide.Top => (FaceDirection.Front, TileSide.Bottom),
                    TileSide.Right => (FaceDirection.Right, TileSide.Bottom),
                    TileSide.Bottom => (FaceDirection.Back, TileSide.Bottom),
                    TileSide.Left => (FaceDirection.Left, TileSide.Bottom),
                    _ => (FaceDirection.Bottom, TileSide.Top)
                };
            case FaceDirection.Left:
                return tileSide switch
                {
                    TileSide.Top => (FaceDirection.Top, TileSide.Left),
                    TileSide.Right => (FaceDirection.Front, TileSide.Left),
                    TileSide.Bottom => (FaceDirection.Bottom, TileSide.Left),
                    TileSide.Left => (FaceDirection.Back, TileSide.Right),
                    _ => (FaceDirection.Left, TileSide.Top)
                };
            case FaceDirection.Right:
                return tileSide switch
                {
                    TileSide.Top => (FaceDirection.Top, TileSide.Right),
                    TileSide.Right => (FaceDirection.Back, TileSide.Left),
                    TileSide.Bottom => (FaceDirection.Bottom, TileSide.Right),
                    TileSide.Left => (FaceDirection.Front, TileSide.Right),
                    _ => (FaceDirection.Right, TileSide.Top)
                };
            default:
                return (FaceDirection.Front, TileSide.Top);
        }
    }
}

public class UVTileData
{
    public Vector2Int TileIndex { get; }
    public Vector2 UvBaseOffset { get; }

    public UVTileData(Vector2Int tileIndex, float uvTileSize)
    {
        TileIndex    = tileIndex;
        UvBaseOffset = new Vector2(TileIndex.x * uvTileSize, TileIndex.y * uvTileSize);
    }
}

public class FaceManager
{
    public static readonly List<Face> faces = new();
    public static Texture2D UvTexture { get; private set; }
    public static int TileResolution { get; private set; }

    public static void Initialize(Texture2D texture, int tileRes)
    {
        UvTexture = texture;
        TileResolution = tileRes;

        faces.Clear();

        foreach (FaceDirection direction in System.Enum.GetValues(typeof(FaceDirection)))
        {
            faces.Add(new Face(direction, TileResolution));
        }
        
        foreach (Face face in faces)
        {
            face.GenerateFacePixelOffsets();
        }
    }
    public static Vector2Int WorldSpaceXYToPixelCoord(Face face, float x, float y)
    {
        Vector2 pixelBaseOffset = new Vector2(face.TileIndex.x * TileResolution, face.TileIndex.y * TileResolution);

        int pixelCoordX = Mathf.FloorToInt(pixelBaseOffset.x + x * TileResolution);
        int pixelCoordY = Mathf.FloorToInt(pixelBaseOffset.y + y * TileResolution);

        return new Vector2Int(pixelCoordX, pixelCoordY);
    }

    public static Face GetOppositeFace(Face face)
    {
        switch (face.Direction)
        {
            case FaceDirection.Front:
                return faces.Find(face => face.Direction == FaceDirection.Back);
            case FaceDirection.Back:
                return faces.Find(face => face.Direction == FaceDirection.Front);
            case FaceDirection.Top:
                return faces.Find(face => face.Direction == FaceDirection.Bottom);
            case FaceDirection.Bottom:
                return faces.Find(face => face.Direction == FaceDirection.Top);
            case FaceDirection.Left:
                return faces.Find(face => face.Direction == FaceDirection.Right);
            case FaceDirection.Right:
                return faces.Find(face => face.Direction == FaceDirection.Left);
            default:
                return null;
        }
    }

    public static List<float> GetZOffset(Face face, float x, float y)
    {
        return face.GetZOffset(x, y);
    }


    public static Face GetFace(FaceDirection direction)
    {
        return faces.Find(face => face.Direction == direction);
    }

    public static void Reset()
    {
        faces.Clear();
        UvTexture = null;
        TileResolution = 0;
    }
}


public class Face
{
    private readonly int _tileResolution;

    // Face Data
    public FaceDirection Direction { get; }
    public Dictionary<TileSide, List<List<int>>> PixelOffsets { get; private set; }
    public Vector3 Normal => FaceMappings.Data[Direction].Normal;
    public Vector3 PosOffset => FaceMappings.Data[Direction].PosOffset;

    //Tile Data
    public Vector2Int TileIndex => TileMappings.Data[Direction].TileIndex;
    public Vector2 UvBaseOffset => TileMappings.GetUVBaseOffset(TileMappings.Data[Direction]);
    public float UvTileSize => TileMappings.UVTileSize;

    public Face(FaceDirection direction, int tileResolution)
    {
        _tileResolution = tileResolution;

        this.Direction = direction;
        InitPixelOffsets();
    }

    private void InitPixelOffsets()
    {
        PixelOffsets = new Dictionary<TileSide, List<List<int>>>();

        foreach (TileSide side in System.Enum.GetValues(typeof(TileSide)))
        {
            PixelOffsets.Add(side, new List<List<int>>());
            for (int i = 0; i < _tileResolution; i++)
            {
                PixelOffsets[side].Add(new List<int>());
            }
        }
    }

    public List<float> GetZOffset(float x, float y)
    {
        (FaceDirection verticalAdjFaceDir, TileSide verticalAdjTileSide) = TileMappings.GetAdjacentTileSide(Direction, TileSide.Top);
        Face verticalAdjFace = FaceManager.GetFace(verticalAdjFaceDir);
        List<List<int>> verticalZOffsetList = verticalAdjFace.PixelOffsets[verticalAdjTileSide];

        (FaceDirection horizontalAdjFaceDir, TileSide horizontalAdjTileSide) = TileMappings.GetAdjacentTileSide(Direction, TileSide.Left);
        Face horizontalAdjFace = FaceManager.GetFace(horizontalAdjFaceDir);
        List<List<int>> horizontalZOffsetList = horizontalAdjFace.PixelOffsets[horizontalAdjTileSide];
        
        int verticalLookupIndex     = Mathf.FloorToInt(x * _tileResolution);
        int horizontalLookupIndex   = Mathf.FloorToInt(y * _tileResolution);

        if(verticalAdjTileSide == TileSide.Top || verticalAdjTileSide == TileSide.Left)
        {
            verticalLookupIndex = _tileResolution-verticalLookupIndex-1;
        }
        if(horizontalAdjTileSide == TileSide.Top || horizontalAdjTileSide == TileSide.Left)
        {
            horizontalLookupIndex = _tileResolution-horizontalLookupIndex-1;
        }

        List<int> verticalZOffsets      = verticalZOffsetList[verticalLookupIndex].OrderByDescending(x => x).ToList();
        List<int> horizontalZOffsets    = horizontalZOffsetList[horizontalLookupIndex].OrderByDescending(x => x).ToList();
        List<int> maxValueList          = new List<int>();

        // Compare Logic
        bool maxValueListFound          = false;
        int comparisonDepth = Mathf.Min(verticalZOffsets.Count, horizontalZOffsets.Count);

        for (int i = 0; i < comparisonDepth; i++)
            {
                if (verticalZOffsets[i] > horizontalZOffsets[i])
                {
                    maxValueList = verticalZOffsets;
                    maxValueListFound = true;
                }
                else if (verticalZOffsets[i] < horizontalZOffsets[i])
                {
                    maxValueList = horizontalZOffsets;
                    maxValueListFound = true;
                }
            }

        // If both lists are identical up to their shared length, return the longer list
        if (maxValueListFound == false && (verticalZOffsets.Count > horizontalZOffsets.Count))
        {
            maxValueList = horizontalZOffsets;
            maxValueListFound = true;
        }
        else if (maxValueListFound == false && (horizontalZOffsets.Count > verticalZOffsets.Count))
        {
            maxValueList = verticalZOffsets;
            maxValueListFound = true;
        }

        // If all else is equal, return either (both are effectively the same)
        if (maxValueListFound == false)
        {
            maxValueList = verticalZOffsets;
        }
        
        List<float> zOffsetListWorldXY = new List<float>();

        foreach(int zOffset in maxValueList)
        {
            zOffsetListWorldXY.Add((float)zOffset / _tileResolution);
        }

        return zOffsetListWorldXY;
    }


    public void GenerateFacePixelOffsets()
    {
        InitPixelOffsets();

        // Top and Bottom sides
        for (int x = 0; x < _tileResolution; x++)
        {
            bool topFlag = true;        
            bool bottomFlag = false;    
            int counter = 0;

            for (int y = 0; y < _tileResolution; y++)
            {
                int xCoord = TileIndex.x * _tileResolution + x;
                int yCoord = TileIndex.y * _tileResolution + y;

                bool isPixelEmpty = !IsPixelSolid(xCoord, yCoord);

                if(isPixelEmpty)
                {
                    if(topFlag == false)
                    {
                        PixelOffsets[TileSide.Top][x].Add(_tileResolution-counter);
                        topFlag = true;
                    } 
                    bottomFlag = false;
                }
                else
                {
                    if(bottomFlag == false)
                    {
                        PixelOffsets[TileSide.Bottom][x].Add(counter);
                        bottomFlag = true;
                    }
                    topFlag = false;

                    if((counter+1) == _tileResolution)
                    {
                        PixelOffsets[TileSide.Top][x].Add(0);
                    }
                }
                counter++;
            }
        }

        // Left and Right sides
        for (int y = 0; y < _tileResolution; y++)
        {
            bool leftFlag = false;        
            bool rightFlag = true;    
            int counter = 0;

            for (int x = 0; x < _tileResolution; x++)
            {
                int xCoord = TileIndex.x * _tileResolution + x;
                int yCoord = TileIndex.y * _tileResolution + y;
                bool isPixelEmpty = !IsPixelSolid(xCoord, yCoord);

                if(isPixelEmpty)
                {
                    if(rightFlag == false)
                    {
                        PixelOffsets[TileSide.Right][y].Add(_tileResolution-counter);
                        rightFlag = true;
                    } 
                    leftFlag = false;
                }
                else
                {
                    if(leftFlag == false)
                    {
                        PixelOffsets[TileSide.Left][y].Add(counter);
                        leftFlag = true;
                    }
                    rightFlag = false;

                    // Add last pixel value to Right side
                    if((counter+1) == _tileResolution)
                    {
                        PixelOffsets[TileSide.Right][y].Add(0);
                    }
                }
                counter++;
            }
        }
    }

    public bool IsPixelSolid(int pixelCoordX, int pixelCoordY)
    {
        Face oppFace = FaceManager.GetOppositeFace(this);

        int tileBaseCoordX = TileIndex.x * _tileResolution;
        int tileBaseCoordY = TileIndex.y * _tileResolution;
        int tileBaseCoordXOpp = oppFace.TileIndex.x * _tileResolution;
        int tileBaseCoordYOpp = oppFace.TileIndex.y * _tileResolution;

        int pixelCoordXOpp = 0;
        int pixelCoordYOpp = 0;

        switch(Direction)
        {
            case FaceDirection.Top:
                pixelCoordXOpp = pixelCoordX;
                pixelCoordYOpp = tileBaseCoordY - 1 - (pixelCoordY - tileBaseCoordY);
                break;
            case FaceDirection.Bottom:
                pixelCoordXOpp = pixelCoordX;
                pixelCoordYOpp = tileBaseCoordYOpp + tileBaseCoordYOpp - 1 - pixelCoordY;
                break;
            case FaceDirection.Front:
                pixelCoordXOpp = tileBaseCoordXOpp + tileBaseCoordXOpp - 1 - pixelCoordX;
                pixelCoordYOpp = pixelCoordY;
                break;
            case FaceDirection.Back:
                pixelCoordXOpp = tileBaseCoordX - 1 - (pixelCoordX - tileBaseCoordX);
                pixelCoordYOpp = pixelCoordY;
                break;
            case FaceDirection.Left:
                pixelCoordXOpp = tileBaseCoordXOpp + tileBaseCoordXOpp - 1 - pixelCoordX;
                pixelCoordYOpp = pixelCoordY;
                break;
            case FaceDirection.Right:
                pixelCoordXOpp = tileBaseCoordX - 1 - (pixelCoordX - tileBaseCoordX);
                pixelCoordYOpp = pixelCoordY;
                break;
            default:
                break;
        }

        Color currentPixelColor     = FaceManager.UvTexture.GetPixel(pixelCoordX, pixelCoordY);
        Color oppositePixelColor    = FaceManager.UvTexture.GetPixel(pixelCoordXOpp, pixelCoordYOpp);

        if (currentPixelColor.a == 0.0f || oppositePixelColor.a == 0.0f)
        {
            return false;
        }
        return true;
    }

    public void PrintOffsetsForSide(TileSide side)
    {
        Debug.Log($"Printing offsets for {Direction}-{side}");
        for (int i = 0; i < PixelOffsets[side].Count; i++)
        {
            foreach (var item in PixelOffsets[side][i])
            {
                Debug.Log($"Pixel {i}: {item}");
            }
        }
    }

    public override string ToString()
    {
        return $"Face {Direction} - Tile {TileIndex}";
    }
}

}
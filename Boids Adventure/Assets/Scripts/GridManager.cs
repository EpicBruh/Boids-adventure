using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridManager : MonoBehaviour
{
    public Sprite sprite;
    public Vector2[,] Grid;
    int vertical, horizontal, cols, rows;
    [SerializeField]
    bool debug = false;
    [SerializeField]
    bool spawnTiles = false;
    [SerializeField]
    [Range(0, 2f)]
    float debugRadius = .2f;


    void Start()
    {
        vertical = (int)Camera.main.orthographicSize;
        horizontal = (int)(vertical * (float)Screen.width / Screen.height);
        cols = horizontal * 2;
        rows = vertical * 2;
        Grid = new Vector2[cols, rows];
        float xtheta = 0;
        float ytheta = 10;
        for (int i = 0; i < cols; i++)
        {

            for (int j = 0; j < rows; j++)
            {
                Grid[i, j] = new Vector2(Mathf.Cos(map(Mathf.PerlinNoise(xtheta,ytheta),0,1,0,2*Mathf.PI)),Mathf.Sin(map(Mathf.PerlinNoise(ytheta,xtheta),0,1,0,2*Mathf.PI)));
                
                //if (spawnTiles)
            //        SpawnTile(i, j, Grid[i,j]);
             //   xtheta += Random.Range(1f,4f);
             //   ytheta += Random.Range(-6f, 3f);
                //Grid[i, j] = new Vector2(Mathf.Cos(xtheta), Mathf.Sin(ytheta));
                xtheta += Random.Range(0.01f,0.02f);
                ytheta += Random.Range(0.01f,0.02f);
            }

        }
        
    }

    public Vector2 PositionMap(float x, float y)
    {
        x -= Camera.main.transform.position.x;
        y -= Camera.main.transform.position.y;
        float wholeX = (int)x;
        float wholeY = (int)y;
        float i, j;
        if((wholeX+.5f) - x > (wholeX - .5f) - x)
        {
            i = wholeX - .5f;
        }
        else
        {
            i = wholeX + .5f;
        }
        if ((wholeY + .5f) - y > (wholeY - .5f) - y)
        {
            j = wholeY - .5f;
        }
        else
        {
            j = wholeY + .5f;
        }
        Vector2 index = new Vector2(i + (horizontal - .5f), j + (vertical - .5f));
        return index;
       
    }

    public Vector2 FlowField(Vector2 pos)
    {
        Vector2 index = PositionMap(pos.x, pos.y);
        try
        {
            Vector2 force = Grid[(int)index.x, (int)index.y];
            return force;
        }
        catch
        {
                return new Vector2(0, 0);
        }
    }


    void SpawnTile(int x, int y, Vector2 gridVal)
    {
        GameObject g = new GameObject("X: " + x + "Y: " + y);
        float value = Mathf.PerlinNoise((x +.5f - horizontal) / cols, (y + 0.5f - vertical) / rows);
        g.transform.position = new Vector3(x - (horizontal - .5f), y - (vertical -.5f), 0);
        var s = g.AddComponent<SpriteRenderer>();
        s.color = new Color(value, value, value, 0.8f);
        s.sprite = sprite;
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            for (int i = 0; i < cols; i++)
            {

                for (int j = 0; j < rows; j++)
                {

                    Vector2 pos = new Vector2(i - (horizontal - 0.5f) + Camera.main.transform.position.x, j - (vertical - 0.5f) + Camera.main.transform.position.y);
                    Handles.DrawWireDisc(pos, new Vector3(0, 0, 1), debugRadius);
                    Debug.DrawLine(pos, pos + (Grid[i, j].normalized) * debugRadius);
                }

            }
        }
    }

    public float map(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax)
    {
        float OldRange = OldMax - OldMin;
        float NewRange = NewMax - NewMin;
        float NewValue = ((OldValue - OldMin) * NewRange / OldRange) + NewMin;
        return NewValue;
    }






}

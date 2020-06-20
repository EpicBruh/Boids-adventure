using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Sprite sprite;
    public Vector2[,] Grid;
    int vertical, horizontal, cols, rows;
    [SerializeField]
    Transform target;


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
                Grid[i, j] = new Vector2(Mathf.Cos(Mathf.PerlinNoise(xtheta,ytheta)),Mathf.Sin(Mathf.PerlinNoise(ytheta,xtheta)));
                
                SpawnTile(i, j, Grid[i,j]);
                xtheta += Random.Range(1f,4f);
                ytheta += Random.Range(-6f, 3f);
            }

        }
        
    }

    private void Update()
    {
        
    }

    public Vector2 PositionMap(float x, float y)
    {
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
        return new Vector2(i + (horizontal - .5f), j + (vertical - .5f));
    }

    public Vector2 FlowField(Vector2 pos)
    {
        Vector2 index = PositionMap(pos.x, pos.y);
        if (pos.x >= cols || pos.x < 0 || pos.y >= rows || pos.y < 0)
            return new Vector2();
        Vector2 force = Grid[(int)index.x, (int)index.y];
        return force;
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
        for (int i = 0; i < cols; i++)
        {

            for (int j = 0; j < rows; j++)
            {
                //Vector2 pos = new Vector2(i - (horizontal - 0.5f), j - (vertical - 0.5f));
                
               // Debug.Log(pos);
               // Debug.DrawLine(pos, pos + (Grid[i,j].normalized)*0.2f);
            }

        }
    }






}

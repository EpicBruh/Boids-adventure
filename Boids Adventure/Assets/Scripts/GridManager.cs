using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Sprite sprite;
    public Vector2[,] Grid;
    int vertical, horizontal, cols, rows;
    [Range(0,100)]
    public float scale;
    float xOffset;
    float yOffset;
    bool created = false;

    //public GameObject target;
    public float squareRadius = 0.5f;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            xOffset = Random.Range(1, 999999f);
            yOffset = Random.Range(1, 999999f);
            vertical = (int)Camera.main.orthographicSize;
            horizontal = (int)(vertical * (float)Screen.width / Screen.height);
            cols = horizontal * 2;
            rows = vertical * 2;
            Grid = new Vector2[cols, rows];
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Grid[i, j] = new Vector2(i - (horizontal - squareRadius), j - (vertical - squareRadius));
                    if (created)
                    {
                        ChangeColor(i, j);
                    }
                    else
                    {
                        SpawnTile(i, j);
                    }
                }
            }
            if (!created)
                created = true;
        }
    }

    

    void SpawnTile(int x, int y)
    {
        GameObject g = new GameObject("X: " + x + "Y: " + y);
        g.transform.position = new Vector3(x - (horizontal - squareRadius), y - (vertical - squareRadius), 0);
        var s = g.AddComponent<SpriteRenderer>();
        s.color = CalculateColor(x,y);
        s.sprite = sprite;
    }

    void ChangeColor(int x,int y)
    {
        GameObject g = GameObject.Find("X: " + x + "Y: " + y);
        g.transform.position = new Vector3(x - (horizontal - squareRadius), y - (vertical - squareRadius), 0);
        var s = g.GetComponent<SpriteRenderer>();
        s.color = CalculateColor(x, y);
        s.sprite = sprite;
    }



    Color CalculateColor(int x, int y)
    {
        float xCoord = ((float)x +xOffset)/ cols * scale;
        float yCoord = ((float)y +yOffset)/ rows * scale;
        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        float val = sample > 0.5 ? 1 : 0;
        return new Color(val, val, val);
    }

}

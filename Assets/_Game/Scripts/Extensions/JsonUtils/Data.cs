using System;
using System.Collections.Generic;

[Serializable]
public class Data<T>
{
    public GridData<T> grid;
}

[Serializable]
public class Row<T>
{
    public List<T> values;
}

[Serializable]
public class GridData<T>
{
    public List<Row<T>> rows;

    public int Rows => rows.Count;
    public int Cols => rows[0].values.Count;

    public static GridData<T> Convert2DArrayToGridData(T[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        GridData<T> grid = new GridData<T>();
        grid.rows = new List<Row<T>>();

        for (int i = 0; i < rows; i++)
        {
            Row<T> row = new Row<T>();
            row.values = new List<T>();
            for (int j = 0; j < cols; j++)
            {
                row.values.Add(array[i, j]);
            }
            grid.rows.Add(row);
        }

        return grid;
    }

    public static T[,] ConvertGridDataTo2DArray(GridData<T> grid)
    {
        int rows = grid.rows.Count;
        int cols = grid.rows[0].values.Count;
        T[,] array = new T[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                array[i, j] = grid.rows[i].values[j];
            }
        }

        return array;
    }
}
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LevelDesignTool
{
    //Level Design for 2D grid - ONE LAYER ONLY
    public class Grid2dTool : EditorWindow
    {
        private string[] tabs = new string[] { "Setup", "Level Units" };

        #region Setup Variables
        
        private string levelName = "Level";
        private Vector3 Origin => GridUtils.CalOrigin(gridSize, cellSize);
        private Vector2Int gridSize = new Vector2Int(1, 1);
        private Vector2 cellSize = new Vector2(1f, 1f);
        
        #endregion
        
        #region LevelUnit
        private GameObject prefab;
        private Vector2 guiCellSize = new Vector2(40f, 40f);
        private int[,] matrix;
        private Dictionary<Vector2Int, GameGrid.Interface.ILevelUnitGrid2d> unitLookup = new();
        private Vector2 scroll;
        #endregion
        
        #region Core Window 
        [MenuItem("DesignTools/Grid 2D Tool")]
        private static void ShowWindow() // Hiển thị Window Tổng
        {
            Grid2dTool window = (Grid2dTool)GetWindow(typeof(Grid2dTool));
            window.Show();
        }

        private int currentSelectedIdx = 0;
        private void OnGUI() // Vẽ lên Window
        {
            // Hiển thị các tabs
            currentSelectedIdx = GUILayout.Toolbar(currentSelectedIdx, tabs);
            switch (currentSelectedIdx) {
                case 0: // Setup tab
                    DrawSetupTab();
                    break;
                case 1: // Prefabs Tab
                    DrawLevelUnitsTab();
                    if (prefab == null)
                    {
                        EditorGUILayout.HelpBox("Please select a prefab.", MessageType.Warning);
                    }
                    if (GUILayout.Button("Build Scene from Matrix"))
                    {
                        BuildSceneFromMatrix();
                    }
                    if (GUILayout.Button("Save JSON LevelData"))
                    {
                        Data<int> data = new Data<int>();
                        data.grid = GridData<int>.Convert2DArrayToGridData(matrix);
                        JsonUtils.Save(levelName, data);
                    }
                    if (GUILayout.Button("Load JSON LevelData"))
                    {
                        Data<int> data = JsonUtils.Load<int>(levelName); //TODO: Nên gán mảng Luôn
                        this.gridSize = new Vector2Int(data.grid.Rows, data.grid.Cols);
                        this.matrix = new int[gridSize.x, gridSize.y];
                        this.matrix = GridData<int>.ConvertGridDataTo2DArray(data.grid);
                    }
                    DrawMatrixWithBoxes();
                    break;
            }
        }
        
        #endregion

        #region Matrix With Boxes

        private void DrawMatrixWithBoxes()
        {
            if (matrix == null || matrix.GetLength(0) != gridSize.x || matrix.GetLength(1) != gridSize.y)
                matrix = new int[gridSize.x, gridSize.y];
            
            EditorGUILayout.LabelField("MATRIX DATA TYPE", EditorStyles.boldLabel);
            GUIStyle centeredStyle = new GUIStyle(EditorStyles.numberField)
            {
                alignment = TextAnchor.MiddleCenter
            };
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            for (int y = gridSize.y - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < gridSize.x; x++)
                {
                    int oldValue = matrix[x, y];
                    int newValue = EditorGUILayout.IntField(oldValue, centeredStyle,
                        GUILayout.Width(guiCellSize.x), GUILayout.Height(guiCellSize.x));
                    
                    if (newValue != oldValue)
                    {
                        matrix[x, y] = newValue;

                        var pos = new Vector2Int(x, y);
                        if (unitLookup.TryGetValue(pos, out var unit))
                        {
                            unit.SetCellType(newValue);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        #endregion
        
        #region Setup Tab

        private void DrawSetupTab()
        {
            EditorGUILayout.LabelField("GRID DATA INPUT", EditorStyles.boldLabel);
            
            levelName = EditorGUILayout.TextField("Level Name", levelName);
            Vector2Int newGridSize = EditorGUILayout.Vector2IntField("Grid Size", gridSize);
            
            Vector2 newCellSize = EditorGUILayout.Vector2Field("Cell Size", cellSize);
            if (newGridSize != gridSize || newCellSize != cellSize)
            {
                gridSize = newGridSize;
                cellSize = newCellSize;
            }
        }
        
        #endregion

        #region Level Units Tab
        private void DrawLevelUnitsTab()
        {
            EditorGUILayout.LabelField("LEVEL UNIT DATA", EditorStyles.boldLabel);
            prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false) as GameObject;
        }
        #endregion

        #region Scene Handler
        private void BuildSceneFromMatrix()
        {
            ClearPreviousScene();
            GameObject parent = new GameObject();
            parent.name = levelName;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    int type = matrix[x, y];
                    
                    Vector3 worldPos = GridUtils.GridToWorld(cellSize, Origin, new Vector2Int(x, y));
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    
                    obj.transform.SetParent(parent.transform);
                    obj.transform.position = worldPos;
                    obj.name = $"{levelName}_({x},{y})";
                        
                    if (obj.TryGetComponent<GameGrid.Interface.ILevelUnitGrid2d>(out var unit))
                    {
                        unit.SetCellType(type);
                    }
                } 
            }
        }
        
        private void ClearPreviousScene()
        {
            GameObject old = GameObject.Find(levelName);
            if (old != null)
            {
                DestroyImmediate(old);
            }
        }
        #endregion
    }
}
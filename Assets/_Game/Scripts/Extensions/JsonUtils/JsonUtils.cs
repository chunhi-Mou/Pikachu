using UnityEngine;
using System.IO;
// YÊU CẦU: Data.cs - để lưu dữ liệu
public static class JsonUtils
{
    private static readonly string SAVE_FOLDER = 
    #if UNITY_EDITOR
        Application.dataPath + "/JsonLevelFiles"; // Editor chỉ để debug
    #else
        Application.persistentDataPath + "/JsonLevelFiles"; // Build thì dùng path ghi được
    #endif


    /// <summary>
    /// Khởi tạo thư mục lưu trữ
    /// </summary>
    public static void OnInit()
    {
        // Kiểm tra xem thư mục đã tồn tại chưa
        if (!Directory.Exists(SAVE_FOLDER))
        {
            // Nếu chưa, tạo thư mục mới
            Directory.CreateDirectory(SAVE_FOLDER);
            Debug.Log("Đã tạo thư mục tại: " + SAVE_FOLDER);
        }
    }

    /// <summary>
    /// Lưu một đối tượng Data vào file JSON
    /// </summary>
    public static void Save<T>(string fileName, Data<T> data)
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            OnInit();
        }
        string json = JsonUtility.ToJson(data, true);
        string fullPath = Path.Combine(SAVE_FOLDER, fileName + ".json");
        
        File.WriteAllText(fullPath, json); //VIẾT
        Debug.Log("Đã lưu thành công file: " + fullPath);
    }

    /// <summary>
    /// Tải một đối tượng Data từ file JSON
    /// </summary>
    public static Data<T> Load<T>(string fileName)
    {
        string fullPath = Path.Combine(SAVE_FOLDER, fileName + ".json");

        // Kiểm tra xem file có tồn tại không
        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);//ĐỌC
            Data<T> data = JsonUtility.FromJson<Data<T>>(json);
            Debug.Log("Đã tải thành công file: " + fullPath);
            return data;
        }
        else
        {
            // Cảnh báo nếu file không được tìm thấy
            Debug.LogWarning("Không tìm thấy file để tải: " + fullPath);
            return null;
        }
    }
}

using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DataManager : MonoBehaviour
{
    #region Variables
    public Data data;
    public string fileName = "data.dat";
    private string _dataPath;

    private static DataManager _instance;
    public static DataManager Instance => _instance;

    #endregion

    #region Unity Methods

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // ✅ INICIALIZAR DATA SI ES NULA
            if (data == null)
            {
                data = new Data();
            }
        }
        else 
        {
            Destroy(gameObject);
            return;
        }
        
        _dataPath = Application.persistentDataPath + "/" + fileName;
        Debug.Log("Ruta de guardado: " + _dataPath);
        Load();
    }

    #endregion

    #region Methods

    [ContextMenu("Save")]
    public void Save()
    {
        try
        {
            _dataPath = Application.persistentDataPath + "/" + fileName;
            
            // ✅ VERIFICAR QUE DATA NO SEA NULA
            if (data == null)
            {
                Debug.LogError("❌ Data es nula, no se puede guardar");
                data = new Data();
            }
            
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(_dataPath);
            bf.Serialize(file, data);
            file.Close();
            
            Debug.Log("✅ Guardado exitoso en: " + _dataPath);
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Error al guardar: " + e.Message);
        }
    }

    public void Load()
    {
        try
        {
            _dataPath = Application.persistentDataPath + "/" + fileName;
            
            if (!File.Exists(_dataPath)) 
            {
                Debug.Log("📁 No existe archivo de guardado, creando datos nuevos");
                Save(); // ✅ Guardar datos iniciales
                return;
            }
            
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(_dataPath, FileMode.Open);
            data = (Data)bf.Deserialize(file);
            file.Close();
            
            Debug.Log("✅ Carga exitosa");
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Error al cargar: " + e.Message);
            data = new Data(); // ✅ Crear datos nuevos si hay error
        }
    }

    [ContextMenu("Delete Data")]
    public void DeleteSaveFile()
    {
        try
        {
            _dataPath = Application.persistentDataPath + "/" + fileName;
            
            if (File.Exists(_dataPath))
            {
                File.Delete(_dataPath);
                Debug.Log("✅ Archivo eliminado: " + _dataPath);
                
            }
            else
            {
                Debug.LogWarning("⚠️ No existe archivo para eliminar");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Error al eliminar: " + e.Message);
        }
    }
    #endregion
}
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

    public static DataManager Instance { get; private set; }

    #endregion

    #region Unity Methods

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;

        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (data == null)
        {
            data = new Data();
        }


        _dataPath = Application.persistentDataPath + "/" + fileName;
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

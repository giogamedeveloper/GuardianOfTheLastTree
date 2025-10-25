using System;
using UnityEngine;
//The library that contains the utilities that will allow us to serialize binary data.
using System.Runtime.Serialization.Formatters.Binary;
//The library that contains utilities for working with files and directories.
using System.IO;


public class DataManager : MonoBehaviour
{
    #region Variables

    //Variable that will contain all the information on statistics and achievements.
    public Data data;

    //Name of the file in which we will store the information in the local path.
    public string fileName = "data.dat";

    //Ruta + nombre del fichero
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
            //Para que no se destruya el gameobject entre escenas.
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        //Inicializamos el dataPath con la ruta de la carpeta designada por Unity para el guardado de datos persistentes + el nombre del fichero defindido por filename separados por una barra (/)
        _dataPath = Application.persistentDataPath + "/" + fileName;
        //Para poder localizar la carpeta facilmente, lo mostraremos por consola
        Load();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Guarda la informacion en el disco
    /// </summary>
    [ContextMenu("Save")]
    public void Save()
    {
        _dataPath = Application.persistentDataPath + "/" + fileName;
        //Objeto utilizado para serializar / deserializar informacion a binario
        BinaryFormatter bf = new BinaryFormatter();
        //Creamos /sobreescrinbimos el fichero con los datos
        FileStream file = File.Create(_dataPath);
        //Serializamos la informacion de nuestro objeto de datos
        bf.Serialize(file, data);
        //Cerramos el stream una vez terminamos
        file.Close();
    }

    /// <summary>
    /// Recupera la informacion guardada en el disco
    /// </summary>
    public void Load()
    {
        _dataPath = Application.persistentDataPath + "/" + fileName;
        if (!File.Exists(_dataPath)) return;
        //Objeto utilizado para serializar  / deserializar informacion a binario
        BinaryFormatter bf = new BinaryFormatter();
        //Creamos /sobreescrinbimos el fichero con los datos
        FileStream file = File.Open(_dataPath, FileMode.Open);
        data = (Data)bf.Deserialize(file);
        //Cerramos el stream una vez terminamos
        file.Close();
    }

    /// <summary>
    /// Borra el fichero guardado
    /// </summary>
    [ContextMenu("Delete Data")]
    public void DeleteSaveFile()
    {
        _dataPath = Application.persistentDataPath + "/" + fileName;
        File.Delete(_dataPath);
    }

    #endregion
}

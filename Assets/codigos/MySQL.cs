using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class MySQL : MonoBehaviour
{
    public static MySQL instance;
    private string dbName = "URI=file:DataBase.db";
    private const string fixedName = "Anthony";

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject); 
    }

    void Start()
    {
        CreateTable();
        SaveNewEntry();
        DisplayAllEntries();
    }

    private void CreateTable()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS game_data(
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        name VARCHAR(50) NOT NULL,
                        number INTEGER NOT NULL,
                        timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    private void SaveNewEntry()
    {
        // Obtener el último número usado
        int lastNumber = GetLastNumber();
        int newNumber = lastNumber + 1;

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"INSERT INTO game_data (name, number) VALUES ('{fixedName}', {newNumber});";
                command.ExecuteNonQuery();
                Debug.Log($"Datos guardados: {fixedName} - {newNumber}");
            }
            connection.Close();
        }
    }

    private int GetLastNumber()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT MAX(number) FROM game_data;";
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                return 0; // Si no hay registros, empezar desde 0
            }
        }
    }

    private void DisplayAllEntries()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM game_data ORDER BY timestamp DESC;";

                using (IDataReader reader = command.ExecuteReader())
                {
                    Debug.Log("=== Historial de Guardados ===");
                    while (reader.Read())
                    {
                        string timestamp = Convert.ToDateTime(reader["timestamp"]).ToString("yyyy-MM-dd HH:mm:ss");
                        Debug.Log($"ID: {reader["id"]} | Nombre: {reader["name"]} | Número: {reader["number"]} | Fecha: {timestamp}");
                    }
                }
            }
            connection.Close();
        }
    }

    void OnApplicationQuit()
    {
        // Opcional: Puedes agregar aquí un último guardado al cerrar la aplicación
        Debug.Log("Aplicación cerrada - Último número usado: " + GetLastNumber());
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add The SQLite References
using SQLite_API;           // SQLite_API
using System.Data;          // Data objects for query results
using System.Data.SQLite;   // NuGet Package => system.data.sqlite

//=============
// Aliases
//=============
//using Con = System.Console;
using ConnType = SQLite_API.SQLiteAPI.ConnectionType;
using ProcState = SQLite_API.SQLiteAPI.Status;

public class SQLite_Unity : MonoBehaviour
{
    SQLiteAPI SQLiteDB = new SQLiteAPI();  // New instance of SQLite Database

    // Start is called before the first frame update
    void Start()
    {
        //=============
        // Variables - Standard
        //=============
        ProcState Results;

        //=============
        // Variables - SQLite
        //=============
        SQLiteDB.DB_Name = "database.db";  // Name of the database
        //SQLiteDB.DB_Name = "Users.s3db";  // Name of the database
        SQLiteDB.DB_Path = Application.dataPath + "/Plugins/SQLite_DBs/";  // Where the database is within the Unity project
        /*
         * If you are referencing a database NOT part of the Unity project, you must provide the full path:
         * SQLiteDB.DB_Path = @"D:\Work\Code\SQLite\Working\";
         */
        SQLiteDB.DB_Conn = ConnType.URI;  // Inidicates Unity project
        /*
         * If you are referencing a database NOT part of the Unity project, use DS
         * SQLiteDB.DB_Conn = ConnType.DS;
         */

        // Create the database connection
        Debug.Log("Creating the connection");
        Results = SQLiteDB.CreateConnection();

        if (Results == ProcState.Error)
        {
            Debug.LogError("Error in creating database connection: " + SQLiteDB.Error);
        }

        // Create the appropriate tables
        Debug.Log("Creating the tables.");
        if (Results == ProcState.Good)
        {
            Results = CreateTables(ref SQLiteDB);
            if (Results == ProcState.Error)
            {
                Debug.LogError("Error in creating database tables: " + SQLiteDB.Error);
            }
        }

        // Populate the tables with data
        Debug.Log("Adding data to the tables.");
        if (Results == ProcState.Good)
        {
            Results = InsertData(ref SQLiteDB);
            if (Results == ProcState.Error)
            {
                Debug.LogError("Error in adding data to the tables: " + SQLiteDB.Error);
            }
        }

        // Read information from the database
        Debug.Log("Reading from the database.");
        if (Results == ProcState.Good)
        {
            Results = ReadFromDatabase(ref SQLiteDB);
            if (Results == ProcState.Error)
            {
                Debug.LogError("Error reading from database: " + SQLiteDB.Error);
            }
        }

    } // void Start

    private ProcState CreateTables(ref SQLiteAPI SQLiteDB)
    /*
    ===============================================================================================
    PURPOSE:
    Create the tables in the database that will be used.
    -----------------------------------------------------------------------------------------------
    NOTES:
    - Execution will stop at the first error.

    - While it is possible that these two commands could be entered as one complex command; this
      shows an example of how the execution of the first will affect the execution of the second.
    ===============================================================================================
    */
    {
        //=============
        // Variables - Standard
        //=============
        ProcState Results = ProcState.Good;

        //=============
        // Body
        //=============
        // Create the first table
        SQLiteDB.SQL =
            "DROP TABLE IF EXISTS SampleTable0; " +
            "CREATE TABLE SampleTable0 " +
            "(" +
            " Col1 VARCHAR(20), " +
            " Col2 INT " +
            ")";
        Results = SQLiteDB.ExecuteNonQuery();

        // Create the second table
        if (Results == ProcState.Good)
        {
            SQLiteDB.SQL =
                "DROP TABLE IF EXISTS SampleTable1; " +
                "CREATE TABLE SampleTable1 " +
                "(" +
                " Col3 VARCHAR(20), " +
                " Col4 INT " +
                ");";
            Results = SQLiteDB.ExecuteNonQuery();
        }

        //=============
        // Cleanup Environment
        //=============
        // Return the results
        return Results;
    } // private ProcState CreateTables

    private ProcState InsertData(ref SQLiteAPI SQLiteDB)
    /*
    ===============================================================================================
    PURPOSE:
    Insert data into the tables created.
    -----------------------------------------------------------------------------------------------
    NOTES:
    - Execution will stop at the first error.

    - This shows an example of multiple commands sent as once.
    ===============================================================================================
    */
    {
        //=============
        // Variables - Standard
        //=============
        ProcState Results = ProcState.Good;

        //=============
        // Body
        //=============
        // Add data to the first table
        SQLiteDB.SQL =
            "INSERT INTO SampleTable0 " +
            "(Col1, Col2) " +
            "VALUES " +
            "('Test Text ', 1);" +
            "" +
            "INSERT INTO SampleTable0 " +
            "(Col1, Col2) " +
            "VALUES " +
            "('Test1 Text1 ', 2);" +
            "" +
            "INSERT INTO SampleTable1 " +
            "(Col3, Col4) " +
            "VALUES " +
            "('Test4 Text4 ', 4);";
        Results = SQLiteDB.ExecuteNonQuery();

        //=============
        // Cleanup Environment
        //=============
        // Return the results
        return Results;
    } // private ProcState InsertData

    private ProcState ReadFromDatabase(ref SQLiteAPI SQLiteDB)
    /*
    ===============================================================================================
    PURPOSE:
    Obtain some data from the database and read the results from the QueryResults object.
    ===============================================================================================
    */
    {
        //=============
        // Variables - Standard
        //=============
        ProcState Results = ProcState.Good;

        //=============
        // Setup Environment
        //=============
        SQLiteDB.SQL =
            "SELECT Col1, Col2 FROM SampleTable0; " +
            "SELECT Col3, Col4 FROM SampleTable1;";
        Results = SQLiteDB.ExecuteQuery();

        //=============
        // Body
        //=============
        //=============
        // Show some information about the results
        //=============
        if (Results == ProcState.Good)
        {
            Debug.Log("There are " + SQLiteDB.QueryResults.Tables.Count.ToString() + " tables in the result.");

            DataTableCollection collection = SQLiteDB.QueryResults.Tables;
            for (int i = 0; i < collection.Count; i++)
            {
                // Table Information
                DataTable table = collection[i];  // Collection ID
                Debug.Log("Table Collection "+ i +": Table Name \""+ table.TableName + "\"");  // Table name

                // Column Information
                foreach (DataColumn column in SQLiteDB.QueryResults.Tables[i].Columns)
                {
                    Debug.Log("Column Name: " + column.ColumnName + " => Column Type: " + column.DataType + "");
                }

                // We know there are 2 columns, so they are hard-coded
                string Col1Name = SQLiteDB.QueryResults.Tables[i].Columns[0].ColumnName;
                string Col2Name = SQLiteDB.QueryResults.Tables[i].Columns[1].ColumnName;

                // Table Data
                foreach (DataRow r in SQLiteDB.QueryResults.Tables[i].Rows)
                {
                    Debug.Log(r[Col1Name].ToString() + " " + r[Col2Name].ToString());
                }
            }
        }

        //=============
        // Cleanup Environment
        //=============
        // Return the results
        return Results;
    } // private ProcState ReadFromDatabase

    // Update is called once per frame
    void Update()
    {
        
    }
}

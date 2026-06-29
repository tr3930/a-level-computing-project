using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace CompSciProjectFinal
{
    public class ProgramConfiguration
    {
        public string programDataPath { get; set; }
        public bool emailEnabled { get; set; }
    }
    
    internal class ProgramConfigurationManagement
    {
        public static ProgramConfiguration GetAllProgramConfiguration() //This function will return all configuration about the program
        {
            string fileText = File.ReadAllText("configuration.json"); //Opens the configuration file which will be in the same folder as the exexutable
            ProgramConfiguration configuration = JsonSerializer.Deserialize<ProgramConfiguration>(fileText); //Converts the JSON to a "ProgramConfiguration" object where all data can be accessed
            return configuration; //Returns this object
        }

        public static bool CheckIfProgramDataFolderExists() //Verifys if the program data folder actually exists
        {
            string programDataPath = GetAllProgramConfiguration().programDataPath;
            if (Directory.Exists(programDataPath))
            {
                return true; //Data path does exist
            }
            else
            {
                return false; //Data path doesn't exist
            }
        }

        public static void CreateNewProgramDataFolder()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog(); //I am aware that this uses WinForms and I really hate it but it appears to be the only way of doing it without more NuGet packages
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK) //Has the user actually selected a folder?
            {
                string path = folderBrowserDialog.SelectedPath;
                ProgramConfiguration programConfiguration = new ProgramConfiguration()
                {
                    programDataPath = path,
                    emailEnabled = true
                };
                SetProgramConfiguration(programConfiguration); //
                Directory.CreateDirectory(path + "/Images"); //Create paths within program data folder
                Directory.CreateDirectory(path + "/Images/Products");
                Directory.CreateDirectory(path + "/Images/Materials");
                Directory.CreateDirectory(path + "/ActiveUse");
                Directory.CreateDirectory(path + "/Invoices");
                DatabaseManagementV2.InsertStructureAndStartingDataIntoBlankDbFile(); //Add the structure to the database
                
            }
        }

        public static void SetProgramConfiguration(ProgramConfiguration programConfiguration)
        {
            string fileText = JsonSerializer.Serialize(programConfiguration);
            File.WriteAllText("configuration.json", fileText);
        }

        public static string GetDataPath()
        {
            return GetAllProgramConfiguration().programDataPath; //Gets all program configuration and returns file path
        }

        public static bool IsEmailEnabled() //return a boolean to check if the email system is enabled
        {
            return GetAllProgramConfiguration().emailEnabled;
        }

        public static bool DoesEmailMessagesFileExist()
        {
            return File.Exists(GetDataPath() + "/email_messages.json");
        }
    }
}

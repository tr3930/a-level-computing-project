using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CompSciProjectFinal
{
    internal class FileProcessing
    {
        private static List<ActiveUseFileReference> filesInUse = new List<ActiveUseFileReference>();

        public static BitmapImage LoadBitmapImageFromLocationString(string location)
        {
            return new BitmapImage(new Uri(location));
        }

        public static void ExportBitmapImageAsPng(BitmapSource image, string fileName)
        {
            if (image == null)
            {
                throw new ArgumentNullException("You are a total dunce");
            }
            else
            {
                BitmapEncoder encoder = new PngBitmapEncoder(); //Encoder to convert the bitmap used the program as a .png
                encoder.Frames.Add(BitmapFrame.Create(image)); //Add the given image as a frame in the png
                FileStream fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create); //Filestream to save the image
                encoder.Save(fileStream); //Saves the image
            }
        }

        //Due to issues that came up in development about files requiring replacement being in use. This will copy those files to a different directory so that the original data files will not be overwritten.
        private static ActiveUseFileReference LoadFileIntoActiveUseDirectory(string fileLocation)
        {
            if (filesInUse.Count == 0)
            {
                ClearActiveUseFolder(); //If there isn't any active files in use by the program, the directory will be cleared
            }
            ActiveUseFileReference fileReference = new ActiveUseFileReference() //New file reference object
            {
                fileId = DataFunctions.GenerateRandomAlphaNeumericString(8), //Data for the object
                trueFileLocation = fileLocation
            };
            File.Copy(fileLocation, ProgramConfigurationManagement.GetDataPath() + "/ActiveUse/" + fileReference.fileId); //Copy file to ActiveUse directory
            return fileReference; //Return the reference
        }

        private static void UpdateActiveUseList(ActiveUseFileReference activeUseFileReference) //Procedure for handling the list of active files
        {
            bool hasFoundEntry = false;
            for (int i = 0; i < filesInUse.Count; i++)
            {
                if (filesInUse[i].trueFileLocation == activeUseFileReference.trueFileLocation)
                {
                    hasFoundEntry = true;
                    filesInUse[i] = activeUseFileReference;
                    break;
                }
            }
            if (!hasFoundEntry)
            {
                filesInUse.Add(activeUseFileReference);
            }
        }

        public static string LoadFile(string fileLocation, bool forceReload = false)
        {
            if (!File.Exists(fileLocation))
            {
                throw new FileNotFoundException(fileLocation + " does not exist");
            }
            else
            {
                if (forceReload)
                {
                    UpdateActiveUseList(LoadFileIntoActiveUseDirectory(fileLocation));
                    return LoadFile(fileLocation, false);
                }
                else
                {
                    for (int i = 0; i < filesInUse.Count; i++)
                    {
                        if (filesInUse[i].trueFileLocation == fileLocation)
                        {
                            return ProgramConfigurationManagement.GetDataPath() + "/ActiveUse/" + filesInUse[i].fileId;
                        }
                    }
                     UpdateActiveUseList(LoadFileIntoActiveUseDirectory(fileLocation));
                     return LoadFile(fileLocation, false);
                }
            }
        }

        private static void ClearActiveUseFolder() //Clears out the active use folder. This is to prevent buildup of files between program usage sessions
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(ProgramConfigurationManagement.GetDataPath() + "/ActiveUse");
            foreach (FileInfo fileInfo in directoryInfo.GetFiles()) //Iterates through every file
            {
                fileInfo.Delete(); //Deletes the file
            }
        }


        public static void SaveTextFile(string fileText, string filePath) //saves a text document to a file
        {
            File.WriteAllText(filePath, fileText);
        }
    }
}

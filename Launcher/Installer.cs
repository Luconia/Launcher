using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Launcher
{
    internal class Installer
    {
        string roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public Installer() 
        {
            Logger.LogInfo("Initializing Installer");
            InitVersion();
        }

        private void InitVersion()
        {
            var existsFile = File.Exists(roamingDirectory + "\\Luconia\\version.json");
            var existsDirectory = Directory.Exists(roamingDirectory + "\\Luconia");

            if (!existsDirectory)
            {
                Logger.LogWarning("Can't find Luconia directory");
                Logger.LogInfo("Creating Directory...");

                Directory.CreateDirectory(roamingDirectory + "\\Luconia");
                Logger.LogInfo("Directory created");
            }

            if (!existsFile) 
            {
                Logger.LogWarning("Can't find version file");
                Logger.LogInfo("Creating File...");
                File.Create(roamingDirectory + "\\Luconia\\version.json");
                Logger.LogInfo("File created");
            }
        }
    }
}

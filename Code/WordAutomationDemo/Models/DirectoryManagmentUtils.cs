using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WordAutomationDemo.Models
{

    public static class DirectoryManagmentUtils
    {
        public const int DisposeTimeout = 5;
        const string FolderKey = "WorkSessionDirectory";
        const string DemoPathKey = "CurrentDemoPath";
        static readonly object modifyUserDirectoriesLocker = new object();

        public class DemoDirectoryInfo
        {
            public string Name { get; set; }
            public DateTime LastUsageTime { get; set; }
        }

        public static string DocumentBrowsingFolderPath { get { return Path.Combine(DirectoryManagmentUtils.CurrentDataDirectory); } }
        public static string SampleDocumentsFolderPath { get { return Path.Combine(DirectoryManagmentUtils.CurrentDataDirectory, "SampleDocuments"); } }
        public static string CurrentDataDirectory
        {
            get
            {
                //if (!Utils.IsSiteMode)
                //    return InitialDemoFilesPath;
                lock (modifyUserDirectoriesLocker)
                {
                    var currentDataDirectory = (string)Context.Session[FolderKey];
                    DemoDirectoryInfo directoryInfo = ActualDemoDirectories.Where(i => i.Name == currentDataDirectory).FirstOrDefault();
                    if (directoryInfo == null || ((string)Context.Session[DemoPathKey] != Context.Request.Path && Context.Request.HttpMethod == "GET"))
                    {
                        Context.Session[DemoPathKey] = Context.Request.Path;
                        Context.Session[FolderKey] = currentDataDirectory = CreateNewDemoFolder();
                        directoryInfo = new DemoDirectoryInfo { Name = currentDataDirectory, LastUsageTime = DateTime.Now };
                        ActualDemoDirectories.Add(directoryInfo);
                    }
                    else
                    {
                        directoryInfo.LastUsageTime = DateTime.Now;
                    }
                    return currentDataDirectory;
                }
            }
        }



        static IList<DemoDirectoryInfo> actualDemoDirectories;
        static IList<DemoDirectoryInfo> ActualDemoDirectories
        {
            get
            {
                if (actualDemoDirectories == null)
                    actualDemoDirectories = new List<DemoDirectoryInfo>();
                return actualDemoDirectories;
            }
        }
        static HttpContext Context { get { return HttpContext.Current; } }
        public static string RootDemoFilesPath { get { return Context.Request.MapPath("~/App_Data/"); } }
        public static string RootAppDocPath { get { return Context.Request.MapPath("~/ApplicationDocuments/"); } }
        
        public static string UserFilesPath { get { return Context.Request.MapPath("~/App_Data/UserDocs/"); } }
        public static string InitialDemoFilesPath { get { return Path.Combine(RootDemoFilesPath, "Documents"); } }
        public static string InitialDemoFilesPathTemplates { get { return Path.Combine(UserFilesPath, "Templates"); } }
        public static string InitialDemoFilesPathDemo { get { return Path.Combine(UserFilesPath, "General"); } }
        public static string InitialDemoFilesPathWords { get { return Path.Combine(RootAppDocPath, "Words"); } }
        //public static string InitialDemoFilesPathTemplates { get { return Path.Combine(RootDemoFilesPath); } }
        //public static string InitialDemoFilesPathDemo { get { return Path.Combine(RootDemoFilesPath); } }
        public static string InitialDemoFilesPathPPTs { get { return Path.Combine(RootAppDocPath, "PPTs"); } }
        public static string ProjectsDocumentFilesPath { get { return Path.Combine(RootAppDocPath, "ProjectDocuments"); } }
        public static string TaskDocumentFilesPath { get { return Path.Combine(RootAppDocPath, "TaskDocuments"); } }
        public static string InitialDemoFilesPathDocument { get { return Path.Combine(RootDemoFilesPath, "Documents"); } }
        public static string InitialDemoFilesPathSection { get { return Path.Combine(RootDemoFilesPath, "Sections"); } }
        public static string ReadOnlyFilesPath { get { return Path.Combine(UserFilesPath, "ReadOnlyFiles"); } }
        public static string InitialDemoFilesPathExcels { get { return Path.Combine(RootAppDocPath, "Excels"); } }


        static string CreateNewDemoFolder()
        {
            string demoFilesDirectoty = GenerateDemoFilesFolderName();
            //CopyDemoFiles(InitialDemoFilesPath, demoFilesDirectoty);
            return demoFilesDirectoty;
        }
        //static void CopyDemoFiles(string sourceFilePath, string destinationPath)
        //{
        //    IEnumerable<string> documentFileCollection = GetFilesInDirectory(sourceFilePath, "*.xlsx", "*.xls", "*.csv", "*.docx", "*.doc", "*.rtf", "*.txt");
        //    if (!Directory.Exists(destinationPath))
        //        Directory.CreateDirectory(destinationPath);
        //    foreach (var filePath in documentFileCollection)
        //    {
        //        string destinationFile = Path.Combine(destinationPath, Path.GetFileName(filePath));
        //        File.Copy(filePath, destinationFile, true);
        //        File.SetAttributes(destinationFile, FileAttributes.Normal);
        //    }

        //    foreach (string directoryPath in Directory.GetDirectories(sourceFilePath))
        //    {
        //        string directoryName = Path.GetFileName(directoryPath);
        //        CopyDemoFiles(directoryPath, Path.Combine(destinationPath, directoryName));
        //    }
        //}
        static IEnumerable<string> GetFilesInDirectory(string path, params string[] allowedExtensions)
        {
            IEnumerable<string> documentFileCollection = new string[0];
            foreach (string extension in allowedExtensions)
            {
                documentFileCollection = documentFileCollection.Concat(Directory.GetFiles(path, extension));
            }
            return documentFileCollection;
        }
        static string GenerateDemoFilesFolderName()
        {
            //string currentFolder = "Demo";
            string currentFolder = "General";
            //while (string.IsNullOrEmpty(currentFolder) || Directory.Exists(Path.Combine(RootDemoFilesPath, currentFolder)))
            //{
            //    //currentFolder = Guid.NewGuid().ToString();
            //    currentFolder = "Demo";
            //}
            //if (!Directory.Exists(Path.Combine(RootDemoFilesPath, "General")))
            //{
            //    currentFolder = "General";

            //}
            return Path.Combine(UserFilesPath, currentFolder);
        }

        public static void PurgeOldUserDirectories()
        {
            //if (!Utils.IsSiteMode)
            //    return;

            lock (modifyUserDirectoriesLocker)
            {
                string[] existingDirectories = Directory.GetDirectories(RootDemoFilesPath);
                foreach (string directoryPath in existingDirectories)
                {
                    Guid guid = Guid.Empty;
                    if (!Guid.TryParse(Path.GetFileName(directoryPath), out guid)) continue;

                    DemoDirectoryInfo directoryInfo = ActualDemoDirectories.Where(i => i.Name == directoryPath).SingleOrDefault();
                    if (directoryInfo == null || (DateTime.Now - directoryInfo.LastUsageTime).TotalMinutes > DisposeTimeout)
                    {
                        Directory.Delete(directoryPath, true);
                        if (directoryInfo != null)
                            ActualDemoDirectories.Remove(directoryInfo);
                    }
                }
            }
        }
        public static string GetDeletedByTimeoutMessage()
        {
            return "The current work session has been expired. The page will be automatically re-requested.";
        }
        public static void AssertTimeout()
        {
            //if (!Utils.IsSiteMode)
            //    return;
            throw new Exception(GetDeletedByTimeoutMessage());
        }
    }

    public class OfficeDemoPage : System.Web.UI.Page
    {
        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            DirectoryManagmentUtils.PurgeOldUserDirectories();
        }
    }
}
using AutomateRobotService.Modele;
using AutomateRobotService.Tools;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
namespace AutomateRobotService.Controller
{
    internal class MainController
    {
        private MainWindow mainWindow;
        private Logger logger;
        public static string directoryPath;
        private List<string> fileNamesTemplates;
        private static string fileNamesTemplatesPath = Environment.CurrentDirectory + "/fileNamestemplates.txt";
        public MainController(MainWindow main)
        {
            mainWindow = main;
            logger = new Logger();
            fileNamesTemplates = new List<string>();
            LoadFileNameTemplates();
        }

        public void LoadFileNameTemplates()
        {
            if (File.Exists(fileNamesTemplatesPath))
            {

                Stream stream = Stream.Null;
                try
                {
                stream = File.OpenRead(fileNamesTemplatesPath);

                }catch(Exception ex)
                {
                    MessageBox.Show($"Error Source: \n{ex.Source}\nError Help Link:\n {ex.HelpLink}\nError Message:\n {ex.Message}\n ");
                }

                StreamReader reader = new StreamReader(stream);

                string line = "";

                while((line = reader.ReadLine()) != null) {
                    fileNamesTemplates.Add(line);
                }
                Debug.WriteLine("Pomyślnie wczytano szablony plików");
            }
            else
            {
                File.Create(fileNamesTemplatesPath).Dispose();
                LoadFileNameTemplates();
            }

        }
        public IEnumerable<string> GetUniqueFilesNames()
        {
            if(directoryPath is not null)
            {
                IEnumerable<string> fileNames = Directory.EnumerateFiles(directoryPath);
                List<string> UniqueFileNames = new List<string>();
                foreach (string fileName in fileNames) {
            
                    foreach (string template in fileNamesTemplates) { 
                        if(template == "^.*_\\d{2}\\.")
                        {
                            Match match = Regex.Match(fileName, template);
                            if (match.Success)
                            {
                                string[] nameSplitted = match.Groups[0].Value.Split('\\');
                                string name = nameSplitted[nameSplitted.Length - 1];
                                name = name.Substring(0, name.Length - 3);
                                if (!UniqueFileNames.Contains(name))
                                {

                                    UniqueFileNames.Add(name);
                                }
                            }
                        }
                        else
                        {

                            Match match = Regex.Match(fileName, template);
                            if (match.Success) {
                                string[] nameSplitted = match.Groups[0].Value.Split('\\');
                                if (!UniqueFileNames.Contains(nameSplitted[nameSplitted.Length - 1]))
                                {
                                
                                    UniqueFileNames.Add(nameSplitted[nameSplitted.Length - 1]);
                                }
                            }
                        }
                        
                    }
                    

                }

                return UniqueFileNames;

            }
            return null;
        } 

        public void MainProcess() {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            mainWindow.LabelCleaningProgress.Visibility = Visibility.Visible;
                            mainWindow.ProgressBarCleaningProgress.Visibility = Visibility.Visible;
                        });

                         Debug.Write("\nDziała");

                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            if(mainWindow.TextDirectoryPath.Text != string.Empty)
                            {

                                directoryPath = mainWindow.TextDirectoryPath.Text;
                            }
                            else { return; }
                        }));
                    
                        List<string> fileNames = GetUniqueFilesNames().ToList();

                        int proccesedFiles = fileNames.Count;

                        List<FileModel> Files = new List<FileModel>();

                        List<string> DeletedFiles = new List<string>();
                        foreach (string name in fileNames)
                        {

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                mainWindow.LabelCleaningProgress.Content = $"Postęp czyszczenia: ({proccesedFiles}/{fileNames.Count}";
                                mainWindow.ProgressBarCleaningProgress.Value = (proccesedFiles/fileNames.Count)*100;
                            });

                            if (name != "")
                            {


                                foreach (string file in Directory.EnumerateFiles(directoryPath)) {
                            if (file.Contains(name))
                                    {
                                        

                                Files.Add(new FileModel
                                {
                                    FileName = file,
                                    CreatedAt = Directory.GetCreationTime(file)
                                });
                                    }
                                }

                                Files = Files.OrderBy(f => f.CreatedAt).ToList();
                                Files.Reverse();
                                 Debug.WriteLine("Posortowane pliki od najstarszej do najmłodszej\n\n");
                                foreach (FileModel file in Files)
                                {
                                    Debug.WriteLine($"Nazwa: {file.FileName}\nUtworzony: {file.CreatedAt}");
                                }
                                Stack<FileModel> stack = new Stack<FileModel>();
                                Debug.WriteLine("Wygląd stosu: \n");
                                foreach (FileModel file in Files)
                                {
                                    stack.Push(file);
                                    Debug.WriteLine($" Plik: {file.FileName}\n DataUtworzenia: {file.CreatedAt}\n\n");
                                }
                                
                                while (stack.Count > 2)
                                {
                                    FileModel toDelete = stack.Pop();
                                    File.Delete(toDelete.FileName);
                                    DeletedFiles.Add(toDelete.FileName);
                                };
                                
                                Files.Clear();
                                proccesedFiles += 1;
                        
                            }

                           

                        }
                    
                    if (DeletedFiles.Count > 0)
                        {

                        logger.LogEvent(DeletedFiles.Count, DeletedFiles);
                        }
                        GC.Collect();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error Source: \n{ex.Source}\nError Help Link:\n {ex.HelpLink}\nError Message:\n {ex.Message}\n ");
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        mainWindow.LabelCleaningProgress.Visibility = Visibility.Hidden;
                        mainWindow.ProgressBarCleaningProgress.Visibility = Visibility.Hidden;
                    });
        }


        public void CounterDown(Label timeDisplay)
        {
            if(MainWindow.ActualTime == TimeSpan.Zero) {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainWindow.ActualTime = TimeSpan.FromMinutes(MainWindow.Interval);

                });
            }

            Application.Current.Dispatcher.Invoke(() => { 
            MainWindow.ActualTime -= TimeSpan.FromSeconds(1);
            });
            Debug.WriteLine($"\nTimer zmniejszony: {MainWindow.ActualTime.Hours} godz {MainWindow.ActualTime.Minutes} min {MainWindow.ActualTime.Seconds} sekund");
            Application.Current.Dispatcher.Invoke(() =>
            {

                timeDisplay.Content = $"Następne czyszczenie za: {MainWindow.ActualTime.Hours} godz {MainWindow.ActualTime.Minutes} min {MainWindow.ActualTime.Seconds} sekund";

            });
        }
        
    }
}

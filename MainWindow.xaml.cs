using System.Windows;
using System.Windows.Input;
using AutomateRobotService.Controller;
using AutomateRobotService.Tools;
using Microsoft.WindowsAPICodePack.Dialogs;
namespace AutomateRobotService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Zmienne
        private Timer? timer = null;
        private Timer? timerCounter = null;
        public static TimeSpan ActualTime = TimeSpan.Zero;
        public static int Interval = 0;
        private MainController controller { get; set; }
        public WarningWindow warn { get; set; }
        //Funkcje
        public MainWindow()
        {
            InitializeComponent();
            controller = new(this);
            warn = new WarningWindow();
            warn.Hide();
            
        }

        private void TextTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(!AdditionalFunctions.IsTextAllowed(e.Text)) { e.Handled = true; } else { e.Handled = false;  }
        }
        // Buttons Events section
        private void ButtonDirectoryBrowser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();

            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = Environment.CurrentDirectory;

            dialog.AddToMostRecentlyUsedList = false;
            dialog.AllowNonFileSystemItems = false;
            dialog.DefaultDirectory = Environment.CurrentDirectory;
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dialog.FileName;
                if(folder != null)
                {
                    TextDirectoryPath.Text = folder;
                }
                else
                {
                    warn.SetWarning("Error: Błąd pobierania ścieżki folderu. Spróbuj ponownie lub skontaktuj się z Administratorem programu.");
                    warn.Show();
                }
            }
        }

        

        private void ButtonAddValue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int value = int.Parse(TextTime.Text);
                value += 1;
                TextTime.Text = value.ToString();

            }
            catch (Exception ex)
            {
                warn.SetWarning(ex.Message);
                warn.Show();
            }
        }

        private void ButtonDellValue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int value = int.Parse(TextTime.Text);
                if(value > 1) value -= 1;
                else value = 1;
                TextTime.Text = value.ToString();

            }
            catch (Exception ex)
            {
                warn.SetWarning(ex.Message);
                warn.Show();
            }
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if(TextDirectoryPath.Text != "")
            {

                if(timer is null)
                {
                
                    ChangeLabelStatus(true);
                    int interval = 0;
                    try
                    {
                        interval = int.Parse(TextTime.Text);
                        Interval = interval;
                    }
                    catch(Exception ex)
                    {
                        warn.SetWarning(ex.Message);
                        warn.Show();
                    }
            

                    timer = new Timer(x => controller.MainProcess(), null, 0, (int)TimeSpan.FromMinutes(interval).TotalMilliseconds);
                    ActualTime = TimeSpan.FromMinutes(interval);
                    timerCounter = new Timer(x => controller.CounterDown(LabelTimeLeft), null, 0, 1000);

                    ButtonStop.IsEnabled = true;

                }
                else
                {
                    warn.SetWarning("Error: Proces, który próbujesz uruchomić jest w trakcie działania!" +
                        " \nZatrzymaj proces, aby móc uruchomić go ponownie");
                    warn.Show();
                }
            }
            else
            {
                warn.SetWarning("Ścieżka do folderu jest nieprawidłowa! Wybierz odpowiedni folder i spróbuj ponownie!");
                warn.Show();
            }
        }

        private async void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            
            if(timer is not null && timerCounter is not null)
            {
                ChangeLabelStatus(false);
                await timer.DisposeAsync();
                await timerCounter.DisposeAsync();
                LabelTimeLeft.Content = "Następne czyszczenie za: 0 godz 0 min 0 sekund";
                ButtonStop.IsEnabled = false;
                timer = null;
                timerCounter = null;
            }
            
        }
        //End Of Buttons Events Section

        //External functions section
        public void ChangeLabelStatus(bool isRunning)
        {
            if (isRunning) { LabelStatus.Content = "Status: Działa"; }
            else { LabelStatus.Content = "Status: Zatrzymane"; }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void RegesReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            controller.LoadFileNameTemplates();
        }


        //End of external functions section
    }
}
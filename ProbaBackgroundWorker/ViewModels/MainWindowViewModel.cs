using System.ComponentModel;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProbaBackgroundWorker.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private double _progressValue;
    [ObservableProperty] private string _messageText;
    BackgroundWorker _bgWorker = new();

    public MainWindowViewModel()
    {
        ProgressValue = 0;
        MessageText = "";
        _bgWorker.WorkerReportsProgress = true;
        _bgWorker.WorkerSupportsCancellation = true;
        _bgWorker.DoWork += DoWork_Handler;
        _bgWorker.ProgressChanged += ProgressChanged_Handler;
        _bgWorker.RunWorkerCompleted += RunWorkerCompleted_Handler;
    }

    private void DoWork_Handler(object sender, DoWorkEventArgs args)
    {
        BackgroundWorker worker = sender as BackgroundWorker;
        for (int i = 1; i <= 20; i++)
        {
            if (worker.CancellationPending)
            {
                args.Cancel = true;
                break;
            }
            else
            {
                worker.ReportProgress(i * 5);
                Thread.Sleep(100);
            }
        }
    }

    private void RunWorkerCompleted_Handler(object sender, RunWorkerCompletedEventArgs args)
    {
        if (args.Cancelled)
        {
            MessageText = "Proces anulowano.";
        }
        else
        {
            MessageText = "Proces został ukończony.";
        }
    }

    private void ProgressChanged_Handler(object sender, ProgressChangedEventArgs args)
    {
        ProgressValue = args.ProgressPercentage;
    }

    [RelayCommand]
    public void Start()
    {
        if (!_bgWorker.IsBusy)
        {
            ProgressValue = 0;
            MessageText = "";
            _bgWorker.RunWorkerAsync();
        }
    }

    [RelayCommand]
    public void Cancel()
    {
        _bgWorker.CancelAsync();
    }
}
using DesktopTool.App.Core.Models;
using DesktopTool.App.Service;
using DesktopTool.App.UI.Helper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DesktopTool.App.UI.ViewModel
{
    public class UploadPdfViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;
        private string _selectedFilePath;
        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set
            {
                _selectedFilePath = value;
                OnPropertyChanged(nameof(SelectedFilePath));
                OnPropertyChanged(nameof(FileName));
                ((RelayCommand)UploadCommand).RaiseCanExecuteChanged();
            }
        }

        private int _uploadProgress;
        public int UploadProgress
        {
            get => _uploadProgress;
            set
            {
                _uploadProgress = value;
                OnPropertyChanged(nameof(UploadProgress));
            }
        }
        public ICommand UploadCommand { get; }
        public string FileName => string.IsNullOrEmpty(SelectedFilePath)
            ? "No file selected"
            : Path.GetFileName(SelectedFilePath);

        public ICommand SelectFileCommand { get; }

        public UploadPdfViewModel(IAuthService authService)
        {
            SelectFileCommand = new RelayCommand(SelectFile);
            _authService = authService;
            UploadCommand = new RelayCommand(async () => await UploadFileAsync(), () => !string.IsNullOrEmpty(SelectedFilePath));
        }

        private void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Select a PDF file"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFilePath = openFileDialog.FileName;
                ((RelayCommand)UploadCommand).RaiseCanExecuteChanged();
            }
        }

        private async Task UploadFileAsync()
        {
            UploadProgress = 0;

            var progress = new Progress<int>(value => UploadProgress = value);

            var success = await _authService.UploadPdfAsync(SelectedFilePath, progress);

            if (success)
            {
                // Notify user: success
            }
            else
            {
                // Notify user: failure
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

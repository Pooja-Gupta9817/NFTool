using App.UI.ViewModel;
using DesktopTool.App.Core.Interfaces;
using DesktopTool.App.UI.Helper;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DesktopTool.App.UI.ViewModel
{
    public class TeacherViewModel : INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;
        public string LoggedInUserName { get; }
        public string LoggedInUserEmail { get; }


        public TeacherViewModel(IServiceProvider serviceProvider, IUserContext userContext)
        {
            _serviceProvider = serviceProvider;
            LoggedInUserName = userContext.Name;     // E.g., "Payal"
            LoggedInUserEmail = userContext.Email;

            OpenUploadPdfCommand = new RelayCommand(OpenUploadPdf);
            OpenUploadMarksCommand = new RelayCommand(OpenUploadMarks);
            OpenStudentListCommand = new RelayCommand(OpenStudentList);
        }

        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public ICommand OpenUploadPdfCommand { get; }
        public ICommand OpenUploadMarksCommand { get; }
        public ICommand OpenStudentListCommand { get; }

        private void OpenUploadPdf() =>
            CurrentViewModel = _serviceProvider.GetRequiredService<UploadPdfViewModel>();

        private void OpenUploadMarks() =>
            CurrentViewModel = _serviceProvider.GetRequiredService<UploadMarksViewModel>();

        private void OpenStudentList() =>
            CurrentViewModel = _serviceProvider.GetRequiredService<StudentListViewModel>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}

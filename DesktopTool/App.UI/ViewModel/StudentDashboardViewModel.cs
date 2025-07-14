using DesktopTool.App.Core.Interfaces;
using DesktopTool.App.UI.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DesktopTool.App.UI.ViewModel
{
    public class StudentDashboardViewModel : INotifyPropertyChanged
    {
        private readonly IUserContext _userContext;
        private readonly IAIAssistantService _aiService;
        private readonly IConfiguration _configuration;

        public int TotalFiles { get; set; } = 0;
        public string AIFeedbackSummary { get; set; } = "You haven't uploaded any file yet.";
        public string StudyPlanSummary { get; set; } = "You're on track. Keep studying daily!";
        public ObservableCollection<string> RecentFiles { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> ChatHistory { get; set; } = new ObservableCollection<string>();
        public string Question { get; set; }
        public ICommand AskCommand { get; }

        public StudentDashboardViewModel(IUserContext userContext, IAIAssistantService aiService, IConfiguration configuration)
        {
            _userContext = userContext;
            _aiService = aiService;
            _configuration = configuration;

            AskCommand = new RelayCommand(() => _ = AskAI());
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            TotalFiles = 5;
            AIFeedbackSummary = "You need to revise Chapter 3 and practice MCQs.";
            StudyPlanSummary = "📌 Focus on Algebra this week.";

            RecentFiles.Add("Maths_Chapter3.pdf");
            RecentFiles.Add("Science_Topic1.pdf");
            RecentFiles.Add("History_Notes.pdf");

            OnPropertyChanged(nameof(TotalFiles));
            OnPropertyChanged(nameof(AIFeedbackSummary));
            OnPropertyChanged(nameof(StudyPlanSummary));
            OnPropertyChanged(nameof(RecentFiles));
        }

        private bool _isAsking = false;
        private async Task AskAI()
        {
            if (_isAsking || string.IsNullOrWhiteSpace(Question)) return;

            _isAsking = true;

            string prompt = Question;
            ChatHistory.Add("🧑 You: " + prompt);
            Question = string.Empty;
            OnPropertyChanged(nameof(Question));

            ChatHistory.Add("🤖 AI is typing...");
            int aiTypingIndex = ChatHistory.Count - 1;
            OnPropertyChanged(nameof(ChatHistory));

            try
            {
                var response = await _aiService.AskAsync(prompt);
                ChatHistory[aiTypingIndex] = $"🤖 AI: {response}";
            }
            catch (Exception ex)
            {
                ChatHistory[aiTypingIndex] = "❌ Error: " + ex.Message;
            }
            finally
            {
                _isAsking = false;
                OnPropertyChanged(nameof(ChatHistory));
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

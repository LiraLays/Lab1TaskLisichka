using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Lab1_TaskScheduler.Models
{
	public class TaskItem : INotifyPropertyChanged
	{
		private string _title = string.Empty;
		private string _description = string.Empty;
		private DateTime _deadline;
		private int _priority;
		private bool _isCompleted;

		public string Title
		{
			get => _title;
			set { _title = value; OnPropertyChanged(); }
		}

		public string Description
		{
			get => _description;
			set { _description = value; OnPropertyChanged(); }
		}

		public DateTime Deadline
		{
			get => _deadline;
			set { _deadline = value; OnPropertyChanged(); OnPropertyChanged(nameof(DeadlineDisplay)); }
		}

		public int Priority
		{
			get => _priority;
			set { _priority = value; OnPropertyChanged(); OnPropertyChanged(nameof(PriorityDisplay)); }
		}

		public DateTime CreatedDate { get; set; } = DateTime.Now;

		public bool IsCompleted
		{
			get => _isCompleted;
			set { _isCompleted = value; OnPropertyChanged(); }
		}

		// Дополнительные свойства для отображения
		public string DeadlineDisplay => Deadline.ToString("dd.MM.yyyy HH:mm");
		public string PriorityDisplay => $"Приоритет: {Priority}";

		public override string ToString()
		{
			return $"{Title} (Приоритет: {Priority}, Дедлайн: {Deadline:dd.MM.yyyy HH:mm})";
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
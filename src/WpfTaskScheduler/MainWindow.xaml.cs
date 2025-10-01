using WpfTaskScheduler.ViewModels;
using Lab1_TaskScheduler.Services;
using Lab1_TaskScheduler.Models;
using Lab1_TaskScheduler.Contracts;
using System;
using System.Windows;
using System.Windows.Media;
using System.Linq;

namespace WpfTaskScheduler
{
	public partial class MainWindow : Window
	{
		private MainViewModel _viewModel;
		private ITaskSchedulerService _taskService;

		public MainWindow()
		{
			InitializeComponent();

			// Инициализация сервисов
			_taskService = new TaskSchedulerService();
			_viewModel = new MainViewModel(_taskService);
			DataContext = _viewModel;

			// Подписка на изменения фильтра
			FilterCheckBox.Checked += FilterCheckBox_Changed;
			FilterCheckBox.Unchecked += FilterCheckBox_Changed;
		}

		private void UpdateConditionIndicators()
		{
			// Обновляем цвет индикаторов на основе условий
			PreConditionIndicator.Fill = _viewModel.PreConditionMet ? Brushes.Green : Brushes.Red;
			PostConditionIndicator.Fill = _viewModel.PostConditionMet ? Brushes.Green : Brushes.Red;
		}

		public void AddTaskSucessful(string date,string prior,string name)
		{
            // Создаем тестовую задачу (в реальном приложении будет диалог ввода)
            var newTask = new TaskItem
            {
                Title = name,
                Description = "Описание задачи",
                Deadline = DateTime.Parse(date),
                Priority = Int32.Parse(prior)
            };

            // Проверка предусловий
            try
            {
                // Проверяем предусловия через Guard
                if (newTask != null &&
                    !string.IsNullOrWhiteSpace(newTask.Title) &&
                    newTask.Deadline > DateTime.Now &&
                    newTask.Priority >= 1 && newTask.Priority <= 5)
                {
                    _viewModel.UpdatePreCondition(true);
                }
                else
                {
                    _viewModel.UpdatePreCondition(false, "Невалидные данные задачи");
                    return;
                }
            }
            catch (Exception ex)
            {
                _viewModel.UpdatePreCondition(false, ex.Message);
                return;
            }

            // Выполняем операцию
            bool result = _taskService.AddTask(newTask);
            _viewModel.UpdatePostCondition(result, result ? "" : "Ошибка добавления");

            // Обновляем UI
            _viewModel.RefreshTasks();
            UpdateConditionIndicators();
        }
		private void AddTaskButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				AddWindow addWindow = new AddWindow();
				addWindow.DataContext = _viewModel;
				addWindow.Show();
				addWindow.main = this;
				_viewModel.ResetConditions();

				
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				_viewModel.UpdatePostCondition(false, ex.Message);
				UpdateConditionIndicators();
			}
		}

		private void MoveTaskButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				_viewModel.ResetConditions();

				var selectedTask = TasksListBox.SelectedItem as TaskItem;
				if (selectedTask == null)
				{
					MessageBox.Show("Выберите задачу для переноса", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				// Проверка предусловий
				try
				{
					if (selectedTask != null && _taskService.Tasks.Contains(selectedTask))
					{
						_viewModel.UpdatePreCondition(true);
					}
					else
					{
						_viewModel.UpdatePreCondition(false, "Задача не найдена в списке");
						return;
					}
				}
				catch (Exception ex)
				{
					_viewModel.UpdatePreCondition(false, ex.Message);
					return;
				}

				// Переносим на завтра
				bool result = _taskService.MoveTask(selectedTask, DateTime.Now.AddDays(1));
				_viewModel.UpdatePostCondition(result, result ? "" : "Ошибка переноса");

				// Обновляем UI
				_viewModel.RefreshTasks();
				UpdateConditionIndicators();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				_viewModel.UpdatePostCondition(false, ex.Message);
				UpdateConditionIndicators();
			}
		}

		private void FilterCheckBox_Changed(object sender, RoutedEventArgs e)
		{
			if (FilterCheckBox.IsChecked == true)
			{
				FilterVbox.Visibility = Visibility.Visible;
				ApplyFilter();
			}
			else
			{
				FilterVbox.Visibility = Visibility.Collapsed;
				// Показываем все задачи при отключении фильтра
				_viewModel.RefreshTasks();
				_viewModel.ResetConditions();
				UpdateConditionIndicators();
			}
		}

		private void ApplyFilter()
		{
			bool byDeadline = DeadlineFilterCheckBox?.IsChecked == true;
			bool byPriority = PriorityFilterCheckBox?.IsChecked == true;

			// Если ни один чекбокс не выбран, не применяем фильтр
			if (!byDeadline && !byPriority)
			{
				_viewModel.RefreshTasks();
				_viewModel.ResetConditions();
				UpdateConditionIndicators();
				return;
			}

			try
			{
				_viewModel.ResetConditions();

				// Проверка предусловий
				if (byDeadline || byPriority)
				{
					_viewModel.UpdatePreCondition(true);
				}
				else
				{
					_viewModel.UpdatePreCondition(false, "Выберите критерий фильтрации");
					return;
				}

				var filteredTasks = _taskService.FilterTasks(byDeadline, byPriority);
				_viewModel.Tasks.Clear();
				foreach (var task in filteredTasks)
				{
					_viewModel.Tasks.Add(task);
				}

				_viewModel.UpdatePostCondition(true);
				UpdateConditionIndicators();

				// Показываем сообщение, если результатов нет
				if (filteredTasks.Count == 0)
				{
					MessageBox.Show("Задачи по выбранным критериям не найдены", "Фильтр",
						MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
				_viewModel.UpdatePostCondition(false, ex.Message);
				UpdateConditionIndicators();
			}
		}

		private void FilterCriteria_Changed(object sender, RoutedEventArgs e)
		{
			if (FilterCheckBox.IsChecked == true)
			{
				ApplyFilter();
			}
		}

		private void ShowContractButton_Click(object sender, RoutedEventArgs e)
		{
			// Определяем, какая операция активна
			string operationName = "";
			if (AddTaskButton.IsFocused || sender == AddTaskButton)
				operationName = "AddTask";
			else if (MoveTaskButton.IsFocused || sender == MoveTaskButton)
				operationName = "MoveTask";
			else
				operationName = "FilterTasks";

			if (ContractProvider.Contracts.TryGetValue(operationName, out var contract))
			{
				string message = $"""
                Операция: {contract.Name}

                Предусловия (Pre):
                {contract.PreCondition}

                Постусловия (Post):
                {contract.PostCondition}

                Эффекты:
                {contract.Effects}

                Валидный пример:
                {contract.ValidExample}

                Невалидный пример:
                {contract.InvalidExample}
                """;

				MessageBox.Show(message, "Контракт операции", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			// Для обратной совместимости с оригинальным XAML
			FilterCheckBox_Changed(sender, e);
		}
	}
}
using Lab1_TaskScheduler.Models;
using Lab1_TaskScheduler.Services;
using System;
using Xunit;

namespace Lab1_TaskScheduler.Tests
{
	public class BrokenTests
	{
		[Fact]
		public void AddTask_WithPastDeadline_ShouldSuccess_ButActuallyFails()
		{
			// Arrange
			var service = new TaskSchedulerService();
			var invalidTask = new TaskItem
			{
				Title = "������ � ��������� ���������",
				Description = "��� ������ ������ ������� ������, �� ���� ������� �����",
				Deadline = DateTime.Now.AddDays(-1), // ������� � ������� - ���������!
				Priority = 3
			};

			// Act
			var result = service.AddTask(invalidTask); // ���� ����� ������ ��������� ����������!

			// Assert
			// ������������ ����������� - �� ������� �����, �� �� ����� ���� ������ ���� ������
			Assert.True(result); // ��� ����������� ��������!
			Assert.Contains(invalidTask, service.Tasks); // � ��� ���� ��������!

			// ���������� ���� ������ ��� �� ��������� ���:
			// var exception = Assert.Throws<ArgumentException>(() => service.AddTask(invalidTask));
			// Assert.Contains("������� ������ ���� � �������", exception.Message);
		}

		[Fact]
		public void MoveTask_ToYesterday_ShouldWork_ButActuallyFails()
		{
			// Arrange
			var service = new TaskSchedulerService();
			var task = new TaskItem
			{
				Title = "�������� ������",
				Deadline = DateTime.Now.AddDays(2),
				Priority = 3
			};

			service.AddTask(task);
			var yesterday = DateTime.Now.AddDays(-1); // ����� - ���������� ����!

			// Act
			var result = service.MoveTask(task, yesterday); // ������ ��������� ����������!

			// Assert
			// ������������ ����������� - �� ������� �����
			Assert.True(result); // ��� �����������!
			Assert.Equal(yesterday, task.Deadline); // � ��� �����������!

			// ���������� ����:
			// var exception = Assert.Throws<ArgumentException>(() => service.MoveTask(task, yesterday));
			// Assert.Contains("����� ������� ������ ���� � �������", exception.Message);
		}

		[Fact]
		public void FilterTasks_WithNoCriteria_ShouldReturnAllTasks_ButActuallyFails()
		{
			// Arrange
			var service = new TaskSchedulerService();
			var task1 = new TaskItem { Title = "������ 1", Deadline = DateTime.Now.AddDays(1), Priority = 2 };
			var task2 = new TaskItem { Title = "������ 2", Deadline = DateTime.Now.AddDays(2), Priority = 4 };

			service.AddTask(task1);
			service.AddTask(task2);

			// Act & Assert
			// �����������: �������� ���������� ��� ���������, ��� ������ ������� ����������
			var result = service.FilterTasks(byDeadline: false, byPriority: false); // ������ ���� EXCEPTION!

			// ������������ �����������:
			Assert.Equal(2, result.Count); // ��� �����������!
			Assert.Contains(task1, result); // ��� �����������!
			Assert.Contains(task2, result); // ��� �����������!

			// ���������� ����:
			// var exception = Assert.Throws<ArgumentException>(() => service.FilterTasks(false, false));
			// Assert.Contains("������ ���� ������ ���� �� ���� �������� ����������", exception.Message);
		}

		[Fact]
		public void TaskItem_WithPriorityZero_ShouldBeValid_ButActuallyFails()
		{
			// Arrange
			var task = new TaskItem
			{
				Title = "������ � ������� �����������",
				Deadline = DateTime.Now.AddDays(1),
				Priority = 0 // ���������� ���������!
			};

			var service = new TaskSchedulerService();

			// Act & Assert
			// �����������: �������� �������� ������ � ����������� 0
			var result = service.AddTask(task); // ������ ���� EXCEPTION!

			Assert.True(result); // ��� �����������!
			Assert.Equal(0, task.Priority); // ��� �����������!

			// ���������� ����:
			// var exception = Assert.Throws<ArgumentException>(() => service.AddTask(task));
			// Assert.Contains("��������� ������ ���� �� 1 �� 5", exception.Message);
		}

		[Fact]
		public void AddNullTask_ShouldWork_ButActuallyFails()
		{
			// Arrange
			var service = new TaskSchedulerService();
			TaskItem nullTask = null!; // ���� �������� null - ��������� ������!

			// Act & Assert
			// �����������: �������� �������� null
			var result = service.AddTask(nullTask); // ������ ���� EXCEPTION!

			Assert.True(result); // ��� ���������� �����������!

			// ���������� ����:
			// var exception = Assert.Throws<ArgumentException>(() => service.AddTask(nullTask));
			// Assert.Contains("������ �� ����� ���� null", exception.Message);
		}
	}
}
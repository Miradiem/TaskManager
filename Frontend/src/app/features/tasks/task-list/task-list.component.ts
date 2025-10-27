import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/auth/auth.service';
import { Task, TaskFilters, TaskStatus, TASK_STATUS_LABELS } from '../../../core/models/task.model';
import { TaskFormComponent } from '../task-form/task-form.component';

@Component({
  selector: 'app-task-list',
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    DatePipe
  ]
})
export class TaskListComponent implements OnInit {
  tasks: Task[] = [];
  totalCount = 0;
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;
  isLoading = false;

  selectedStatus?: TaskStatus;
  dueDateFrom?: string;
  dueDateTo?: string;

  TaskStatus = TaskStatus;
  TASK_STATUS_LABELS = TASK_STATUS_LABELS;

  statusOptions = Object.values(TaskStatus).filter(value => typeof value === 'number') as TaskStatus[];

  constructor(
    private apiService: ApiService,
    public authService: AuthService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.isLoading = true;
    
    const filters: TaskFilters = {
      status: this.selectedStatus,
      dueDateFrom: this.dueDateFrom,
      dueDateTo: this.dueDateTo,
      page: this.currentPage,
      pageSize: this.pageSize
    };

    this.apiService.getTasks(filters).subscribe({
      next: (response) => {
        this.tasks = response.tasks;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.isLoading = false;
      },
      error: (error) => {
        this.snackBar.open(`Failed to load tasks: ${error.message || 'Unknown error'}`, 'Close', { duration: 5000 });
        this.isLoading = false;
      }
    });
  }


  onFilterChange(): void {
    this.currentPage = 1;
    this.loadTasks();
  }

  applyFilters(): void {
    if (this.dueDateFrom && !this.isValidDate(this.dueDateFrom)) {
      this.snackBar.open('Invalid "Due Date From" format. Please enter a valid date.', 'Close', { duration: 5000 });
      return;
    }
    
    if (this.dueDateTo && !this.isValidDate(this.dueDateTo)) {
      this.snackBar.open('Invalid "Due Date To" format. Please enter a valid date.', 'Close', { duration: 5000 });
      return;
    }

    if (this.dueDateFrom && this.dueDateTo && new Date(this.dueDateFrom) > new Date(this.dueDateTo)) {
      this.snackBar.open('"Due Date From" cannot be later than "Due Date To".', 'Close', { duration: 5000 });
      return;
    }

    this.currentPage = 1;
    this.loadTasks();
  }

  private isValidDate(dateString: string): boolean {
    if (!dateString) return true;
    
    const date = new Date(dateString);
    const year = date.getFullYear();
    
    return !isNaN(date.getTime()) && year >= 1900 && year <= 2100;
  }

  clearFilters(): void {
    this.selectedStatus = undefined;
    this.dueDateFrom = undefined;
    this.dueDateTo = undefined;
    this.currentPage = 1;
    this.loadTasks();
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(TaskFormComponent, {
      width: '500px',
      height: 'auto',
      maxHeight: '90vh',
      data: { mode: 'create' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadTasks();
      }
    });
  }

  openEditDialog(task: Task): void {
    const dialogRef = this.dialog.open(TaskFormComponent, {
      width: '500px',
      height: 'auto',
      maxHeight: '90vh',
      data: { mode: 'edit', task }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadTasks();
      }
    });
  }

  deleteTask(task: Task): void {
    if (confirm(`Are you sure you want to delete "${task.title}"?`)) {
      this.apiService.deleteTask(task.id).subscribe({
        next: () => {
          this.snackBar.open('Task deleted successfully', 'Close', { duration: 3000 });
          this.loadTasks();
        },
        error: (error) => {
          this.snackBar.open('Failed to delete task', 'Close', { duration: 3000 });
          console.error('Error deleting task:', error);
        }
      });
    }
  }


  getStatusClass(status: TaskStatus): string {
    switch (status) {
      case TaskStatus.New: return 'status-new';
      case TaskStatus.InProgress: return 'status-in-progress';
      case TaskStatus.Done: return 'status-done';
      default: return '';
    }
  }

  getCurrentUser(): string {
    const user = this.authService.getCurrentUser();
    return user ? `${user.firstName} ${user.lastName}` : 'User';
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadTasks();
    }
  }

  onPageSizeChange(newPageSize: number): void {
    this.pageSize = newPageSize;
    this.currentPage = 1;
    this.loadTasks();
  }
}

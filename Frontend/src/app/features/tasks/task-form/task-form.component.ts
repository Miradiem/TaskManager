import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { Task, CreateTaskRequest, UpdateTaskRequest, TaskStatus, TASK_STATUS_LABELS } from '../../../core/models/task.model';

export interface TaskFormData {
  mode: 'create' | 'edit';
  task?: Task;
}

@Component({
  selector: 'app-task-form',
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ]
})
export class TaskFormComponent implements OnInit {
  taskForm: FormGroup;
  isEditMode = false;
  isLoading = false;

  TaskStatus = TaskStatus;
  TASK_STATUS_LABELS = TASK_STATUS_LABELS;

  statusOptions = Object.values(TaskStatus).filter(value => typeof value === 'number') as TaskStatus[];

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private dialogRef: MatDialogRef<TaskFormComponent>,
    private snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: TaskFormData
  ) {
    this.isEditMode = data.mode === 'edit';
    this.taskForm = this.createForm();
  }

  ngOnInit(): void {
    if (this.isEditMode && this.data.task) {
      this.populateForm(this.data.task);
    }
  }

  private createForm(): FormGroup {
    const formConfig: any = {
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(1000)]],
      dueDate: ['']
    };

    if (this.isEditMode) {
      formConfig.status = [TaskStatus.New, [Validators.required]];
    }

    return this.fb.group(formConfig);
  }

  private populateForm(task: Task): void {
    this.taskForm.patchValue({
      title: task.title,
      description: task.description || '',
      status: task.status,
      dueDate: task.dueDate ? new Date(task.dueDate).toISOString().split('T')[0] : ''
    });
  }

  onSubmit(): void {
    if (this.taskForm.valid) {
      this.isLoading = true;
      const formValue = this.taskForm.value;

      if (this.isEditMode && this.data.task) {
        const updateRequest: UpdateTaskRequest = {
          title: formValue.title,
          description: formValue.description || undefined,
          status: formValue.status,
          dueDate: formValue.dueDate ? new Date(formValue.dueDate).toISOString() : undefined
        };

        this.apiService.updateTask(this.data.task.id, updateRequest).subscribe({
          next: () => {
            this.snackBar.open('Task updated successfully', 'Close', { duration: 3000 });
            this.dialogRef.close(true);
            this.isLoading = false;
          },
          error: (error) => {
            this.snackBar.open('Failed to update task', 'Close', { duration: 3000 });
            console.error('Error updating task:', error);
            this.isLoading = false;
          }
        });
      } else {
        const createRequest: CreateTaskRequest = {
          title: formValue.title,
          description: formValue.description || undefined,
          dueDate: formValue.dueDate ? new Date(formValue.dueDate).toISOString() : undefined
        };

        this.apiService.createTask(createRequest).subscribe({
          next: (response) => {
            this.snackBar.open('Task created successfully', 'Close', { duration: 3000 });
            this.dialogRef.close(true);
            this.isLoading = false;
          },
          error: (error) => {
            this.snackBar.open(`Failed to create task: ${error.message || 'Unknown error'}`, 'Close', { duration: 5000 });
            this.isLoading = false;
          }
        });
      }
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  getTitle(): string {
    return this.isEditMode ? 'Edit Task' : 'Create Task';
  }

  getSubmitButtonText(): string {
    return this.isEditMode ? 'Update Task' : 'Create Task';
  }
}
export interface Task {
  id: string;
  title: string;
  description?: string;
  status: TaskStatus;
  dueDate?: string;
  createdAt: string;
  updatedAt: string;
}

export enum TaskStatus {
  New = 0,
  InProgress = 1,
  Done = 2
}


export interface CreateTaskRequest {
  title: string;
  description?: string;
  dueDate?: string;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string;
  status: TaskStatus;
  dueDate?: string;
}

export interface TaskListResponse {
  tasks: Task[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface TaskFilters {
  status?: TaskStatus;
  dueDateFrom?: string;
  dueDateTo?: string;
  page?: number;
  pageSize?: number;
}

export const TASK_STATUS_LABELS: Record<TaskStatus, string> = {
  [TaskStatus.New]: 'New',
  [TaskStatus.InProgress]: 'In Progress',
  [TaskStatus.Done]: 'Done'
};


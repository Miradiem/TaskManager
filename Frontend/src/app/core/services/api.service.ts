import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse } from '../models/user.model';
import { Task, CreateTaskRequest, UpdateTaskRequest, TaskListResponse, TaskFilters } from '../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/auth/login`, credentials);
  }

  register(userData: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.baseUrl}/auth/register`, userData);
  }

  getTasks(filters?: TaskFilters): Observable<TaskListResponse> {
    const params = this.buildQueryParams(filters);
    return this.http.get<TaskListResponse>(`${this.baseUrl}/tasks`, { params });
  }

  getTaskById(id: string): Observable<Task> {
    return this.http.get<Task>(`${this.baseUrl}/tasks/${id}`);
  }

  createTask(task: CreateTaskRequest): Observable<Task> {
    return this.http.post<Task>(`${this.baseUrl}/tasks`, task);
  }

  updateTask(id: string, task: UpdateTaskRequest): Observable<Task> {
    return this.http.put<Task>(`${this.baseUrl}/tasks/${id}`, task);
  }

  deleteTask(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/tasks/${id}`);
  }

  private buildQueryParams(filters?: TaskFilters): { [key: string]: string } {
    if (!filters) return {};
    
    const params: { [key: string]: string } = {};
    
    if (filters.status !== undefined) params['status'] = filters.status.toString();
    if (filters.dueDateFrom) params['dueDateFrom'] = filters.dueDateFrom;
    if (filters.dueDateTo) params['dueDateTo'] = filters.dueDateTo;
    if (filters.page !== undefined) params['page'] = filters.page.toString();
    if (filters.pageSize !== undefined) params['pageSize'] = filters.pageSize.toString();
    
    return params;
  }
}
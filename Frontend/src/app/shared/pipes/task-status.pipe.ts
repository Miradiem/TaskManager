import { Pipe, PipeTransform } from '@angular/core';
import { TaskStatus } from '../../core/models/task.model';

@Pipe({
  name: 'taskStatus',
  standalone: true
})
export class TaskStatusPipe implements PipeTransform {
  transform(status: TaskStatus): string {
    const statusMap: Record<TaskStatus, string> = {
      [TaskStatus.New]: 'New',
      [TaskStatus.InProgress]: 'In Progress',
      [TaskStatus.Done]: 'Done'
    };
    
    return statusMap[status] || 'Unknown';
  }
}

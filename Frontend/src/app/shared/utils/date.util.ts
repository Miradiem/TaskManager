export class DateUtil {
  /**
   * Formats a date to a readable string
   */
  static formatDate(date: Date | string | null | undefined): string {
    if (!date) return '';
    
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    
    if (isNaN(dateObj.getTime())) return '';
    
    return dateObj.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  /**
   * Formats a date to a short string (MM/DD/YYYY)
   */
  static formatDateShort(date: Date | string | null | undefined): string {
    if (!date) return '';
    
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    
    if (isNaN(dateObj.getTime())) return '';
    
    return dateObj.toLocaleDateString('en-US');
  }

  /**
   * Checks if a date is today
   */
  static isToday(date: Date | string | null | undefined): boolean {
    if (!date) return false;
    
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    const today = new Date();
    
    return dateObj.toDateString() === today.toDateString();
  }

  /**
   * Checks if a date is in the past
   */
  static isPast(date: Date | string | null | undefined): boolean {
    if (!date) return false;
    
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    return dateObj < today;
  }
}

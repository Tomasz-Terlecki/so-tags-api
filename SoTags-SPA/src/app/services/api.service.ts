import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiBaseUrl = 'http://localhost:8080';

  constructor(private http: HttpClient) {}

  getSoTags(
    pageNumber: number = 1,
    pageSize: number = 10,
    sortBy: string = 'None',
    sortDirection: string = 'asc'
  ): Observable<any> {
    let url = `${this.apiBaseUrl}/sotags?pageNumber=${pageNumber}&pageSize=${pageSize}&sortBy=${this.switchSortBy(sortBy)}&sortDirection=${this.switchSortDirection(sortDirection)}`;
    
    return this.http.get(url);
  }

  refetchSoTags(): Observable<any> {
    return this.http.post(`${this.apiBaseUrl}/sotags/refetch`, {});
  }

  private switchSortBy(field: string): number {
    switch (field) {
      case 'Name':
        return 1;
      case 'Share':
        return 2;
      default:
        return 0;
    }
  }

  private switchSortDirection(direction: string): number {
    switch (direction) {
      case 'desc':
        return 1;
      case 'asc':
      default:
        return 0;
    }
  }
}

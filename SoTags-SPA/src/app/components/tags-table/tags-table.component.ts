import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { SoTag, PaginatedResponse } from '../../models/sotag.model';

@Component({
  selector: 'app-tags-table',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tags-table.component.html',
  styleUrl: './tags-table.component.css'
})
export class TagsTableComponent implements OnInit {
  tags = signal<SoTag[]>([]);
  totalCount = signal(0);
  pageNumber = signal(1);
  pageSize = signal(10);
  sortBy = signal('None');
  sortDirection = signal('desc');
  isLoading = signal(false);
  error = signal<string | null>(null);

  // For ngModel binding
  sortByValue = 'None';
  sortDirectionValue = 'desc';
  pageSizeValue = 10;

  sortByOptions = ['None', 'Name', 'Share'];
  sortDirections = ['asc', 'desc'];
  pageSizeOptions = [5, 10, 25, 50];

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadTags();
  }

  loadTags() {
    this.isLoading.set(true);
    this.error.set(null);

    this.apiService
      .getSoTags(
        this.pageNumber(),
        this.pageSize(),
        this.sortBy(),
        this.sortDirection()
      )
      .subscribe({
        next: (response: PaginatedResponse<SoTag>) => {
          this.tags.set(response.items);
          this.totalCount.set(response.totalCount);
          this.isLoading.set(false);
        },
        error: (err: unknown) => {
          console.error('Error loading tags:', err);
          this.error.set('Błąd podczas ładowania tagów');
          this.isLoading.set(false);
        }
      });
  }

  onPageChange(page: number) {
    if (page >= 1 && page <= this.totalPages()) {
      this.pageNumber.set(page);
      this.loadTags();
    }
  }

  onPageSizeChange(size: number | string) {
    const numSize = typeof size === 'string' ? parseInt(size, 10) : size;
    this.pageSizeValue = numSize;
    this.pageSize.set(numSize);
    this.pageNumber.set(1);
    this.loadTags();
  }

  onSortByChange(field: string) {
    this.sortByValue = field;
    this.sortBy.set(field);
    this.pageNumber.set(1);
    this.loadTags();
  }

  onSortDirectionChange(direction: string) {
    this.sortDirectionValue = direction;
    this.sortDirection.set(direction);
    this.pageNumber.set(1);
    this.loadTags();
  }

  refetchTags() {
    this.isLoading.set(true);
    this.error.set(null);

    this.apiService.refetchSoTags().subscribe({
      next: () => {
        this.pageNumber.set(1);
        this.loadTags();
      },
      error: (err: unknown) => {
        console.error('Error refetching tags:', err);
        this.error.set('Błąd podczas odświeżania tagów');
        this.isLoading.set(false);
      }
    });
  }

  totalPages(): number {
    return Math.ceil(this.totalCount() / this.pageSize());
  }

  get paginationInfo(): string {
    const start = (this.pageNumber() - 1) * this.pageSize() + 1;
    const end = Math.min(this.pageNumber() * this.pageSize(), this.totalCount());
    return `Wyświetlanie ${start} - ${end} z ${this.totalCount()}`;
  }
}

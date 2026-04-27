export interface SoTag {
  id: number;
  name: string;
  count: number;
  isRequired: boolean;
  isModeratorOnly: boolean;
  hasSynonyms: boolean;
  share: number;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

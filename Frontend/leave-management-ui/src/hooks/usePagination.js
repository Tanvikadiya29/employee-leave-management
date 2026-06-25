import { useState } from 'react';

export function usePagination(initialPageSize = 10) {
  const [page, setPage] = useState(1);
  const [pageSize] = useState(initialPageSize);

  const goToPage = (newPage) => setPage(newPage);
  const reset = () => setPage(1);

  return { page, pageSize, goToPage, reset };
}

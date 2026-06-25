import { format, parseISO } from 'date-fns';

export function formatDate(dateStr) {
  if (!dateStr) return '—';
  try {
    return format(parseISO(dateStr), 'dd MMM yyyy');
  } catch {
    return dateStr;
  }
}

export function formatDateTime(dateStr) {
  if (!dateStr) return '—';
  try {
    return format(parseISO(dateStr), 'dd MMM yyyy, HH:mm');
  } catch {
    return dateStr;
  }
}

export function toISODate(date) {
  if (!date) return '';
  if (typeof date === 'string') return date;
  return format(date, 'yyyy-MM-dd');
}

export function todayISO() {
  return format(new Date(), 'yyyy-MM-dd');
}

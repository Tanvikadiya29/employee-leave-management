import StatusBadge from '../common/StatusBadge';
import LeaveActions from './LeaveActions';
import { formatDate, formatDateTime } from '../../utils/dateUtils';

export default function LeaveList({ leaves, isAdmin, onApprove, onReject, onCancel }) {
  if (!leaves || leaves.length === 0) {
    return (
      <div className="text-center py-12 text-gray-500">
        <p className="text-lg">No leave requests found.</p>
      </div>
    );
  }

  return (
    <div className="overflow-x-auto rounded-lg border border-gray-200">
      <table className="min-w-full divide-y divide-gray-200 text-sm">
        <thead className="bg-gray-50">
          <tr>
            {isAdmin && <th className="px-4 py-3 text-left font-medium text-gray-600">Employee</th>}
            <th className="px-4 py-3 text-left font-medium text-gray-600">From</th>
            <th className="px-4 py-3 text-left font-medium text-gray-600">To</th>
            <th className="px-4 py-3 text-left font-medium text-gray-600">Days</th>
            <th className="px-4 py-3 text-left font-medium text-gray-600">Reason</th>
            <th className="px-4 py-3 text-left font-medium text-gray-600">Status</th>
            <th className="px-4 py-3 text-left font-medium text-gray-600">Applied On</th>
            {isAdmin && <th className="px-4 py-3 text-left font-medium text-gray-600">Reviewed By</th>}
            <th className="px-4 py-3 text-right font-medium text-gray-600">Actions</th>
          </tr>
        </thead>
        <tbody className="bg-white divide-y divide-gray-100">
          {leaves.map((leave) => (
            <tr key={leave.id} className="hover:bg-gray-50 transition-colors">
              {isAdmin && (
                <td className="px-4 py-3">
                  <div className="font-medium text-gray-800">{leave.employeeName}</div>
                  <div className="text-xs text-gray-500">{leave.department || ''}</div>
                </td>
              )}
              <td className="px-4 py-3 text-gray-700">{formatDate(leave.fromDate)}</td>
              <td className="px-4 py-3 text-gray-700">{formatDate(leave.toDate)}</td>
              <td className="px-4 py-3 text-gray-700 font-medium">{leave.totalDays}</td>
              <td className="px-4 py-3 text-gray-600 max-w-xs truncate" title={leave.reason}>{leave.reason}</td>
              <td className="px-4 py-3"><StatusBadge status={leave.status} /></td>
              <td className="px-4 py-3 text-gray-500">{formatDate(leave.createdAt)}</td>
              {isAdmin && (
                <td className="px-4 py-3 text-gray-500 text-xs">
                  {leave.reviewedByName || '—'}
                  {leave.reviewedAt && <div>{formatDateTime(leave.reviewedAt)}</div>}
                  {leave.remarks && <div className="italic text-gray-400">"{leave.remarks}"</div>}
                </td>
              )}
              <td className="px-4 py-3 text-right">
                <LeaveActions
                  leave={leave}
                  isAdmin={isAdmin}
                  onApprove={onApprove}
                  onReject={onReject}
                  onCancel={onCancel}
                />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

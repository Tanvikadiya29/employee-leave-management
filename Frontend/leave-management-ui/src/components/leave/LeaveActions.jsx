import { FiCheck, FiX, FiTrash2 } from 'react-icons/fi';

export default function LeaveActions({ leave, isAdmin, onApprove, onReject, onCancel }) {
  if (leave.status !== 'Pending') {
    return <span className="text-gray-400 text-xs">—</span>;
  }

  if (isAdmin) {
    return (
      <div className="flex justify-end gap-2">
        <button
          onClick={() => onApprove(leave)}
          className="flex items-center gap-1 px-2.5 py-1.5 text-xs bg-green-50 text-green-700 border border-green-200 rounded-lg hover:bg-green-100 transition-colors"
          title="Approve"
        >
          <FiCheck className="w-3.5 h-3.5" /> Approve
        </button>
        <button
          onClick={() => onReject(leave)}
          className="flex items-center gap-1 px-2.5 py-1.5 text-xs bg-red-50 text-red-700 border border-red-200 rounded-lg hover:bg-red-100 transition-colors"
          title="Reject"
        >
          <FiX className="w-3.5 h-3.5" /> Reject
        </button>
      </div>
    );
  }

  return (
    <button
      onClick={() => onCancel(leave)}
      className="flex items-center gap-1 px-2.5 py-1.5 text-xs bg-gray-50 text-gray-600 border border-gray-200 rounded-lg hover:bg-red-50 hover:text-red-600 hover:border-red-200 transition-colors"
      title="Cancel"
    >
      <FiTrash2 className="w-3.5 h-3.5" /> Cancel
    </button>
  );
}

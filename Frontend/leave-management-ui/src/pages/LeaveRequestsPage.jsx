import { useState, useEffect, useCallback } from 'react';
import { toast } from 'react-toastify';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import Navbar from '../components/common/Navbar';
import Sidebar from '../components/common/Sidebar';
import LeaveList from '../components/leave/LeaveList';
import LeaveForm from '../components/leave/LeaveForm';
import ConfirmModal from '../components/common/ConfirmModal';
import Pagination from '../components/common/Pagination';
import LoadingSpinner from '../components/common/LoadingSpinner';
import { useAuth } from '../context/AuthContext';
import { getAllLeaves, getMyLeaves, createLeave, approveLeave, rejectLeave, cancelLeave } from '../api/leaveApi';
import { usePagination } from '../hooks/usePagination';
import { FiPlus, FiFilter } from 'react-icons/fi';

const remarksSchema = yup.object({
  remarks: yup.string().max(500, 'Remarks cannot exceed 500 characters').optional(),
});

export default function LeaveRequestsPage() {
  const { isAdmin } = useAuth();
  const { page, pageSize, goToPage, reset } = usePagination(10);

  const [leaves, setLeaves] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [statusFilter, setStatusFilter] = useState('');

  const [showLeaveForm, setShowLeaveForm] = useState(false);
  const [leaveFormLoading, setLeaveFormLoading] = useState(false);

  const [actionTarget, setActionTarget] = useState(null);
  const [actionType, setActionType] = useState(null);
  const [actionLoading, setActionLoading] = useState(false);
  const [showRemarksModal, setShowRemarksModal] = useState(false);

  const { register: regRemarks, handleSubmit: handleRemarks, reset: resetRemarks, formState: { errors: remarksErrors } } = useForm({
    resolver: yupResolver(remarksSchema),
  });

  const fetchLeaves = useCallback(async () => {
    setLoading(true);
    try {
      const res = isAdmin
        ? await getAllLeaves(page, pageSize, statusFilter)
        : await getMyLeaves(page, pageSize);
      setLeaves(res.data?.data || []);
      setTotalCount(res.data?.totalCount || 0);
    } catch {
      toast.error('Failed to load leave requests.');
    } finally {
      setLoading(false);
    }
  }, [isAdmin, page, pageSize, statusFilter]);

  useEffect(() => {
    fetchLeaves();
  }, [fetchLeaves]);

  const handleCreateLeave = async (data) => {
    setLeaveFormLoading(true);
    try {
      await createLeave({
        fromDate: data.fromDate,
        toDate: data.toDate,
        reason: data.reason,
      });
      toast.success('Leave request submitted successfully!');
      setShowLeaveForm(false);
      reset();
      fetchLeaves();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to submit leave request.');
    } finally {
      setLeaveFormLoading(false);
    }
  };

  const openApprove = (leave) => {
    setActionTarget(leave);
    setActionType('approve');
    setShowRemarksModal(true);
    resetRemarks({});
  };

  const openReject = (leave) => {
    setActionTarget(leave);
    setActionType('reject');
    setShowRemarksModal(true);
    resetRemarks({});
  };

  const handleReviewSubmit = async (data) => {
    setActionLoading(true);
    try {
      if (actionType === 'approve') {
        await approveLeave(actionTarget.id, { remarks: data.remarks || null });
        toast.success('Leave request approved!');
      } else {
        await rejectLeave(actionTarget.id, { remarks: data.remarks || null });
        toast.success('Leave request rejected.');
      }
      setShowRemarksModal(false);
      setActionTarget(null);
      fetchLeaves();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Action failed.');
    } finally {
      setActionLoading(false);
    }
  };

  const [cancelTarget, setCancelTarget] = useState(null);
  const [cancelLoading, setCancelLoading] = useState(false);

  const handleCancel = async () => {
    setCancelLoading(true);
    try {
      await cancelLeave(cancelTarget.id);
      toast.success('Leave request cancelled.');
      setCancelTarget(null);
      fetchLeaves();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to cancel leave.');
    } finally {
      setCancelLoading(false);
    }
  };

  const handleFilterChange = (val) => {
    setStatusFilter(val);
    reset();
  };

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col">
      <Navbar />
      <div className="flex flex-1">
        <Sidebar />
        <main className="flex-1 p-6">
          <div className="flex items-center justify-between mb-6">
            <div>
              <h1 className="text-2xl font-bold text-gray-800">
                {isAdmin ? 'All Leave Requests' : 'My Leave Requests'}
              </h1>
              <p className="text-gray-500 text-sm mt-1">{totalCount} total requests</p>
            </div>
            {!isAdmin && (
              <button
                onClick={() => setShowLeaveForm(true)}
                className="flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white text-sm font-medium px-4 py-2.5 rounded-lg transition-colors"
              >
                <FiPlus className="w-4 h-4" /> Apply for Leave
              </button>
            )}
          </div>

          {isAdmin && (
            <div className="mb-4 flex items-center gap-3">
              <FiFilter className="text-gray-400 w-4 h-4" />
              <span className="text-sm text-gray-600 font-medium">Filter:</span>
              {['', 'Pending', 'Approved', 'Rejected'].map((s) => (
                <button
                  key={s}
                  onClick={() => handleFilterChange(s)}
                  className={`px-3 py-1.5 text-xs rounded-lg border transition-colors ${
                    statusFilter === s
                      ? 'bg-blue-600 text-white border-blue-600'
                      : 'border-gray-300 text-gray-600 hover:bg-gray-50'
                  }`}
                >
                  {s || 'All'}
                </button>
              ))}
            </div>
          )}

          {loading ? (
            <LoadingSpinner message="Loading leave requests..." />
          ) : (
            <>
              <LeaveList
                leaves={leaves}
                isAdmin={isAdmin}
                onApprove={openApprove}
                onReject={openReject}
                onCancel={(leave) => setCancelTarget(leave)}
              />
              <Pagination
                page={page}
                pageSize={pageSize}
                totalCount={totalCount}
                onPageChange={goToPage}
              />
            </>
          )}
        </main>
      </div>

      <LeaveForm
        isOpen={showLeaveForm}
        onClose={() => setShowLeaveForm(false)}
        onSubmit={handleCreateLeave}
        loading={leaveFormLoading}
      />

      {showRemarksModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
          <div className="bg-white rounded-2xl shadow-xl p-6 w-full max-w-sm mx-4">
            <h3 className="text-lg font-semibold text-gray-800 mb-1">
              {actionType === 'approve' ? 'Approve Leave Request' : 'Reject Leave Request'}
            </h3>
            <p className="text-sm text-gray-500 mb-4">
              Add an optional remark for the employee.
            </p>
            <form onSubmit={handleRemarks(handleReviewSubmit)} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Remarks (optional)</label>
                <textarea
                  rows={3}
                  {...regRemarks('remarks')}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Add a comment..."
                />
                {remarksErrors.remarks && (
                  <p className="text-red-500 text-xs mt-1">{remarksErrors.remarks.message}</p>
                )}
              </div>
              <div className="flex gap-3 justify-end">
                <button
                  type="button"
                  onClick={() => { setShowRemarksModal(false); setActionTarget(null); }}
                  disabled={actionLoading}
                  className="px-4 py-2 text-sm border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={actionLoading}
                  className={`px-4 py-2 text-sm text-white rounded-lg disabled:opacity-50 ${
                    actionType === 'approve'
                      ? 'bg-green-600 hover:bg-green-700'
                      : 'bg-red-600 hover:bg-red-700'
                  }`}
                >
                  {actionLoading ? 'Processing...' : actionType === 'approve' ? 'Approve' : 'Reject'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      <ConfirmModal
        isOpen={Boolean(cancelTarget)}
        title="Cancel Leave Request"
        message="Are you sure you want to cancel this leave request? This action cannot be undone."
        onConfirm={handleCancel}
        onCancel={() => setCancelTarget(null)}
        loading={cancelLoading}
        confirmLabel="Yes, Cancel It"
      />
    </div>
  );
}

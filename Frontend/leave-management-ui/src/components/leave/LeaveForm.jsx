import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { todayISO } from '../../utils/dateUtils';

const schema = yup.object({
  fromDate: yup
    .string()
    .required('From date is required')
    .test('not-past', 'Start date cannot be in the past', (val) => {
      if (!val) return true;
      return val >= todayISO();
    }),
  toDate: yup
    .string()
    .required('To date is required')
    .test('after-from', 'End date must be on or after start date', function (val) {
      const { fromDate } = this.parent;
      if (!val || !fromDate) return true;
      return val >= fromDate;
    }),
  reason: yup
    .string()
    .required('Reason is required')
    .min(5, 'Reason must be at least 5 characters')
    .max(1000, 'Reason cannot exceed 1000 characters'),
});

export default function LeaveForm({ isOpen, onClose, onSubmit, loading }) {
  const { register, handleSubmit, reset, formState: { errors } } = useForm({
    resolver: yupResolver(schema),
  });

  useEffect(() => {
    if (isOpen) reset({});
  }, [isOpen, reset]);

  if (!isOpen) return null;

  const inputClass = 'w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500';
  const errorClass = 'text-red-500 text-xs mt-1';
  const today = todayISO();

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
      <div className="bg-white rounded-2xl shadow-xl p-6 w-full max-w-md mx-4">
        <h2 className="text-xl font-semibold text-gray-800 mb-5">Apply for Leave</h2>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">From Date *</label>
              <input type="date" min={today} {...register('fromDate')} className={inputClass} />
              {errors.fromDate && <p className={errorClass}>{errors.fromDate.message}</p>}
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">To Date *</label>
              <input type="date" min={today} {...register('toDate')} className={inputClass} />
              {errors.toDate && <p className={errorClass}>{errors.toDate.message}</p>}
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Reason *</label>
            <textarea
              rows={4}
              {...register('reason')}
              className={inputClass}
              placeholder="Explain the reason for your leave (min 5 characters)..."
            />
            {errors.reason && <p className={errorClass}>{errors.reason.message}</p>}
          </div>

          <div className="flex gap-3 justify-end pt-2">
            <button
              type="button"
              onClick={onClose}
              disabled={loading}
              className="px-4 py-2 text-sm border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-4 py-2 text-sm bg-blue-600 hover:bg-blue-700 text-white rounded-lg disabled:opacity-50"
            >
              {loading ? 'Submitting...' : 'Submit Request'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

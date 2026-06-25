import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';

const createSchema = yup.object({
  firstName: yup.string().required('First name is required').min(2, 'Min 2 characters'),
  lastName: yup.string().required('Last name is required').min(2, 'Min 2 characters'),
  email: yup.string().email('Invalid email').required('Email is required'),
  password: yup
    .string()
    .required('Password is required')
    .min(8, 'Min 8 characters')
    .matches(
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$/,
      'Must have uppercase, lowercase, digit and special character'
    ),
  department: yup.string().optional(),
  designation: yup.string().optional(),
  dateOfJoining: yup.string().optional(),
});

const updateSchema = yup.object({
  firstName: yup.string().optional().min(2, 'Min 2 characters'),
  lastName: yup.string().optional().min(2, 'Min 2 characters'),
  department: yup.string().optional(),
  designation: yup.string().optional(),
  dateOfJoining: yup.string().optional(),
});

export default function EmployeeForm({ isOpen, onClose, onSubmit, loading, employee }) {
  const isEdit = Boolean(employee);
  const schema = isEdit ? updateSchema : createSchema;

  const { register, handleSubmit, reset, formState: { errors } } = useForm({
    resolver: yupResolver(schema),
  });

  useEffect(() => {
    if (isOpen) {
      if (employee) {
        reset({
          firstName: employee.firstName || '',
          lastName: employee.lastName || '',
          department: employee.department || '',
          designation: employee.designation || '',
          dateOfJoining: employee.dateOfJoining || '',
        });
      } else {
        reset({});
      }
    }
  }, [isOpen, employee, reset]);

  if (!isOpen) return null;

  const inputClass = 'w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500';
  const errorClass = 'text-red-500 text-xs mt-1';

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
      <div className="bg-white rounded-2xl shadow-xl p-6 w-full max-w-lg mx-4 max-h-[90vh] overflow-y-auto">
        <h2 className="text-xl font-semibold text-gray-800 mb-5">
          {isEdit ? 'Edit Employee' : 'Add Employee'}
        </h2>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">First Name *</label>
              <input {...register('firstName')} className={inputClass} placeholder="Jane" />
              {errors.firstName && <p className={errorClass}>{errors.firstName.message}</p>}
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Last Name *</label>
              <input {...register('lastName')} className={inputClass} placeholder="Smith" />
              {errors.lastName && <p className={errorClass}>{errors.lastName.message}</p>}
            </div>
          </div>

          {!isEdit && (
            <>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Email *</label>
                <input type="email" {...register('email')} className={inputClass} placeholder="jane@company.com" />
                {errors.email && <p className={errorClass}>{errors.email.message}</p>}
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Password *</label>
                <input type="password" {...register('password')} className={inputClass} placeholder="Min 8 chars, uppercase, digit, special" />
                {errors.password && <p className={errorClass}>{errors.password.message}</p>}
              </div>
            </>
          )}

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Department</label>
              <input {...register('department')} className={inputClass} placeholder="Engineering" />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Designation</label>
              <input {...register('designation')} className={inputClass} placeholder="Developer" />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Date of Joining</label>
            <input type="date" {...register('dateOfJoining')} className={inputClass} />
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
              {loading ? 'Saving...' : isEdit ? 'Save Changes' : 'Add Employee'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

import { FiEdit2, FiTrash2 } from 'react-icons/fi';

export default function EmployeeList({ employees, onEdit, onDelete }) {
  if (!employees || employees.length === 0) {
    return (
      <div className="text-center py-12 text-gray-500">
        <p className="text-lg">No employees found.</p>
        <p className="text-sm mt-1">Add your first employee using the button above.</p>
      </div>
    );
  }

  return (
    <div className="overflow-x-auto rounded-lg border border-gray-200">
      <table className="min-w-full divide-y divide-gray-200 text-sm">
        <thead className="bg-gray-50">
          <tr>
            <th className="px-4 py-3 text-left font-medium text-gray-600">Name</th>
            <th className="px-4 py-3 text-left font-medium text-gray-600">Email</th>
            <th className="px-4 py-3 text-left font-medium text-gray-600">Department</th>
            <th className="px-4 py-3 text-left font-medium text-gray-600">Designation</th>
            <th className="px-4 py-3 text-left font-medium text-gray-600">Joined</th>
            <th className="px-4 py-3 text-right font-medium text-gray-600">Actions</th>
          </tr>
        </thead>
        <tbody className="bg-white divide-y divide-gray-100">
          {employees.map((emp) => (
            <tr key={emp.id} className="hover:bg-gray-50 transition-colors">
              <td className="px-4 py-3 font-medium text-gray-800">{emp.fullName || `${emp.firstName} ${emp.lastName}`}</td>
              <td className="px-4 py-3 text-gray-600">{emp.email}</td>
              <td className="px-4 py-3 text-gray-600">{emp.department || '—'}</td>
              <td className="px-4 py-3 text-gray-600">{emp.designation || '—'}</td>
              <td className="px-4 py-3 text-gray-600">{emp.dateOfJoining || '—'}</td>
              <td className="px-4 py-3 text-right">
                <div className="flex justify-end gap-2">
                  <button
                    onClick={() => onEdit(emp)}
                    className="p-1.5 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                    title="Edit"
                  >
                    <FiEdit2 className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => onDelete(emp)}
                    className="p-1.5 text-red-500 hover:bg-red-50 rounded-lg transition-colors"
                    title="Deactivate"
                  >
                    <FiTrash2 className="w-4 h-4" />
                  </button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

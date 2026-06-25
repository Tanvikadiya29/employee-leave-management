import { useState, useEffect, useCallback } from 'react';
import { toast } from 'react-toastify';
import Navbar from '../components/common/Navbar';
import Sidebar from '../components/common/Sidebar';
import EmployeeList from '../components/employee/EmployeeList';
import EmployeeForm from '../components/employee/EmployeeForm';
import ConfirmModal from '../components/common/ConfirmModal';
import Pagination from '../components/common/Pagination';
import LoadingSpinner from '../components/common/LoadingSpinner';
import { getAllEmployees, createEmployee, updateEmployee, deleteEmployee } from '../api/employeeApi';
import { usePagination } from '../hooks/usePagination';
import { FiPlus, FiSearch } from 'react-icons/fi';

export default function EmployeesPage() {
  const { page, pageSize, goToPage, reset } = usePagination(10);
  const [employees, setEmployees] = useState([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [formLoading, setFormLoading] = useState(false);

  const [showForm, setShowForm] = useState(false);
  const [editEmployee, setEditEmployee] = useState(null);

  const [deleteTarget, setDeleteTarget] = useState(null);
  const [deleteLoading, setDeleteLoading] = useState(false);

  const [search, setSearch] = useState('');

  const fetchEmployees = useCallback(async () => {
    setLoading(true);
    try {
      const res = await getAllEmployees(page, pageSize);
      setEmployees(res.data?.data || []);
      setTotalCount(res.data?.totalCount || 0);
    } catch {
      toast.error('Failed to load employees.');
    } finally {
      setLoading(false);
    }
  }, [page, pageSize]);

  useEffect(() => {
    fetchEmployees();
  }, [fetchEmployees]);

  const handleCreate = async (data) => {
    setFormLoading(true);
    try {
      await createEmployee({
        firstName: data.firstName,
        lastName: data.lastName,
        email: data.email,
        password: data.password,
        department: data.department || null,
        designation: data.designation || null,
        dateOfJoining: data.dateOfJoining || null,
      });
      toast.success('Employee added successfully!');
      setShowForm(false);
      reset();
      fetchEmployees();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to add employee.');
    } finally {
      setFormLoading(false);
    }
  };

  const handleUpdate = async (data) => {
    setFormLoading(true);
    try {
      await updateEmployee(editEmployee.id, {
        firstName: data.firstName || undefined,
        lastName: data.lastName || undefined,
        department: data.department || undefined,
        designation: data.designation || undefined,
        dateOfJoining: data.dateOfJoining || undefined,
      });
      toast.success('Employee updated successfully!');
      setShowForm(false);
      setEditEmployee(null);
      fetchEmployees();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to update employee.');
    } finally {
      setFormLoading(false);
    }
  };

  const handleDelete = async () => {
    setDeleteLoading(true);
    try {
      await deleteEmployee(deleteTarget.id);
      toast.success(`${deleteTarget.firstName} has been deactivated.`);
      setDeleteTarget(null);
      fetchEmployees();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to deactivate employee.');
    } finally {
      setDeleteLoading(false);
    }
  };

  const openEdit = (emp) => {
    setEditEmployee(emp);
    setShowForm(true);
  };

  const closeForm = () => {
    setShowForm(false);
    setEditEmployee(null);
  };

  const filteredEmployees = employees.filter((e) => {
    if (!search) return true;
    const q = search.toLowerCase();
    return (
      (e.firstName + ' ' + e.lastName).toLowerCase().includes(q) ||
      e.email.toLowerCase().includes(q) ||
      (e.department || '').toLowerCase().includes(q)
    );
  });

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col">
      <Navbar />
      <div className="flex flex-1">
        <Sidebar />
        <main className="flex-1 p-6">
          <div className="flex items-center justify-between mb-6">
            <div>
              <h1 className="text-2xl font-bold text-gray-800">Employees</h1>
              <p className="text-gray-500 text-sm mt-1">{totalCount} active employees</p>
            </div>
            <button
              onClick={() => { setEditEmployee(null); setShowForm(true); }}
              className="flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white text-sm font-medium px-4 py-2.5 rounded-lg transition-colors"
            >
              <FiPlus className="w-4 h-4" /> Add Employee
            </button>
          </div>

          <div className="mb-4 relative max-w-xs">
            <FiSearch className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 w-4 h-4" />
            <input
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Search by name, email, dept..."
              className="w-full border border-gray-300 rounded-lg pl-9 pr-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {loading ? (
            <LoadingSpinner message="Loading employees..." />
          ) : (
            <>
              <EmployeeList
                employees={filteredEmployees}
                onEdit={openEdit}
                onDelete={(emp) => setDeleteTarget(emp)}
              />
              {!search && (
                <Pagination
                  page={page}
                  pageSize={pageSize}
                  totalCount={totalCount}
                  onPageChange={goToPage}
                />
              )}
            </>
          )}
        </main>
      </div>

      <EmployeeForm
        isOpen={showForm}
        onClose={closeForm}
        onSubmit={editEmployee ? handleUpdate : handleCreate}
        loading={formLoading}
        employee={editEmployee}
      />

      <ConfirmModal
        isOpen={Boolean(deleteTarget)}
        title="Deactivate Employee"
        message={`Are you sure you want to deactivate ${deleteTarget?.firstName} ${deleteTarget?.lastName}? They will no longer be able to log in.`}
        onConfirm={handleDelete}
        onCancel={() => setDeleteTarget(null)}
        loading={deleteLoading}
        confirmLabel="Deactivate"
      />
    </div>
  );
}

import { Routes, Route, Navigate } from 'react-router-dom';
import ProtectedRoute from './ProtectedRoute';
import LoginPage from '../pages/LoginPage';
import DashboardPage from '../pages/DashboardPage';
import EmployeesPage from '../pages/EmployeesPage';
import LeaveRequestsPage from '../pages/LeaveRequestsPage';

const AppRoutes = () => (
  <Routes>
    <Route path="/login" element={<LoginPage />} />
    <Route path="/" element={<Navigate to="/dashboard" replace />} />

    <Route element={<ProtectedRoute />}>
      <Route path="/dashboard" element={<DashboardPage />} />
      <Route path="/leave-requests" element={<LeaveRequestsPage />} />
    </Route>

    <Route element={<ProtectedRoute requiredRole="Admin" />}>
      <Route path="/employees" element={<EmployeesPage />} />
    </Route>

    <Route path="*" element={<Navigate to="/dashboard" replace />} />
  </Routes>
);

export default AppRoutes;

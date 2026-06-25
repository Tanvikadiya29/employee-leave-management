import { NavLink } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { FiGrid, FiUsers, FiCalendar } from 'react-icons/fi';

export default function Sidebar() {
  const { isAdmin } = useAuth();

  const linkClass = ({ isActive }) =>
    `flex items-center gap-3 px-4 py-2.5 rounded-lg text-sm font-medium transition-colors ${
      isActive
        ? 'bg-blue-50 text-blue-700'
        : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'
    }`;

  return (
    <aside className="w-56 bg-white border-r border-gray-200 min-h-full px-3 py-4">
      <nav className="flex flex-col gap-1">
        <NavLink to="/dashboard" className={linkClass}>
          <FiGrid className="w-4 h-4" />
          Dashboard
        </NavLink>
        {isAdmin && (
          <NavLink to="/employees" className={linkClass}>
            <FiUsers className="w-4 h-4" />
            Employees
          </NavLink>
        )}
        <NavLink to="/leave-requests" className={linkClass}>
          <FiCalendar className="w-4 h-4" />
          {isAdmin ? 'All Leave Requests' : 'My Leave Requests'}
        </NavLink>
      </nav>
    </aside>
  );
}

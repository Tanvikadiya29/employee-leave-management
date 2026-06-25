import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { FiLogOut, FiUser } from 'react-icons/fi';

export default function Navbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <header className="bg-white border-b border-gray-200 px-6 py-3 flex items-center justify-between shadow-sm">
      <div className="flex items-center gap-2">
        <div className="w-8 h-8 bg-blue-600 rounded-lg flex items-center justify-center">
          <span className="text-white text-xs font-bold">LM</span>
        </div>
        <span className="font-semibold text-gray-800 text-lg">Leave Management</span>
      </div>
      <div className="flex items-center gap-4">
        <div className="flex items-center gap-2 text-sm text-gray-600">
          <FiUser className="w-4 h-4" />
          <span className="font-medium">{user?.fullName}</span>
          <span className="bg-blue-100 text-blue-700 text-xs px-2 py-0.5 rounded-full">{user?.role}</span>
        </div>
        <button
          onClick={handleLogout}
          className="flex items-center gap-1.5 text-sm text-gray-500 hover:text-red-600 transition-colors"
        >
          <FiLogOut className="w-4 h-4" />
          Logout
        </button>
      </div>
    </header>
  );
}

import { useState, useEffect } from 'react';
import Navbar from '../components/common/Navbar';
import Sidebar from '../components/common/Sidebar';
import { useAuth } from '../context/AuthContext';
import { getAllEmployees } from '../api/employeeApi';
import { getAllLeaves, getMyLeaves } from '../api/leaveApi';
import { FiUsers, FiClock, FiCheckCircle, FiXCircle } from 'react-icons/fi';

function StatCard({ icon: Icon, label, value, color }) {
  return (
    <div className="bg-white rounded-xl border border-gray-200 p-5 flex items-center gap-4 shadow-sm">
      <div className={`w-12 h-12 rounded-xl flex items-center justify-center ${color}`}>
        <Icon className="w-6 h-6 text-white" />
      </div>
      <div>
        <p className="text-2xl font-bold text-gray-800">{value ?? '—'}</p>
        <p className="text-sm text-gray-500">{label}</p>
      </div>
    </div>
  );
}

export default function DashboardPage() {
  const { isAdmin, user } = useAuth();
  const [stats, setStats] = useState({});
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        if (isAdmin) {
          const [empRes, leaveRes] = await Promise.all([
            getAllEmployees(1, 1),
            getAllLeaves(1, 1),
          ]);
          const totalEmp = empRes.data?.totalCount ?? 0;
          const totalLeaves = leaveRes.data?.totalCount ?? 0;

          const [pendingRes, approvedRes, rejectedRes] = await Promise.all([
            getAllLeaves(1, 1, 'Pending'),
            getAllLeaves(1, 1, 'Approved'),
            getAllLeaves(1, 1, 'Rejected'),
          ]);

          setStats({
            totalEmployees: totalEmp,
            pendingLeaves: pendingRes.data?.totalCount ?? 0,
            approvedLeaves: approvedRes.data?.totalCount ?? 0,
            rejectedLeaves: rejectedRes.data?.totalCount ?? 0,
          });
        } else {
          const [pendingRes, approvedRes, rejectedRes] = await Promise.all([
            getMyLeaves(1, 1),
            getMyLeaves(1, 1),
            getMyLeaves(1, 1),
          ]);
          const myRes = await getMyLeaves(1, 50);
          const myLeaves = myRes.data?.data || [];
          setStats({
            pendingLeaves: myLeaves.filter((l) => l.status === 'Pending').length,
            approvedLeaves: myLeaves.filter((l) => l.status === 'Approved').length,
            rejectedLeaves: myLeaves.filter((l) => l.status === 'Rejected').length,
            totalLeaves: myLeaves.length,
          });
        }
      } catch {
        /* stats remain empty */
      } finally {
        setLoading(false);
      }
    };
    fetchStats();
  }, [isAdmin]);

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col">
      <Navbar />
      <div className="flex flex-1">
        <Sidebar />
        <main className="flex-1 p-6">
          <div className="mb-6">
            <h1 className="text-2xl font-bold text-gray-800">Dashboard</h1>
            <p className="text-gray-500 text-sm mt-1">
              Welcome back, <span className="font-medium">{user?.fullName}</span>!
              {isAdmin ? " Here's an overview of the system." : " Here's a summary of your leaves."}
            </p>
          </div>

          {loading ? (
            <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
              {[...Array(4)].map((_, i) => (
                <div key={i} className="h-24 bg-gray-200 rounded-xl animate-pulse" />
              ))}
            </div>
          ) : (
            <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
              {isAdmin ? (
                <>
                  <StatCard icon={FiUsers} label="Total Employees" value={stats.totalEmployees} color="bg-blue-500" />
                  <StatCard icon={FiClock} label="Pending Leaves" value={stats.pendingLeaves} color="bg-yellow-500" />
                  <StatCard icon={FiCheckCircle} label="Approved Leaves" value={stats.approvedLeaves} color="bg-green-500" />
                  <StatCard icon={FiXCircle} label="Rejected Leaves" value={stats.rejectedLeaves} color="bg-red-500" />
                </>
              ) : (
                <>
                  <StatCard icon={FiClock} label="My Pending" value={stats.pendingLeaves} color="bg-yellow-500" />
                  <StatCard icon={FiCheckCircle} label="My Approved" value={stats.approvedLeaves} color="bg-green-500" />
                  <StatCard icon={FiXCircle} label="My Rejected" value={stats.rejectedLeaves} color="bg-red-500" />
                  <StatCard icon={FiUsers} label="Total Requests" value={stats.totalLeaves} color="bg-blue-500" />
                </>
              )}
            </div>
          )}

          <div className="mt-8 bg-white rounded-xl border border-gray-200 p-6 shadow-sm">
            <h2 className="text-lg font-semibold text-gray-800 mb-2">Quick Guide</h2>
            {isAdmin ? (
              <ul className="text-sm text-gray-600 space-y-1 list-disc list-inside">
                <li>Go to <strong>Employees</strong> to manage employee accounts</li>
                <li>Go to <strong>All Leave Requests</strong> to approve or reject pending leaves</li>
                <li>Use the status filter to view Pending / Approved / Rejected separately</li>
              </ul>
            ) : (
              <ul className="text-sm text-gray-600 space-y-1 list-disc list-inside">
                <li>Go to <strong>My Leave Requests</strong> to apply for leave</li>
                <li>You can cancel a leave request only while it's still <strong>Pending</strong></li>
                <li>Once Approved or Rejected, the decision is final</li>
              </ul>
            )}
          </div>
        </main>
      </div>
    </div>
  );
}

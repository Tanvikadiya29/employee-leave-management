import { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const storedToken = localStorage.getItem('lms_token');
    const storedUser = localStorage.getItem('lms_user');
    if (storedToken && storedUser) {
      try {
        setToken(storedToken);
        setUser(JSON.parse(storedUser));
      } catch {
        localStorage.removeItem('lms_token');
        localStorage.removeItem('lms_user');
      }
    }
    setLoading(false);
  }, []);

  const login = (tokenValue, userData) => {
    localStorage.setItem('lms_token', tokenValue);
    localStorage.setItem('lms_user', JSON.stringify(userData));
    setToken(tokenValue);
    setUser(userData);
  };

  const logout = () => {
    localStorage.removeItem('lms_token');
    localStorage.removeItem('lms_user');
    setToken(null);
    setUser(null);
  };

  const isAdmin = user?.role === 'Admin';
  const isEmployee = user?.role === 'Employee';

  return (
    <AuthContext.Provider value={{ user, token, login, logout, isAdmin, isEmployee, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
};

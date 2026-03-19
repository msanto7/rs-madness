import { BrowserRouter, Routes, Route, Navigate } from 'react-router';
import { AuthProvider } from './hooks/useAuth';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';

function RankingPage() {
  return <div style={{ padding: '2rem' }}><h1>Ranking Page (Coming Soon)</h1></div>;
}

function App() {
  return (
    // wrap everything in auth function
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          {/* anonymous routes */}
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          {/* protected routes */}
          <Route path="/ranking" element={
            <ProtectedRoute>
              <RankingPage />
            </ProtectedRoute>
          } />

          {/* redirect unknowns */}
          <Route path="*" element={<Navigate to="/ranking" />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
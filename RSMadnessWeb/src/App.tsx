import { BrowserRouter, Routes, Route, Navigate } from 'react-router';
import { AuthProvider } from './hooks/useAuth';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import RankingPage from './pages/RankingPage';
import Layout from './components/Layout';

function LeaderboardPage() {
  return <h1>Leaderboard Page (Coming Soon)</h1>;
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
          <Route element={<ProtectedRoute><Layout /></ProtectedRoute>}>
            <Route path="/ranking" element={<RankingPage />} />
            <Route path="/leaderboard" element={<LeaderboardPage />} />
          </Route>

          {/* redirect unknowns */}
          <Route path="*" element={<Navigate to="/ranking" />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
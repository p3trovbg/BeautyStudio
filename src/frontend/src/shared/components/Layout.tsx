import React from 'react';
import { Link, Outlet, useLocation } from 'react-router-dom';
import { useAuthStore } from '@/shared/hooks/useAuth';

export const Layout: React.FC = () => {
    const { isAuthenticated, role, logout } = useAuthStore();
    const location = useLocation();

    const navItems = [
        { label: 'Appointments', path: '/appointments' },
        { label: 'Owners', path: '/owners' },
        { label: 'Customers', path: '/customers' },
    ];

    return (
        <div className="min-h-screen bg-background">
            <header className="sticky top-0 z-50 border-b border-border bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
                <div className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
                    <Link to="/" className="text-xl font-bold text-primary">
                        📅 AppointmentSystem
                    </Link>

                    <nav className="hidden gap-6 md:flex">
                        {navItems.map((item) => (
                            <Link
                                key={item.path}
                                to={item.path}
                                className={`text-sm font-medium transition-colors hover:text-primary ${location.pathname.startsWith(item.path)
                                        ? 'text-primary'
                                        : 'text-muted-foreground'
                                    }`}
                            >
                                {item.label}
                            </Link>
                        ))}
                    </nav>

                    <div className="flex items-center gap-4">
                        {isAuthenticated ? (
                            <>
                                <span className="rounded-full bg-primary/10 px-3 py-1 text-xs font-medium text-primary">
                                    {role}
                                </span>
                                <button
                                    onClick={logout}
                                    className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors"
                                >
                                    Logout
                                </button>
                            </>
                        ) : (
                            <Link
                                to="/login"
                                className="rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90 transition-colors"
                            >
                                Login
                            </Link>
                        )}
                    </div>
                </div>
            </header>

            <main className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
                <Outlet />
            </main>
        </div>
    );
};

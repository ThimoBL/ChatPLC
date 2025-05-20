import React from 'react';
import './App.css';
import {Outlet} from 'react-router';
import AuthProvider from "./components/providers/AuthProvider";

export default function App() {
    return (
        <AuthProvider>
            <Outlet/>
        </AuthProvider>
    );
}
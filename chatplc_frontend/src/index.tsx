import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import axios from "axios";
import {createBrowserRouter, RouterProvider} from 'react-router-dom';
import DashboardPage from './components/pages/DashboardPage';
import Layout from "./layouts/Layout";
import TempPage from "./components/pages/TempPage";

const root = ReactDOM.createRoot(
    document.getElementById('root') as HTMLElement
);

axios.defaults.baseURL = import.meta.env.VITE_API_URL;

const routes = createBrowserRouter([
    {
        Component: App,
        children: [
            {
                path: '/',
                Component: Layout,
                children: [
                    {
                        path: '',
                        Component: DashboardPage
                    },
                    {
                        path: 'temp',
                        Component: TempPage
                    }
                ]
            }
        ]
    }
]);

root.render(
    <React.StrictMode>
        <RouterProvider router={routes}/>
    </React.StrictMode>
);

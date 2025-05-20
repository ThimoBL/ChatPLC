import {Outlet} from "react-router-dom";
import {DashboardLayout, Navigation} from "@toolpad/core";
import {ReactRouterAppProvider} from "@toolpad/core/react-router";
import React, {useMemo} from "react";
import HourglassBottomIcon from '@mui/icons-material/HourglassBottom';
import DashboardIcon from "@mui/icons-material/Dashboard";
import {useAuth} from "../components/providers/AuthProvider";

const NAVIGATION: Navigation = [
    {
        kind: 'header',
        title: 'Main Items',
    },
    {
        title: 'Ask Question',
        icon: <DashboardIcon/>,
    },
    {
        segment: 'temp',
        title: 'Temp',
        icon: <HourglassBottomIcon/>,
    }
];

const BRANDING = {
    title: 'ChatPLC',
    logo: <img src="https://static-00.iconduck.com/assets.00/computer-laptop-code-icon-512x512-iobdd6vx.png"
               alt="ChatPLC logo"/>,
};

export default function Layout() {
    const {session, signOutUrl} = useAuth();

    const authentication = useMemo(() => {
        return {
            signIn: async () => {
                window.location.href = "/bff/login";
            },
            signOut: async () => {
                window.location.href = signOutUrl;
            },
        }
    }, [session]);

    return (
        <ReactRouterAppProvider navigation={NAVIGATION} branding={BRANDING} authentication={authentication}
                                session={session}>
            <DashboardLayout>
                <Outlet/>
            </DashboardLayout>
        </ReactRouterAppProvider>
    )
}
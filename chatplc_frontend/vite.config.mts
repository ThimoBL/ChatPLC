import {defineConfig} from 'vite'
import plugin from '@vitejs/plugin-react';
import {fileURLToPath} from "node:url";
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import {env} from 'process';

const targetUrl = 'https://localhost:6001';

//############################# Create certificate #############################
const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ''
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateArg = process.argv.map(arg => arg.match(/--name=(?<value>.+)/i)).filter(Boolean)[0];
// @ts-ignore
const certificateName = certificateArg ? certificateArg.groups.value : "reactapp1.client";

if (!certificateName) {
    console.error('Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<<app>> explicitly.')
    process.exit(-1);
}

const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (0 !== child_process.spawnSync('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password',
    ], {stdio: 'inherit',}).status) {
        throw new Error("Could not create certificate.");
    }
}

export default defineConfig({
    plugins: [plugin(),],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },

    server: {
        proxy: {
            '^/test': {
                target: targetUrl,
                secure: false
            },
            '^/question': {
                target: targetUrl,
                secure: false
            },
            '^/file': {
                target: targetUrl,
                secure: false
            },
            '^/bff': {
                target: targetUrl,
                secure: false
            },
            '^/signin-oidc': {
                target: targetUrl,
                secure: false
            },
            '^/signout-callback-oidc': {
                target: targetUrl,
                secure: false
            }
        },
        https: {
            key: fs.readFileSync(keyFilePath),
            cert: fs.readFileSync(certFilePath),
        },
        port: 3000,
        // host: true,
    },
})
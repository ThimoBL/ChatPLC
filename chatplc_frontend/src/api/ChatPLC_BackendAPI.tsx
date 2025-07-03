import axios from "axios";
import { toast } from "react-toastify";
import {ApiResult} from "../models/ApiResult";

export async function fetchPlcCodeJson(question: string): Promise<ApiResult> {
    try {
        const myHeaders = new Headers();
        myHeaders.append("X-CSRF", "1");

        const response = await axios.post<ApiResult>('/question/json', {
            question: question,
        }, {
            headers: {
                "X-CSRF": 1,
            }
        });

        return response.data;
    } catch (error) {
        console.error('Error fetching PLC code:', error);
        throw error;
    }
}

export async function fetchTestEndpoint(): Promise<string> {
    try {
        const response = await axios.get<string>('/test');
        console.log("Response from test endpoint:", response.data);
        return response.data;
    } catch (error) {
        console.error('Error testing endpoint:', error);
        throw error;
    }
}

export async function AcceptCode(codeResponse: string): Promise<string> {
    try {
        const response = await axios.post<string>('/file/upload', {
            code: codeResponse,
        }, {
            headers: {
                "X-CSRF": 1,
            }
        })

        return response.data;
    } catch (error) {
        console.error('Error saving codeResponse:', error);
        throw error;
    }
}
import axios from "axios";
import {ApiResult} from "../models/ApiResult";

export async function fetchPlcCodeJson(question: string): Promise<ApiResult> {
    try {
        const response = await axios.post<ApiResult>('/question/json', {
            question: question,
        });

        return response.data;
    } catch (error) {
        console.error('Error fetching PLC code:', error);
        throw error;
    }
}

export async function fetchTestEndpoint(): Promise<string> {
    try {
        const response = await axios.get<string>('test');
        console.log("Response from test endpoint:", response.data);
        return response.data;
    } catch (error) {
        console.error('Error testing endpoint:', error);
        throw error;
    }
}
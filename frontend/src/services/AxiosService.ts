import axios, { type AxiosRequestConfig } from "axios";
import type Token from "../interfaces/Token";
import json from "../../package.json";

//Add authorization geader to the request config.
export const getAuthorizationHeader = async (config: AxiosRequestConfig) => {
  const accessToken = localStorage.getItem("access_token");
  if (accessToken) {
    config.headers!.Authorization = `Bearer ${accessToken}`;
  }

  return config;
};

const config = {
  baseURL: json.urlProd,
  headers: {
    "Content-type": "application/json",
  },
  crossDomain: true,
};
const http = axios.create(config);

//Add authorization header to each request.
http.interceptors.request.use(getAuthorizationHeader);

// Define a function to refresh the access token using the refresh token.
const refreshAccessToken = async () => {
  const refreshToken = localStorage.getItem("refresh_token");
  if (refreshToken) {
    try {
      if (localStorage.getItem("refresh_token") != null) {
        const response = await axios.get<Token>(
          `${json.urlProd}refresh?refreshToken=${btoa(
            localStorage.getItem("refresh_token")!
          )}`
        );
        const newAccessToken = response.data.access;
        localStorage.setItem("access_token", newAccessToken);
        localStorage.setItem("refresh_token", response.data.refresh);
        return newAccessToken;
      }
    } catch (error) {
      return null;
    }
  } else {
    return null;
  }
};

//Checks responses for error message. If error status code is 401, it tries to get new access token.
http.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    if (error.response && error.response.status === 401) {
      const originalRequest = error.config;
      const accessToken = localStorage.getItem("access_token");
      if (accessToken && error.config && !error.config.__isRetryRequest) {
        try {
          // Make a request to refresh the access token
          const newAccessToken = await refreshAccessToken();
          if (newAccessToken) {
            // Update the original request with the new access token
            originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
            return http(originalRequest);
          }
        } catch (err) {
          return Promise.reject(err);
        }
      }
    }
    return Promise.reject(error);
  }
);

export default http;

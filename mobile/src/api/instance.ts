import { ofetch } from "ofetch";

const API_URL = "http://localhost:3000/";

export const api = ofetch.create({
  baseURL: API_URL,
  credentials: "include",
});

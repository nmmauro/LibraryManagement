export interface User {
    id?: string;
    username: string;
    email: string;
    token?: string;
    role: string;
  }
  
  export interface LoginRequest {
    email: string;
    password: string;
  }
  
  export interface RegisterRequest {
    username: string;
    email: string;
    password: string;
    role: string;
  }
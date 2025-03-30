
export interface User {
  id: string;
  username: string;
  email: string;
  fullName?: string;
  role: 'admin' | 'client';
  active: boolean;
  created_at: string;
  last_login?: string;
}

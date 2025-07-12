export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  createdAt: string;
  emailConfirmed: boolean;
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  phoneNumber?: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

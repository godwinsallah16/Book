import { publicApiClient } from '../utils';
import type { User } from '../types/user.types';

export const userService = {
  async getProfile(): Promise<User> {
    // Replace with actual profile endpoint, e.g. '/auth/profile' or '/user/profile'
    const response = await publicApiClient.get<User>('/auth/profile');
    return response.data;
  },
};

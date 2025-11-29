import Constants from 'expo-constants';

const API_BASE_URL = 'https://commssenderapi.bregan.me/api';

// Get API key from environment variable
// In Expo, use EXPO_PUBLIC_ prefix for variables that should be available in the app
const API_KEY = Constants.expoConfig?.extra?.apiKey || process.env.EXPO_PUBLIC_API_KEY;

if (!API_KEY) {
  console.warn('API_KEY is not set. Push token registration will fail.');
}

export const API_CONFIG = {
  BASE_URL: API_BASE_URL,
  API_KEY: API_KEY,
  ENDPOINTS: {
    REGISTER_PUSH_TOKEN: '/PushToken/RegisterPushToken',
  },
};

export async function registerPushToken(pushToken: string): Promise<{ success: boolean; error?: string }> {
  try {
    const response = await fetch(`${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.REGISTER_PUSH_TOKEN}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'X-CommsSender-ApiKey': API_CONFIG.API_KEY || '',
      },
      body: JSON.stringify(pushToken),
    });

    if (!response.ok) {
      const errorText = await response.text();
      return {
        success: false,
        error: `HTTP ${response.status}: ${errorText}`,
      };
    }

    return { success: true };
  } catch (error) {
    return {
      success: false,
      error: error instanceof Error ? error.message : 'Unknown error occurred',
    };
  }
}

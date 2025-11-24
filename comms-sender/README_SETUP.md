# CommsSender App Setup

## Setting up the API Key

To register push tokens with your C# API, you need to configure your API key:

### Option 1: Using .env file (Recommended for Development)

1. Create a `.env` file in the root directory:
   ```bash
   cp .env.example .env
   ```

2. Edit `.env` and add your API key:
   ```
   EXPO_PUBLIC_API_KEY=your-actual-api-key-here
   ```

3. The `.env` file is already in `.gitignore`, so your API key won't be committed to git.

### Option 2: Using app.json (For Production Builds)

For production builds with EAS, add your API key to `app.json`:

```json
{
  "expo": {
    "extra": {
      "apiKey": "your-api-key-here"
    }
  }
}
```

**Important:** Don't commit `app.json` with the actual API key. Use EAS Secrets instead for production.

### Option 3: Using EAS Secrets (Recommended for Production)

For production builds, use EAS Secrets:

```bash
eas secret:create --scope project --name API_KEY --value your-actual-api-key
```

Then update `app.json` to reference the secret:

```json
{
  "expo": {
    "extra": {
      "apiKey": "${API_KEY}"
    }
  }
}
```

## Running the App

1. Install dependencies:
   ```bash
   npm install
   ```

2. Start the development server:
   ```bash
   npm start
   ```

3. Run on a physical device (push notifications don't work on simulators):
   - iOS: `npm run ios` (requires a physical device connected)
   - Android: `npm run android` (requires a physical device connected)

## API Integration

The app will:
1. Request notification permissions
2. Get an Expo push token
3. Display the token on screen
4. Allow you to register the token with your API at `https://commsapi.bregan.me/PushToken/RegisterPushToken`

The registration sends a POST request with:
- Header: `X-CommsSender-ApiKey: <your-api-key>`
- Body: The push token as a JSON string

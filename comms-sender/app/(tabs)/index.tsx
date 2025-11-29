import { useState, useEffect, useRef } from 'react';
import { StyleSheet, ScrollView, Platform, View, Text, Button, Alert } from 'react-native';
import Constants from 'expo-constants';
import * as Device from 'expo-device';
import * as Notifications from 'expo-notifications';
import { registerPushToken } from '@/config/api';

Notifications.setNotificationHandler({
  handleNotification: async () => ({
    shouldShowAlert: true,
    shouldPlaySound: true,
    shouldSetBadge: true,
    shouldShowBanner: true,
    shouldShowList: true,
  }),
});

export default function HomeScreen() {
  const [pushToken, setPushToken] = useState<string>('Loading...');
  const [notificationCount, setNotificationCount] = useState(0);
  const [registrationStatus, setRegistrationStatus] = useState<string>('Not registered');
  const [lastNotification, setLastNotification] = useState<string>('None');
  const notificationListener = useRef<Notifications.EventSubscription | null>(null);
  const responseListener = useRef<Notifications.EventSubscription | null>(null);

  async function registerForPushNotificationsAsync() {
    let token;

    if (Platform.OS === 'android') {
      await Notifications.setNotificationChannelAsync('default', {
        name: 'default',
        importance: Notifications.AndroidImportance.MAX,
        vibrationPattern: [0, 250, 250, 250],
        lightColor: '#FF231F7C',
      });
    }

    if (Device.isDevice) {
      const { status: existingStatus } = await Notifications.getPermissionsAsync();
      let finalStatus = existingStatus;
      if (existingStatus !== 'granted') {
        const { status } = await Notifications.requestPermissionsAsync();
        finalStatus = status;
      }
      if (finalStatus !== 'granted') {
        Alert.alert('Failed to get push token', 'Permission not granted for notifications');
        return;
      }
      
      try {
        const projectId = Constants?.expoConfig?.extra?.eas?.projectId ?? Constants?.easConfig?.projectId;
        if (!projectId) {
          Alert.alert('Error', 'Project ID not found');
          return;
        }
        token = (await Notifications.getExpoPushTokenAsync({ projectId })).data;
      } catch (error) {
        Alert.alert('Error', `Failed to get push token: ${error}`);
      }
    } else {
      Alert.alert('Must use physical device', 'Push notifications don\'t work on simulator/emulator');
    }

    return token;
  }

  async function handleRegisterToken() {
    if (!pushToken || pushToken === 'Loading...' || pushToken === 'Not available') {
      Alert.alert('Error', 'No push token available to register');
      return;
    }

    setRegistrationStatus('Registering...');
    const result = await registerPushToken(pushToken);
    
    if (result.success) {
      setRegistrationStatus('✓ Registered successfully');
      Alert.alert('Success', 'Push token registered with API');
    } else {
      setRegistrationStatus(`✗ Failed: ${result.error}`);
      Alert.alert('Registration Failed', result.error || 'Unknown error');
    }
  }

  useEffect(() => {
    registerForPushNotificationsAsync().then(token => {
      if (token) {
        setPushToken(token);
      } else {
        setPushToken('Not available');
      }
    });

    notificationListener.current = Notifications.addNotificationReceivedListener(notification => {
      setLastNotification(JSON.stringify(notification.request.content, null, 2));
      setNotificationCount(prev => prev + 1);
    });

    responseListener.current = Notifications.addNotificationResponseReceivedListener(response => {
      console.log('Notification tapped:', response);
    });

    return () => {
      if (notificationListener.current) {
        notificationListener.current.remove();
      }
      if (responseListener.current) {
        responseListener.current.remove();
      }
    };
  }, []);

  return (
    <View style={styles.container}>
      <ScrollView style={styles.scrollView} contentContainerStyle={styles.contentContainer}>
        <View style={styles.header}>
          <Text style={styles.title}>Push Notifications</Text>
          <Text style={styles.subtitle}>Ready to receive notifications</Text>
        </View>

        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Push Token</Text>
          <View style={styles.infoBox}>
            <Text style={styles.tokenText} selectable>
              {pushToken}
            </Text>
          </View>
          <Text style={styles.helpText}>
            This token will be used by your C# API to send notifications
          </Text>
          <View style={styles.buttonContainer}>
            <Button 
              title="Register Token with API" 
              onPress={handleRegisterToken}
              disabled={pushToken === 'Loading...' || pushToken === 'Not available'}
            />
          </View>
          <Text style={styles.statusInfo}>
            Status: {registrationStatus}
          </Text>
        </View>

        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Debug Information</Text>
          <View style={styles.infoBox}>
            <Text style={styles.debugItem}>
              Platform: <Text style={styles.bold}>{Platform.OS}</Text>
            </Text>
            <Text style={styles.debugItem}>
              App Version: <Text style={styles.bold}>{Constants.expoConfig?.version || '1.0.0'}</Text>
            </Text>
            <Text style={styles.debugItem}>
              Device: <Text style={styles.bold}>
                {Platform.OS === 'ios' ? 'iOS Device' : Platform.OS === 'android' ? 'Android Device' : 'Web'}
              </Text>
            </Text>
            <Text style={styles.debugItem}>
              Notifications Received: <Text style={styles.bold}>{notificationCount}</Text>
            </Text>
            <Text style={styles.debugItem}>
              Last Notification: <Text style={styles.bold}>
                {lastNotification === 'None' ? 'None' : 'See below'}
              </Text>
            </Text>
          </View>
          {lastNotification !== 'None' && (
            <View style={styles.infoBox}>
              <Text style={styles.tokenText} selectable>
                {lastNotification}
              </Text>
            </View>
          )}
        </View>

        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Status</Text>
          <View style={styles.statusBox}>
            <Text style={styles.statusText}>✓ App Running</Text>
            <Text style={styles.statusText}>✓ Ready for Push Notifications</Text>
          </View>
        </View>
      </ScrollView>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
  },
  scrollView: {
    flex: 1,
  },
  contentContainer: {
    padding: 20,
  },
  header: {
    marginBottom: 30,
    alignItems: 'center',
  },
  title: {
    fontSize: 32,
    fontWeight: 'bold',
    color: '#000',
  },
  subtitle: {
    fontSize: 16,
    marginTop: 8,
    color: '#666',
  },
  section: {
    marginBottom: 30,
  },
  sectionTitle: {
    fontSize: 20,
    fontWeight: '600',
    color: '#000',
  },
  infoBox: {
    borderRadius: 8,
    padding: 16,
    marginTop: 12,
    backgroundColor: '#f5f5f5',
    borderWidth: 1,
    borderColor: '#e0e0e0',
  },
  tokenText: {
    fontSize: 12,
    fontFamily: Platform.OS === 'ios' ? 'Courier' : 'monospace',
    color: '#000',
  },
  helpText: {
    fontSize: 12,
    marginTop: 8,
    color: '#999',
    fontStyle: 'italic',
  },
  debugItem: {
    fontSize: 14,
    marginBottom: 8,
    color: '#000',
  },
  bold: {
    fontWeight: 'bold',
  },
  statusBox: {
    borderRadius: 8,
    padding: 16,
    marginTop: 12,
    backgroundColor: '#e8f5e9',
    borderWidth: 1,
    borderColor: '#c8e6c9',
  },
  statusText: {
    fontSize: 14,
    marginBottom: 4,
    color: '#2e7d32',
  },
  buttonContainer: {
    marginTop: 12,
  },
  statusInfo: {
    fontSize: 12,
    marginTop: 8,
    color: '#666',
  },
});

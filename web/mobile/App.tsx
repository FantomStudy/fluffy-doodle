import React from "react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NavigationContainer } from "@react-navigation/native";
import { SafeAreaProvider } from "react-native-safe-area-context";
import { StatusBar } from "expo-status-bar";
import { RootNavigator } from "./src/navigation/RootNavigator";

const queryClient = new QueryClient();

const App = () => (
  <SafeAreaProvider>
    <QueryClientProvider client={queryClient}>
      <NavigationContainer>
        <StatusBar style="dark" />
        <RootNavigator />
      </NavigationContainer>
    </QueryClientProvider>
  </SafeAreaProvider>
);

export default App;

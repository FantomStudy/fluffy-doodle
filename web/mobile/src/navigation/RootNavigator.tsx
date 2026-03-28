import type { RootStackParamList } from "./types";
import { createNativeStackNavigator } from "@react-navigation/native-stack";
import * as React from "react";
import { AlgorithmScreen } from "../screens/AlgorithmScreen";
import { ChallengeScreen } from "../screens/ChallengeScreen";
import { CourseDetailScreen } from "../screens/CourseDetailScreen";
import { KnowledgeScreen } from "../screens/KnowledgeScreen";
import { LessonDetailScreen } from "../screens/LessonDetailScreen";
import { LoginScreen } from "../screens/LoginScreen";
import { RegisterScreen } from "../screens/RegisterScreen";
import { MainTabs } from "./MainTabs";

const Stack = createNativeStackNavigator<RootStackParamList>();

export const RootNavigator = () => (
  <Stack.Navigator screenOptions={{ headerShown: false }} initialRouteName="Auth">
    <Stack.Screen name="Auth" component={AuthNavigator} />
    <Stack.Screen name="Main" component={MainTabs} />
    <Stack.Screen
      name="CourseDetail"
      component={CourseDetailScreen}
      options={{ headerShown: true, title: "Курс" }}
    />
    <Stack.Screen
      name="LessonDetail"
      component={LessonDetailScreen}
      options={{ headerShown: true, title: "Урок" }}
    />
    <Stack.Screen name="Challenge" component={ChallengeScreen} />
    <Stack.Screen name="Knowledge" component={KnowledgeScreen} />
    <Stack.Screen name="Algorithm" component={AlgorithmScreen} />
  </Stack.Navigator>
);

const AuthStack = createNativeStackNavigator();

const AuthNavigator = () => (
  <AuthStack.Navigator screenOptions={{ headerShown: false }}>
    <AuthStack.Screen name="Login" component={LoginScreen} />
    <AuthStack.Screen name="Register" component={RegisterScreen} />
  </AuthStack.Navigator>
);

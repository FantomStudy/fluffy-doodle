import type { MainTabParamList } from "./types";
import { createBottomTabNavigator } from "@react-navigation/bottom-tabs";
import { BookOpen, Home, User, Users } from "lucide-react-native";
import * as React from "react";
import { useMe } from "../hooks/useMe";
import { ChildProgressScreen } from "../screens/ChildProgressScreen";
import { CoursesScreen } from "../screens/CoursesScreen";
import { HomeScreen } from "../screens/HomeScreen";
import { ProfileScreen } from "../screens/ProfileScreen";
import { colors, semantic } from "../theme/tokens";

const Tab = createBottomTabNavigator<MainTabParamList>();

export const MainTabs = () => {
  const { data: me } = useMe();
  const isParent = me?.roleName === "parent";

  if (isParent) {
    return (
      <Tab.Navigator
        screenOptions={{
          headerShown: false,
          tabBarActiveTintColor: colors.purple600,
          tabBarInactiveTintColor: semantic.foregroundMuted,
          tabBarStyle: { borderTopColor: semantic.borderSubtle },
        }}
      >
        <Tab.Screen
          name="ChildProgress"
          component={ChildProgressScreen}
          options={{
            tabBarLabel: "Профиль ребёнка",
            tabBarIcon: ({ color, size }) => <Users color={color} size={size} />,
          }}
        />
      </Tab.Navigator>
    );
  }

  return (
    <Tab.Navigator
      screenOptions={{
        headerShown: false,
        tabBarActiveTintColor: colors.purple600,
        tabBarInactiveTintColor: semantic.foregroundMuted,
        tabBarStyle: { borderTopColor: semantic.borderSubtle },
      }}
    >
      <Tab.Screen
        name="Home"
        component={HomeScreen}
        options={{
          tabBarLabel: "Главная",
          tabBarIcon: ({ color, size }) => <Home color={color} size={size} />,
        }}
      />
      <Tab.Screen
        name="Courses"
        component={CoursesScreen}
        options={{
          tabBarLabel: "Уроки",
          tabBarIcon: ({ color, size }) => <BookOpen color={color} size={size} />,
        }}
      />
      <Tab.Screen
        name="Profile"
        component={ProfileScreen}
        options={{
          tabBarLabel: "Профиль",
          tabBarIcon: ({ color, size }) => <User color={color} size={size} />,
        }}
      />
    </Tab.Navigator>
  );
};

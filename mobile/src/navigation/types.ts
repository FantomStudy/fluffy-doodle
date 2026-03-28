import type { NavigatorScreenParams } from "@react-navigation/native";

export type AuthStackParamList = {
  Login: undefined;
  Register: undefined;
};

export type MainTabParamList = {
  Home: undefined;
  Courses: undefined;
  Profile: undefined;
  ChildProgress: undefined;
};

export type CoursesStackParamList = {
  CoursesList: undefined;
  CourseDetail: { courseId: number };
  LessonDetail: { courseId: number; lessonId: number };
};

export type RootStackParamList = {
  Auth: NavigatorScreenParams<AuthStackParamList>;
  Main: NavigatorScreenParams<MainTabParamList>;
  CourseDetail: { courseId: number };
  LessonDetail: { courseId: number; lessonId: number };
  Challenge: undefined;
  Knowledge: undefined;
  Algorithm: undefined;
};

declare global {
  namespace ReactNavigation {
    interface RootParamList extends RootStackParamList {}
  }
}

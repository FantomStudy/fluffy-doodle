# Fluffy Doodle Mobile

React Native (Expo) версия обучающей платформы Fluffy Doodle.

## Стек

- **React Native** (Expo SDK 52)
- **TypeScript**
- **React Navigation** (native-stack + bottom-tabs)
- **TanStack React Query** (общий API/хуки с веб-версией)
- **ofetch** для HTTP-запросов

## Структура

```
mobile/
├── App.tsx                     # Точка входа
├── app.json                    # Конфигурация Expo
├── package.json
├── tsconfig.json
├── babel.config.js
└── src/
    ├── api/                    # API слой (идентичен вебу)
    │   ├── instance.ts
    │   ├── auth.ts
    │   ├── courses.ts
    │   └── user.ts
    ├── hooks/                  # React Query хуки
    │   ├── useLogin.ts
    │   ├── useRegister.ts
    │   ├── useMe.ts
    │   ├── useProfile.ts
    │   ├── useChildProgress.ts
    │   └── useCourses.ts
    ├── theme/
    │   └── tokens.ts           # Дизайн-токены (из index.css)
    ├── components/
    │   ├── ui/                 # Базовые UI-компоненты
    │   │   ├── Badge.tsx
    │   │   ├── Button.tsx
    │   │   ├── Card.tsx
    │   │   ├── Input.tsx
    │   │   └── index.ts
    │   ├── LoginForm.tsx
    │   ├── RegisterForm.tsx
    │   ├── ProfilePage.tsx
    │   ├── ChildProgressPage.tsx
    │   ├── CourseCard.tsx
    │   ├── LessonCard.tsx
    │   ├── QuizChallenge.tsx
    │   ├── CodingChallenge.tsx
    │   └── FlowchartChallenge.tsx
    ├── navigation/
    │   ├── types.ts            # Типы навигации
    │   ├── RootNavigator.tsx   # Корневой навигатор
    │   └── MainTabs.tsx        # Табы (разные для ученика/родителя)
    └── screens/
        ├── LoginScreen.tsx
        ├── RegisterScreen.tsx
        ├── HomeScreen.tsx
        ├── CoursesScreen.tsx
        ├── ProfileScreen.tsx
        ├── ChildProgressScreen.tsx
        ├── CourseDetailScreen.tsx
        ├── LessonDetailScreen.tsx
        ├── ChallengeScreen.tsx
        ├── KnowledgeScreen.tsx
        └── AlgorithmScreen.tsx
```

## Функционал

- ✅ Авторизация (вход / регистрация / роли: ученик, родитель)
- ✅ Главная с прогрессом и курсами
- ✅ Каталог курсов → детали курса → уроки
- ✅ Три типа интерактивных заданий:
  - 📝 Тест (quiz) с теорией и проверкой
  - 🔷 Блок-схема (flowchart) — сборка алгоритма из блоков
  - ⚙️ Кодинг (algorithm) — редактор кода с тестами
- ✅ Профиль с аватаром, уровнем, статистикой и достижениями
- ✅ Профиль ребёнка (для роли "родитель")
- ✅ Геймификация: звёзды, XP, уровни, серии
- ✅ Victory screen с наградами после заданий

## Запуск

```bash
cd mobile
npm install
npx expo start
```

## Конфигурация API

Измени baseURL в `src/api/instance.ts` на адрес бэкенда:

```ts
const API_URL = "http://YOUR_SERVER:3000/";
```

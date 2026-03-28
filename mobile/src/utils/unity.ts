import { Linking } from "react-native";

const unityScenes = ["if", "variables", "level3", "cycles"] as const;

const getRandomScene = () => {
  const randomIndex = Math.floor(Math.random() * unityScenes.length);

  return unityScenes[randomIndex];
};

const getUnityHost = () => {
  const unityHost = process.env.EXPO_PUBLIC_UNITY_HOST?.trim();

  if (!unityHost) return null;

  return unityHost.endsWith("/") ? unityHost.slice(0, -1) : unityHost;
};

export const getUnityPlayUrl = () => {
  const unityHost = getUnityHost();

  if (!unityHost) return null;

  return `${unityHost}/practice/play?scene=${getRandomScene()}&mobile=1`;
};

export const openUnityLevel = async () => {
  const unityUrl = getUnityPlayUrl();

  if (!unityUrl) return false;

  await Linking.openURL(unityUrl);

  return true;
};

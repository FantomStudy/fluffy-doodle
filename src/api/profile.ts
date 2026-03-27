import { requestJson, requestRaw } from "./client";

interface Achievement {
  id: number;
  name: string;
  description: string;
  icon: string;
}

interface UserProfile {
  id: number;
  login: string;
  fullName: string;
  phoneNumber: string;
  avatar: string;
  stars: number;
  achievements: Achievement[];
}

const getProfile = () => requestJson<UserProfile>("/user/profile");

const uploadAvatar = (file: File) => {
  const formData = new FormData();
  formData.append("avatar", file);

  return requestRaw("/user/avatar", {
    method: "POST",
    body: formData,
  });
};

export { getProfile, uploadAvatar };
export type { UserProfile, Achievement };

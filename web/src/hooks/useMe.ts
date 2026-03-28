import { useQuery } from "@tanstack/react-query";
import { getMe } from "@/api/user";

export const useMe = () =>
  useQuery({
    queryKey: ["me"],
    queryFn: getMe,
  });

import { useQuery } from "@tanstack/react-query";
import { getChildProgress } from "@/api/user";

export const useChildProgress = () =>
  useQuery({
    queryKey: ["child-progress"],
    queryFn: getChildProgress,
  });

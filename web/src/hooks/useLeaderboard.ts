import { useQuery } from "@tanstack/react-query";
import { getLeaderboard } from "@/api/user";

export const useLeaderboard = (limit = 10) =>
  useQuery({
    queryKey: ["leaderboard", limit],
    queryFn: () => getLeaderboard(limit),
  });

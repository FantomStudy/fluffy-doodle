import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { buyFrame, getFrames, setActiveFrame } from "@/api/frames";

export const useFrames = () =>
  useQuery({
    queryKey: ["frames"],
    queryFn: getFrames,
  });

export const useBuyFrame = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: buyFrame,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["frames"] });
      queryClient.invalidateQueries({ queryKey: ["me"] });
      queryClient.invalidateQueries({ queryKey: ["profile"] });
    },
  });
};

export const useSetActiveFrame = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: setActiveFrame,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["frames"] });
      queryClient.invalidateQueries({ queryKey: ["me"] });
      queryClient.invalidateQueries({ queryKey: ["profile"] });
    },
  });
};
